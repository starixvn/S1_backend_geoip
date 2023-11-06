namespace GeoIP.WebApi.Extension.Middlewares
{
    public class ResponseLogMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public ResponseLogMiddleware(RequestDelegate next, ILoggerFactory factory)
        {
            _next = next;
            _logger = factory.CreateLogger("ResponseLog");
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;
            if (path.EndsWith("home.html"))
            {
                await _next(context);
            }
            else
            {
                var bodyStream = context.Response.Body;

                var responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;

                await _next(context);

                responseBodyStream.Seek(0, SeekOrigin.Begin);
                var recordContent = new StreamReader(responseBodyStream).ReadToEnd();
                _logger.LogInformation($"Response: {recordContent}");
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                await responseBodyStream.CopyToAsync(bodyStream);
            }
        }
    }
}