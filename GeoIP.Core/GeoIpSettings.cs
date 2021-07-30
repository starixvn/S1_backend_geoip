namespace GeoIP.Core
{
    public class GeoIpSettings
    {
        public string WhitelistCsvFile { get; set; }

        public string[] ForbiddenCountries { get; set; }
    }
}