using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeoIP.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GeoIP
{
    public class FileLoader : IFile
    {
        private readonly ILogger _logger;

        public FileLoader(IServiceProvider provider)
        {
            _logger = provider.GetService<ILoggerFactory>().CreateLogger<FileLoader>();
        }
        
        public List<IPRange> Read(string file)
        {
            var ipRangeSets = new HashSet<IPRange>(new IPRangeComparer());
            try
            {
                foreach (var line in File.ReadAllLines(file))
                {
                    if (line.StartsWith("#")) continue;
                    
                    var ipRangeRow = line.Split(',').Select(o => o.Trim('"')).ToArray();
                    var invalidFormat = false;
                        
                    foreach (var ipRangeColumn in ipRangeRow)
                    {
                        invalidFormat = string.IsNullOrWhiteSpace(ipRangeColumn);
                        if (invalidFormat)
                        {
                            break;
                        }
                    }

                    if (invalidFormat)
                    {
                        _logger.LogTrace($"Invalid format in {file}");
                        continue;
                    }

                    var existed = !ipRangeSets.Add(new IPRange()
                    {
                        From = uint.Parse(ipRangeRow[0]), To = uint.Parse(ipRangeRow[1]), Description = ipRangeRow[2]
                    });

                    if (existed)
                    {
                        _logger.LogWarning($"The IP range from {uint.Parse(ipRangeRow[0])} to {uint.Parse(ipRangeRow[1])} was existed.");
                    }
                    
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"CSV file: {file}");
            }

            return ipRangeSets.ToList();
        }
        
        private class IPRangeComparer : IEqualityComparer<IPRange>
        {
            public bool Equals(IPRange x, IPRange y)
            {
                return (x.From == y.From || x.To == y.To);
            }

            public int GetHashCode(IPRange obj)
            {
                return (int)obj.From + (int)obj.To;
            }
        }
    }
}