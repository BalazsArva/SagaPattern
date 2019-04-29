﻿namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class AddReservationCommand
    {
        public AddReservationCommand(string productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public string ProductId { get; }

        public int Quantity { get; }
    }
}