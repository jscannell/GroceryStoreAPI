using GroceryStoreAPI.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace GroceryStoreAPI
{
    public class Startup
    {
        const string ApiTitle = "Grocery Store API";
        const string ApiName = "grocery";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddApiExplorer()
                .AddFormatterMappings()
                .AddDataAnnotations()
                .AddJsonFormatters();

            services.AddSwaggerGen(c =>
            {
                // Format operation ids for autorest
                c.CustomOperationIds((apiDescription) =>
                {
                    var action = (ControllerActionDescriptor)apiDescription.ActionDescriptor;
                    return string.Format("{0}_{1}", action.ControllerName, action.ActionName);
                });

                c.SwaggerDoc(ApiName, new Info { Title = ApiTitle, Version = "v1" });
            });

            services.AddSingleton<IDatabase>(new Database(Configuration.GetConnectionString("Database")));
            services.AddRepositories();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IDatabase database)
        {
            if (!env.IsDevelopment())
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = ApiTitle;
                c.SwaggerEndpoint($"/swagger/{ApiName}/swagger.json", ApiTitle);
                c.RoutePrefix = string.Empty;
            });

            database.LoadAsync().Wait();
        }
    }
}
