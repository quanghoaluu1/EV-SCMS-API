# 🔍 Render Deployment Issues & Fixes

**Date:** November 6, 2025  
**Project:** EV-SCMS-API-PRM  
**Deployment Platform:** Render.com

---

## 🚨 CRITICAL ISSUES

### 1. **HTTPS Redirection on Render (SEVERITY: HIGH)**

**Problem:** All services have `app.UseHttpsRedirection()` in Program.cs which will cause issues on Render.

**Affected Services:**
- ✗ Auth Service
- ✗ Booking Service
- ✗ Chat Service
- ✗ Customer Service
- ✗ Inventory Service
- ✗ Maintenance Service
- ✗ Notification Service
- ✗ Payment Service
- ✗ Report Service
- ✗ Staff Service
- ✗ Vehicle Service

**Why it's a problem:**
- Render terminates HTTPS at the edge/load balancer
- Services run on HTTP internally (port 8080)
- `UseHttpsRedirection()` causes infinite redirect loops
- Services will fail health checks and return 502/504 errors

**Solution:** Remove `app.UseHttpsRedirection();` from all Program.cs files in production, or make it conditional:
```csharp
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
```

**Files to fix:**
- Services/Auth/Auth.Api/Program.cs (line 70)
- Services/Booking/Booking.Api/Program.cs
- Services/Chat/Chat.WebApi/Program.cs (line 183)
- Services/Customer/Customer.Api/Program.cs (line 44)
- Services/Inventory/Inventory.Api/Program.cs
- Services/Maintenance/Maintenance.Api/Program.cs (line 78)
- Services/Notification/Notification.Api/Program.cs
- Services/Payment/Payment.Api/Program.cs (line 49)
- Services/Report/Report.Api/Program.cs (line 47)
- Services/Staff/Staff.Api/Program.cs
- Services/Vehicle/Vehicle.Api/Program.cs (line 49)
- ApiGateway/Program.cs (NOT NEEDED - API Gateway must handle HTTPS at edge)

---

### 2. **Hardcoded Secrets in appsettings.json (SEVERITY: CRITICAL)**

**Problem:** Sensitive credentials are hardcoded in appsettings.json files that will be committed to git and exposed in Docker images.

#### Auth Service (Services/Auth/Auth.Api/appsettings.json)
- ✗ Database connection string with password
- ✗ JWT Key: `452e803c2a92d3167d9f411eca3650200d2cdf039de2aa03c80d12313c9198b2!`
- ✗ Redis password: `GuFpr7hcvDKvX5Vo9goQtLJMmZexbQxm`
- ✗ SendGrid API Key: `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`
- ✗ Localhost URLs for StaffHost and CustomerHost

#### Chat Service (Services/Chat/Chat.WebApi/appsettings.json)
- ✗ MongoDB connection string with credentials
- ✗ Redis password
- ✗ Gemini API Key: `AIzaSyAC3Gwbk-fZucaJdpezbEnywLN5KyAVZjY`
- ✗ JWT Key (same as Auth)
- ✗ Localhost URL for StaffHost

#### Payment Service (Services/Payment/Payment.Api/appsettings.json)
- ✗ Database connection string with password
- ✗ Payos credentials (CLIENT_ID, API_KEY, CHECKSUM_KEY)
- ✗ Localhost URLs for CustomerHost and BookingHost

#### Notification Service (Services/Notification/Notification.Api/appsettings.json)
- ✗ RabbitMQ connection string with password

#### Booking Service (Services/Booking/Booking.Api/appsettings.json)
- ✗ Database connection string with password
- ✗ RabbitMQ connection string with password
- ✗ Localhost URLs for StaffHost, CustomerHost, VehicleHost

#### Customer Service (Services/Customer/Customer.Api/appsettings.json)
- ✗ Database connection string with password
- ✗ Localhost URL for AuthHost

#### Staff Service (Services/Staff/Staff.Api/appsettings.json)
- ✗ Database connection string with password
- ✗ Localhost URL for AuthHost

#### Vehicle Service (Services/Vehicle/Vehicle.Api/appsettings.json)
- ✗ Database connection string with password
- ✗ CustomerHost is already set to Render URL (good!) but should be env var

