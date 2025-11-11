# Render.com Deployment Guide - EV-SCMS

## 🚀 Quick Deployment Steps

### Option 1: Blueprint Deployment (Recommended - All Services at Once)

1. **Push to GitHub**
   ```bash
   git add .
   git commit -m "Updated all Dockerfiles and render.yaml"
   git push origin main
   ```

2. **Deploy on Render**
   - Go to [Render Dashboard](https://dashboard.render.com/)
   - Click **"New"** → **"Blueprint"**
   - Connect your GitHub repository: `EVSCMS_API_PRM`
   - Render will automatically detect `render.yaml`
   - Click **"Apply"**
   - All 12 services will be created and deployed automatically! 🎉

3. **Configure Environment Variables**
   - After deployment, go to each service's dashboard
   - Configure the variables marked with `sync: false` (see below)

---

### Option 2: Individual Service Deployment

If you prefer to deploy services individually:

1. Go to [Render Dashboard](https://dashboard.render.com/)
2. Click **"New"** → **"Web Service"**
3. Connect your GitHub repository
4. Configure:
   - **Name**: e.g., `ev-scms-chat-api`
   - **Environment**: Docker
   - **Region**: Singapore
   - **Branch**: main
   - **Dockerfile Path**: e.g., `Services/Chat/Dockerfile`
   - **Docker Build Context**: `.` (repository root)
5. Add environment variables
6. Click **"Create Web Service"**

---

## 🔧 Environment Variables Configuration

### 1. Auth Service (`ev-scms-auth-api`)

**Required:**
```bash
# Database
ConnectionStrings__DefaultConnection=postgresql://user:pass@host:5432/authdb

# JWT Configuration (Generate key: openssl rand -base64 32)
Jwt__Key=your-secret-key-here

# gRPC Hosts
StaffHost=ev-scms-staff-api.onrender.com
CustomerHost=ev-scms-customer-api.onrender.com
```

---

### 2. Chat Service (`ev-scms-chat-api`) 🆕

**Required:**
```bash
# MongoDB (Get free cluster from MongoDB Atlas)
MongoDbSettings__ConnectionString=mongodb+srv://user:pass@cluster.mongodb.net/

# JWT (Use same key as Auth service)
JwtSettings__SecretKey=same-as-auth-jwt-key

# Google Gemini AI (Get from Google AI Studio)
GeminiSettings__ApiKey=your-gemini-api-key

# gRPC Host
StaffHost=ev-scms-staff-api.onrender.com
```

**Get MongoDB Connection String:**
1. Go to [MongoDB Atlas](https://www.mongodb.com/cloud/atlas)
2. Create free cluster
3. Create database user
4. Get connection string
5. Replace `<password>` with your password

**Get Gemini API Key:**
1. Go to [Google AI Studio](https://makersuite.google.com/app/apikey)
2. Create API key
3. Copy and paste

---

### 3. Booking Service (`ev-scms-booking-api`)

**Required:**
```bash
# Database
ConnectionStrings__DefaultConnection=postgresql://user:pass@host:5432/bookingdb

# gRPC Hosts
CustomerHost=ev-scms-customer-api.onrender.com
VehicleHost=ev-scms-vehicle-api.onrender.com
StaffHost=ev-scms-staff-api.onrender.com
```

---

### 4. Customer Service (`ev-scms-customer-api`)

**Required:**
```bash
# Database
ConnectionStrings__DefaultConnection=postgresql://user:pass@host:5432/customerdb

# gRPC Host
AuthHost=ev-scms-auth-api.onrender.com
```

---

### 5. Inventory Service (`ev-scms-inventory-api`)

**Required:**
```bash
# Database
ConnectionStrings__DefaultConnection=postgresql://user:pass@host:5432/inventorydb
```

---

### 6. Maintenance Service (`ev-scms-maintenance-api`)

**Required:**
```bash
# Database
ConnectionStrings__DefaultConnection=postgresql://user:pass@host:5432/maintenancedb

# gRPC Hosts
CustomerHost=ev-scms-customer-api.onrender.com
VehicleHost=ev-scms-vehicle-api.onrender.com
StaffHost=ev-scms-staff-api.onrender.com
BookingHost=ev-scms-booking-api.onrender.com
```

---

### 7. Notification Service (`ev-scms-notification-api`)

**Required:**
```bash
# RabbitMQ (Get free instance from CloudAMQP)
RabbitMQ__ConnectionString=amqps://user:pass@host.cloudamqp.com/vhost
```

**Get RabbitMQ Connection String:**
1. Go to [CloudAMQP](https://www.cloudamqp.com/)
2. Create free "Little Lemur" instance
3. Copy AMQP URL

---

### 8. Payment Service (`ev-scms-payment-api`)

**Required:**
```bash
# Database
ConnectionStrings__DefaultConnection=postgresql://user:pass@host:5432/paymentdb

# Payos Configuration (Get from Payos.vn)
Payos__CLIENT_ID=your-payos-client-id
Payos__API_KEY=your-payos-api-key
Payos__CHECKSUM_KEY=your-payos-checksum-key

# gRPC Hosts
CustomerHost=ev-scms-customer-api.onrender.com
BookingHost=ev-scms-booking-api.onrender.com
```

---

### 9. Report Service (`ev-scms-report-api`)

**Required:**
```bash
# gRPC Hosts
BookingHost=ev-scms-booking-api.onrender.com
MaintenanceHost=ev-scms-maintenance-api.onrender.com
```

---

### 10. Staff Service (`ev-scms-staff-api`)

**Required:**
```bash
# Database
ConnectionStrings__DefaultConnection=postgresql://user:pass@host:5432/staffdb

# gRPC Host
AuthHost=ev-scms-auth-api.onrender.com
```

---

### 11. Vehicle Service (`ev-scms-vehicle-api`)

**Required:**
```bash
# Database
ConnectionStrings__DefaultConnection=postgresql://user:pass@host:5432/vehicledb

# gRPC Host
CustomerHost=ev-scms-customer-api.onrender.com
```

---

### 12. API Gateway (`ev-scms-api-gateway`)

**Pre-configured** (no additional configuration needed)

All service hosts are already configured in render.yaml.

---

## 🗄️ Database Setup Options

### Option 1: Render PostgreSQL (Free Tier)
1. In Render Dashboard → **"New"** → **"PostgreSQL"**
2. Name: `ev-scms-postgres`
3. Plan: Free
4. Create database
5. Copy **Internal Database URL**
6. Use this URL for `ConnectionStrings__DefaultConnection`

**Note**: Free tier expires after 90 days. Good for testing.

### Option 2: Supabase (Free Forever)
1. Go to [Supabase](https://supabase.com/)
2. Create new project
3. Get connection string from **Database Settings**
4. Format: `postgresql://postgres:[password]@[host]:5432/postgres`

### Option 3: ElephantSQL (Free Tier)
1. Go to [ElephantSQL](https://www.elephantsql.com/)
2. Create free "Tiny Turtle" instance
3. Copy connection string

---

## 📋 Pre-Deployment Checklist

- [ ] All code changes committed and pushed to GitHub
- [ ] MongoDB Atlas cluster created (for Chat service)
- [ ] Google Gemini API key obtained (for Chat service)
- [ ] RabbitMQ instance created (for Notification service)
- [ ] Payos credentials obtained (for Payment service)
- [ ] PostgreSQL databases prepared (or will use Render PostgreSQL)

---

## 🎯 Post-Deployment Verification

### 1. Check Service Health

All services should have health endpoint at `/health`:

```bash
# Example: Check Auth service
curl https://ev-scms-auth-api.onrender.com/health

# Should return: 200 OK
```

### 2. Test API Gateway

```bash
# API Gateway should be accessible
curl https://ev-scms-api-gateway.onrender.com

# Test routing through gateway (example)
curl https://ev-scms-api-gateway.onrender.com/api/auth/health
```

### 3. Monitor Logs

In Render Dashboard:
1. Click on any service
2. Go to **"Logs"** tab
3. Check for errors or warnings

### 4. Test Chat Service with SignalR

```javascript
// Example: Connect to Chat Hub
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://ev-scms-chat-api.onrender.com/chatHub")
    .build();

await connection.start();
```

---

## 🐛 Troubleshooting

### Service Won't Start

**Check:**
1. Environment variables are configured correctly
2. Database connection strings are valid
3. External dependencies (MongoDB, RabbitMQ) are accessible
4. Logs in Render dashboard for specific error messages

### Health Check Failing

**Solution:**
1. Verify `/health` endpoint exists in the service
2. Check if service is binding to port 8080
3. Review `ASPNETCORE_URLS` environment variable

### gRPC Connection Issues

**Solution:**
1. Ensure gRPC host URLs are correct (format: `service-name.onrender.com`)
2. Check if target services are running
3. Verify network connectivity between services

### Chat Service Not Connecting

**Check:**
1. MongoDB connection string is valid
2. SignalR hub path is correct (`/chatHub`)
3. CORS is configured properly
4. WebSocket support is enabled

### Free Tier Services Spinning Down

**Note**: Render free tier services spin down after 15 minutes of inactivity.

**Solutions:**
1. First request will take ~30 seconds to wake up (cold start)
2. Use [Cron-job.org](https://cron-job.org) to ping services every 10 minutes
3. Upgrade to paid tier for always-on services

---

## 🎉 Deployment Complete!

Once all services are running and healthy, your EV-SCMS microservices architecture is live!

### Service URLs

After deployment, you'll have:
- **API Gateway**: `https://ev-scms-api-gateway.onrender.com`
- **Auth API**: `https://ev-scms-auth-api.onrender.com`
- **Booking API**: `https://ev-scms-booking-api.onrender.com`
- **Chat API**: `https://ev-scms-chat-api.onrender.com` 🆕
- **Customer API**: `https://ev-scms-customer-api.onrender.com`
- **Inventory API**: `https://ev-scms-inventory-api.onrender.com`
- **Maintenance API**: `https://ev-scms-maintenance-api.onrender.com`
- **Notification API**: `https://ev-scms-notification-api.onrender.com`
- **Payment API**: `https://ev-scms-payment-api.onrender.com`
- **Report API**: `https://ev-scms-report-api.onrender.com`
- **Staff API**: `https://ev-scms-staff-api.onrender.com`
- **Vehicle API**: `https://ev-scms-vehicle-api.onrender.com`

---

## 📞 Need Help?

- **Render Documentation**: https://render.com/docs
- **Render Community**: https://community.render.com/
- **Project Issues**: Check service logs in Render dashboard

---

## 🔄 Updates and Redeployment

When you push changes to GitHub:
1. Render automatically detects the changes
2. Rebuilds the affected services
3. Deploys the new version
4. Zero downtime deployment! 🎉

**Auto-deploy is enabled for all services on the `main` branch.**

---

**Happy Deploying! 🚀**

