# 🐳 Docker Compose Local Testing Guide

## 📋 Overview

This guide will help you run the entire EV-SCMS microservices architecture locally using Docker Compose.

---

## 🚀 Quick Start

### 1. Prerequisites

- **Docker** installed (v20.10+)
- **Docker Compose** installed (v2.0+)
- At least **8GB RAM** available
- At least **10GB disk space**

Check installation:
```bash
docker --version
docker-compose --version
```

### 2. Configure Environment Variables

Copy the example environment file and fill in your API keys:

```bash
cp .env.example .env
```

Edit `.env` and add:
- `GEMINI_API_KEY` - Get from [Google AI Studio](https://makersuite.google.com/app/apikey)
- `PAYOS_CLIENT_ID`, `PAYOS_API_KEY`, `PAYOS_CHECKSUM_KEY` - Get from [Payos.vn](https://payos.vn)

> **Note**: Services will still run without these keys, but Chat AI and Payment features won't work.

### 3. Build and Start All Services

```bash
# Build and start all services in detached mode
docker-compose up -d --build
```

This will:
- Build Docker images for all 12 services
- Start infrastructure (PostgreSQL, MongoDB, RabbitMQ, Redis)
- Start all microservices
- Create networks and volumes

**First build may take 10-15 minutes** depending on your machine.

### 4. Verify Services Are Running

```bash
# Check status of all services
docker-compose ps

# View logs of all services
docker-compose logs -f

# View logs of a specific service
docker-compose logs -f chat-api
```

---

## 🌐 Access Services

### Microservices

| Service | URL | Description |
|---------|-----|-------------|
| **API Gateway** | http://localhost:8000 | Main entry point (Ocelot) |
| **Auth API** | http://localhost:8001 | Authentication & Authorization |
| **Booking API** | http://localhost:8002 | Booking management |
| **Chat API** | http://localhost:8003 | Real-time chat with AI |
| **Customer API** | http://localhost:8004 | Customer management |
| **Inventory API** | http://localhost:8005 | Inventory management |
| **Maintenance API** | http://localhost:8006 | Maintenance scheduling |
| **Notification API** | http://localhost:8007 | Notification service |
| **Payment API** | http://localhost:8008 | Payment processing |
| **Report API** | http://localhost:8009 | Reporting & analytics |
| **Staff API** | http://localhost:8010 | Staff management |
| **Vehicle API** | http://localhost:8011 | Vehicle management |

### Infrastructure

| Service | URL | Credentials |
|---------|-----|-------------|
| **PostgreSQL** | localhost:5432 | User: `evscms`<br>Password: `evscms_password` |
| **MongoDB** | localhost:27017 | User: `evscms`<br>Password: `evscms_password` |
| **RabbitMQ UI** | http://localhost:15672 | User: `guest`<br>Password: `guest` |
| **Redis** | localhost:6379 | No password |

---

## 🧪 Testing Services

### Health Check

Test all service health endpoints:

```bash
# API Gateway
curl http://localhost:8000

# Auth Service
curl http://localhost:8001/health

# Booking Service
curl http://localhost:8002/health

# Chat Service
curl http://localhost:8003/health

# Customer Service
curl http://localhost:8004/health

# And so on...
```

### Swagger UI

Each service has Swagger UI for API testing:

```
http://localhost:8001/swagger  # Auth API
http://localhost:8002/swagger  # Booking API
http://localhost:8003/swagger  # Chat API
# ... etc
```

### API Gateway Routing

Test routing through API Gateway:

```bash
# Health check through gateway
curl http://localhost:8000/api/auth/health
curl http://localhost:8000/api/booking/health
curl http://localhost:8000/api/chat/health
```

---

## 🗄️ Database Management

### PostgreSQL

Connect with any PostgreSQL client:

```bash
# Using psql
psql -h localhost -p 5432 -U evscms -d evscms_auth

# List all databases
\l

# Connect to specific database
\c evscms_booking
```

**Databases created:**
- `evscms_auth`
- `evscms_booking`
- `evscms_customer`
- `evscms_inventory`
- `evscms_maintenance`
- `evscms_payment`
- `evscms_staff`
- `evscms_vehicle`

### MongoDB

Connect using MongoDB Compass or CLI:

```bash
# Connection string
mongodb://evscms:evscms_password@localhost:27017

# Using mongosh
mongosh "mongodb://evscms:evscms_password@localhost:27017/evscms_chat"
```

### Run Migrations

If your services use Entity Framework migrations:

```bash
# Example: Run migrations for Auth service
docker-compose exec auth-api dotnet ef database update

# Or connect to the container and run manually
docker-compose exec auth-api bash
dotnet ef database update
```

---

## 📊 Monitoring & Logs

### View Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f auth-api

# Last 100 lines
docker-compose logs --tail=100 chat-api

# Follow logs of multiple services
docker-compose logs -f auth-api customer-api booking-api
```

### Resource Usage

```bash
# Check resource usage
docker stats

# Check disk usage
docker system df
```

### Service Health

```bash
# Check which services are running
docker-compose ps

# Restart a specific service
docker-compose restart chat-api

# Restart all services
docker-compose restart
```

---

## 🔧 Common Commands

### Start Services

```bash
# Start all services
docker-compose up -d

# Start specific services
docker-compose up -d auth-api customer-api

# Start with live logs
docker-compose up
```

### Stop Services

```bash
# Stop all services
docker-compose stop

# Stop specific service
docker-compose stop chat-api
```

### Restart Services

```bash
# Restart all services
docker-compose restart

# Restart specific service
docker-compose restart auth-api
```

### Rebuild Services

```bash
# Rebuild all services
docker-compose up -d --build

# Rebuild specific service
docker-compose up -d --build chat-api

# Force rebuild (no cache)
docker-compose build --no-cache
docker-compose up -d
```

### Remove Everything

```bash
# Stop and remove containers, networks
docker-compose down

# Also remove volumes (WARNING: deletes all data!)
docker-compose down -v

# Remove images as well
docker-compose down --rmi all
```

---

## 🐛 Troubleshooting

### Service Won't Start

**Problem**: Service container exits immediately

**Solution**:
```bash
# Check logs for the service
docker-compose logs auth-api

# Check if port is already in use
lsof -i :8001

# Rebuild the service
docker-compose up -d --build auth-api
```

### Database Connection Issues

**Problem**: Service can't connect to database

**Solution**:
```bash
# Check if PostgreSQL is healthy
docker-compose ps postgres

# Check PostgreSQL logs
docker-compose logs postgres

# Verify database was created
docker-compose exec postgres psql -U evscms -l

# Restart the service
docker-compose restart auth-api
```

### Port Already in Use

**Problem**: "Port is already allocated"

**Solution**:
```bash
# Find process using the port
lsof -i :8001

# Kill the process or change port in docker-compose.yml
# Edit docker-compose.yml and change:
# "8001:8080" to "8101:8080"
```

### Out of Memory

**Problem**: Services crashing or slow performance

**Solution**:
- Increase Docker memory allocation (Docker Desktop → Settings → Resources)
- Run fewer services at once:
  ```bash
  docker-compose up -d postgres mongodb redis rabbitmq
  docker-compose up -d auth-api customer-api booking-api
  ```

### Can't Access Service

**Problem**: curl to http://localhost:8001 times out

**Solution**:
```bash
# Check if service is running
docker-compose ps auth-api

# Check if service is listening on correct port
docker-compose exec auth-api netstat -tlnp

# Check service logs
docker-compose logs auth-api

# Restart service
docker-compose restart auth-api
```

---

## 🧹 Cleanup

### Remove Stopped Containers

```bash
docker-compose down
```

### Remove All Data (Volumes)

```bash
# WARNING: This deletes all database data!
docker-compose down -v
```

### Free Up Disk Space

```bash
# Remove unused images, containers, networks
docker system prune -a

# Remove unused volumes
docker volume prune
```

---

## 🎯 Development Workflow

### Typical Development Flow

1. **Start infrastructure only**:
   ```bash
   docker-compose up -d postgres mongodb redis rabbitmq
   ```

2. **Run your service locally** (outside Docker) for faster development

3. **Test with other services** in Docker:
   ```bash
   docker-compose up -d auth-api customer-api
   ```

4. **Make changes** and rebuild when ready:
   ```bash
   docker-compose up -d --build your-service
   ```

### Hot Reload (Development)

For hot reload during development, mount your source code as a volume:

```yaml
# Add to docker-compose.yml under the service
volumes:
  - ./Services/Chat:/src/Services/Chat
```

---

## 📝 Testing Checklist

- [ ] All services start successfully
- [ ] All health endpoints return 200
- [ ] PostgreSQL databases are created
- [ ] MongoDB connection works
- [ ] RabbitMQ management UI accessible
- [ ] Redis connection works
- [ ] API Gateway routes requests correctly
- [ ] gRPC communication between services works
- [ ] Chat service connects to MongoDB
- [ ] Notification service connects to RabbitMQ
- [ ] All Swagger UIs are accessible

---

## 🔐 Security Notes

### Local Development Only

The credentials in `docker-compose.yml` are for **local development only**:
- **Do NOT** use these in production
- **Do NOT** commit real API keys to Git
- Use `.env` file for sensitive data (already in `.gitignore`)

### Production Deployment

For production, use:
- Strong, unique passwords
- Environment variables from secure vault
- TLS/SSL for all connections
- Proper network isolation
- See [RENDER_DEPLOYMENT_GUIDE.md](./RENDER_DEPLOYMENT_GUIDE.md)

---

## 📚 Additional Resources

- **Docker Docs**: https://docs.docker.com/
- **Docker Compose Docs**: https://docs.docker.com/compose/
- **PostgreSQL Docs**: https://www.postgresql.org/docs/
- **MongoDB Docs**: https://docs.mongodb.com/
- **RabbitMQ Docs**: https://www.rabbitmq.com/documentation.html

---

## 🆘 Getting Help

If you encounter issues:

1. Check service logs: `docker-compose logs -f service-name`
2. Verify all infrastructure is healthy: `docker-compose ps`
3. Review this guide's troubleshooting section
4. Check Docker resource allocation
5. Try rebuilding: `docker-compose up -d --build`

---

## 🎉 Success!

When all services show as "Up" and health checks pass, you have a fully functional local EV-SCMS environment! 🚀

```bash
docker-compose ps
# All services should show "Up"
```

Happy testing! 🐳

