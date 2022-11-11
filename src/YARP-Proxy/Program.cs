using Microsoft.AspNetCore.Server.Kestrel.Core;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(o => o.Protocols = HttpProtocols.Http1AndHttp2AndHttp3);
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("YarpReverseProxy"))
    .AddTransforms(transforms =>
    {
        transforms.AddRequestTransform(transform =>
        {
            var requestId = Guid.NewGuid().ToString("N");
            transform.ProxyRequest.Headers.Add("X-Request-Id", requestId);
            return ValueTask.CompletedTask;
        });
    });

var app = builder.Build();

app.MapGet("/", () => "Hello YARP!");
app.MapReverseProxy();

app.Run();
