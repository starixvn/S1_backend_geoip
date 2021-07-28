namespace GeoIP
{
    public class GeoIpSettings
    {
        public string WhitelistCsvFile { get; set; }

        public string[] ForbiddenCountries { get; set; }
    }
}