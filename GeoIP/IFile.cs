using System.Collections.Generic;
using GeoIP.Models;

namespace GeoIP
{
    public interface IFile
    {
        List<IPRange> Read(string file);
    }
}