using System;
using System.Linq;
using System.Threading.Tasks;
using GeoIP.Core;
using GeoIP.Core.Models;
using GeoIP.WebApi.Extension;
using GeoIP.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace GeoIP.WebApi.Controllers
{
    [Route("GeoIp/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CheckIpController : ControllerBase
    {
        private readonly GeoIpService _service;
        private readonly ILogger<CheckIpController> _logger;
        private readonly GeoIpSettings _settings;
        
        public CheckIpController(GeoIpService service, IOptions<GeoIpSettings> settings,
            ILogger<CheckIpController> logger)
        {
            _service = service;
            _logger = logger;
            _settings = settings.Value;
            _service.ApplyForbidden(new ForbiddenCountries(settings.Value.ForbiddenCountries));
            _service.ApplyAllowedAreas(_service.LoadCsv(settings.Value.WhitelistCsvFile));
        }
        
        [HttpGet("{ip}")]
        public async Task<ActionResult<CountryModel>> Get(string ip)
        {
            var country = await _service.Check(ip);
            var countryModel = new CountryModel
            {
                Code = country.Code,
                Forbidden = country.Forbidden,
                Message = country.Message
            };

            _logger.LogInformation($"IPAddress: [{ip}] CountryCode: [{country.Code}] Forbidden is {country.Forbidden}");
            
            return Ok(countryModel);
        }
        
        [HttpGet("ClientIp")]
        public async Task<ActionResult<IPCountryModel>> GetAndCheckClientIP()
        {
            var ip = IPTool.GetClientIPAddress(Request);
            var country = await _service.Check(ip);
            var ipCountryModel = new IPCountryModel
            {
                Code = country.Code,
                Forbidden = country.Forbidden,
                Message = country.Message,
                RemoteIpAddress= ip
            };

            _logger.LogInformation($"IPAddress: [{ip}] CountryCode: [{country.Code}] Forbidden is {country.Forbidden}");

            return Ok(ipCountryModel);
        }
    }
}