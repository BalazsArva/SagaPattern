using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using SagaDemo.Common.DataAccess.RavenDb.Configuration;

namespace SagaDemo.Common.DataAccess.RavenDb.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public const string DefaultConfigSectionName = "DataAccess:RavenDb";

        public static IServiceCollection AddRavenDb(this IServiceCollection services, IConfiguration configuration, string configSectionName = DefaultConfigSectionName)
        {
            var ravenDbConfigSection = configuration.GetSection(configSectionName);
            var ravenDbConfig = new RavenDbConfiguration();

            ravenDbConfigSection.Bind(ravenDbConfig);

            var documentStore = new DocumentStore
            {
                Database = ravenDbConfig.Database,
                Urls = ravenDbConfig.Urls
            }.Initialize();

            return services.AddSingleton(documentStore);
        }
    }
}