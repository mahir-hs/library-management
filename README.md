# Library Management System

> **Enterprise Software Engineer Technical Assessment** — Position: Software Engineer (.NET)
>
> **Status:** Backend API fully implemented; Frontend scaffolded (Angular 19, ready for UI implementation)
>
> **Last updated:** 2026-08-01

---

## Table of Contents

1. [Overview](#overview)
2. [Tech Stack](#tech-stack)
3. [Architecture](#architecture)
4. [Modules & Features](#modules--features)
5. [Project Structure](#project-structure)
6. [Prerequisites](#prerequisites)
7. [Setup & Running](#setup--running)
8. [Configuration](#configuration)
9. [API Documentation](#api-documentation)
10. [Testing](#testing)
11. [Database Migrations](#database-migrations)
12. [Assumptions & Design Decisions](#assumptions--design-decisions)
13. [Bonus Features](#bonus-features)
14. [Evaluation Criteria](#evaluation-criteria)
15. [Submission](#submission)

---

## Overview

A full-stack **Library Management System** built as a technical assessment for a Software Engineer (.NET) role. The system covers 7 functional modules — Authentication, Branch Management, Book Management, Member Management, Borrow & Return, Reservation Queue, and Reports — with a RESTful backend API and an Angular frontend SPA.

The backend is **complete** — all controllers, CQRS handlers, validators, middleware, and database migrations are implemented and working. The frontend is **scaffolded** with routing, guards, interceptors, models, and services — ready for UI component implementation.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| **Backend** | ASP.NET Core `.NET 10`, C# |
| **ORM** | Entity Framework Core + Npgsql (PostgreSQL) |
| **Database** | PostgreSQL 16+ |
| **Auth** | JWT Bearer tokens (symmetric key, HMACSHA256, 15-min access + refresh token) |
| **CQRS** | MediatR (commands + queries, separate handlers) |
| **Validation** | FluentValidation (pipeline behavior) |
| **Logging** | Serilog (console + rolling file) |
| **API Docs** | Swagger / OpenAPI (Swashbuckle) |
| **Frontend** | Angular 19 (standalone, reactive forms, plain CSS) |
| **Testing** | xUnit |
| **Password Hashing** | ASP.NET Identity PasswordHasher |

---

## Architecture

The project follows **Onion (Clean) Architecture** with the dependency rule: inner layers have zero knowledge of outer layers.

```
Frontend (Angular SPA)
       ↓
API (ASP.NET Core Controllers + Middleware)
       ↓
Application (MediatR Handlers, DTOs, Validators, Behaviors)
       ↓
Domain (Entities, Enums, Interfaces, Specifications, Result/PaginatedResult)
       ↓
Infrastructure (EF Core DbContext, Repositories, UnitOfWork, Auth/Token/Email Services)
```

### Key Principles

- **SOLID** — Single Responsibility, Open/Closed, Dependency Inversion throughout
- **Dependency Injection** — All services registered via DI container in `Program.cs`
- **CQRS** — Commands (write) and Queries (read) are separated via MediatR
- **Specification Pattern** — Encapsulates query logic for reusable, composable filters
- **Repository + Unit of Work** — Abstracts data access behind interfaces
- **FluentValidation** — Pipeline behavior validates all incoming requests automatically
- **Centralized Exception Handling** — Middleware catches all exceptions and returns standardized error responses
- **Asynchronous Programming** — All I/O operations use `async`/`await`

---

## Modules & Features

### 1. Authentication (JWT) & Role-based Authorization
- Register / Login with JWT token issuance
- Role-based access control (`Admin`, `Librarian`, `Member`)
- Refresh token support with revocation
- Password hashing via ASP.NET Identity `PasswordHasher`

### 2. Branch Management
- CRUD operations for library branches
- Branch activation/deactivation
- Search and filtering

### 3. Book Management
- CRUD operations for books (title, ISBN, author, category, publisher, year, language)
- Book copy management (barcode, status, shelf location per branch)
- Search by title, ISBN, author, category
- Pagination support

### 4. Member Management
- CRUD operations for library members
- Branch assignment
- Search and filtering

### 5. Borrow & Return Management
- Borrow a book copy (with due date = borrow date + 14 days)
- Return a book copy (with overdue fine calculation)
- Track active borrows, returned, and overdue records
- Max 5 active borrows per member

### 6. Reservation Queue
- Reserve a book when no copies are available
- FIFO queue with `PositionInQueue` auto-assigned by creation order
- Fulfill reservation when a copy is returned
- Cancel and expire reservations
- 48-hour expiry after book becomes available

### 7. Reports
- Borrow history with date filtering
- Overdue books report
- Member activity report

---

## Project Structure

```
library-management/
├── backend/
│   ├── LibraryManagement.slnx
│   ├── LibraryManagement.Api/              # ASP.NET Core Web API (Controllers, Middleware, DI, Swagger)
│   ├── LibraryManagement.Application/       # CQRS Handlers, DTOs, Validators, Behaviors, Mapping
│   ├── LibraryManagement.Domain/            # Entities, Enums, Interfaces, Specifications, Result
│   ├── LibraryManagement.Infrastructure/    # EF Core DbContext, Repositories, UnitOfWork, Auth/Token/Email Services
│   ├── LibraryManagement.Contracts/         # Shared contracts
│   ├── LibraryManagement.Tests/             # xUnit test project
│   ├── requirement.txt                      # Technical assessment requirements
│   ├── PROJECT.md                           # Detailed project overview
│   ├── ARCHITECTURE.md                      # Architecture decisions and patterns
│   ├── PLAN.md                              # Implementation plan
│   ├── PROGRESS.md                          # Progress tracker
│   ├── TASKS.md                             # Task breakdown
│   └── IMPLEMENTATION_STEPS.md              # Step-by-step implementation guide
├── frontend/
│   └── library-management-ui/               # Angular 19 SPA
│       ├── src/
│       │   ├── app/
│       │   │   ├── components/              # Auth, Dashboard, Branches, Books, Members, Borrows, Reservations, Reports
│       │   │   ├── guards/                  # Auth guard, Role guard
│       │   │   ├── interceptors/            # Auth interceptor, Error interceptor
│       │   │   ├── models/                  # TypeScript interfaces for all entities
│       │   │   ├── services/                # API service layer
│       │   │   └── shared/                  # Toast, Spinner components
│       │   ├── environments/                # Environment configs
│       │   └── styles.scss
│       ├── angular.json
│       ├── package.json
│       └── README.md
└── README.md                                # ← This file
```

---

## Prerequisites

| Tool | Version |
|------|---------|
| **.NET SDK** | 8.0+ (or .NET 10 SDK) |
| **Node.js** | 18+ |
| **Angular CLI** | 19+ (`npm install -g @angular/cli`) |
| **PostgreSQL** | 16+ |
| **Docker** (optional) | For containerized setup |

---

## Setup & Running

### 1. Clone the Repository

```bash
git clone <repository-url>
cd library-management
```

### 2. Backend Setup

#### a. Configure the Database

Edit `backend/LibraryManagement.Api/appsettings.json` and set your PostgreSQL connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=library_management;Username=postgres;Password=your_password"
  },
  "Jwt": {
    "Key": "your-super-secret-key-at-least-32-characters-long",
    "Issuer": "LibraryManagement",
    "Audience": "LibraryManagement",
    "ExpireMinutes": 15
  },
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/log-.txt", "rollingInterval": "Day" } }
    ]
  }
}
```

> **⚠️ Never commit secrets, passwords, or API keys to version control.** Use environment variables or user secrets for sensitive configuration.

#### b. Apply Migrations & Run the API

```bash
cd backend

# Restore NuGet packages
dotnet restore

# Apply database migrations
dotnet ef migrations apply --project LibraryManagement.Infrastructure --startup-project LibraryManagement.Api

# Run the API
dotnet run --project LibraryManagement.Api
```

The API runs on `http://localhost:5243` by default.

#### c. Seed Data (Optional)

The API seeds an initial admin user and sample data on first run. Check `Program.cs` or the seed data configuration for details.

### 3. Frontend Setup

```bash
cd frontend/library-management-ui

# Install dependencies
npm install

# Run the development server
ng serve
```

The frontend runs on `http://localhost:4200`.

### 4. Verify the Setup

1. Open Swagger UI at `http://localhost:5243/swagger` to explore and test the API endpoints.
2. Open the frontend at `http://localhost:4200` and log in with the seeded admin credentials.

---

## Configuration

### Environment Variables

The application supports the following environment variables (or use `appsettings.json`):

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core environment | `Development` |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | `Host=localhost;Port=5432;Database=library_management;Username=postgres;Password=postgres` |
| `Jwt__Key` | JWT signing key (min 32 chars) | — |
| `Jwt__ExpireMinutes` | Access token expiry in minutes | `15` |

### Frontend Environment

Edit `frontend/library-management-ui/src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5243/api/v1'
};
```

---

## API Documentation

The API is documented via **Swagger / OpenAPI** at `http://localhost:5243/swagger` when running in development.

### Base URL

```
http://localhost:5243/api/v1
```

### Authentication Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/auth/register` | Register a new user |
| `POST` | `/auth/login` | Login and receive JWT tokens |

### Module Endpoints

| Module | Base Path | Description |
|--------|-----------|-------------|
| Branches | `/branches` | CRUD for library branches |
| Books | `/books` | CRUD for books and book copies |
| Members | `/members` | CRUD for library members |
| Borrows | `/borrows` | Borrow/return operations |
| Reservations | `/reservations` | Reservation queue management |
| Reports | `/reports` | Borrow history, overdue, member activity |
| Users | `/users` | Current user info |

All endpoints (except `/auth/register` and `/auth/login`) require a valid JWT token in the `Authorization: Bearer <token>` header.

---

## Testing

```bash
cd backend

# Run all tests
dotnet test

# Run tests with verbose output
dotnet test --verbosity normal

# Run tests for a specific project
dotnet test LibraryManagement.Tests/LibraryManagement.Tests.csproj
```

The test project (`LibraryManagement.Tests`) covers domain logic, application handlers, and infrastructure services.

---

## Database Migrations

The project uses **Entity Framework Core** with PostgreSQL via Npgsql.

```bash
# Create a new migration
dotnet ef migrations add <MigrationName> --project LibraryManagement.Infrastructure --startup-project LibraryManagement.Api

# Apply migrations to the database
dotnet ef migrations apply --project LibraryManagement.Infrastructure --startup-project LibraryManagement.Api

# Revert the last migration
dotnet ef migrations remove --project LibraryManagement.Infrastructure --startup-project LibraryManagement.Api
```

---

## Assumptions & Design Decisions

### Authentication
- **JWT with symmetric key** (HMACSHA256) — simple and sufficient for an assessment project. In production, asymmetric keys (RSA) would be preferred.
- **15-minute access token + refresh token** — balances security (short-lived access tokens) with usability (no frequent re-logins).
- **Password hashing** via ASP.NET Identity `PasswordHasher` — uses PBKDF2 with HMAC-SHA256, 128-bit salt, 256-bit subkey.

### Authorization
- **Role-based** (Admin, Librarian, Member) — simple and meets the requirement. Attribute-based or policy-based authorization could be added for finer-grained control.

### Database
- **PostgreSQL** — robust, free, and widely used. EF Core provides excellent PostgreSQL support via Npgsql.
- **Logical deletes** are not implemented; soft delete can be added via a global query filter on `IsDeleted`.

### Borrow/Return
- **14-day borrow duration** — standard library lending period.
- **Max 5 active borrows per member** — prevents hoarding.
- **Overdue fine** — calculated on return (configurable rate).

### Reservation
- **FIFO queue** — fair and simple. Position is auto-assigned by creation timestamp.
- **48-hour expiry** — gives the next person in line a reasonable window to claim the book.

### CQRS
- **MediatR** is used for CQRS — commands and queries are separated into distinct request/response types with independent handlers. This improves testability and separation of concerns.

### Validation
- **FluentValidation** pipeline behavior — all DTOs are validated automatically before reaching handlers. No manual validation in controllers.

### Error Handling
- **Centralized exception handling middleware** — catches all unhandled exceptions and returns consistent JSON error responses with appropriate HTTP status codes.

### Logging
- **Serilog** with console and rolling file sinks — structured logging for debugging and monitoring.

---

## Bonus Features

The following bonus features have been implemented or are planned:

| Feature | Status |
|---------|--------|
| CQRS (MediatR) | ✅ Implemented |
| Domain Events | 🔲 Planned |
| Optimistic Concurrency | 🔲 Planned |
| API Versioning | 🔲 Planned |
| Health Checks | 🔲 Planned |
| Docker | 🔲 Planned |
| Redis (Caching) | 🔲 Planned |
| Background Jobs | 🔲 Planned |
| Excel/PDF Export | 🔲 Planned |
| Email Notifications | 🔲 Planned |
| CI/CD Pipeline | 🔲 Planned |

---

## Evaluation Criteria

This project is assessed against the following criteria (total: 100 marks):

| Category | Marks |
|----------|-------|
| Functional Requirements | 25 |
| Frontend Implementation | 10 |
| Architecture & Project Structure | 15 |
| Code Quality & Maintainability | 10 |
| SOLID & Dependency Injection | 10 |
| Design Patterns | 10 |
| Database Design | 5 |
| Security | 5 |
| Performance | 5 |
| Unit Testing | 5 |
| Documentation & Git Practices | 10 |

---

## Submission

This project is part of a technical assessment for the **Software Engineer (.NET)** position.

- **Repository:** `<repository-url>`
- **Backend:** ASP.NET Core REST API at `http://localhost:5243`
- **Frontend:** Angular SPA at `http://localhost:4200`
- **API Docs:** `http://localhost:5243/swagger`

### How to Run

1. Set up PostgreSQL and update the connection string in `appsettings.json`
2. Run `dotnet run --project backend/LibraryManagement.Api`
3. Run `ng serve` in `frontend/library-management-ui`
4. Open Swagger to explore the API
5. Open the frontend in a browser

---

## License

This project is created as part of a technical assessment and is not intended for production use.
