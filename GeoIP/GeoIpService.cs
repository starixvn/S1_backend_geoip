using System.Collections.Generic;
using System.Threading.Tasks;
using GeoIP.DAL;
using GeoIP.Models;
using Microsoft.Extensions.Logging;

namespace GeoIP
{
    public class GeoIpService
    {
        private readonly IFile _file;
        private ILogger<GeoIpService> _logger;
        private readonly IIpCountryDataLayers _ipCountry;
        private ForbiddenCountries _forbiddenCountries = new ForbiddenCountries(new[] {""});
        private AllowedAreas _whiteList = new AllowedAreas(new List<IPRange>());

        public GeoIpService(IFile file, ILogger<GeoIpService> logger, IIpCountryDataLayers ipCountry)
        {
            _file = file;
            _logger = logger;
            _ipCountry = ipCountry;
        }
        public async Task<Country> Check(string ipAddr)
        {
            var ip = _whiteList.GetIpToLong(ipAddr);
            var ipCountry = await _ipCountry.GetIpCountry(ip);
            if (ipCountry != null)
            {
                return new Country()
                {
                    Code = ipCountry.Code,
                    Forbidden = !_whiteList.Exists(ipAddr) && _forbiddenCountries.Exists(ipCountry.Code),
                };
            }

            return new Country();
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