#### Maintenance Service (Services/Maintenance/Maintenance.Api/appsettings.json)
- ✗ Database connection string with password (stored as "DefaultConnection" not "ConnectionStrings:DefaultConnection")
- ✗ Localhost URLs for all dependent services

#### Inventory Service (Services/Inventory/Inventory.Api/appsettings.json)
- ✗ Database connection string with password (stored as "DefaultConnection" not "ConnectionStrings:DefaultConnection")

**Solution:** Replace all hardcoded values with environment variable placeholders or remove them entirely:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "Jwt": {
    "Key": ""
  }
}
```
Then set these values in Render dashboard environment variables.

---

### 3. **Inconsistent Connection String Keys (SEVERITY: HIGH)**

**Problem:** Some services use different keys for database connection strings:

**Correct format (render.yaml expects):**
- `ConnectionStrings__DefaultConnection`

**Services using WRONG key:**
- ✗ Maintenance Service: Uses `"DefaultConnection"` at root level (line 10)
- ✗ Inventory Service: Uses `"DefaultConnection"` at root level (line 10)

**Correct Services:**
- ✓ Auth Service
- ✓ Booking Service
- ✓ Customer Service
- ✓ Payment Service
- ✓ Staff Service
- ✓ Vehicle Service

**Solution:** Update appsettings.json for Maintenance and Inventory:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  }
}
```
And update their Program.cs to use:
```csharp
builder.Configuration.GetConnectionString("DefaultConnection")
```
instead of:
```csharp
builder.Configuration["DefaultConnection"]
```

---

### 4. **Localhost URLs in appsettings.json (SEVERITY: HIGH)**

**Problem:** Service-to-service communication is configured with localhost URLs which won't work on Render.

**Affected Services:**
- Auth: StaffHost, CustomerHost → localhost
- Booking: StaffHost, CustomerHost, VehicleHost → localhost
- Chat: StaffHost → localhost
- Customer: AuthHost → localhost
- Maintenance: StaffHost, CustomerHost, VehicleHost, BookingHost → localhost
- Payment: CustomerHost, BookingHost → localhost
- Report: BookingHost, MaintenanceHost → localhost
- Staff: AuthHost → localhost

**Expected format on Render:**
```json
{
  "StaffHost": "https://evscms-staff-api.onrender.com",
  "CustomerHost": "https://evscms-customer-api.onrender.com"
}
```

**Solution:** These should be set via environment variables in render.yaml (which is already configured), but appsettings.json should have empty strings or be omitted.

---

### 5. **Health Check Docker Image Issues (SEVERITY: MEDIUM)**

**Problem:** All Dockerfiles use `curl` for health checks, but `curl` is not installed in `mcr.microsoft.com/dotnet/aspnet:8.0` base image.

**Affected Dockerfiles:**
- All service Dockerfiles (Auth, Booking, Chat, Customer, Inventory, Maintenance, Notification, Payment, Report, Staff, Vehicle)
- ApiGateway Dockerfile

**Current health check:**
```dockerfile
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1
```

**Solution Options:**

**Option A:** Install curl in Dockerfile (adds ~5MB to image size):
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks
USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

RUN useradd -m -u 1001 appuser && chown -R appuser:appuser /app
USER appuser
```

**Option B:** Use wget (already available in some base images):
```dockerfile
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1
```

**Option C:** Remove Docker health checks and rely on Render's HTTP health checks (RECOMMENDED):
- Render already monitors the `/health` endpoint via `healthCheckPath: /health` in render.yaml
- Remove HEALTHCHECK from Dockerfiles to reduce complexity

---

### 6. **API Gateway CORS Configuration (SEVERITY: MEDIUM)**

**Problem:** API Gateway CORS is hardcoded to `http://localhost:5173` only.

**File:** ApiGateway/Program.cs (lines 11-20)

**Current:**
```csharp
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
```

**Solution:** Make CORS origins configurable via environment variable:
```csharp
var allowedOrigins = builder.Configuration.GetValue<string>("AllowedOrigins") 
    ?? "http://localhost:5173";

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins.Split(','))
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
```

Then add to render.yaml for API Gateway:
```yaml
- key: AllowedOrigins
  value: https://your-frontend.onrender.com,http://localhost:5173
```

---

