using Microsoft.Extensions.Caching.Distributed;

namespace YARP_Proxy.Middlewares;

public sealed class RedisCachingMiddleware : IMiddleware
{
    private readonly IDistributedCache _redisCache;

    public RedisCachingMiddleware(IDistributedCache redisCache)
    {
        _redisCache = redisCache;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string? cachedEntry = await _redisCache.GetStringAsync("cachehit");

        if (cachedEntry is not null)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync(cachedEntry);
            return;
        }    

        await next(context);
    }
}
