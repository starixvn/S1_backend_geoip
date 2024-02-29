using System.Collections.Generic;

using GeoIP.Core.Models;

namespace GeoIP.Core
{
    public interface IFile
    {
        List<IPRange> Read(string file);
    }
}