namespace SagaDemo.LoyaltyPointsAPI.Operations.Commands
{
    public class EarnPointsCommand
    {
        public EarnPointsCommand(int points, int userId)
        {
            Points = points;
            UserId = userId;
        }

        public int Points { get; }

        public int UserId { get; }
    }
}