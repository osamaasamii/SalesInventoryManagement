# Sales & Inventory Management API

A RESTful Web API for managing sales, inventory, and orders — built with ASP.NET Core and Clean Architecture.

## 📋 Overview

This project simulates a real-world business system where customers can browse products, place orders, and have their inventory automatically tracked. It was built as a learning project to apply production-grade backend practices: layered architecture, secure authentication, validated input, centralized error handling, and automated testing.

## ✨ Features

- **Product, Category, and Customer management** — full CRUD operations
- **Order processing** with real-time stock validation and automatic inventory deduction, executed as a single atomic transaction
- **JWT Authentication** with ASP.NET Core Identity, including role-based authorization (`Admin` / `Customer`)
- **Request validation** using FluentValidation, with clear, localized error messages
- **Object mapping** between Entities and DTOs via AutoMapper
- **Centralized Global Exception Handling** using the `IExceptionHandler` interface and RFC 7807 `ProblemDetails` responses
- **Unit Tests** covering validation rules and critical business logic (stock checks, order totals)

## 🛠️ Tech Stack

| Category | Technology |
|---|---|
| Framework | .NET 8, ASP.NET Core Web API |
| Database | SQL Server, Entity Framework Core (Code-First) |
| Authentication | ASP.NET Core Identity, JWT Bearer |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Testing | xUnit, Moq |
| Documentation | Swagger / OpenAPI |

## 🏗️ Architecture

The project follows **Clean Architecture**, split into four layers with a strict dependency rule: outer layers depend on inner layers, never the reverse.

```
SalesInventoryManagement.Domain          → Entities & core business rules (no external dependencies)
SalesInventoryManagement.Application     → Interfaces, DTOs, Services, Validators (business logic)
SalesInventoryManagement.Infrastructure  → EF Core, Identity, Repository implementations
SalesInventoryManagement.API             → Controllers, middleware, DI configuration
SalesInventoryManagement.Tests           → Unit tests (xUnit + Moq)
```

**Design patterns applied:**
- **Repository Pattern** — abstracts data access per entity
- **Unit of Work** — coordinates multiple repositories under a single `DbContext` and a single `SaveChangesAsync()` call, ensuring atomicity
- **Dependency Injection** — every layer depends on abstractions (interfaces), not concrete implementations

## 📁 Project Structure

```
SalesInventoryManagement.Domain/
└── Entities/          (Product, Category, Customer, Order, OrderItem)

SalesInventoryManagement.Application/
├── DTOs/
├── Interfaces/
├── Services/
├── Validators/
├── Mapping/
└── Exceptions/         (NotFoundException, BusinessRuleException, UnauthorizedException)

SalesInventoryManagement.Infrastructure/
├── Data/               (ApplicationDbContext)
├── Identity/            (ApplicationUser)
├── Repositories/        (GenericRepository, UnitOfWork, OrderRepository)
└── Services/            (JwtService, AuthService)

SalesInventoryManagement.API/
├── Controllers/
└── Middleware/           (GlobalExceptionHandler)

SalesInventoryManagement.Tests/
├── Validators/
└── Services/
```

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB or Developer Edition)
- Visual Studio 2022 (or any IDE with .NET support)

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/osamaasamii/Sales-Inventory-Management-API.git
   ```

2. **Configure secrets** (connection string and JWT key are not committed to source control)

   Right-click the API project → **Manage User Secrets**, and add:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=SalesInventoryDb;Trusted_Connection=True;TrustServerCertificate=True"
     },
     "Jwt": {
       "Key": "YourOwnSecretKeyAtLeast16CharsLong"
     }
   }
   ```

3. **Apply migrations**

   In Package Manager Console (Default project: `Infrastructure`):
   ```
   Update-Database
   ```

4. **Run the project**

   Press `F5`. Swagger UI will open automatically at `/swagger`.

### Running Tests

```bash
dotnet test
```

## 📡 API Overview

| Controller | Endpoints | Auth Required |
|---|---|---|
| `AuthController` | `POST /api/Auth/register`, `POST /api/Auth/login` | No |
| `ProductsController` | `GET`, `GET /{id}`, `POST` | No |
| `CategoriesController` | `GET`, `GET /{id}`, `POST` | No |
| `CustomersController` | `GET`, `GET /{id}`, `POST` | No |
| `OrdersController` | `GET`, `GET /{id}`, `POST` | ✅ Yes (JWT) |

To call protected endpoints in Swagger, click **Authorize** and paste the JWT token returned from `/login` (without the `Bearer` prefix).

## 🗺️ Roadmap

This project is actively developed. Planned next steps:

- [ ] Payment Gateway integration (Stripe)
- [ ] Containerization with Docker
- [ ] Caching with Redis
- [ ] Background processing with RabbitMQ
- [ ] Cloud deployment to Azure

## 👤 Author

**Osama Sami** — Backend Developer (.NET / ASP.NET Core)
