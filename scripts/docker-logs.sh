#!/bin/bash

# ============================================================================
# Docker Compose Logs Script
# ============================================================================

set -e

echo "📋 EV-SCMS Service Logs"
echo ""

# If no arguments, show menu
if [ $# -eq 0 ]; then
    echo "Select a service to view logs:"
    echo ""
    echo "  0) All services"
    echo "  1) API Gateway"
    echo "  2) Auth API"
    echo "  3) Booking API"
    echo "  4) Chat API"
    echo "  5) Customer API"
    echo "  6) Inventory API"
    echo "  7) Maintenance API"
    echo "  8) Notification API"
    echo "  9) Payment API"
    echo " 10) Report API"
    echo " 11) Staff API"
    echo " 12) Vehicle API"
    echo " 13) PostgreSQL"
    echo " 14) MongoDB"
    echo " 15) RabbitMQ"
    echo " 16) Redis"
    echo ""
    read -p "Enter choice (0-16): " choice
    echo ""
    
    case $choice in
        0) docker-compose logs -f ;;
        1) docker-compose logs -f api-gateway ;;
        2) docker-compose logs -f auth-api ;;
        3) docker-compose logs -f booking-api ;;
        4) docker-compose logs -f chat-api ;;
        5) docker-compose logs -f customer-api ;;
        6) docker-compose logs -f inventory-api ;;
        7) docker-compose logs -f maintenance-api ;;
        8) docker-compose logs -f notification-api ;;
        9) docker-compose logs -f payment-api ;;
        10) docker-compose logs -f report-api ;;
        11) docker-compose logs -f staff-api ;;
        12) docker-compose logs -f vehicle-api ;;
        13) docker-compose logs -f postgres ;;
        14) docker-compose logs -f mongodb ;;
        15) docker-compose logs -f rabbitmq ;;
        16) docker-compose logs -f redis ;;
        *) echo "Invalid choice" ;;
    esac
else
    # Show logs for specified service
    docker-compose logs -f "$@"
fi

