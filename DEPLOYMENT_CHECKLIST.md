# 🚀 EV-SCMS Deployment Checklist

## ✅ Files Updated (Completed)

- [x] **ApiGateway/Dockerfile** - Updated
- [x] **Services/Auth/Dockerfile** - Updated (Added Common dependency)
- [x] **Services/Booking/Dockerfile** - Updated
- [x] **Services/Chat/Dockerfile** - Created (NEW) ⭐
- [x] **Services/Customer/Dockerfile** - Updated
- [x] **Services/Inventory/Dockerfile** - Updated
- [x] **Services/Maintenance/Dockerfile** - Updated
- [x] **Services/Notification/Dockerfile** - Updated
- [x] **Services/Payment/Dockerfile** - Updated
- [x] **Services/Report/Dockerfile** - Updated
- [x] **Services/Staff/Dockerfile** - Updated
- [x] **Services/Vehicle/Dockerfile** - Updated
- [x] **render.yaml** - Updated (Added Chat service)

---

## 📋 Pre-Deployment Tasks

### External Services Setup

- [ ] **MongoDB Atlas** (for Chat Service)
  - [ ] Create free cluster
  - [ ] Create database user
  - [ ] Whitelist IP addresses (0.0.0.0/0 for all)
  - [ ] Copy connection string
  - [ ] Test connection

