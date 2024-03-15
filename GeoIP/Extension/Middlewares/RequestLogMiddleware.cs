using System.Text;

using Microsoft.AspNetCore.Http.Extensions;

namespace GeoIP.WebApi.Extension.Middlewares
{
    public class RequestLogMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public RequestLogMiddleware(RequestDelegate next, ILoggerFactory factory)
        {
            _next = next;
            _logger = factory.CreateLogger("RequestLog");
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();
            var url = context.Request.GetDisplayUrl();
            if (url.EndsWith("home.html"))
            {
                await _next(context);
            }
            else
            {
                var requestBodyStream = new MemoryStream();
                var originalRequestBody = context.Request.Body;

                await context.Request.Body.CopyToAsync(requestBodyStream);
                requestBodyStream.Seek(0, SeekOrigin.Begin);

                using var requestBody = new StreamReader(requestBodyStream, Encoding.UTF8, true, 1024, true);

                var codePage = requestBody.CurrentEncoding.CodePage;
                var content = await requestBody.ReadToEndAsync();

                var log = new StringBuilder();
                log.AppendLine($"REQUEST URL: {url}, Method: {context.Request.Method}, CodePage: {codePage}, BODY: {content}");
                _logger.LogInformation(log.ToString());

                requestBodyStream.Seek(0, SeekOrigin.Begin);
                context.Request.Body = requestBodyStream;
                context.Items.Add("Body", content);

                await _next(context);
                context.Request.Body = originalRequestBody;
            }
        }
    }
}