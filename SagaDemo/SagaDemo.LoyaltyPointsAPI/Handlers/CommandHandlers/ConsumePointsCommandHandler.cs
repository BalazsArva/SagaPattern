using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SagaDemo.Common.DataAccess.EntityFrameworkCore.Helpers;
using SagaDemo.LoyaltyPointsAPI.DataAccess;
using SagaDemo.LoyaltyPointsAPI.DataAccess.Entities;
using SagaDemo.LoyaltyPointsAPI.Operations.Commands;

namespace SagaDemo.LoyaltyPointsAPI.Handlers.CommandHandlers
{
    public class ConsumePointsCommandHandler : ICommandHandler<ConsumePointsCommand>
    {
        private const string ConsumePointsReason = "ConsumePoints";

        private readonly ILoyaltyDbContextFactory dbContextFactory;
        private readonly IValidator<ConsumePointsCommand> commandValidator;

        public ConsumePointsCommandHandler(ILoyaltyDbContextFactory dbContextFactory, IValidator<ConsumePointsCommand> commandValidator)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            this.commandValidator = commandValidator ?? throw new ArgumentNullException(nameof(commandValidator));
        }

        public async Task HandleAsync(ConsumePointsCommand command, CancellationToken cancellationToken)
        {
            await commandValidator.ValidateAndThrowAsync(command, cancellationToken: cancellationToken).ConfigureAwait(false);

            try
            {
                using (var context = dbContextFactory.CreateDbContext())
                {
                    context.PointsConsumedEvents.Add(new PointsConsumedEvent
                    {
                        PointChange = command.Points,
                        UtcDateTimeRecorded = DateTime.UtcNow,
                        UserId = command.UserId,
                        TransactionId = command.TransactionId,
                        Reason = ConsumePointsReason,
                    });

                    // Assume optimistic concurrency so that points aren't changed between retrieving the sum and adding the consume record.
                    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolationError())
            {
                // This means this consumption has already been registered, can be ignored.
                return;
            }
        }
    }
}