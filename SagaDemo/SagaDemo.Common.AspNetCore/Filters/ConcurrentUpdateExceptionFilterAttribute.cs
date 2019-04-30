using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SagaDemo.Common.Errors;

namespace SagaDemo.Common.AspNetCore.Filters
{
    public class ConcurrentUpdateExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is ConcurrentUpdateException concurrentUpdateException)
            {
                context.ExceptionHandled = true;
                context.Result = new StatusCodeResult(StatusCodes.Status409Conflict);
            }
        }
    }
}