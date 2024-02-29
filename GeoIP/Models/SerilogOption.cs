using Serilog.Events;

namespace GeoIP.WebApi.Models
{
    public class SerilogOption
    {
        /// <summary>
        ///     Output type name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Minimum log level for record
        /// </summary>
        public LogMinimumLevel MinimumLevel { get; set; }

        /// <summary>
        ///     log root path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     log file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///     File max size
        /// </summary>
        public int FileSizeLimitBytes { get; set; }

        /// <summary>
        ///     是否啟用單檔大小限制
        /// </summary>
        public bool RollOnFileSizeLimit { get; set; }

        /// <summary>
        ///     單日檔案數上限(null為不限)
        /// </summary>
        public int? RetainedFileCountLimit { get; set; }
    }

    public class LogMinimumLevel
    {
        public LogEventLevel Default { get; set; }

        public List<LogOverwriteDetail> Overwrite { get; set; }
    }

    public class LogOverwriteDetail
    {
        public string Name { get; set; }

        public LogEventLevel Level { get; set; }
    }
}