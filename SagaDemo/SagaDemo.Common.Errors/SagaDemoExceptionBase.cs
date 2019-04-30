using System;
using System.Runtime.Serialization;

namespace SagaDemo.Common.Errors
{
    public abstract class SagaDemoExceptionBase : Exception
    {
        protected SagaDemoExceptionBase()
        {
        }

        protected SagaDemoExceptionBase(string message) : base(message)
        {
        }

        protected SagaDemoExceptionBase(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SagaDemoExceptionBase(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}