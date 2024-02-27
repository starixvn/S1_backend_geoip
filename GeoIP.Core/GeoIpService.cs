using System.Collections.Generic;
using System.Threading.Tasks;
using GeoIP.Core.DAL;
using GeoIP.Core.Models;
using Microsoft.Extensions.Logging;

namespace GeoIP.Core
{
    public class GeoIpService
    {
        private readonly ILogger<GeoIpService> _logger;
        private readonly IIpCountryDataLayers _ipCountry;
        private ForbiddenCountries _forbiddenCountries = new ForbiddenCountries(new List<string> { "" });
        private readonly BlockedIp _blockedIp;
        private readonly AllowedIp _allowedIp;

        public GeoIpService(ILogger<GeoIpService> logger, BlockedIp blockedIp, AllowedIp allowedIp, IIpCountryDataLayers ipCountry)
        {
            _logger = logger;
            _ipCountry = ipCountry;
            _allowedIp = allowedIp;
            _blockedIp = blockedIp;
        }
        public async Task<Country> Check(string ipAddress)
        {
            var country = new Country();
            try
            {
                var uip = IpConvert.GetIpToLong(ipAddress);
                var ipCountry = await _ipCountry.GetIpCountry(uip);
                if (ipCountry != null)
                {
                    if (_blockedIp.Exists(ipAddress))
                    {
                        _logger.LogInformation($"IPAddress: [{ipAddress}] is in block ip list.");
                        return new Country()
                        {
                            Code = ipCountry.Code,
                            Forbidden = true //block
                        };
                    }
                    if (_allowedIp.Exists(ipAddress))
                    {
                        _logger.LogInformation($"IPAddress: [{ipAddress}] is in allow ip list.");
                        return new Country()
                        {
                            Code = ipCountry.Code,
                            Forbidden = false //access
                        };
                    }
                    if (!_forbiddenCountries.Exists(ipCountry.Code))
                    {
                        return new Country()
                        {
                            Code = ipCountry.Code,
                            Forbidden = false //access
                        };
                    }
                    _logger.LogInformation($"IPAddress: [{ipAddress}] CountryCode: [{country.Code}] is in block country list or can't recognize country code.");
                }
            }
            catch (IPFormatException e)
            {
                country.Message = e.Message;
                _logger.LogError(e, $"The IP: {ipAddress} occured {e.Message}.");
            }

            return country;
        }

        public void ApplyForbidden(ForbiddenCountries forbiddenCountries)
        {
            _forbiddenCountries = forbiddenCountries;
        }
    }
}