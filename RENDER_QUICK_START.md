# 🚀 Render.com Quick Start Guide

## Service Endpoints (After Deployment)

| Service | Dockerfile Path | Port | Example URL |
|---------|----------------|------|-------------|
| **API Gateway** | `ApiGateway/Dockerfile` | 8080 | `https://ev-scms-api-gateway.onrender.com` |
| Auth | `Services/Auth/Dockerfile` | 8080 | `https://ev-scms-auth-api.onrender.com` |
| Booking | `Services/Booking/Dockerfile` | 8080 | `https://ev-scms-booking-api.onrender.com` |
| Customer | `Services/Customer/Dockerfile` | 8080 | `https://ev-scms-customer-api.onrender.com` |
| Inventory | `Services/Inventory/Dockerfile` | 8080 | `https://ev-scms-inventory-api.onrender.com` |
| Maintenance | `Services/Maintenance/Dockerfile` | 8080 | `https://ev-scms-maintenance-api.onrender.com` |
| Notification | `Services/Notification/Dockerfile` | 8080 | `https://ev-scms-notification-api.onrender.com` |
| Payment | `Services/Payment/Dockerfile` | 8080 | `https://ev-scms-payment-api.onrender.com` |
| Report | `Services/Report/Dockerfile` | 8080 | `https://ev-scms-report-api.onrender.com` |
| Staff | `Services/Staff/Dockerfile` | 8080 | `https://ev-scms-staff-api.onrender.com` |
| Vehicle | `Services/Vehicle/Dockerfile` | 8080 | `https://ev-scms-vehicle-api.onrender.com` |

## 📋 Deploy in 5 Steps

### 1. Prepare Repository
```bash
git add .
git commit -m "Add Dockerfiles for deployment"
git push origin main
```

### 2. Create Service on Render
- Dashboard → New + → Web Service
- Connect your GitHub/GitLab repository

### 3. Configure Build Settings
```
Name:              ev-scms-[service]-api
Region:            Oregon (or closest to users)
Branch:            main
Root Directory:    (leave empty)
Runtime:           Docker
Dockerfile Path:   Services/[ServiceName]/Dockerfile
```

### 4. Add Environment Variables
Required for all services:
```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=[your-database-url]
```

Service-specific (Auth):
```bash
JwtSettings__SecretKey=[generate-secure-random-string]
JwtSettings__Issuer=ev-scms-auth
JwtSettings__Audience=ev-scms-api
JwtSettings__ExpirationMinutes=60
```

Service-specific (Booking, Notification):
```bash
RabbitMQ__Host=[rabbitmq-host]
RabbitMQ__Port=5672
RabbitMQ__Username=[username]
RabbitMQ__Password=[password]
```

Service-specific (Payment):
```bash
PaymentGateway__ApiKey=[your-payment-gateway-api-key]
PaymentGateway__SecretKey=[your-payment-gateway-secret]
```

### 5. Deploy
Click **"Create Web Service"** - Render will automatically:
- Clone your repository
- Build Docker image
- Deploy container
- Provide HTTPS URL

## 🧪 Local Testing

### Test Individual Service
```bash
# Build
docker build -f Services/Auth/Dockerfile -t ev-scms-auth:latest .

# Run
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="your-conn-string" \
  ev-scms-auth:latest

# Test
curl http://localhost:8080/health
```

### Test All Services
```bash
# Start all services with databases
docker-compose up -d

# View logs
docker-compose logs -f auth-api

# Stop all
docker-compose down
```

## 🔑 Generate Secure Secrets

### JWT Secret (32+ characters)
```bash
openssl rand -base64 32
```

### Connection String Format
```
Server=[host];Database=[dbname];User Id=[user];Password=[pass];TrustServerCertificate=True
```

## 📊 Service Dependencies

```
┌─────────────┐
│ Api Gateway │ (Ocelot)
└──────┬──────┘
       │
┌──────┴──────────────────────────────────┐
│                                          │
├── Auth Service      ──→ SQL Server       │
├── Booking Service   ──→ SQL + RabbitMQ   │
├── Customer Service  ──→ SQL Server       │
├── Inventory Service ──→ SQL Server       │
├── Maintenance Svc   ──→ SQL Server       │
├── Notification Svc  ──→ SQL + RabbitMQ   │
├── Payment Service   ──→ SQL + Gateway    │
├── Report Service    ──→ SQL Server       │
├── Staff Service     ──→ SQL Server       │
└── Vehicle Service   ──→ SQL Server       │
```

## 💡 Pro Tips

### 1. Use Blueprint (render.yaml)
Deploy all services at once:
```bash
# Push render.yaml to repository
git add render.yaml
git commit -m "Add Render blueprint"
git push

# In Render: New → Blueprint → Select repo
```

### 2. Environment Groups
Create shared environment variables:
- Dashboard → Environment Groups
- Add common vars (e.g., `ASPNETCORE_ENVIRONMENT`)
- Link to multiple services

### 3. Health Checks
Add to your `Program.cs`:
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Map health endpoint
app.MapHealthChecks("/health");

app.Run();
```

### 4. Auto-Deploy
- Settings → Auto-Deploy
- Enable for `main` branch
- Every push triggers deployment

### 5. Scale Horizontally
- Dashboard → Service → Settings
- Increase instance count
- Render handles load balancing

## 🆘 Troubleshooting

### Build Fails
```bash
# Test locally first
docker build -f Services/Auth/Dockerfile -t test .

# Check Render logs
# Dashboard → Service → Logs
```

### Service Won't Start
1. Check environment variables are set
2. Verify database connection string
3. Check Render logs for errors
4. Ensure port 8080 is configured

### Database Connection Issues
1. Whitelist Render IPs in database firewall
2. Use correct connection string format
3. Test connection from Render shell
4. Check SSL/TLS requirements

### "Project not found" Error
- Ensure build context is repo root
- Root Directory should be empty
- Dockerfile path should be `Services/[Name]/Dockerfile`

## 📈 Monitoring

Access via Dashboard:
- **Metrics**: CPU, Memory, Request rate
- **Logs**: Real-time streaming logs
- **Events**: Deployments, restarts
- **Shell**: Access container shell

## 💰 Cost Estimates (as of 2024)

| Plan | Price | Resources | Best For |
|------|-------|-----------|----------|
| Free | $0 | 512MB RAM, 0.1 CPU | Testing |
| Starter | $7/mo | 512MB RAM, 0.5 CPU | Development |
| Standard | $25/mo | 2GB RAM, 1 CPU | Production |
| Pro | $85/mo | 4GB RAM, 2 CPU | High traffic |

**10 services on Starter plan**: ~$70/month

## ✅ Pre-Deployment Checklist

- [ ] All Dockerfiles created and tested locally
- [ ] `.dockerignore` file in repository root
- [ ] Database hosted and accessible
- [ ] Environment variables documented
- [ ] Health check endpoints implemented
- [ ] Secrets generated and secure
- [ ] CORS configured for your domains
- [ ] Logging configured
- [ ] Repository pushed to GitHub/GitLab
- [ ] Render account created

## 🔗 Useful Links

- **Render Dashboard**: https://dashboard.render.com
- **Render Docs**: https://render.com/docs
- **Docker Docs**: https://docs.docker.com
- **ASP.NET Core**: https://docs.microsoft.com/aspnet/core

## 📞 Support

- Render Status: https://status.render.com
- Render Community: https://community.render.com
- Documentation: See `DOCKER_DEPLOYMENT_GUIDE.md`

---

**Ready to deploy?** Start with one service (Auth), test it, then deploy the rest! 🚀

