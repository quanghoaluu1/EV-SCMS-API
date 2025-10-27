# Deploy on Render: connect this repo, select this folder as root, Render will build using this Dockerfile.
# Note: Build context should be the repository root to access shared projects (GrpcContract, Messaging)

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared projects
COPY ["GrpcContract/GrpcContract.csproj", "GrpcContract/"]
COPY ["Messaging/Messaging.csproj", "Messaging/"]

# Copy Maintenance service projects
COPY ["Services/Maintenance/Maintenance.Api/Maintenance.Api.csproj", "Services/Maintenance/Maintenance.Api/"]
COPY ["Services/Maintenance/Maintenance.Application/Maintenance.Application.csproj", "Services/Maintenance/Maintenance.Application/"]
COPY ["Services/Maintenance/Maintenance.Domain/Maintenance.Domain.csproj", "Services/Maintenance/Maintenance.Domain/"]
COPY ["Services/Maintenance/Maintenance.Infrastructure/Maintenance.Infrastructure.csproj", "Services/Maintenance/Maintenance.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Services/Maintenance/Maintenance.Api/Maintenance.Api.csproj"

# Copy source code
COPY ["GrpcContract/", "GrpcContract/"]
COPY ["Messaging/", "Messaging/"]
COPY ["Services/Maintenance/", "Services/Maintenance/"]

# Build and publish
WORKDIR "/src/Services/Maintenance/Maintenance.Api"
RUN dotnet build "Maintenance.Api.csproj" -c Release -o /app/build
RUN dotnet publish "Maintenance.Api.csproj" -c Release -o /app/publish --no-restore

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

ENTRYPOINT ["dotnet", "Maintenance.Api.dll"]

