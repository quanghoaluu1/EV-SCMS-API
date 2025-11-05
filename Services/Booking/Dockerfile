# Deploy on Render: connect this repo, select this folder as root, Render will build using this Dockerfile.
# Note: Build context should be the repository root to access shared projects (GrpcContract, Messaging)

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared projects
COPY ["GrpcContract/GrpcContract.csproj", "GrpcContract/"]
COPY ["Messaging/Messaging.csproj", "Messaging/"]

# Copy Booking service projects
COPY ["Services/Booking/Booking.Api/Booking.Api.csproj", "Services/Booking/Booking.Api/"]
COPY ["Services/Booking/Booking.Application/Booking.Application.csproj", "Services/Booking/Booking.Application/"]
COPY ["Services/Booking/Booking.Domain/Booking.Domain.csproj", "Services/Booking/Booking.Domain/"]
COPY ["Services/Booking/Booking.Infrastructure/Booking.Infrastructure.csproj", "Services/Booking/Booking.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Services/Booking/Booking.Api/Booking.Api.csproj"

# Copy source code
COPY ["GrpcContract/", "GrpcContract/"]
COPY ["Messaging/", "Messaging/"]
COPY ["Services/Booking/", "Services/Booking/"]

# Build and publish
WORKDIR "/src/Services/Booking/Booking.Api"
RUN dotnet build "Booking.Api.csproj" -c Release -o /app/build
RUN dotnet publish "Booking.Api.csproj" -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user
RUN useradd -m -u 1001 appuser && chown -R appuser:appuser /app
USER appuser

# Copy published output
COPY --from=build --chown=appuser:appuser /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Booking.Api.dll"]
