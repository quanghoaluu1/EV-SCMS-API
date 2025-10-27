# Docker Deployment Guide for Render.com

This guide explains how to deploy the EV-SCMS microservices to Render.com using the provided Dockerfiles.

## 📁 Project Structure

Each microservice follows Clean Architecture with 4 layers:
```
Services/
├── Auth/
│   ├── Dockerfile
│   ├── Auth.Api/
│   ├── Auth.Application/
│   ├── Auth.Domain/
│   └── Auth.Infrastructure/
├── Booking/
│   ├── Dockerfile
│   └── ...
└── [Other Services]/
```

## 🐳 Dockerfile Features

Each Dockerfile includes:

### ✅ Multi-stage Build
- **Stage 1 (Build)**: Uses `mcr.microsoft.com/dotnet/sdk:8.0` for compilation
- **Stage 2 (Runtime)**: Uses `mcr.microsoft.com/dotnet/aspnet:8.0` for minimal production image

### ✅ Security
- Non-root user (`appuser` with UID 1001)
- Production environment by default
- Minimal attack surface

### ✅ Performance
- Optimized layer caching (dependencies restored first)
- Only necessary files copied
- Health checks included

### ✅ Configuration
- Port: `8080` (as required by Render)
- Environment: `Production`
- ASPNETCORE_URLS: `http://+:8080`

## 🚀 Deploying to Render.com

### Prerequisites
1. A Render.com account
2. Your repository pushed to GitHub/GitLab

### Deployment Steps

#### For Each Microservice:

1. **Create New Web Service**
   - Go to Render Dashboard
   - Click "New +" → "Web Service"
   - Connect your repository

2. **Configure the Service**
   ```
   Name: ev-scms-auth-api (example)
   Region: Choose nearest to your users
   Branch: main
   Root Directory: Leave empty (build context is repo root)
   Runtime: Docker
   Dockerfile Path: Services/Auth/Dockerfile
   ```

3. **Build Command**
   - Leave empty (Docker handles the build)

4. **Instance Type**
   - Choose based on your needs (Free/Starter/Standard)

5. **Environment Variables**
   Add required environment variables:
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=<your-db-connection-string>
   JwtSettings__SecretKey=<your-jwt-secret>
   JwtSettings__Issuer=<your-issuer>
   JwtSettings__Audience=<your-audience>
   ```

6. **Advanced Settings**
   ```
   Port: 8080 (auto-detected)
   Health Check Path: /health (if you have health endpoints)
   ```

7. **Deploy**
   - Click "Create Web Service"
   - Render will automatically build and deploy

### Services to Deploy

Deploy each of these services separately:

| Service | Dockerfile Path | Recommended Name |
|---------|----------------|------------------|
| **API Gateway** | `ApiGateway/Dockerfile` | `ev-scms-api-gateway` |
| Auth | `Services/Auth/Dockerfile` | `ev-scms-auth-api` |
| Booking | `Services/Booking/Dockerfile` | `ev-scms-booking-api` |
| Customer | `Services/Customer/Dockerfile` | `ev-scms-customer-api` |
| Inventory | `Services/Inventory/Dockerfile` | `ev-scms-inventory-api` |
| Maintenance | `Services/Maintenance/Dockerfile` | `ev-scms-maintenance-api` |
| Notification | `Services/Notification/Dockerfile` | `ev-scms-notification-api` |
| Payment | `Services/Payment/Dockerfile` | `ev-scms-payment-api` |
| Report | `Services/Report/Dockerfile` | `ev-scms-report-api` |
| Staff | `Services/Staff/Dockerfile` | `ev-scms-staff-api` |
| Vehicle | `Services/Vehicle/Dockerfile` | `ev-scms-vehicle-api` |

## 🧪 Local Testing

### Test a Dockerfile locally:

```bash
# Build the image (from repository root)
docker build -f Services/Auth/Dockerfile -t ev-scms-auth:latest .

