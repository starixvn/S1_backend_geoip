namespace GeoIP.WebApi.Models
{
    public class IPCountryModel
    {
        public string Code { get; set; }
        public bool Forbidden { get; set; }
        public string Message { get; set; }
        public string RemoteIpAddress { get; set; }
    }
}