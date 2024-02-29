using System.Collections.Generic;
using System.Linq;
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
        private readonly IEnumerable<IPCountry> _ipCode;

        public GeoIpService(ILogger<GeoIpService> logger, BlockedIp blockedIp, AllowedIp allowedIp, IIpCountryDataLayers ipCountry, IEnumerable<IPCountry> ipCode = null)
        {
            _logger = logger;
            _ipCountry = ipCountry;
            _allowedIp = allowedIp;
            _blockedIp = blockedIp;
            _ipCode = ipCode;
        }
        public async Task<Country> Check(string ipAddress)
        {
            var country = new Country();
            try
            {
                var uip = IpConvert.GetIpToLong(ipAddress);
                var ipCountry = _ipCode.FirstOrDefault(x => x.IpFrom < uip && x.IpTo > uip);
                //var ipCountry = await _ipCountry.GetIpCountry(uip);
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
                    if (string.IsNullOrWhiteSpace(ipCountry.Code)
                        || _forbiddenCountries.Exists(ipCountry.Code.Trim()))
                    {
                        _logger.LogInformation($"IPAddress: [{ipAddress}] CountryCode: [{ipCountry.Code}] is in block country list or can't recognize country code.");
                        return new Country()
                        {
                            Code = ipCountry.Code,
                            Forbidden = true //block
                        };
                    }
                    country.Forbidden = false;
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