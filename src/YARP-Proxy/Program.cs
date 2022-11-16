using Microsoft.AspNetCore.Server.Kestrel.Core;
using Yarp.ReverseProxy.Transforms;
using YARP_Proxy.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(o => o.Protocols = HttpProtocols.Http1AndHttp2AndHttp3);
    options.ConfigureHttpsDefaults(o => o.AllowAnyClientCertificate());
});

builder.Services.AddStackExchangeRedisCache(redisOptions 
    => redisOptions.Configuration = builder.Configuration.GetConnectionString("Redis"));

builder.Services.AddTransient<RedisCachingMiddleware>();

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

app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.UseMiddleware<RedisCachingMiddleware>();
});

app.Run();
