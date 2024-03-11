using GeoIP.Core;
using GeoIP.Core.BLL;
using GeoIP.Core.DAL;
using GeoIP.Core.Library;
using GeoIP.Core.Models;
using StackExchange.Redis;
using Elastic.Apm.StackExchange.Redis;

namespace GeoIP.WebApi.Extension
{
    public static class ServiceCollectionExtension
    {

        public static IServiceCollection AddDatabaseConnection(this IServiceCollection services, IConfiguration config)
        {
            var provider = services.BuildServiceProvider();
            var logger = provider.GetRequiredService<ILogger<MySqlClient>>();
            var serviceType = Environment.GetEnvironmentVariable("ServiceType") ?? config.GetValue<string>("ServiceType");

            services.AddSingleton<IMySqlClient>(param =>
                    new MySqlClient(config.GetConnectionString($"{serviceType}_ReportDB"), logger, DataBase.Report))
                .AddSingleton<IMySqlClient>(param =>
                    new MySqlClient(config.GetConnectionString($"{serviceType}_ReportIPTable"), logger, DataBase.ReportIPTable));

            return services;
        }

        public static IServiceCollection AddRedisConnection(this IServiceCollection services, IConfiguration config)
        {
            var redisConnection = ConnectionMultiplexer.Connect(config.GetValue<string>("RedisConnection"));
            redisConnection.UseElasticApm();
            services.AddSingleton<IConnectionMultiplexer>(redisConnection);
            services.AddSingleton<ICacheHelper, RedisCacheHelper>();

            return services;
        }

        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .Configure<GeoIpSettings>(option =>
                {
                    configuration.GetSection("GeoIpSettings").Bind(option);
                })
                .AddSingleton<ILogger<GeoIpService>, Logger<GeoIpService>>()
                .AddSingleton<GeoIpService>()
                .AddSingleton<GeoIpSettings>()
                .AddSingleton<ConfigService>()
                .AddSingleton<AllowIpBLL>()
                .AddSingleton<BlockIpBLL>()
                .AddSingleton<IFile, FileLoader>()
                .AddSingleton<IIpCountryDataLayers, IpCountryDataLayers>()
                .AddSingleton<IMemberBlockIpDataLayers, MemberBlockIpDataLayers>()
                .AddSingleton<IMemberAllowIpDataLayers, MemberAllowIpDataLayers>();


            services.AddOptionListByIpTableDB<IPCountry>("Service_IPCode_Get");

            //處理SiteConfig
            services.AddOptionListByReportDb<ServerInfo>("ServerInfoList_Get");
            services.AddOptionListByReportDb<ConfigObject>("SiteConfig_Get", new { ConfigGroup = "GeoIp" });
            services.AddSiteConfig();
            services.AddSingleton<ConfigBLL>();
            return services;
        }

        public static IServiceCollection AddSiteConfig(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            // 準備ServerInfo
            var serverInfoList = new List<ServerInfo>() { new ServerInfo() { SiteId = 0, SiteName = "default" } };
            serverInfoList.AddRange(provider.GetRequiredService<IEnumerable<ServerInfo>>());

            // 準備Config
            var siteConfigGroup = provider.GetRequiredService<IEnumerable<ConfigObject>>().GroupBy(i => i.SiteId);

            foreach (var serverInfo in serverInfoList)
            {
                //設定SiteConfig
                var siteConfig = siteConfigGroup.FirstOrDefault(i => i.Key == serverInfo.SiteId) ?? siteConfigGroup.First(i => i.Key == 0);
                var config = siteConfig.ToDictionary(i => i.ConfigKey, i => i.ConfigValue);
                services.AddSingleton(new SiteConfigObject(serverInfo, config));
            }

            return services;
        }
        public static IServiceCollection AddOptionListByIpTableDB<T>(this IServiceCollection services, string sql, object parameters = null)
        {
            var provider = services.BuildServiceProvider();
            var configService = provider.GetRequiredService<ConfigService>();
            var option = configService.Query<T>(sql, parameters);
            services.AddSingleton(typeof(IEnumerable<T>), p => option);
            return services;
        }

        public static IServiceCollection AddOptionListByReportDb<T>(this IServiceCollection services, string sql, object parameters = null)
        {
            var provider = services.BuildServiceProvider();
            var configService = provider.GetRequiredService<ConfigService>();
            var option = configService.QueryByReportDb<T>(sql, parameters);
            services.AddSingleton(typeof(IEnumerable<T>), p => option);
            return services;
        }
    }
}