#!/bin/bash

# ============================================================================
# Docker Compose Clean Script
# ============================================================================

set -e

echo "🧹 Cleaning up EV-SCMS Docker environment..."
echo ""

read -p "⚠️  This will remove all containers, networks, and volumes. Continue? (y/N) " -n 1 -r
echo ""

if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo "🗑️  Removing containers, networks, and volumes..."
    docker-compose down -v
    
    echo ""
    echo "🗑️  Removing images..."
    docker-compose down --rmi local
    
    echo ""
    echo "✅ Cleanup complete!"
    echo ""
    echo "📝 To start fresh: ./scripts/docker-start.sh"
else
    echo "❌ Cleanup cancelled"
fi

echo ""

