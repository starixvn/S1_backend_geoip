using GeoIP.Core.DAL;
using GeoIP.Core.Library;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoIP.Core.BLL
{
    public class AllowIpBLL
    {
        private readonly ICacheHelper _redisHelper;
        private readonly ILogger<AllowIpBLL> _logger;
        private readonly IMemberAllowIpDataLayers _memberAllowIpDataLayers;

        public AllowIpBLL(IServiceProvider provider)
        {
            _redisHelper = provider.GetRequiredService<ICacheHelper>();
            _logger = provider.GetRequiredService<ILogger<AllowIpBLL>>();
            _memberAllowIpDataLayers = provider.GetRequiredService<IMemberAllowIpDataLayers>();
        }

        public IEnumerable<string> GetAllowIpListWithCache()
        {
            return _redisHelper.TryGetOrCreate("GeoIpSetting:AllowIpList", TimeSpan.FromMinutes(5),
                () => _memberAllowIpDataLayers.GetAllowIp());
        }
    }
}
