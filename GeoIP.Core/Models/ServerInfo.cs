using System.Collections.Generic;

using Newtonsoft.Json;

namespace GeoIP.Core.Models
{
    public class ServerInfo
    {
        public int SiteId { set; get; }
        public string SiteName { set; get; }
        public string host { set; get; }
        public string LangList { set; get; }
        public string CurrencyList { set; get; }
        public string DefLang { set; get; }
        public int DefCurrency { set; get; }
        public IEnumerable<int> ICurrencyList => JsonConvert.DeserializeObject<IEnumerable<int>>(CurrencyList);

        public IEnumerable<string> ILangList => JsonConvert.DeserializeObject<IEnumerable<string>>(LangList);
    }
}
