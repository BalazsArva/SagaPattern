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
    public class RefundPointsCommandHandler : ICommandHandler<RefundPointsCommand>
    {
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

            try
            {
                using (var context = dbContextFactory.CreateDbContext())
                {
                    var consumptionEvent = await context.PointsConsumedEvents.FirstAsync(e => e.TransactionId == command.TransactionId).ConfigureAwait(false);

                    var points = consumptionEvent.PointChange;
                    var transactionId = command.TransactionId;

                    context.PointsRefundedEvents.Add(new PointsRefundedEvent
                    {
                        PointChange = points,
                        Reason = $"Refunded {points} points consumed by transaction {transactionId}.",
                        UtcDateTimeRecorded = DateTime.UtcNow,
                        UserId = consumptionEvent.UserId,
                        TransactionId = command.TransactionId
                    });

                    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolationError())
            {
                // This means this consumption has already been refunded, can be ignored.
                return;
            }
        }
    }
}