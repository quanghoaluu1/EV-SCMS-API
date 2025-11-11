#!/bin/bash

# ============================================================================
# Docker Compose Health Check Script
# ============================================================================

set -e

echo "🔍 Testing EV-SCMS Services..."
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to test endpoint
test_endpoint() {
    local name=$1
    local url=$2
    
    echo -n "Testing $name... "
    
    if curl -sf "$url" > /dev/null 2>&1; then
        echo -e "${GREEN}✓ OK${NC}"
        return 0
    else
        echo -e "${RED}✗ FAILED${NC}"
        return 1
    fi
}

echo "🌐 Testing Microservices:"
echo ""

test_endpoint "API Gateway    " "http://localhost:8000"
test_endpoint "Auth API       " "http://localhost:8001/health"
test_endpoint "Booking API    " "http://localhost:8002/health"
test_endpoint "Chat API       " "http://localhost:8003/health"
test_endpoint "Customer API   " "http://localhost:8004/health"
test_endpoint "Inventory API  " "http://localhost:8005/health"
test_endpoint "Maintenance API" "http://localhost:8006/health"
test_endpoint "Notification API" "http://localhost:8007/health"
test_endpoint "Payment API    " "http://localhost:8008/health"
test_endpoint "Report API     " "http://localhost:8009/health"
test_endpoint "Staff API      " "http://localhost:8010/health"
test_endpoint "Vehicle API    " "http://localhost:8011/health"

echo ""
echo "🗄️  Testing Infrastructure:"
echo ""

# Test PostgreSQL
echo -n "Testing PostgreSQL... "
if docker-compose exec -T postgres pg_isready -U evscms > /dev/null 2>&1; then
    echo -e "${GREEN}✓ OK${NC}"
else
    echo -e "${RED}✗ FAILED${NC}"
fi

# Test MongoDB
echo -n "Testing MongoDB... "
if docker-compose exec -T mongodb mongosh --eval "db.runCommand('ping').ok" --quiet > /dev/null 2>&1; then
    echo -e "${GREEN}✓ OK${NC}"
else
    echo -e "${RED}✗ FAILED${NC}"
fi

# Test RabbitMQ
echo -n "Testing RabbitMQ... "
if curl -sf http://localhost:15672 > /dev/null 2>&1; then
    echo -e "${GREEN}✓ OK${NC}"
else
    echo -e "${RED}✗ FAILED${NC}"
fi

# Test Redis
echo -n "Testing Redis... "
if docker-compose exec -T redis redis-cli ping > /dev/null 2>&1; then
    echo -e "${GREEN}✓ OK${NC}"
else
    echo -e "${RED}✗ FAILED${NC}"
fi

echo ""
echo "📊 Container Status:"
echo ""
docker-compose ps

echo ""
echo "✅ Health check complete!"
echo ""

