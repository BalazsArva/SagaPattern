using System.Threading;
using System.Threading.Tasks;

namespace SagaDemo.LoyaltyPointsAPI.Utilities
{
    public interface IPointsBalanceCalculator
    {
        Task<int> CalculateTotalAsync(int userId, CancellationToken cancellationToken);
    }
}