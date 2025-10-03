var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddGrpcClient<Auth.Grpc.AuthGrpc.AuthGrpcClient>(o =>
{
    o.Address = new Uri("https://localhost:5001"); // Auth service address
});

var app = builder.Build();
app.MapReverseProxy();
app.Run();