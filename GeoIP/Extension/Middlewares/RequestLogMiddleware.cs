using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;

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

                var requestBodyText = new StreamReader(requestBodyStream).ReadToEndAsync();
                var headers = context.Request.Headers;
                
                LogContext.PushProperty("header", JsonConvert.SerializeObject(headers));
                if (!context.Request.Headers.TryGetValue("X-Forwarded-For", out var ip))
                {
                    ip = context.Connection.RemoteIpAddress.MapToIPv4()
                        .ToString();
                }
                _logger.LogInformation(
                    $"REQUEST METHOD: {context.Request.Method}, REQUEST BODY: {await requestBodyText}, REQUEST URL: {url}, CLIENT_IP: {ip}");

                requestBodyStream.Seek(0, SeekOrigin.Begin);
                context.Request.Body = requestBodyStream;
                context.Items.Add("Body", requestBodyText);

                await _next(context);
                context.Request.Body = originalRequestBody;
            }
        }
    }
}