- [ ] **Google Gemini API** (for Chat AI)
  - [ ] Visit [Google AI Studio](https://makersuite.google.com/app/apikey)
  - [ ] Create API key
  - [ ] Copy API key

- [ ] **RabbitMQ** (for Notification Service)
  - [ ] Sign up at [CloudAMQP](https://www.cloudamqp.com/)
  - [ ] Create free instance
  - [ ] Copy AMQP URL

- [ ] **PostgreSQL Databases**
  - [ ] Option A: Create Render PostgreSQL (Free, expires in 90 days)
  - [ ] Option B: Create Supabase database (Free forever)
  - [ ] Option C: Use ElephantSQL (Free tier)
  - [ ] Copy connection string(s)

- [ ] **Payos Payment Gateway** (for Payment Service)
  - [ ] Register at [Payos.vn](https://payos.vn)
  - [ ] Get CLIENT_ID
  - [ ] Get API_KEY
  - [ ] Get CHECKSUM_KEY

### Git Repository

- [ ] All changes committed
  ```bash
  git add .
  git commit -m "Updated Dockerfiles and render.yaml for all services"
  ```
- [ ] Pushed to GitHub
  ```bash
  git push origin main
  ```

---

## 🎯 Render Deployment

### Option 1: Blueprint Deployment (Recommended)

- [ ] Go to [Render Dashboard](https://dashboard.render.com/)
- [ ] Click **"New"** → **"Blueprint"**
- [ ] Connect GitHub repository
- [ ] Select repository: `EVSCMS_API_PRM`
- [ ] Render detects `render.yaml` automatically
- [ ] Click **"Apply"**
- [ ] Wait for all services to be created (~5-10 minutes)

### Option 2: Individual Service Deployment

If deploying individually, for each service:

- [ ] Click **"New"** → **"Web Service"**
- [ ] Connect repository
- [ ] Set Environment: **Docker**
- [ ] Set Dockerfile path (e.g., `Services/Chat/Dockerfile`)
- [ ] Set Build Context: **`.`** (root directory)
- [ ] Set Region: **Singapore**
- [ ] Click **"Create Web Service"**

---

## 🔧 Environment Variables Configuration

### 1. API Gateway
- [x] Pre-configured (no action needed)

### 2. Auth Service
- [ ] `ConnectionStrings__DefaultConnection` - PostgreSQL URL
- [ ] `Jwt__Key` - Generate: `openssl rand -base64 32`
- [ ] `StaffHost` - `ev-scms-staff-api.onrender.com`
- [ ] `CustomerHost` - `ev-scms-customer-api.onrender.com`

### 3. Booking Service
- [ ] `ConnectionStrings__DefaultConnection` - PostgreSQL URL
- [ ] `CustomerHost` - `ev-scms-customer-api.onrender.com`
- [ ] `VehicleHost` - `ev-scms-vehicle-api.onrender.com`
- [ ] `StaffHost` - `ev-scms-staff-api.onrender.com`

### 4. Chat Service ⭐ NEW
- [ ] `MongoDbSettings__ConnectionString` - MongoDB Atlas URL
- [ ] `JwtSettings__SecretKey` - Same as Auth `Jwt__Key`
- [ ] `GeminiSettings__ApiKey` - Google Gemini API key
- [ ] `StaffHost` - `ev-scms-staff-api.onrender.com`

### 5. Customer Service
- [ ] `ConnectionStrings__DefaultConnection` - PostgreSQL URL
- [ ] `AuthHost` - `ev-scms-auth-api.onrender.com`

### 6. Inventory Service
- [ ] `ConnectionStrings__DefaultConnection` - PostgreSQL URL

### 7. Maintenance Service
- [ ] `ConnectionStrings__DefaultConnection` - PostgreSQL URL
- [ ] `CustomerHost` - `ev-scms-customer-api.onrender.com`
- [ ] `VehicleHost` - `ev-scms-vehicle-api.onrender.com`
- [ ] `StaffHost` - `ev-scms-staff-api.onrender.com`
- [ ] `BookingHost` - `ev-scms-booking-api.onrender.com`

### 8. Notification Service
- [ ] `RabbitMQ__ConnectionString` - CloudAMQP URL

### 9. Payment Service
- [ ] `ConnectionStrings__DefaultConnection` - PostgreSQL URL
- [ ] `Payos__CLIENT_ID` - Payos client ID
- [ ] `Payos__API_KEY` - Payos API key
- [ ] `Payos__CHECKSUM_KEY` - Payos checksum key
- [ ] `CustomerHost` - `ev-scms-customer-api.onrender.com`
- [ ] `BookingHost` - `ev-scms-booking-api.onrender.com`

### 10. Report Service
- [ ] `BookingHost` - `ev-scms-booking-api.onrender.com`
- [ ] `MaintenanceHost` - `ev-scms-maintenance-api.onrender.com`

### 11. Staff Service
- [ ] `ConnectionStrings__DefaultConnection` - PostgreSQL URL
- [ ] `AuthHost` - `ev-scms-auth-api.onrender.com`

### 12. Vehicle Service
- [ ] `ConnectionStrings__DefaultConnection` - PostgreSQL URL
- [ ] `CustomerHost` - `ev-scms-customer-api.onrender.com`

---

## 🧪 Post-Deployment Testing

### Service Health Checks

Test each service's health endpoint:

- [ ] API Gateway: `curl https://ev-scms-api-gateway.onrender.com`
- [ ] Auth: `curl https://ev-scms-auth-api.onrender.com/health`
- [ ] Booking: `curl https://ev-scms-booking-api.onrender.com/health`
- [ ] Chat: `curl https://ev-scms-chat-api.onrender.com/health` ⭐
- [ ] Customer: `curl https://ev-scms-customer-api.onrender.com/health`
- [ ] Inventory: `curl https://ev-scms-inventory-api.onrender.com/health`
- [ ] Maintenance: `curl https://ev-scms-maintenance-api.onrender.com/health`
- [ ] Notification: `curl https://ev-scms-notification-api.onrender.com/health`
- [ ] Payment: `curl https://ev-scms-payment-api.onrender.com/health`
- [ ] Report: `curl https://ev-scms-report-api.onrender.com/health`
- [ ] Staff: `curl https://ev-scms-staff-api.onrender.com/health`
- [ ] Vehicle: `curl https://ev-scms-vehicle-api.onrender.com/health`

### API Gateway Routing

- [ ] Test routing through gateway:
  ```bash
  curl https://ev-scms-api-gateway.onrender.com/api/auth/health
  curl https://ev-scms-api-gateway.onrender.com/api/booking/health
  curl https://ev-scms-api-gateway.onrender.com/api/chat/health
  ```

### Chat Service Specific

- [ ] Test SignalR connection
- [ ] Test chat message sending
- [ ] Test AI chat functionality
- [ ] Verify MongoDB data storage

### Database Connections

- [ ] Run database migrations for each service
- [ ] Verify tables are created
- [ ] Test basic CRUD operations

---

## 📊 Monitoring

### Render Dashboard

- [ ] Check all services are "Running" (green status)
- [ ] Review logs for each service
- [ ] Verify no critical errors in logs
- [ ] Check resource usage (CPU, Memory)

### Performance

- [ ] Test response times
- [ ] Monitor cold start times (free tier)
- [ ] Check gRPC communication between services

---

## 🎉 Success Criteria

All items checked = Successful Deployment! 🚀

- [ ] All 12 services deployed
- [ ] All health checks passing
- [ ] Environment variables configured
- [ ] External services connected (MongoDB, RabbitMQ, etc.)
- [ ] API Gateway routing working
- [ ] No errors in logs
- [ ] Chat service with AI working
- [ ] gRPC communication functional
- [ ] Database connections established

---

## 📚 Reference Documents

- [DOCKERFILES_UPDATE_SUMMARY.md](./DOCKERFILES_UPDATE_SUMMARY.md) - Detailed changes and architecture
- [RENDER_DEPLOYMENT_GUIDE.md](./RENDER_DEPLOYMENT_GUIDE.md) - Step-by-step deployment instructions
- [render.yaml](./render.yaml) - Render Blueprint configuration

---

## 🔄 Next Steps After Deployment

1. **Set up monitoring** - Use Render's built-in metrics
2. **Configure alerts** - Set up notifications for service failures
3. **Document API endpoints** - Update Swagger/OpenAPI docs
4. **Performance testing** - Load test all services
5. **Security review** - Check JWT, CORS, and authentication flows
6. **Backup strategy** - Set up database backups
7. **CI/CD pipeline** - Consider GitHub Actions for testing

---

## 🆘 Troubleshooting Guide

| Issue | Solution |
|-------|----------|
| Service won't start | Check environment variables and logs |
| Health check fails | Verify `/health` endpoint and port 8080 |
| gRPC connection error | Verify host URLs (format: `service-name.onrender.com`) |
| Database connection error | Check connection string format and credentials |
| Chat SignalR connection fails | Verify CORS and WebSocket support |
| Cold start too slow | Consider upgrading to paid tier |

---

## 📞 Support Resources

- **Render Docs**: https://render.com/docs
- **Render Community**: https://community.render.com/
- **MongoDB Docs**: https://docs.mongodb.com/
- **Google Gemini Docs**: https://ai.google.dev/docs
- **RabbitMQ Docs**: https://www.rabbitmq.com/documentation.html

---

**Last Updated**: November 5, 2025  
**Status**: ✅ Ready for Deployment

