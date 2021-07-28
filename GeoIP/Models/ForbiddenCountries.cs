using System.Collections.Generic;
using System.Linq;

namespace GeoIP.Models
{
    public class ForbiddenCountries
    {
        public List<string> Countries { get; }

        public ForbiddenCountries(IEnumerable<string> countries)
        {
            Countries = countries.ToList();
        }

        public bool Exists(string countryCode)
        {
            return string.IsNullOrEmpty(countryCode) || Countries.Contains(countryCode);
        }
    }
}