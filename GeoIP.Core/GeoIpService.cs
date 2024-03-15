using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GeoIP.Core.BLL;
using GeoIP.Core.Library;
using GeoIP.Core.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GeoIP.Core
{
    public class GeoIpService
    {
        private readonly ILogger<GeoIpService> _logger;
        private readonly Dictionary<string, string> _forbiddenCountries;
        private readonly IEnumerable<IPCountry> _IPCountryList;
        private readonly ICacheHelper _cacheHelper;

        public GeoIpService(IServiceProvider provider)
        {
            _logger = provider.GetRequiredService<ILogger<GeoIpService>>();
            _IPCountryList = provider.GetRequiredService<IEnumerable<IPCountry>>();
            var configBLL = provider.GetRequiredService<ConfigBLL>();
            _forbiddenCountries = configBLL.GetConfigValue<List<string>>("BlockCountry").ToDictionary(i => i, i => i);
            _cacheHelper = provider.GetRequiredService<ICacheHelper>();
        }

        public async Task<Country> Check(string ipAddress)
        {
            try
            {
                var uip = IpConvert.GetIpToLong(ipAddress);
                var ipCountry = _IPCountryList.FirstOrDefault(x => x.IpFrom < uip && x.IpTo > uip);

                // ip異常找不到國家回傳block
                if (ipCountry == null)
                {
                    return new Country(ipAddress, "not found in IPCountryList");
                }

                // 檢查黑名單
                if (await _cacheHelper.KeyExistsAsync($"BlockIp:{ipAddress}"))
                {
                    return new Country(ipAddress, "in block ip list.")
                    {
                        Code = ipCountry.Code,
                        Forbidden = true //block
                    };
                }

                // 檢查白名單
                if (await _cacheHelper.KeyExistsAsync($"AllowedIp:{ipAddress}"))
                {
                    return new Country(ipAddress, "in allow ip list.")
                    {
                        Code = ipCountry.Code,
                        Forbidden = false //access
                    };
                }

                // 檢查黑名單國家
                if (string.IsNullOrWhiteSpace(ipCountry.Code) || _forbiddenCountries.TryGetValue(ipCountry.Code.Trim(), out _))
                {
                    return new Country(ipAddress, $"CountryCode: [{ipCountry.Code}] is in block country list or can't recognize country code.")
                    {
                        Code = ipCountry.Code,
                        Forbidden = true //block
                    };
                }

                return new Country(ipAddress, "Succeed")
                {
                    Code = ipCountry.Code,
                    Forbidden = false
                };
            }
            catch (IPFormatException e)
            {
                _logger.LogError(e, e.Message);
                return new Country(ipAddress, e.Message);
            }
        }
    }
}