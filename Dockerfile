# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and project files first (layer caching)
COPY ProductsAPI.slnx ./
COPY src/Domain/ProductsAPI.Domain.csproj             src/Domain/
COPY src/Application/ProductsAPI.Application.csproj   src/Application/
COPY src/Infrastructure/ProductsAPI.Infrastructure.csproj src/Infrastructure/
COPY src/API/ProductsAPI.API.csproj                   src/API/
COPY tests/API.Tests/ProductsAPI.API.Tests.csproj     tests/API.Tests/
COPY tests/Application.Tests/ProductsAPI.Application.Tests.csproj tests/Application.Tests/
COPY tests/Infrastructure.Tests/ProductsAPI.Infrastructure.Tests.csproj tests/Infrastructure.Tests/

# Restore all packages
RUN dotnet restore src/API/ProductsAPI.API.csproj

# Copy everything and build
COPY . .
RUN dotnet publish src/API/ProductsAPI.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Stage 2: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create log directory
RUN mkdir -p /app/logs

# Copy published output
COPY --from=build /app/publish .

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "ProductsAPI.API.dll"]
