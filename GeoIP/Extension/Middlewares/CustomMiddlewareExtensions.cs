namespace GeoIP.WebApi.Extension.Middlewares
{
    public static class CustomMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLogMiddleware>();
        }

        public static IApplicationBuilder UseResponseLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseLogMiddleware>();
        }
    }
}