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

A self-hosted family coordination tool. Create a workspace, invite your family, and stop forgetting the things that matter.

## What it does

**Recurring reminders** — define obligations that repeat on a schedule: car service every 2 years, annual medical tests, insurance renewals, home system checks. Set how many days before you want an email reminder. When it is due, convert the template into a real event with one click.

**Family calendar** — create events with a title, time, location, and optional reminder. Events can be private (only you see them) or shared with the whole workspace. Assign events to specific family members to control who gets notified.

## Why it exists

Too many recurring things to track: 2 kids, 2 cars, 2 pets, insurance policies, medical tests, house systems. Google Calendar handles single events fine but offers no structured way to manage obligations that repeat every 1–3 years with advance reminders.

## Current scope

- User registration and workspace management
- Recurring pattern templates with email reminders
- Workspace calendar with per-event privacy and notification settings
- Email delivery via SendGrid

**Not yet implemented:** conflict detection, SMS/push notifications, recurring calendar events, event proposals, gamification.

## Status

Early development. Built as a pet project, open to contributions.

---

# 🧰 Technical aspects

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
