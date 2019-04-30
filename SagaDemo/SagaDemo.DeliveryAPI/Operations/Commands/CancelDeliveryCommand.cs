﻿namespace SagaDemo.DeliveryAPI.Operations.Commands
{
    public class CancelDeliveryCommand : IDeliveryCommand
    {
        public CancelDeliveryCommand(string transactionId)
        {
            TransactionId = transactionId;
        }

        public string TransactionId { get; }
    }
}