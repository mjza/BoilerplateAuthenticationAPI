using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using WebApi.Extensions;
using WebApi.Helpers.Auth;
using WebApi.Middleware.Auth;
using WebApi.Services.Auth;

namespace WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }        

        // This method gets called by the runtime. Use this method to add services to the container.
        // Most of the functions here are functions in the ServicesEtenctions.cs file
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureAppSettings(Configuration);

            services.ConfigureDBContext(Configuration);

            services.ConfigureCors();

            services.ConfigureIISIntegration();

            services.ConfigureApplicationServices();

            services.ConfigureSwagger();

            services.ConfigureLocalization();

            services.AddControllers()
                    .AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true)
                    .SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AccountDbContext accountDbContext)
        {
            // migrate database changes on startup (includes initial db creation)
            accountDbContext.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // generated swagger json and swagger ui middleware
                app.UseSwagger();
                app.UseSwaggerUI(options => {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1");
                });
            }

            app.UseHttpsRedirection();

            var localizeOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizeOptions.Value);

            app.UseRouting();

            app.UseAuthorization();            

            // global error handler
            app.UseMiddleware<ErrorHandlerMiddleware>();

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
