﻿using System;
using System.Linq;
using SagaDemo.OrderAPI.Entitites;
using SagaDemo.OrderAPI.Operations.Commands;
using SagaDemo.OrderAPI.Operations.DataStructures;

namespace SagaDemo.OrderAPI.Mappers
{
    public static class CreateOrderCommandMapper
    {
        public static CreateOrderCommand ToCommand(OrderTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            return new CreateOrderCommand(
                transaction.OrderDetails.UserId,
                new Order
                {
                    Items = transaction
                        .OrderDetails
                        .Items
                        .Select(i => new Operations.DataStructures.OrderItem
                        {
                            ProductId = i.ProductId,
                            Quantity = i.Quantity
                        })
                        .ToList()
                },
                new Operations.DataStructures.Address
                {
                    City = transaction.OrderDetails.Address.City,
                    Country = transaction.OrderDetails.Address.Country,
                    House = transaction.OrderDetails.Address.House,
                    State = transaction.OrderDetails.Address.State,
                    Street = transaction.OrderDetails.Address.Street,
                    Zip = transaction.OrderDetails.Address.Zip
                });
        }
    }
}