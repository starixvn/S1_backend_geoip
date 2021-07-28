using System;
using System.Linq;
using System.Threading.Tasks;
using GeoIP.Models;
using GeoIP.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace GeoIP.WebApi.Controllers
{
    [Route("api/[controller]")]
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
        
        [HttpGet("clientip")]
        public async Task<ActionResult<IPCountryModel>> GetAndCheckClientIP()
        {
            var ip = GetClientIPAddress();
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
        private string GetClientIPAddress()
        {
            var remoteIp = GetHeaderValueAs<string>("X-Forwarded-For");
            if (!string.IsNullOrEmpty(remoteIp) && remoteIp.Contains(","))
            {
                return remoteIp.Split(',').First().Trim();
            }

            if (string.IsNullOrEmpty(remoteIp))
            {
                remoteIp = GetHeaderValueAs<string>("REMOTE_ADDR");
            }

            if (string.IsNullOrEmpty(remoteIp))
            {
                remoteIp = GetHeaderValueAs<string>("REMOTE_HOST");
            }

            if (string.IsNullOrEmpty(remoteIp) && Request?.HttpContext?.Connection?.RemoteIpAddress != null)
            {
                remoteIp = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                remoteIp = remoteIp == "0.0.0.1" ? "127.0.0.1" : remoteIp;
            }
            return remoteIp ;
        }
        private T GetHeaderValueAs<T>(string headerName)
        {
            if (Request?.Headers?.TryGetValue(headerName, out StringValues values) ?? false)
            {
                string rawValues = values.ToString(); 

                if (!string.IsNullOrWhiteSpace(rawValues))
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
            return default;
        }
    }
}