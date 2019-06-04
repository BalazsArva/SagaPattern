using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SagaDemo.InventoryAPI.Extensions
{
    public static class EntityFrameworkQueryableExtensions
    {
        public static async Task<ISet<TSource>> ToSetAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken)
        {
            var resultList = await source.ToListAsync(cancellationToken).ConfigureAwait(false);

            return new HashSet<TSource>(resultList);
        }
    }
}