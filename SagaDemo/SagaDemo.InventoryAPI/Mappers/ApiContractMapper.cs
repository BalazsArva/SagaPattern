﻿using System.Linq;
using SagaDemo.InventoryAPI.Contracts.Requests;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Mappers
{
    public static class ApiContractMapper
    {
        public static CreateProductCommand ToServiceCommand(CreateProductRequest apiRequest)
        {
            if (apiRequest == null)
            {
                return null;
            }

            return new CreateProductCommand(apiRequest.Name, apiRequest.PointsCost);
        }

        public static AddReservationsCommand ToServiceCommand(string transactionId, AddReservationsRequest apiRequest)
        {
            if (apiRequest == null)
            {
                return null;
            }

            return new AddReservationsCommand(apiRequest.Items.Select(r => ToServiceCommand(r)), transactionId);
        }

        public static AddReservationCommand ToServiceCommand(AddReservationRequest apiRequest)
        {
            if (apiRequest == null)
            {
                return null;
            }

            return new AddReservationCommand(apiRequest.ProductId, apiRequest.Quantity);
        }

        public static AddStocksCommand ToServiceCommand(AddStocksRequest apiRequest)
        {
            if (apiRequest == null)
            {
                return null;
            }

            return new AddStocksCommand(apiRequest.Items.Select(r => ToServiceCommand(r)), apiRequest.TransactionId);
        }

        public static RemoveStocksCommand ToServiceCommand(RemoveStocksRequest apiRequest)
        {
            if (apiRequest == null)
            {
                return null;
            }

            return new RemoveStocksCommand(apiRequest.Items.Select(r => ToServiceCommand(r)), apiRequest.TransactionId);
        }

        public static AddStockCommand ToServiceCommand(AddStockRequest apiRequest)
        {
            if (apiRequest == null)
            {
                return null;
            }

            return new AddStockCommand(apiRequest.ProductId, apiRequest.Quantity);
        }

        public static RemoveStockCommand ToServiceCommand(RemoveStockRequest apiRequest)
        {
            if (apiRequest == null)
            {
                return null;
            }

            return new RemoveStockCommand(apiRequest.ProductId, apiRequest.Quantity);
        }

        public static TakeoutItemsCommand ToServiceCommand(string transactionId, TakeoutItemsRequest apiRequest)
        {
            if (apiRequest == null)
            {
                return null;
            }

            return new TakeoutItemsCommand(apiRequest.Items.Select(r => ToServiceCommand(r)), transactionId);
        }

        public static TakeoutItemCommand ToServiceCommand(TakeoutItemRequest apiRequest)
        {
            if (apiRequest == null)
            {
                return null;
            }

            return new TakeoutItemCommand(apiRequest.ProductId, apiRequest.Quantity);
        }
    }
}