using System;
using System.Runtime.Serialization;

namespace SagaDemo.Common.Errors
{
    public class EntityNotFoundException : SagaDemoExceptionBase
    {
        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(string message) : base(message)
        {
        }

        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}