
using System.Text;
using Chat.Application.Interfaces;
using Chat.Application.Services;
using Chat.Domain.Settings;
using Chat.Infrastructure.Interfaces;
using Chat.Infrastructure.Repositories;
using Chat.WebApi.Hubs;
using Common.Redis;
using Common.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.Configure<RedisSettings>(
    builder.Configuration.GetSection("RedisSettings"));
builder.Services.Configure<GeminiSettings>(
    builder.Configuration.GetSection("GeminiSettings"));
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
// Redis
builder.Services.AddSingleton<IRedisService, RedisService>();

// Services
builder.Services.AddScoped<IChatService,ChatService>();
builder.Services.AddScoped<IAiChatService, AiChatService>();
builder.Services.AddSingleton<IPresenceService, PresenceService>();
// builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});
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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    
    .AddJwtBearer(options =>
    {
        var jwtSection = builder.Configuration.GetSection("JwtSettings");
        var secretKey = jwtSection["Key"];
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException(
                "JWT SecretKey is missing in appsettings.json. " +
                "Please add JwtSettings:SecretKey configuration.");
        }

        if (string.IsNullOrEmpty(issuer))
        {
            throw new InvalidOperationException(
                "JWT Issuer is missing in appsettings.json. " +
                "Please add JwtSettings:Issuer configuration.");
        }

        if (string.IsNullOrEmpty(audience))
        {
            throw new InvalidOperationException(
                "JWT Audience is missing in appsettings.json. " +
                "Please add JwtSettings:Audience configuration.");
        }


        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        // SignalR authentication - Lấy token từ query string
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                
                if (!string.IsNullOrEmpty(accessToken) && 
                    path.StartsWithSegments("/chathub"))
                {
                    context.Token = accessToken;
                }
                
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                // Add custom header for expired tokens
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                // Log authentication failures
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();
                
                logger.LogWarning(
                    "JWT Authentication failed: {Error} - {ErrorDescription}",
                    context.Error,
                    context.ErrorDescription);
                
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    // Test Redis connection
    try
    {
        var redis = scope.ServiceProvider.GetRequiredService<IRedisService>();
        await redis.SetStringAsync("startup_test", "ok", TimeSpan.FromSeconds(10));
        var value = await redis.GetStringAsync("startup_test");
        
        if (value == "ok")
        {
            logger.LogInformation("✅ Redis connection: SUCCESS");
        }
        else
        {
            logger.LogWarning("⚠️ Redis connection: WARNING - Value mismatch");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Redis connection: FAILED");
    }

    // Verify JWT configuration
    try
    {
        var jwtSection = builder.Configuration.GetSection("JwtSettings");
        var secretKey = jwtSection["Key"];
        
        if (!string.IsNullOrEmpty(secretKey))
        {
            logger.LogInformation("✅ JWT Configuration: SUCCESS");
        }
        else
        {
            logger.LogError("❌ JWT Configuration: FAILED - SecretKey is missing");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ JWT Configuration: FAILED");
    }

    // Test MongoDB connection
    try
    {
        var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
        var testCollection = mongoContext.GetCollection<object>("_startup_test");
        
        logger.LogInformation("✅ MongoDB connection: SUCCESS");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ MongoDB connection: FAILED");
    }
}

Console.WriteLine("Chat Service is running...");
app.Run();