# Run the container
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="your-connection-string" \
  -e JwtSettings__SecretKey="your-secret-key" \
  ev-scms-auth:latest

# Test the endpoint
curl http://localhost:8080/health
```

### Test with Docker Compose:

Create a `docker-compose.yml` in the root:

```yaml
version: '3.8'

services:
  auth-api:
    build:
      context: .
      dockerfile: Services/Auth/Dockerfile
    ports:
      - "8081:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=db;Database=AuthDb;User Id=sa;Password=YourPassword123!

  booking-api:
    build:
      context: .
      dockerfile: Services/Booking/Dockerfile
    ports:
      - "8082:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=db;Database=BookingDb;User Id=sa;Password=YourPassword123!

  # Add other services as needed
```

Run with:
```bash
docker-compose up --build
```

## 📋 Important Notes

### Build Context
⚠️ **Critical**: The build context MUST be the repository root, not the service folder, because:
- Dockerfiles reference shared projects (`GrpcContract`, `Messaging`)
- These shared projects are outside the service folders

### On Render.com:
- Set **Root Directory**: Leave empty or set to `/`
- Set **Dockerfile Path**: `Services/[ServiceName]/Dockerfile`

### Health Checks
The Dockerfiles include health check configuration. To make them work:

1. Add a health check endpoint in your `Program.cs`:
```csharp
app.MapHealthChecks("/health");
```

2. Add the health checks package if not already present:
```bash
dotnet add package Microsoft.AspNetCore.Diagnostics.HealthChecks
```

### Database Connections
- Use Render's PostgreSQL or external database services
- Configure connection strings via environment variables
- Never hardcode credentials in code or Dockerfiles

### Logging
The services will log to stdout/stderr, which Render automatically captures.

## 🔧 Troubleshooting

### Build fails with "project not found"
- Ensure build context is the repository root
- Check that all `.csproj` paths are correct

### Application doesn't start
- Check Render logs for startup errors
- Verify environment variables are set correctly
- Ensure database is accessible from Render

### Port issues
- Render requires port 8080 (configured in Dockerfiles)
- Don't change `ASPNETCORE_URLS` in Render env vars

### Shared projects not found
- Verify `GrpcContract` and `Messaging` projects exist
- Ensure they're included in the repository

## 🔐 Production Checklist

- [ ] All environment variables configured securely
- [ ] Database connection strings use production databases
- [ ] JWT secrets are strong and unique
- [ ] CORS policies configured correctly
- [ ] API versioning implemented
- [ ] Rate limiting configured
- [ ] Logging and monitoring set up
- [ ] Health check endpoints working
- [ ] SSL/TLS certificates configured (Render provides this)
- [ ] Auto-deploy enabled on main branch

## 📊 Monitoring

Render provides:
- CPU and Memory metrics
- Request logs
- Container logs
- Health check status

Consider adding:
- Application Insights / Serilog for structured logging
- Custom metrics endpoints
- Error tracking (Sentry, Raygun, etc.)

## 💰 Cost Optimization

1. **Right-size instances**: Start small, scale as needed
2. **Use free tier**: For development/testing
3. **Suspend unused services**: In Render settings
4. **Optimize Docker images**: Keep them minimal (already optimized)

## 🔄 CI/CD

Render automatically deploys on git push to configured branch.

For more control, add a `render.yaml`:

```yaml
services:
  - type: web
    name: ev-scms-auth-api
    env: docker
    dockerfilePath: Services/Auth/Dockerfile
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
      - key: ConnectionStrings__DefaultConnection
        sync: false # Set in Render dashboard
```

## 📚 Additional Resources

- [Render Docker Documentation](https://render.com/docs/docker)
- [.NET on Docker Best Practices](https://docs.microsoft.com/en-us/dotnet/core/docker/introduction)
- [ASP.NET Core Deployment](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/)

## 🆘 Support

If you encounter issues:
1. Check Render logs
2. Test Docker build locally
3. Verify environment variables
4. Check Render status page
5. Contact Render support

