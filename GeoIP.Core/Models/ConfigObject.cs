using System;
using System.Collections.Generic;
using System.Text;

namespace GeoIP.Core.Models
{
    public class ConfigObject
    {
        public int SiteId { get; set; }
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
    }
}
