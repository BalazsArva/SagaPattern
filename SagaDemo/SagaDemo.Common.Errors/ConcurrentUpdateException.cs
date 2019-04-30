using System;
using System.Runtime.Serialization;

namespace SagaDemo.Common.Errors
{
    public class ConcurrentUpdateException : SagaDemoExceptionBase
    {
        public ConcurrentUpdateException()
        {
        }

        public ConcurrentUpdateException(string message) : base(message)
        {
        }

        public ConcurrentUpdateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ConcurrentUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}