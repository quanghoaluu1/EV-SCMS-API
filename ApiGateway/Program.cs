using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text.RegularExpressions;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Env.Load();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Read ocelot.json and replace placeholders with environment variables
var ocelotConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "ocelot.json");
var ocelotConfig = File.ReadAllText(ocelotConfigPath);  

// Replace all #{VAR_NAME} placeholders with actual environment variables
ocelotConfig = Regex.Replace(ocelotConfig, @"#\{([^}]+)\}", match =>
{
    var envVarName = match.Groups[1].Value;
    var envVarValue = Environment.GetEnvironmentVariable(envVarName);
    
    if (string.IsNullOrEmpty(envVarValue))
    {
        Console.WriteLine($"[WARNING] Environment variable {envVarName} is not set!");
        return match.Value; // Keep placeholder if env var not found
    }
    
    Console.WriteLine($"[DEBUG] Replaced {envVarName} = {envVarValue}");
    return envVarValue;
});

// Write the processed config to a temporary file
var tempOcelotPath = Path.Combine(Directory.GetCurrentDirectory(), "ocelot.processed.json");
File.WriteAllText(tempOcelotPath, ocelotConfig);

// Load the processed configuration
builder.Configuration.AddJsonFile("ocelot.processed.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();
builder.Services.AddOcelot();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Urls.Add("https://0.0.0.0:7088");
app.UseCors("AllowAll");
await app.UseOcelot();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();