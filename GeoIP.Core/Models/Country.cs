namespace GeoIP.Core.Models
{
    public class Country
    {
        public string Code { get; set; }
        public bool Forbidden { get; set; }
        public string Message { get; set; }
        public string RemoteIpAddress { get; set; }
        public Country(string ip, string message)
        {
            RemoteIpAddress = ip;
            Code = string.Empty;
            Forbidden = true;
            Message = $"IPAddress: [{ip}] {message}";
        }
    }
}