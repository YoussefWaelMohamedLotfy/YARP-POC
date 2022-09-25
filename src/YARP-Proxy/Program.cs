var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("YarpReverseProxy"));

var app = builder.Build();

app.MapGet("/", () => "Hello YARP!");
app.MapReverseProxy();

app.Run();
