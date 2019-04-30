namespace SagaDemo.DeliveryAPI.Operations.Commands
{
    public interface IDeliveryCommand
    {
        string TransactionId { get; }
    }
}