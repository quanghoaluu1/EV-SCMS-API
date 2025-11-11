-- ============================================================================
-- PostgreSQL Database Initialization Script
-- ============================================================================
-- Creates separate databases for each microservice
-- ============================================================================

-- Create databases for each service
CREATE DATABASE evscms_auth;
CREATE DATABASE evscms_booking;
CREATE DATABASE evscms_customer;
CREATE DATABASE evscms_inventory;
CREATE DATABASE evscms_maintenance;
CREATE DATABASE evscms_payment;
CREATE DATABASE evscms_staff;
CREATE DATABASE evscms_vehicle;

-- Grant all privileges to evscms user
GRANT ALL PRIVILEGES ON DATABASE evscms_auth TO evscms;
GRANT ALL PRIVILEGES ON DATABASE evscms_booking TO evscms;
GRANT ALL PRIVILEGES ON DATABASE evscms_customer TO evscms;
GRANT ALL PRIVILEGES ON DATABASE evscms_inventory TO evscms;
GRANT ALL PRIVILEGES ON DATABASE evscms_maintenance TO evscms;
GRANT ALL PRIVILEGES ON DATABASE evscms_payment TO evscms;
GRANT ALL PRIVILEGES ON DATABASE evscms_staff TO evscms;
GRANT ALL PRIVILEGES ON DATABASE evscms_vehicle TO evscms;

