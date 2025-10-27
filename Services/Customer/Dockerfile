# Deploy on Render: connect this repo, select this folder as root, Render will build using this Dockerfile.
# Note: Build context should be the repository root to access shared projects (GrpcContract, Messaging)

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared projects
COPY ["GrpcContract/GrpcContract.csproj", "GrpcContract/"]
COPY ["Messaging/Messaging.csproj", "Messaging/"]

# Copy Customer service projects
COPY ["Services/Customer/Customer.Api/Customer.Api.csproj", "Services/Customer/Customer.Api/"]
COPY ["Services/Customer/Customer.Application/Customer.Application.csproj", "Services/Customer/Customer.Application/"]
COPY ["Services/Customer/Customer.Domain/Customer.Domain.csproj", "Services/Customer/Customer.Domain/"]
COPY ["Services/Customer/Customer.Infrastructure/Customer.Infrastructure.csproj", "Services/Customer/Customer.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Services/Customer/Customer.Api/Customer.Api.csproj"

# Copy source code
COPY ["GrpcContract/", "GrpcContract/"]
COPY ["Messaging/", "Messaging/"]
COPY ["Services/Customer/", "Services/Customer/"]

# Build and publish
WORKDIR "/src/Services/Customer/Customer.Api"
RUN dotnet build "Customer.Api.csproj" -c Release -o /app/build
RUN dotnet publish "Customer.Api.csproj" -c Release -o /app/publish --no-restore

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

ENTRYPOINT ["dotnet", "Customer.Api.dll"]

