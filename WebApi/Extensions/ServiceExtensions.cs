using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
//using Pomelo.EntityFrameworkCore.MySql.Storage;
//using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using WebApi.Helpers.Auth;

namespace WebApi.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed(origin => true)
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {

            });
        }

        public static void ConfigureDBContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AccountDbContext>(options =>
            {
                // https://www.koskila.net/ef-core-returns-null-for-a-navigation-property/
                // Used lazy loading here to resolving the missing tokens on account in time of revoking 
                options.UseLazyLoadingProxies().UseSqlServer(configuration.GetConnectionString("SqlServerConnection"));
            });
        }

    }
}
