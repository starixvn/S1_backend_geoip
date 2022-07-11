using System;
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
        private readonly IFile _file;
        private readonly ILogger<GeoIpService> _logger;
        private readonly IIpCountryDataLayers _ipCountry;
        private ForbiddenCountries _forbiddenCountries = new ForbiddenCountries(new[] {""});
        private AllowedAreas _whiteList = new AllowedAreas(new List<IPRange>());
        private readonly BlockIP _blockIp;

        public GeoIpService(IFile file, ILogger<GeoIpService> logger, BlockIP blockIp, IIpCountryDataLayers ipCountry, IMemberBlockIpDataLayers memberBlockIp)
        {
            _file = file;
            _logger = logger;
            _ipCountry = ipCountry;
            _blockIp = blockIp;
            _blockIp._blockList = memberBlockIp.GetBlockIp().Result.ToList();
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
                        Forbidden = _blockIp.Exists(ipAddress) || (_forbiddenCountries.Exists(ipCountry.Code) && !_whiteList.Exists(uip)),
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