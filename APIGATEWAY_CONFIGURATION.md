# 🚪 API Gateway Configuration Guide

## Overview

The API Gateway uses **Ocelot** to route requests to the appropriate microservices. It serves as the single entry point for all client applications.

## 🏗️ Architecture

```
┌──────────────────┐
│  Client Apps     │
│  (Web, Mobile)   │
└────────┬─────────┘
         │
         ▼
┌─────────────────────┐
│   API Gateway       │  Port: 8080
│   (Ocelot)          │  Dockerfile: ApiGateway/Dockerfile
└──────┬──────────────┘
       │
       ├──────────────────────────────────────┐
       │                                      │
       ├─► Auth Service         (8081)       │
       ├─► Booking Service      (8082)       │
       ├─► Customer Service     (8083)       │
       ├─► Inventory Service    (8084)       │
       ├─► Maintenance Service  (8085)       │
       ├─► Notification Service (8086)       │
       ├─► Payment Service      (8087)       │
       ├─► Report Service       (8088)       │
       ├─► Staff Service        (8089)       │
       └─► Vehicle Service      (8090)       │
```

## 📝 Ocelot Configuration

The API Gateway routing is configured in `ApiGateway/ocelot.json`. Here's how to configure it:

### Example Configuration Structure

```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/auth/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "auth-api",
          "Port": 8080
        }
      ],
      "UpstreamPathTemplate": "/api/auth/{everything}",
      "UpstreamHttpMethod": [ "Get", "Post", "Put", "Delete" ]
    },
    {
      "DownstreamPathTemplate": "/api/booking/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "booking-api",
          "Port": 8080
        }
      ],
      "UpstreamPathTemplate": "/api/booking/{everything}",
      "UpstreamHttpMethod": [ "Get", "Post", "Put", "Delete" ]
    }
    // Add more routes for other services...
  ],
  "GlobalConfiguration": {
    "BaseUrl": "http://localhost:8080"
  }
}
```

## 🔧 Local Development Configuration

For local development with Docker Compose, update `ocelot.json`:

```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/auth/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "auth-api",
          "Port": 8080
        }
      ],
      "UpstreamPathTemplate": "/auth/{everything}",
      "UpstreamHttpMethod": [ "Get", "Post", "Put", "Delete", "Patch" ]
    }
  ],
  "GlobalConfiguration": {
    "BaseUrl": "http://localhost:8080"
  }
}
```

**Note**: Service names in `DownstreamHostAndPorts` should match the service names in `docker-compose.yml`.

## ☁️ Production Configuration (Render.com)

For production on Render, update `ocelot.json` with actual service URLs:

```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/auth/{everything}",
      "DownstreamScheme": "https",
      "DownstreamHostAndPorts": [
        {
          "Host": "ev-scms-auth-api.onrender.com",
          "Port": 443
        }
      ],
      "UpstreamPathTemplate": "/auth/{everything}",
      "UpstreamHttpMethod": [ "Get", "Post", "Put", "Delete", "Patch" ]
    },
    {
      "DownstreamPathTemplate": "/api/booking/{everything}",
      "DownstreamScheme": "https",
      "DownstreamHostAndPorts": [
        {
          "Host": "ev-scms-booking-api.onrender.com",
          "Port": 443
        }
      ],
      "UpstreamPathTemplate": "/booking/{everything}",
      "UpstreamHttpMethod": [ "Get", "Post", "Put", "Delete", "Patch" ]
    }
    // Add routes for all other services...
  ],
  "GlobalConfiguration": {
    "BaseUrl": "https://ev-scms-api-gateway.onrender.com"
  }
}
```

## 🔐 Adding Authentication

To secure routes with JWT authentication:

```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/booking/{everything}",
      "DownstreamScheme": "https",
      "DownstreamHostAndPorts": [
        {
          "Host": "ev-scms-booking-api.onrender.com",
          "Port": 443
        }
      ],
      "UpstreamPathTemplate": "/booking/{everything}",
      "UpstreamHttpMethod": [ "Get", "Post", "Put", "Delete" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      }
    }
  ]
}
```

Then update `Program.cs`:

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Load Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// Add Ocelot
builder.Services.AddOcelot();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use Authentication
app.UseAuthentication();
app.UseAuthorization();

// Use Ocelot middleware
await app.UseOcelot();

app.MapControllers();

app.Run();
```

## 📦 Deploying the API Gateway

### Local Testing

```bash
# Build the Docker image
docker build -f ApiGateway/Dockerfile -t ev-scms-api-gateway .

# Run the container
docker run -p 8080:8080 ev-scms-api-gateway

