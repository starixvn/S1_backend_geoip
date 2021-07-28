using System.Collections.Generic;
using GeoIP.Library;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GeoIP.WebApi.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDatabaseConnection(this IServiceCollection services, IConfiguration config)
        {
            var provider = services.BuildServiceProvider();

            var conn = config.GetConnectionString("ConnectionString");

            var logger = provider.GetRequiredService<ILogger<MySqlClient>>();
            services.AddSingleton<IMySqlClient>(serviceProvider => new MySqlClient(conn, logger));

            return services;
        }
    }
}