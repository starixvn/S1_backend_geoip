using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeoIP.Core.DAL;
using GeoIP.Core.Models;
using Microsoft.Extensions.Logging;

namespace GeoIP.Core
{
    public class GeoIpService
    {
        private readonly IFile _file;
        private readonly ILogger<GeoIpService> _logger;
        private readonly IIpCountryDataLayers _ipCountry;
        private ForbiddenCountries _forbiddenCountries = new ForbiddenCountries(new[] {""});
        private AllowedAreas _whiteList = new AllowedAreas(new List<IPRange>());

        public GeoIpService(IFile file, ILogger<GeoIpService> logger, IIpCountryDataLayers ipCountry)
        {
            _file = file;
            _logger = logger;
            _ipCountry = ipCountry;
        }
        public async Task<Country> Check(string ipAddress)
        {
            var country = new Country();
            try
            {
                var uip = _whiteList.GetIpToLong(ipAddress);
                var ipCountry = await _ipCountry.GetIpCountry(uip);
                if (ipCountry != null)
                {
                    return new Country()
                    {
                        Code = ipCountry.Code,
                        Forbidden = !_whiteList.Exists(uip) && _forbiddenCountries.Exists(ipCountry.Code),
                    };
                }
            }
            catch (IPFormatException e)
            {
                country.Message = e.Message;
                _logger.LogWarning(e, $"The IP: {ipAddress} occured {e.Message}.");
            }
            
            return country;
        }
        
        public void ApplyForbidden(ForbiddenCountries forbiddenCountries)
        {
            _forbiddenCountries = forbiddenCountries;
        }

        public void ApplyAllowedAreas(AllowedAreas whiteList)
        {
            _whiteList = whiteList;
        }

        public AllowedAreas LoadCsv(string whiteListFile)
        {
            var ranges = _file.Read(whiteListFile);
            var allowedAreas = new AllowedAreas(ranges);
            return allowedAreas;
        }
    }
}