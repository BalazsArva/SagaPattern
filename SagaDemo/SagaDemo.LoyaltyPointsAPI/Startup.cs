using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SagaDemo.Common.AspNetCore.Extensions;
using SagaDemo.LoyaltyPointsAPI.Extensions;

namespace SagaDemo.LoyaltyPointsAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(opts =>
                {
                    opts.AddCommonFilters();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddLoyaltyDb(Configuration);
            services.AddHandlers();
            services.AddValidators();
            services.AddUtilities();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}