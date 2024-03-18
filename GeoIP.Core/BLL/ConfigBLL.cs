using System;
using System.Collections.Generic;
using System.Linq;

using GeoIP.Core.Models;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace GeoIP.Core.BLL
{
    public class ConfigBLL
    {
        private readonly Dictionary<string, string> _defaultSiteConfig;
        private readonly Dictionary<int, SiteConfigObject> _siteIdConfigs;

        public ConfigBLL(IServiceProvider provider)
        {
            var siteConfigList = provider.GetServices<SiteConfigObject>();
            _defaultSiteConfig = siteConfigList.First(i => i.SiteId == 0).Config;

            var siteConfigs = siteConfigList.Where(i => i.SiteId != 0);            
            _siteIdConfigs = siteConfigs.ToDictionary(i => i.SiteId, i => i);
        }

        public TItem GetConfigValue<TItem>(string configKey, int siteId = 0)
        {
            var defaultItem = _defaultSiteConfig.TryGetValue(configKey, out var json)
                ? JsonConvert.DeserializeObject<TItem>(json) : default;

            if (_siteIdConfigs.TryGetValue(siteId, out var siteConfig))
            {
                return siteConfig.Config.TryGetValue(configKey, out var configValue)
                   ? JsonConvert.DeserializeObject<TItem>(configValue) : defaultItem;
            }

            return defaultItem;
        }
    }
}
