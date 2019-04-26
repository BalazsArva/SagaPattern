using System;
using System.Threading;
using System.Threading.Tasks;
using SagaDemo.LoyaltyPointsAPI.DataAccess;
using SagaDemo.LoyaltyPointsAPI.Operations.Commands;

namespace SagaDemo.LoyaltyPointsAPI.Handlers.CommandHandlers
{
    public class EarnPointsCommandHandler : ICommandHandler<EarnPointsCommand>
    {
        private readonly ILoyaltyDbContextFactory dbContextFactory;

        public EarnPointsCommandHandler(ILoyaltyDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public Task HandleAsync(EarnPointsCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}