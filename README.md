# ProductsAPI — RESTful Backend API

A production-grade RESTful API built with **ASP.NET Core 9** following **Clean Architecture** principles, featuring JWT authentication, Entity Framework Core, and Docker support.

## 🏗 Architecture

```
Solution/
├── src/
│   ├── API/            ← ASP.NET Core Web API (Controllers, Middleware, Filters)
│   ├── Application/    ← Business logic (Services, DTOs, Validators, Interfaces)
│   ├── Domain/         ← Domain entities, exceptions, events, enums
│   └── Infrastructure/ ← EF Core, Repositories, Identity, JWT Auth
└── tests/
    ├── API.Tests/              ← Integration tests (WebApplicationFactory)
    ├── Application.Tests/      ← Unit tests (Moq + xUnit)
    └── Infrastructure.Tests/   ← Repository tests (EF InMemory)
```

## 🚀 Quick Start

### Option 1 — Docker Compose (Recommended)

```bash
docker compose up --build
```
The API will be available at **http://localhost:8080** with Swagger UI at the root.

### Option 2 — Local (requires SQL Server)

1. Update the connection string in `src/API/appsettings.json`
2. Run migrations:
   ```bash
   dotnet ef database update --project src/Infrastructure --startup-project src/API
   ```
3. Start the API:
   ```bash
   dotnet run --project src/API
   ```

## 🔑 Default Credentials

| Role  | Email                     | Password   |
|-------|---------------------------|------------|
| Admin | admin@productsapi.com     | Admin@123  |

> Seeded automatically on first startup.

## 📖 API Endpoints

### Authentication

| Method | Endpoint                         | Description           |
|--------|----------------------------------|-----------------------|
| POST   | `/api/v1/auth/register`          | Register new user     |
| POST   | `/api/v1/auth/login`             | Login & get tokens    |
| POST   | `/api/v1/auth/refresh-token`     | Refresh access token  |
| POST   | `/api/v1/auth/revoke`            | Revoke refresh token  |

### Products

| Method | Endpoint                         | Auth   | Description            |
|--------|----------------------------------|--------|------------------------|
| GET    | `/api/v1/products`               | Public | Get all (paginated)    |
| GET    | `/api/v1/products/{id}`          | Public | Get by ID with items   |
| POST   | `/api/v1/products`               | User   | Create product         |
| PUT    | `/api/v1/products/{id}`          | User   | Update product         |
| DELETE | `/api/v1/products/{id}`          | Admin  | Delete product         |

### Items

| Method | Endpoint                             | Auth   | Description              |
|--------|--------------------------------------|--------|--------------------------|
| GET    | `/api/v1/products/{productId}/items` | Public | Get items for product    |
| GET    | `/api/v1/items/{id}`                 | Public | Get item by ID           |
| POST   | `/api/v1/items`                      | User   | Create item              |
| PUT    | `/api/v1/items/{id}`                 | User   | Update item              |
| DELETE | `/api/v1/items/{id}`                 | Admin  | Delete item              |

## 🔐 Authentication Flow

```
POST /api/v1/auth/login
    → { accessToken, refreshToken, accessTokenExpiry }

Use: Authorization: Bearer <accessToken>

POST /api/v1/auth/refresh-token (when access token expires)
    Body: { accessToken, refreshToken }
    → new { accessToken, refreshToken }
```

## 🛠 Tech Stack

| Component       | Technology                                        |
|-----------------|---------------------------------------------------|
| Framework       | ASP.NET Core 9                                    |
| ORM             | Entity Framework Core 9 with SQL Server           |
| Authentication  | JWT Bearer + Refresh Token rotation               |
| Validation      | FluentValidation 11                               |
| Mapping         | AutoMapper 14                                     |
| Logging         | Serilog (Console + Rolling File)                  |
| Documentation   | Swagger / OpenAPI (Swashbuckle 8)                 |
| API Versioning  | Asp.Versioning 8.1                                |
| Testing         | xUnit + Moq + WebApplicationFactory + EF InMemory |
| Containerization| Docker + Docker Compose                           |

## 🧪 Running Tests

```bash
# All tests
dotnet test

# Unit tests only
dotnet test tests/Application.Tests

# Infrastructure tests
dotnet test tests/Infrastructure.Tests

# Integration tests (require API builds)
dotnet test tests/API.Tests
```

## 🐳 Docker

```bash
# Build and run with SQL Server
docker compose up --build

# Stop
docker compose down

# Remove volumes (clean slate)
docker compose down -v
```

## 📊 Database Schema

```sql
CREATE TABLE [dbo].[Product] (
    [Id]          INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [ProductName] NVARCHAR(255) NOT NULL,
    [CreatedBy]   NVARCHAR(100) NOT NULL,
    [CreatedOn]   DATETIME      NOT NULL,
    [ModifiedBy]  NVARCHAR(100) NULL,
    [ModifiedOn]  DATETIME      NULL
)

CREATE TABLE [dbo].[Item] (
    [Id]        INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [ProductId] INT NOT NULL FOREIGN KEY REFERENCES Product(Id),
    [Quantity]  INT NOT NULL
)
```

## ⚙️ Configuration

Key settings in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ProductsApiDb;..."
  },
  "Jwt": {
    "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "ProductsAPI",
    "Audience": "ProductsAPIClients",
    "ExpiryMinutes": 30,
    "RefreshTokenExpiryDays": 7
  }
}
```

> ⚠️ **Production**: Replace `Jwt:Secret` with a secure random key and store it in environment variables or a secrets vault.

## 🔒 Security Features

- JWT with short-lived access tokens (30 min) and refresh token rotation (7 days)
- Role-based authorization (Admin, User, ReadOnly)
- FluentValidation on all inputs
- CORS policy
- HSTS / HTTPS redirect
- Response compression
- Global exception handling (no stack traces in responses)
- Account lockout after 5 failed attempts
