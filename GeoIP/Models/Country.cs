namespace GeoIP.Models
{
    public class Country
    {
        public string Code { get; set; }
        public bool Forbidden { get; set; }
        public string Message { get; set; }

        public Country()
        {
            Code = string.Empty;
            Forbidden = true;
            Message = "Succeed";
        }
    }
}