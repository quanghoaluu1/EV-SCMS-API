# 🐳 Docker Compose Quick Reference

## 🚀 Quick Start

```bash
# 1. Setup environment (first time only)
cp env.example .env
# Edit .env and add your API keys

# 2. Start everything
./scripts/docker-start.sh
# OR
docker-compose up -d --build

# 3. Check status
docker-compose ps

# 4. View logs
./scripts/docker-logs.sh
# OR
docker-compose logs -f
```

---

## 📍 Service URLs

| Service | URL | Health Check |
|---------|-----|--------------|
| API Gateway | http://localhost:8000 | http://localhost:8000 |
| Auth API | http://localhost:8001 | http://localhost:8001/health |
| Booking API | http://localhost:8002 | http://localhost:8002/health |
| Chat API | http://localhost:8003 | http://localhost:8003/health |
| Customer API | http://localhost:8004 | http://localhost:8004/health |
| Inventory API | http://localhost:8005 | http://localhost:8005/health |
| Maintenance API | http://localhost:8006 | http://localhost:8006/health |
| Notification API | http://localhost:8007 | http://localhost:8007/health |
| Payment API | http://localhost:8008 | http://localhost:8008/health |
| Report API | http://localhost:8009 | http://localhost:8009/health |
| Staff API | http://localhost:8010 | http://localhost:8010/health |
| Vehicle API | http://localhost:8011 | http://localhost:8011/health |

---

## 🗄️ Infrastructure

| Service | Access | Credentials |
|---------|--------|-------------|
| PostgreSQL | localhost:5432 | User: `evscms`<br>Pass: `evscms_password` |
| MongoDB | localhost:27017 | User: `evscms`<br>Pass: `evscms_password` |
| RabbitMQ | http://localhost:15672 | User: `guest`<br>Pass: `guest` |
| Redis | localhost:6379 | No password |

---

## 🔧 Essential Commands

### Start & Stop
```bash
# Start all services
docker-compose up -d

# Stop all services
docker-compose stop

# Restart all services
docker-compose restart

# Stop and remove everything
docker-compose down

# Stop and remove including volumes (⚠️  deletes data!)
docker-compose down -v
```

### View Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f auth-api

# Last 100 lines
docker-compose logs --tail=100 chat-api
```

### Service Management
```bash
# Restart specific service
docker-compose restart auth-api

# Rebuild specific service
docker-compose up -d --build auth-api

# Scale a service (if applicable)
docker-compose up -d --scale auth-api=2
```

### Health Check
```bash
# Check all services
docker-compose ps

# Test endpoints
./scripts/docker-test.sh

# Manual health check
curl http://localhost:8001/health
```

---

## 🛠️ Helper Scripts

```bash
# Start all services
./scripts/docker-start.sh

# Stop all services
./scripts/docker-stop.sh

# View logs (interactive menu)
./scripts/docker-logs.sh

# Test all endpoints
./scripts/docker-test.sh

# Clean up everything
./scripts/docker-clean.sh
```

---

## 🗃️ Database Access

### PostgreSQL
```bash
# Connect to PostgreSQL
docker-compose exec postgres psql -U evscms

# List databases
docker-compose exec postgres psql -U evscms -c "\l"

# Connect to specific database
docker-compose exec postgres psql -U evscms -d evscms_auth

# Run SQL file
docker-compose exec -T postgres psql -U evscms -d evscms_auth < backup.sql
```

### MongoDB
```bash
# Connect to MongoDB
docker-compose exec mongodb mongosh -u evscms -p evscms_password

# List databases
docker-compose exec mongodb mongosh -u evscms -p evscms_password --eval "show dbs"

# Connect to specific database
docker-compose exec mongodb mongosh -u evscms -p evscms_password evscms_chat
```

### Redis
```bash
# Connect to Redis CLI
docker-compose exec redis redis-cli

# Test connection
docker-compose exec redis redis-cli ping

# Get all keys
docker-compose exec redis redis-cli keys "*"
```

---

## 🐛 Troubleshooting

### Service Won't Start
```bash
# Check logs
docker-compose logs service-name

# Rebuild service
docker-compose up -d --build service-name

