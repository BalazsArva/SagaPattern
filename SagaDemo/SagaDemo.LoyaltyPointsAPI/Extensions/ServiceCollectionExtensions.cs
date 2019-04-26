using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SagaDemo.LoyaltyPointsAPI.DataAccess;
using SagaDemo.LoyaltyPointsAPI.Handlers.CommandHandlers;
using SagaDemo.LoyaltyPointsAPI.Operations.Commands;

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

        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            return services
                .AddSingleton<ICommandHandler<EarnPointsCommand>, EarnPointsCommandHandler>()
                .AddSingleton<ICommandHandler<ConsumePointsCommand>, ConsumePointsCommandHandler>()
                .AddSingleton<ICommandHandler<RefundPointsCommand>, RefundPointsCommandHandler>();
        }
    }
}