﻿using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SagaDemo.Common.AspNetCore.Extensions;
using SagaDemo.Common.DataAccess.RavenDb.Extensions;
using SagaDemo.DeliveryAPI.Extensions;
using Swashbuckle.AspNetCore.Swagger;

namespace SagaDemo.DeliveryAPI
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
                .AddMvc(mvcOptions => mvcOptions.AddCommonFilters())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddRavenDb(Configuration);
            services.AddDeliveryServices();

            services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);
                c.SwaggerDoc(ApiVersions.V1, new Info { Title = "Delivery API", Version = ApiVersions.V1 });
            });
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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{ApiVersions.V1}/swagger.json", "Delivery API");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}