# Test the gateway
curl http://localhost:8080
```

### With Docker Compose

```bash
# Start all services including the gateway
docker-compose up -d

# The gateway will be available at http://localhost:8080
# Routes example:
# - http://localhost:8080/auth/login
# - http://localhost:8080/booking/list
# - http://localhost:8080/customer/profile
```

### Deploy to Render.com

1. **Using Blueprint** (`render.yaml`):
   - Push your code with `render.yaml`
   - In Render Dashboard: New → Blueprint
   - Select your repository
   - Gateway deploys automatically

2. **Manual Deployment**:
   - Dashboard → New Web Service
   - Connect repository
   - **Name**: `ev-scms-api-gateway`
   - **Dockerfile Path**: `ApiGateway/Dockerfile`
   - Deploy!

## 🛠️ Advanced Features

### Rate Limiting

Add to route configuration:

```json
{
  "RateLimitOptions": {
    "ClientWhitelist": [],
    "EnableRateLimiting": true,
    "Period": "1s",
    "PeriodTimespan": 1,
    "Limit": 10
  }
}
```

### Load Balancing

Multiple downstream instances:

```json
{
  "DownstreamHostAndPorts": [
    { "Host": "auth-api-1", "Port": 8080 },
    { "Host": "auth-api-2", "Port": 8080 }
  ],
  "LoadBalancerOptions": {
    "Type": "RoundRobin"
  }
}
```

### Request Aggregation

Combine multiple service calls:

```json
{
  "Aggregates": [
    {
      "RouteKeys": [
        "user-details",
        "booking-history"
      ],
      "UpstreamPathTemplate": "/user/complete-profile/{id}"
    }
  ]
}
```

### CORS Configuration

In `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// After var app = builder.Build();
app.UseCors("AllowAll");
```

## 🧪 Testing the Gateway

### Test Basic Routing

```bash
# Health check
curl http://localhost:8080

# Auth service through gateway
curl -X POST http://localhost:8080/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"password123"}'

# Booking service through gateway
curl -X GET http://localhost:8080/booking/list \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Test with .http file

Create `ApiGateway/test-gateway.http`:

```http
### Login through gateway
POST http://localhost:8080/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}

### Get bookings through gateway
GET http://localhost:8080/booking/list
Authorization: Bearer {{token}}

### Create customer through gateway
POST http://localhost:8080/customer/create
Content-Type: application/json
Authorization: Bearer {{token}}

{
  "name": "John Doe",
  "email": "john@example.com"
}
```

## 🔍 Monitoring & Logging

Ocelot logs routing information. To enable detailed logging, update `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Ocelot": "Debug"
    }
  }
}
```

## 📊 Performance Tips

1. **Enable Response Caching**:
   ```json
   {
     "FileCacheOptions": {
       "TtlSeconds": 20,
       "Region": "auth"
     }
   }
   ```

2. **Use Quality of Service**:
   ```json
   {
     "QoSOptions": {
       "ExceptionsAllowedBeforeBreaking": 3,
       "DurationOfBreak": 1000,
       "TimeoutValue": 5000
     }
   }
   ```

3. **Enable HTTP/2** in production for better performance

## 🚨 Troubleshooting

### Gateway can't reach services

**Local Development**:
- Ensure all services are running: `docker-compose ps`
- Check service names match `docker-compose.yml`
- Verify network: `docker network inspect ev-scms-network`

**Production (Render)**:
- Update `ocelot.json` with actual Render URLs
- Use HTTPS scheme and port 443
- Check service status on Render Dashboard

### 404 Not Found

- Verify route configuration in `ocelot.json`
- Check `UpstreamPathTemplate` matches your request
- Ensure `DownstreamPathTemplate` is correct

### Authentication not working

- Verify JWT configuration matches Auth service
- Check token is being passed in headers
- Ensure `AuthenticationOptions` is configured

## 📚 Resources

- [Ocelot Documentation](https://ocelot.readthedocs.io/)
- [API Gateway Pattern](https://microservices.io/patterns/apigateway.html)
- [JWT Authentication](https://jwt.io/)

## ✅ Checklist

Before deploying to production:

- [ ] All routes configured in `ocelot.json`
- [ ] Service URLs updated for production
- [ ] HTTPS enabled for downstream services
- [ ] Authentication configured
- [ ] CORS policy set correctly
- [ ] Rate limiting configured
- [ ] Logging level appropriate
- [ ] Health checks working
- [ ] Load tested
- [ ] Monitoring enabled

---

**Your API Gateway is now ready!** Use it as the single entry point for all client applications. 🎉

