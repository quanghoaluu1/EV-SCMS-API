# Deploy on Render: connect this repo, select this folder as root, Render will build using this Dockerfile.
# Note: Build context should be the repository root to access shared projects (Common)

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared projects
COPY ["Common/Common.csproj", "Common/"]

# Copy Chat service projects
COPY ["Services/Chat/Chat.WebApi/Chat.WebApi.csproj", "Services/Chat/Chat.WebApi/"]
COPY ["Services/Chat/Chat.Application/Chat.Application.csproj", "Services/Chat/Chat.Application/"]
COPY ["Services/Chat/Chat.Domain/Chat.Domain.csproj", "Services/Chat/Chat.Domain/"]
COPY ["Services/Chat/Chat.Infrastructure/Chat.Infrastructure.csproj", "Services/Chat/Chat.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Services/Chat/Chat.WebApi/Chat.WebApi.csproj"

# Copy source code
COPY ["Common/", "Common/"]
COPY ["Services/Chat/", "Services/Chat/"]

# Build and publish
WORKDIR "/src/Services/Chat/Chat.WebApi"
RUN dotnet build "Chat.WebApi.csproj" -c Release -o /app/build
RUN dotnet publish "Chat.WebApi.csproj" -c Release -o /app/publish --no-restore

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

ENTRYPOINT ["dotnet", "Chat.WebApi.dll"]

