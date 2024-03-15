using GeoIP.Core;
using GeoIP.Core.Models;
using GeoIP.WebApi.Extension;

using Microsoft.AspNetCore.Mvc;

namespace GeoIP.WebApi.Controllers
{
    [Route("GeoIp/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CheckIpController : ControllerBase
    {
        private readonly GeoIpService _geoIpService;

        public CheckIpController(IServiceProvider provider)
        {
            _geoIpService = provider.GetRequiredService<GeoIpService>();
        }

        [HttpGet("{ip}")]
        public async Task<ActionResult<Country>> Get(string ip)
        {
            var country = await _geoIpService.Check(ip);
            return Ok(country);
        }

        [HttpGet("ClientIp")]
        public async Task<ActionResult<Country>> GetAndCheckClientIP()
        {
            var ip = IPTool.GetClientIPAddress(Request);
            var country = await _geoIpService.Check(ip);
            return Ok(country);
        }
    }
}