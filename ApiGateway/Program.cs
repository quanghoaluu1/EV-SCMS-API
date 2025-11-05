using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text.RegularExpressions;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);
// var environment = builder.Environment.EnvironmentName;
// builder.Configuration.AddJsonFile($"ocelot.{environment}.json", optional: true, reloadOnChange: true);
builder.Configuration.AddJsonFile("ocelot.json", optional: true, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

// //Bỏ comment cái này để xài localhost và comment cái trên lại
// builder.Configuration.AddJsonFile("ocelot.processed.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();
// builder.Services.AddOcelot();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.Urls.Add("https://0.0.0.0:7088"); // HTTPS only in Development
}
app.UseCors("CorsPolicy");
await app.UseOcelot();

app.UseAuthorization();

app.MapControllers();

app.Run();