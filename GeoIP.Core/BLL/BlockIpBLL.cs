using GeoIP.Core.DAL;
using GeoIP.Core.Library;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoIP.Core.BLL
{
    public class BlockIpBLL
    {
        private readonly ICacheHelper _redisHelper;
        private readonly ILogger<BlockIpBLL> _logger;
        private readonly IMemberBlockIpDataLayers _memberBlockIpDataLayers;

        public BlockIpBLL(IServiceProvider provider)
        {
            _redisHelper = provider.GetRequiredService<ICacheHelper>();
            _logger = provider.GetRequiredService<ILogger<BlockIpBLL>>();
            _memberBlockIpDataLayers = provider.GetRequiredService<IMemberBlockIpDataLayers>();
        }

        public IEnumerable<string> GetBlockIpListWithCache()
        {
            return _redisHelper.TryGetOrCreate("GeoIpSetting:BlockIpList", TimeSpan.FromMinutes(5),
                () => _memberBlockIpDataLayers.GetBlockIp());
        }
    }
}
