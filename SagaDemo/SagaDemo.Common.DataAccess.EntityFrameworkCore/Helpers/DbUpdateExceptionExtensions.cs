using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace SagaDemo.Common.DataAccess.EntityFrameworkCore.Helpers
{
    public static class DbUpdateExceptionExtensions
    {
        private const int SqlServerUniqueIndexViolationErrorCode = 2601;
        private const int SqlServerUniqueConstraintViolationErrorCode = 2627;

        public static bool IsUniqueConstraintViolationError(this DbUpdateException dbUpdateException)
        {
            // TODO: Try it out whether this logic is correct.
            // This answer https://stackoverflow.com/a/31516402/4205470 writes doubly nested inner exceptions but it concerns EF 6.
            return
                dbUpdateException.InnerException is SqlException sqlException &&
                (sqlException.Number == SqlServerUniqueIndexViolationErrorCode || sqlException.Number == SqlServerUniqueConstraintViolationErrorCode);
        }
    }
}