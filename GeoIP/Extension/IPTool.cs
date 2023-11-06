namespace GeoIP.WebApi.Extension
{
    public static class IPTool
    {
        public static string GetClientIPAddress(HttpRequest request)
        {
            var headerNames = new[] { "X-Forwarded-For", "HTTP_X_FORWARDED_FOR", "HTTP_INCAP_CLIENT_IP", "HTTP_TRUE_CLIENT_IP", "HTTP_X_REAL_IP", "REMOTE_ADDR" };

            foreach (var header in headerNames)
            {

                if (request.Headers.ContainsKey(header))
                {
                    string serverVariables = request.Headers[header];
                    var ip = serverVariables.Split(',').FirstOrDefault();
                    if (!string.IsNullOrEmpty(ip))
                    {
                        return ip;
                    }
                }
            }

            return request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}