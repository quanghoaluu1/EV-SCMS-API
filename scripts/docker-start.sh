#!/bin/bash

# ============================================================================
# Docker Compose Quick Start Script
# ============================================================================

set -e

echo "🚀 Starting EV-SCMS Microservices..."
echo ""

# Check if .env exists
if [ ! -f .env ]; then
    echo "⚠️  .env file not found!"
    echo "📝 Creating .env from env.example..."
    cp env.example .env
    echo "✅ .env file created"
    echo ""
    echo "⚠️  IMPORTANT: Edit .env and add your API keys:"
    echo "   - GEMINI_API_KEY (for Chat AI)"
    echo "   - PAYOS credentials (for Payment)"
    echo ""
    read -p "Press Enter to continue (services will start with default values)..."
fi

echo "🏗️  Building and starting all services..."
docker-compose up -d --build

echo ""
echo "⏳ Waiting for services to be ready..."
sleep 10

echo ""
echo "📊 Service Status:"
docker-compose ps

echo ""
echo "✅ Services started successfully!"
echo ""
echo "🌐 Access URLs:"
echo "   API Gateway:     http://localhost:8000"
echo "   Auth API:        http://localhost:8001"
echo "   Booking API:     http://localhost:8002"
echo "   Chat API:        http://localhost:8003"
echo "   Customer API:    http://localhost:8004"
echo "   Inventory API:   http://localhost:8005"
echo "   Maintenance API: http://localhost:8006"
echo "   Notification API:http://localhost:8007"
echo "   Payment API:     http://localhost:8008"
echo "   Report API:      http://localhost:8009"
echo "   Staff API:       http://localhost:8010"
echo "   Vehicle API:     http://localhost:8011"
echo ""
echo "🗄️  Infrastructure:"
echo "   PostgreSQL:      localhost:5432"
echo "   MongoDB:         localhost:27017"
echo "   RabbitMQ UI:     http://localhost:15672"
echo "   Redis:           localhost:6379"
echo ""
echo "📝 Useful commands:"
echo "   View logs:       docker-compose logs -f"
echo "   Stop services:   docker-compose stop"
echo "   Remove all:      docker-compose down -v"
echo ""

