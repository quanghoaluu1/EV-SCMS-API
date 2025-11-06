using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text.RegularExpressions;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Add Ocelot and SwaggerForOcelot
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSwaggerForOcelot(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline
app.Urls.Add("https://0.0.0.0:7088");
app.UseCors("CorsPolicy");

app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
});

app.UseRouting();

await app.UseOcelot();

app.UseAuthorization();

app.MapControllers();

app.Run();