using GeoIP.WebApi.Extension;
using GeoIP.WebApi.Extension.Middlewares;
using GeoIP.WebApi.Models;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddRemoteJsonConfig(builder.Configuration.GetSection("ConfigService").Get<string[]>());

builder.Host.UseSerilog((context, configuration) =>
{
    var excludesMessage = builder.Configuration.GetSection("ExcludesMessage").Get<string[]>();
    var excludesApiEndpoints = builder.Configuration.GetSection("ExcludesApiEndpoint").Get<string[]>();
    
    configuration
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .Filter.ByExcluding(c =>
            c.Properties.Any(p =>
                excludesApiEndpoints.Any(item => p.Value.ToString().Contains(item))
                || excludesMessage.Any(item => c.MessageTemplate.ToString().Contains(item))))
        .Enrich.WithExceptionDetails();
});

builder.Services
    .AddDatabaseConnection(builder.Configuration)
    .RegisterServices(builder.Configuration)
    .AddControllers();

var app = builder.Build();
app.UseRouting();
app.UseRequestLogMiddleware();
app.UseResponseLogMiddleware();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/health", async context =>
    {
        await context.Response.WriteAsync($"OK");
    });
    endpoints.MapControllers();
});

app.Run();
