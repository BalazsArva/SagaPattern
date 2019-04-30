using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SagaDemo.Common.Errors;

namespace SagaDemo.Common.AspNetCore.Filters
{
    public class EntityNotFoundExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is EntityNotFoundException entityNotFoundException)
            {
                var result = new ProblemDetails
                {
                    Title = entityNotFoundException.Message,
                    Detail = entityNotFoundException.Message,
                    Status = StatusCodes.Status404NotFound
                };

                context.ExceptionHandled = true;
                context.Result = new NotFoundObjectResult(result);
            }
        }
    }
}