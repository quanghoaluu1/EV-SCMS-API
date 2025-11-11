#!/bin/bash

# ============================================================================
# Docker Compose Stop Script
# ============================================================================

set -e

echo "🛑 Stopping EV-SCMS Microservices..."
echo ""

docker-compose stop

echo ""
echo "✅ All services stopped!"
echo ""
echo "📝 To start again:    ./scripts/docker-start.sh"
echo "📝 To remove all:     ./scripts/docker-clean.sh"
echo ""

