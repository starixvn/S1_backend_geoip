namespace GeoIP.WebApi.Models
{
    public class CountryModel
    {
        public string Code { get; set; }
        public bool Forbidden { get; set; }
        public string Message { get; set; }
    }
}