using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
        private ForbiddenCountries _forbiddenCountries = new ForbiddenCountries(new[] {""});
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
                var ipCountry = _ipCode.ToList().Find(x => x.IpFrom < uip && x.IpTo > uip);
                //var ipCountry = await _ipCountry.GetIpCountry(uip);
                if (ipCountry != null)
                {
                    return new Country()
                    {
                        Code = ipCountry.Code,
                        Forbidden = _blockedIp.Exists(ipAddress) || 
                                    (_forbiddenCountries.Exists(ipCountry.Code) && !_allowedIp.Exists(ipAddress)),
                    };
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