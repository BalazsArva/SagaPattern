using System;
using SagaDemo.OrderAPI.Contracts.Requests;
using SagaDemo.OrderAPI.Operations.Commands;

namespace SagaDemo.OrderAPI.Mappers
{
    public static class RegisterOrderMapper
    {
        public static RegisterOrderCommand ToCommand(RegisterOrderRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new RegisterOrderCommand
            {
                UserId = request.UserId,
                Address = AddressMapper.ToOperationsDataStructure(request.Address),
                Order = OrderMapper.ToOperationsDataStructure(request.Order)
            };
        }
    }
}