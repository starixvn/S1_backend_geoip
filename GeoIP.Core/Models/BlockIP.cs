using System.Collections.Generic;
using System.Linq;

namespace GeoIP.Core.Models
{
    public class BlockIP
    {
        public List<string> _blockList { get; set; }
        
        public bool Exists(string ip)
        {
            return _blockList.Contains(ip);
        }
    }
}