# Remove and recreate
docker-compose rm -f service-name
docker-compose up -d service-name
```

### Port Already in Use
```bash
# Find process using port
lsof -i :8001

# Kill process or change port in docker-compose.yml
```

### Database Connection Error
```bash
# Check if PostgreSQL is running
docker-compose ps postgres

# Restart PostgreSQL
docker-compose restart postgres

# Check PostgreSQL logs
docker-compose logs postgres
```

### Out of Memory
```bash
# Check resource usage
docker stats

# Stop some services
docker-compose stop report-api notification-api

# Increase Docker memory (Docker Desktop → Settings → Resources)
```

---

## 📊 Monitoring

### Resource Usage
```bash
# Live stats
docker stats

# Disk usage
docker system df

# Service status
docker-compose ps
```

### Container Details
```bash
# Inspect container
docker-compose exec service-name bash

# View environment variables
docker-compose exec service-name env

# Check processes
docker-compose top service-name
```

---

## 🧹 Cleanup

### Moderate Cleanup
```bash
# Stop and remove containers
docker-compose down
```

### Full Cleanup
```bash
# Remove containers, networks, and volumes
docker-compose down -v
```

### Deep Cleanup
```bash
# Remove everything including images
docker-compose down -v --rmi all

# Clean up Docker system
docker system prune -a --volumes
```

---

## 🔑 Environment Variables

Required in `.env` file:

```bash
# Chat Service (AI)
GEMINI_API_KEY=your-gemini-api-key

# Payment Service
PAYOS_CLIENT_ID=your-client-id
PAYOS_API_KEY=your-api-key
PAYOS_CHECKSUM_KEY=your-checksum-key
```

---

## 📝 Databases Created

PostgreSQL databases:
- `evscms_auth`
- `evscms_booking`
- `evscms_customer`
- `evscms_inventory`
- `evscms_maintenance`
- `evscms_payment`
- `evscms_staff`
- `evscms_vehicle`

MongoDB database:
- `evscms_chat`

---

## 🎯 Common Workflows

### Fresh Start
```bash
docker-compose down -v
docker-compose up -d --build
./scripts/docker-test.sh
```

### Update Single Service
```bash
# Make code changes
docker-compose up -d --build service-name
docker-compose logs -f service-name
```

### Debug Service
```bash
# Stop service
docker-compose stop service-name

# View logs
docker-compose logs service-name

# Restart with live logs
docker-compose up service-name
```

### Backup Database
```bash
# PostgreSQL
docker-compose exec postgres pg_dump -U evscms evscms_auth > backup.sql

# MongoDB
docker-compose exec mongodb mongodump -u evscms -p evscms_password -d evscms_chat
```

---

## 📚 Documentation

- Full Guide: [DOCKER_COMPOSE_GUIDE.md](./DOCKER_COMPOSE_GUIDE.md)
- Docker Compose File: [docker-compose.yml](./docker-compose.yml)
- Environment Setup: [env.example](./env.example)

---

## ⚠️ Important Notes

1. **Development Only**: These credentials are for local development only
2. **API Keys**: Add real API keys to `.env` file (not committed to Git)
3. **Memory**: Ensure Docker has at least 8GB RAM allocated
4. **Disk Space**: Ensure at least 10GB free disk space
5. **First Build**: Initial build may take 10-15 minutes

---

## ✅ Health Check Checklist

- [ ] All 12 services show "Up" in `docker-compose ps`
- [ ] All health endpoints return 200 OK
- [ ] PostgreSQL accepts connections
- [ ] MongoDB accepts connections
- [ ] RabbitMQ management UI accessible
- [ ] Redis responds to ping
- [ ] API Gateway routes correctly
- [ ] No error logs in `docker-compose logs`

---

## 🆘 Getting Help

1. Check logs: `docker-compose logs -f service-name`
2. Run health check: `./scripts/docker-test.sh`
3. Review [DOCKER_COMPOSE_GUIDE.md](./DOCKER_COMPOSE_GUIDE.md)
4. Check Docker resources: `docker stats`
5. Try rebuilding: `docker-compose up -d --build`

---

**Happy Local Testing! 🐳**

