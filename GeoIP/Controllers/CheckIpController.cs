using GeoIP.Core;
using GeoIP.Core.BLL;
using GeoIP.Core.Models;
using GeoIP.WebApi.Extension;
using GeoIP.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeoIP.WebApi.Controllers
{
    [Route("GeoIp/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CheckIpController : ControllerBase
    {
        private readonly GeoIpService _service;
        private readonly ILogger<CheckIpController> _logger;

        public CheckIpController(GeoIpService service, ConfigBLL configBLL, ILogger<CheckIpController> logger)
        {
            _service = service;
            _logger = logger;
            _service.ApplyForbidden(new ForbiddenCountries(configBLL.GetConfigValue<List<string>>("BlockCountry")));
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
                RemoteIpAddress = ip
            };

            _logger.LogInformation($"IPAddress: [{ip}] CountryCode: [{country.Code}] Forbidden is {country.Forbidden}");

            return Ok(ipCountryModel);
        }
    }
}