using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SagaDemo.LoyaltyPointsAPI.DataAccess;
using SagaDemo.LoyaltyPointsAPI.DataAccess.Entities;
using SagaDemo.LoyaltyPointsAPI.Operations.Commands;

namespace SagaDemo.LoyaltyPointsAPI.Handlers.CommandHandlers
{
    public class RefundPointsCommandHandler : ICommandHandler<RefundPointsCommand>
    {
        private const string Reason = "RefundPoints";

        private readonly ILoyaltyDbContextFactory dbContextFactory;
        private readonly IValidator<RefundPointsCommand> commandValidator;

        public RefundPointsCommandHandler(ILoyaltyDbContextFactory dbContextFactory, IValidator<RefundPointsCommand> commandValidator)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            this.commandValidator = commandValidator ?? throw new ArgumentNullException(nameof(commandValidator));
        }

        public async Task HandleAsync(RefundPointsCommand command, CancellationToken cancellationToken)
        {
            await commandValidator.ValidateAndThrowAsync(command, cancellationToken: cancellationToken).ConfigureAwait(false);

            using (var context = dbContextFactory.CreateDbContext())
            {
                context.PointsChangedEvents.Add(new PointsChangedEvent
                {
                    PointChange = command.Points,
                    Reason = Reason,
                    UtcDateTimeRecorded = DateTime.UtcNow,
                    UserId = command.UserId,
                    TransactionId = command.TransactionId
                });

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}