### 7. **Chat Service CORS Configuration (SEVERITY: MEDIUM)**

**Problem:** Same as API Gateway - hardcoded to localhost only.

**File:** Services/Chat/Chat.WebApi/Program.cs (lines 83-92)

**Solution:** Same as API Gateway above.

---

### 8. **Render Service Name Mismatch (SEVERITY: LOW)**

**Problem:** render.yaml uses service names with "evscms" prefix but ocelot.json uses without prefix.

**render.yaml service names:**
- `evscms-auth-api`
- `evscms-booking-api`
- etc.

**ocelot.json uses:**
- `evscms-auth-api.onrender.com` ✓ (Correct)

**API_GATEWAY_HOST in render.yaml:**
```yaml
- key: API_GATEWAY_HOST
  value: https://ev-scms-api-gateway.onrender.com
```
But service name is `evscms-api-gateway` (no hyphens between EV and SCMS)

**Actual Render URLs will be:**
- `https://evscms-api-gateway.onrender.com` (no hyphens)
- `https://evscms-auth-api.onrender.com` (no hyphens)

**Solution:** Update render.yaml line 40 and other service references to match actual service names (remove hyphen):
```yaml
- key: API_GATEWAY_HOST
  value: https://evscms-api-gateway.onrender.com
```

---

### 9. **Missing .env File Handling (SEVERITY: LOW)**

**Problem:** ApiGateway/Program.cs imports `DotNetEnv` but doesn't use it.

**File:** ApiGateway/Program.cs (line 4)

```csharp
using DotNetEnv;  // ← Imported but never used
```

**Solution:** Either remove the import or add `.env` loading if needed for local development:
```csharp
if (builder.Environment.IsDevelopment())
{
    DotNetEnv.Env.Load();
}
```

---

### 10. **Gemini API Model Name Mismatch (SEVERITY: LOW)**

**Problem:** Inconsistent model naming between render.yaml and appsettings.json.

**render.yaml (line 176):**
```yaml
- key: GeminiSettings__ModelId
  value: gemini-2.5-flash
```

**appsettings.json (line 20):**
```json
"Model": "gemini-2.5-flash"
```

**Configuration key mismatch:**
- render.yaml uses `GeminiSettings__ModelId`
- appsettings.json uses `GeminiSettings:Model`

**Solution:** Update render.yaml to:
```yaml
- key: GeminiSettings__Model
  value: gemini-2.5-flash
```
OR update appsettings.json and application code to use `ModelId`.

---

## 📋 RECOMMENDATIONS

### Priority 1 (Must Fix Before Deployment)
1. ✓ Remove or conditionalize `UseHttpsRedirection()` in all services
2. ✓ Remove hardcoded secrets from all appsettings.json files
3. ✓ Fix connection string key inconsistency in Maintenance and Inventory services
4. ✓ Remove localhost URLs from appsettings.json

### Priority 2 (Should Fix)
5. ✓ Fix Docker health checks (install curl or use Render's HTTP checks)
6. ✓ Make CORS configurable in API Gateway and Chat service
7. ✓ Verify Render service name consistency

### Priority 3 (Nice to Have)
8. ✓ Clean up unused imports (DotNetEnv)
9. ✓ Fix Gemini model configuration key
10. ✓ Add environment validation on startup

---

## 🔧 AUTOMATED FIX SCRIPT

Would you like me to create an automated script to fix all these issues? I can:
1. Update all Program.cs files to remove HTTPS redirection in production
2. Clean all hardcoded secrets from appsettings.json
3. Fix connection string keys
4. Update Dockerfiles for health checks
5. Make CORS configurable

---

## 📝 DEPLOYMENT CHECKLIST

Before deploying to Render:
- [ ] All secrets removed from appsettings.json
- [ ] All Program.cs files updated (no HTTPS redirection)
- [ ] Connection string keys standardized
- [ ] CORS made configurable
- [ ] Docker health checks fixed or removed
- [ ] Service URLs updated in render.yaml
- [ ] Environment variables set in Render dashboard
- [ ] Test each service health endpoint after deployment
- [ ] Verify service-to-service communication

---

**Generated by:** EV-SCMS Deployment Scanner  
**Status:** ⚠️ Multiple critical issues found - DO NOT DEPLOY without fixes

