using Microsoft.AspNetCore.Mvc;
using SagaDemo.Common.AspNetCore.Filters;

namespace SagaDemo.Common.AspNetCore.Extensions
{
    public static class MvcOptionsExtensions
    {
        public static MvcOptions AddCommonFilters(this MvcOptions options)
        {
            options.Filters.Add<ValidationExceptionFilterAttribute>();

            return options;
        }
    }
}