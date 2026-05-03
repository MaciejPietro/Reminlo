<div align="center">
  <img src="assets/logo-1250x1250.png" width="120" alt="ResultKit logo" />
  <h1>Reminlo</h1>
</div>

<p align="center">
  🧱 Simple reminder app.
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-9.0-blue?logo=dotnet" />
  <img src="https://img.shields.io/badge/EF--Core-9.0-success?logo=entity-framework" />
  <img src="https://img.shields.io/badge/License-MIT-informational" />
</p>

---

## 📦 What is Reminlo?

**Reminlo** is a simple reminder app, build as .NET application development using Clean Architecture and modular principles.

It features CQRS, MediatR, authentication (JWT), background jobs (Hangfire), HealthChecks, Audit Logging, ResultKit, RepositoryKit, and more.  
Designed as scalable, maintainable, and testable enterprise solution.

---

## 🧰 Technologies Used

| Technology            | Purpose                      |
| --------------------- | ---------------------------- |
| ASP.NET Core 9        | Web API layer                |
| Entity Framework Core | Data persistence             |
| CQRS & MediatR        | Request/response separation  |
| FluentValidation      | Request validation           |
| Serilog               | Structured logging           |
| Hangfire              | Background jobs & scheduling |
| HealthChecks & UI     | Health endpoints             |
| ResultKit             | Result/Error handling        |
| RepositoryKit         | Generic repository pattern   |

---

## 🚀 Project Structure

```plaintext
📁 Reminlo
|
├── 📁 Reminlo.WebApi         → API layer (Controllers, middleware, DI)
├── 📁 Reminlo.Application    → CQRS, business rules, services, validation
├── 📁 Reminlo.Domain         → Domain models, entities, contracts
├── 📁 Reminlo.Infrastructure → Data access, Identity, integrations
```

## ⚙️ Getting Started

### 🚀 Installation

1. Update the connection string in appsettings.json

```json
"ConnectionStrings": {
  "DefaultConnection": "Your-Connection-String-Here"
}
```

2. Run the API:

```bash
dotnet run --project Reminlo.WebApi
```

- **Swagger:** [https://localhost:7294/swagger](https://localhost:7294/swagger)
- **HealthChecks UI:** [https://localhost:7294/health-ui](https://localhost:7294/health-ui)
- **Hangfire Dashboard:** [https://localhost:7294/hangfire](https://localhost:7294/hangfire)

## ✅ Benefits

- Separation of concerns with Clean Architecture
- Built-in CQRS with MediatR & validation
- Out-of-the-box JWT authentication and authorization
- Automatic health checks and monitoring UI
- Hangfire background jobs, with dashboard and DB authentication
- Full audit logging for all critical data changes and requests
- Generic repository and robust result patterns for maintainable code
- SOLID, modular, scalable, and easily testable structure

## 🔑 Authentication

- JWT-based user authentication (/api/Auth/login)
- Admin-only endpoints and role-based access out of the box
- Hangfire Dashboard login credentials managed via the database
