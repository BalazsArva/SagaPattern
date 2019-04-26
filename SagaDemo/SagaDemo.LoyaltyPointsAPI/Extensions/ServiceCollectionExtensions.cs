using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SagaDemo.LoyaltyPointsAPI.DataAccess;

namespace SagaDemo.LoyaltyPointsAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLoyaltyDb(this IServiceCollection services, IConfiguration configuration)
        {
            var sqlConnectionString = configuration.GetConnectionString("LoyaltyDb");

            var options = new DbContextOptionsBuilder<LoyaltyDbContext>()
                .UseSqlServer(sqlConnectionString)
                .Options;

            services.AddSingleton(options);
            services.AddDbContext<LoyaltyDbContext>(opts => opts.UseSqlServer(sqlConnectionString));
            services.AddSingleton<ILoyaltyDbContextFactory>(svcProvider =>
            {
                return new LoyaltyDbContextFactory(options);
            });

            return services;
        }
    }
}