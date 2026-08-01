# Library Management System — Project Overview

> **Last updated:** 2026-08-01  
> **Status:** Backend API fully implemented and verified; Frontend scaffolded (Angular 19, routes empty, no UI components yet) — ready for frontend implementation  
> **Assessment:** Enterprise Software Engineer Technical Assessment (.NET) — July 2026

---

## 1. What Is This Project?

A full-stack **Library Management System** built as a technical assessment for a Software Engineer (.NET) role. It covers 7 functional modules with a RESTful backend API (ASP.NET Core) and an Angular frontend SPA.

The backend is **complete** — all 8 controllers, CQRS handlers, validators, middleware, and database migrations are implemented and working. The frontend is **scaffolded** but has no UI components, services, guards, or interceptors yet — this is where the next phase of work begins.

---

## 2. Tech Stack

| Layer | Technology |
|-------|-----------|
| **Backend** | ASP.NET Core `.NET 10`, C# |
| **ORM** | Entity Framework Core + Npgsql (PostgreSQL) |
| **Database** | PostgreSQL 16+ |
| **Auth** | JWT Bearer tokens (symmetric key, HMACSHA256, 15-min expiry) |
| **CQRS** | MediatR (commands + queries, separate handlers) |
| **Validation** | FluentValidation (pipeline behavior) |
| **Logging** | Serilog (console + rolling file) |
| **API Docs** | Swagger / OpenAPI (Swashbuckle) |
| **Frontend** | Angular 19 (standalone, reactive forms, plain CSS) |
| **Testing** | xUnit (test project exists, coverage in progress) |
| **Password Hashing** | ASP.NET Identity PasswordHasher |

---

## 3. Architecture — Onion (Clean Architecture)

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

**Key rule:** Domain layer has zero external dependencies. Infrastructure implements interfaces defined in Domain/Application.

### Projects in the Solution

| Project | Path | Responsibility |
|---------|------|---------------|
| `LibraryManagement.Domain` | `backend/LibraryManagement.Domain/` | Entities, enums, interfaces, specs, Result, PaginatedResult, AuditableEntity |
| `LibraryManagement.Application` | `backend/LibraryManagement.Application/` | CQRS handlers, DTOs, validators, behaviors (logging, validation), mapping |
| `LibraryManagement.Infrastructure` | `backend/LibraryManagement.Infrastructure/` | DbContext, EF configs, generic repository, unit of work, Auth/Token/CurrentUser services |
| `LibraryManagement.Api` | `backend/LibraryManagement.Api/` | Controllers, middleware, DI composition root, Program.cs, Swagger, JWT config |
| `LibraryManagement.Tests` | `backend/LibraryManagement.Tests/` | xUnit test project |
| `LibraryManagement.Contracts` | `backend/LibraryManagement.Contracts/` | Shared contracts project |
| `LibraryManagement.Web` | `frontend/library-management-ui/` | Angular SPA (scaffolded, routes empty, no components yet) |

---

## 4. Domain Entities

| Entity | Key Fields | Notes |
|--------|-----------|-------|
| `User` | Id (Guid), Username, Email, PasswordHash, FullName, PhoneNumber, Role (enum), IsActive, LastLoginAt, BranchId | Has `Member` navigation (1:1), `RefreshTokens` collection |
| `Branch` | Id, Name, Address, Phone, IsActive | Has BookCopies and Members |
| `Book` | Id, Title, ISBN, Description, Publisher, PublishedYear, Language, ImageUrl, AuthorId, CategoryId | Logical book; has many BookCopies |
| `BookCopy` | Id, BookId, BranchId, Barcode, Status (enum), ShelfLocation, AcquiredDate | Physical copy; has many BorrowRecords |
| `Member` | Id, FirstName, LastName, Email, Phone, MembershipDate, BranchId | Belongs to a Branch |
| `BorrowRecord` | Id, BookCopyId, MemberId, BorrowedDate, DueDate, ReturnedDate, Status (enum), FineAmount | Tracks a single borrow of one physical copy |
| `Reservation` | Id, BookId, MemberId, Status (enum), PositionInQueue, CreatedAt | FIFO waitlist when no copies available |
| `RefreshToken` | Id, Token, ExpiresAt, IsRevoked, UserId | JWT refresh token support |
| `Category` | Id, Name, Description | Book categorization |
| `Author` | Id, Name, Bio | Book author |

### Enums

- `UserRole` — `Admin`, `Librarian`, `Member`
- `BookCopyStatus` — `Available`, `Borrowed`, `UnderMaintenance`, `Lost`
- `BorrowStatus` — `Active`, `Returned`, `Overdue`
- `ReservationStatus` — `Pending`, `Fulfilled`, `Cancelled`, `Expired`

### Key Business Rules

| Rule | Value |
|------|-------|
| Max borrow duration | 14 days (DueDate = BorrowedDate + 14) |
| Max active borrows per member | 5 |
| Reservation queue | FIFO (PositionInQueue auto-assigned by CreatedAt) |
| Reservation expiry | 48 hours after book becomes available |
| Overdue threshold | DueDate < today |
| Duplicate reservation | Member cannot reserve a book they already have borrowed |
| Book ISBN | Unique across all books |
| Member email | Unique across all members |
| User email | Unique across all users |
| BookCopy barcode | Unique across all copies |

---

## 5. API Endpoints (All Controllers Implemented)

All endpoints use `api/v1/[controller]` routing and return `Result<T>` or `PaginatedResult<T>`.

### Auth
| Method | Endpoint | Auth | Roles |
|--------|----------|------|-------|
| POST | `/api/v1/auth/login` | No | — |
| POST | `/api/v1/auth/register` | No | — |
| POST | `/api/v1/auth/refresh-token` | No | — |

### Users (Current User Profile)
| Method | Endpoint | Auth | Roles |
|--------|----------|------|-------|
| GET | `/api/v1/users/me` | Yes | All |

### Branches
| Method | Endpoint | Auth | Roles |
|--------|----------|------|-------|
| GET | `/api/v1/branches` | Yes | All |
| GET | `/api/v1/branches/:id` | Yes | All |
| POST | `/api/v1/branches` | Yes | Admin, Librarian |
| PUT | `/api/v1/branches/:id` | Yes | Admin, Librarian |
| DELETE | `/api/v1/branches/:id` | Yes | Admin |

### Books
| Method | Endpoint | Auth | Roles |
|--------|----------|------|-------|
| GET | `/api/v1/books` | Yes | All |
| GET | `/api/v1/books/:id` | Yes | All |
| GET | `/api/v1/books/search` | Yes | All |
| GET | `/api/v1/books/available` | Yes | All |
| POST | `/api/v1/books` | Yes | Admin, Librarian |
| PUT | `/api/v1/books/:id` | Yes | Admin, Librarian |
| DELETE | `/api/v1/books/:id` | Yes | Admin |

### Members
| Method | Endpoint | Auth | Roles |
|--------|----------|------|-------|
| GET | `/api/v1/members` | Yes | Admin, Librarian |
| GET | `/api/v1/members/:id` | Yes | All (own) or Admin/Librarian |
| POST | `/api/v1/members` | Yes | Admin, Librarian |
| PUT | `/api/v1/members/:id` | Yes | Admin, Librarian |
| DELETE | `/api/v1/members/:id` | Yes | Admin |

### Borrows
| Method | Endpoint | Auth | Roles |
|--------|----------|------|-------|
| POST | `/api/v1/borrows` | Yes | Admin, Librarian |
| GET | `/api/v1/borrows/:id` | Yes | All |
| GET | `/api/v1/borrows/mine` | Yes | Member |
| GET | `/api/v1/borrows/member/:memberId` | Yes | Admin, Librarian |
| GET | `/api/v1/borrows/overdue` | Yes | Admin, Librarian |
| PUT | `/api/v1/borrows/:id/return` | Yes | Admin, Librarian |

### Reservations
| Method | Endpoint | Auth | Roles |
|--------|----------|------|-------|
| GET | `/api/v1/reservations` | Yes | All |
| POST | `/api/v1/reservations` | Yes | Member |
| PATCH | `/api/v1/reservations/:id/cancel` | Yes | Owner |
| PATCH | `/api/v1/reservations/:id/fulfill` | Yes | Admin, Librarian |

### Reports
| Method | Endpoint | Auth | Roles |
|--------|----------|------|-------|
| GET | `/api/v1/reports/borrow-history` | Yes | Admin, Librarian |
| GET | `/api/v1/reports/overdue` | Yes | Admin, Librarian |
| GET | `/api/v1/reports/member-activity/:memberId` | Yes | Admin, Librarian |

### Other
| Method | Endpoint | Auth |
|--------|----------|------|
| GET | `/health` | No |
| GET | `/info` | No |

---

## 6. Design Patterns Used

| Pattern | Where |
|---------|-------|
| **CQRS (MediatR)** | Separate `IRequest<T>` commands and `IRequest<T>` queries with dedicated handlers |
| **Repository** | `IRepository<T>` generic interface + `GenericRepository<T>` implementation |
| **Unit of Work** | `IUnitOfWork` wrapping `DbContext.SaveChangesAsync()` |
| **Specification** | `SpecificationBase<T>` with `Criteria`, `Includes`, `OrderBy`, pagination |
| **Pipeline Behaviors** | `ValidationBehavior` (FluentValidation) and `LoggingBehavior` (MediatR) |
| **Strategy** | Swappable notification strategies (email via SMTP) |
| **Factory** | Borrow record creation with default values (dueDate, status) |
| **Adapter** | `TokenService` adapts JWT library output to `IAuthService` interface |
| **Result Pattern** | `Result<T>` and `PaginatedResult<T>` for consistent API responses |
| **Auditable Entity** | `AuditableEntity` base class with `CreatedAt`, `CreatedBy`, `ModifiedAt`, `ModifiedBy` |

---

## 7. Frontend (Angular) — Implementation Guide

### Current State

The Angular 19 standalone project is scaffolded at `frontend/library-management-ui/` with:
- `app.routes.ts` — empty (`routes: []`)
- `app.component.ts` — root component with placeholder template
- `app.config.ts` — basic providers (zone detection, router)
- No components, services, guards, interceptors, or models yet
- Plain CSS (no framework)
- `ng serve` runs on `http://localhost:4200`

### Implementation Roadmap (11 tasks, ~16 hours)

Follow this order — each task builds on the previous one:

| Task | Description | Hours | Priority |
|------|-------------|-------|----------|
| **T-022** | Scaffold: routing, layout component, login page, HTTP client with interceptors | 2 | 🔴 Critical |
| **T-023** | Auth service + API service layer (all 7 module services) | 1 | 🔴 Critical |
| **T-024** | Dashboard with summary cards (role-based) | 1 | 🔴 Critical |
| **T-025** | Branch CRUD pages (list, detail, form, delete) | 2 | 🟡 High |
| **T-026** | Book CRUD pages with search and pagination | 2 | 🟡 High |
| **T-027** | Member CRUD pages with branch selector | 2 | 🟡 High |
| **T-028** | Borrow/Return pages (create borrow, return book, my borrows) | 2 | 🟡 High |
| **T-029** | Reservation Queue page (fulfill/cancel, queue position) | 1 | 🟡 High |
| **T-030** | Reports page (borrow history, overdue, member activity) | 1 | 🟢 Medium |
| **T-031** | Responsive design & polish (mobile, toast, spinner, loading) | 1 | 🟢 Medium |
| **T-032** | Write README.md for the frontend project | 1 | 🟢 Medium |

### Frontend File Structure (Target)

```
frontend/library-management-ui/src/app/
├── components/
│   ├── layout/
│   │   ├── app-layout.component.ts
│   │   ├── app-layout.component.html
│   │   ├── app-layout.component.scss
│   │   ├── sidebar.component.ts
│   │   └── header.component.ts
│   ├── auth/
│   │   ├── login.component.ts
│   │   ├── login.component.html
│   │   └── login.component.scss
│   ├── dashboard/
│   │   ├── dashboard.component.ts
│   │   ├── dashboard.component.html
│   │   └── dashboard.component.scss
│   ├── branches/
│   │   ├── branch-list.component.ts
│   │   ├── branch-detail.component.ts
│   │   └── branch-form.component.ts
│   ├── books/
│   │   ├── book-list.component.ts
│   │   ├── book-detail.component.ts
│   │   └── book-form.component.ts
│   ├── members/
│   │   ├── member-list.component.ts
│   │   ├── member-detail.component.ts
│   │   └── member-form.component.ts
│   ├── borrows/
│   │   ├── borrow-list.component.ts
│   │   ├── borrow-form.component.ts
│   │   └── my-borrows.component.ts
│   ├── reservations/
│   │   ├── reservation-list.component.ts
│   │   └── reservation-queue.component.ts
│   └── reports/
│       ├── borrow-history.component.ts
│       ├── overdue.component.ts
│       └── member-activity.component.ts
├── guards/
│   ├── auth.guard.ts
│   └── role.guard.ts
├── interceptors/
│   ├── auth-interceptor.ts
│   └── error-interceptor.ts
├── services/
│   ├── auth.service.ts
│   ├── branch.service.ts
│   ├── book.service.ts
│   ├── member.service.ts
│   ├── borrow.service.ts
│   ├── reservation.service.ts
│   └── report.service.ts
├── models/
│   ├── user.model.ts
│   ├── branch.model.ts
│   ├── book.model.ts
│   ├── member.model.ts
│   ├── borrow.model.ts
│   ├── reservation.model.ts
│   └── report.model.ts
├── shared/
│   ├── header.component.ts
│   ├── sidebar.component.ts
│   ├── spinner.component.ts
│   └── toast.component.ts
├── app.routes.ts
└── app.component.ts
```

### Key Implementation Details

**Authentication Flow:**
- `AuthService` stores JWT token in `sessionStorage`
- `AuthGuard` redirects to `/login` if no token
- `RoleGuard` redirects if user lacks required role
- HTTP interceptor attaches `Authorization: Bearer <token>` header
- HTTP interceptor handles 401 (redirect to login) and 4xx/5xx (show toast)

**API Service Pattern:**
Each service wraps `HttpClient` calls and returns `Observable<T>`:
```typescript
// Example: branch.service.ts
getBranches(): Observable<Result<BranchDto[]>> {
  return this.http.get<Result<BranchDto[]>>(`${this.apiUrl}/branches`);
}
```

**Routing Configuration:**
```typescript
// app.routes.ts — after T-022
export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: '', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: 'branches', component: BranchListComponent, canActivate: [AuthGuard] },
  { path: 'books', component: BookListComponent, canActivate: [AuthGuard] },
  { path: 'members', component: MemberListComponent, canActivate: [AuthGuard] },
  { path: 'borrows', component: BorrowListComponent, canActivate: [AuthGuard] },
  { path: 'reservations', component: ReservationListComponent, canActivate: [AuthGuard] },
  { path: 'reports', component: ReportsPageComponent, canActivate: [AuthGuard, RoleGuard, { data: { roles: ['Admin', 'Librarian'] } }] },
  { path: '**', redirectTo: '' }
];
```

**Environment Configuration:**
```typescript
// environment.ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5243/api/v1'
};
```

---

## 8. Key Files Reference

### Backend — Most Important Files

| File | Purpose |
|------|---------|
| `LibraryManagement.Api/Program.cs` | Composition root — DI, middleware pipeline, JWT, CORS, Swagger, Serilog |
| `LibraryManagement.Api/Controllers/` | 8 controllers: Auth, Users, Books, Borrows, Branches, Members, Reports, Reservations |
| `LibraryManagement.Domain/Entities/` | 10 entity files (User, Book, BookCopy, Branch, Member, BorrowRecord, Reservation, etc.) |
| `LibraryManagement.Domain/Common/` | BaseEntity, AuditableEntity, IEntity, Result, PaginatedResult |
| `LibraryManagement.Domain/Enums/` | UserRole, BookCopyStatus, BorrowStatus, ReservationStatus |
| `LibraryManagement.Application/Features/` | CQRS commands + queries + handlers for all 7 modules |
| `LibraryManagement.Application/DTOs/` | All input/output DTOs per module |
| `LibraryManagement.Application/Validators/` | FluentValidation validators for all input DTOs |
| `LibraryManagement.Application/Common/Behaviors/` | ValidationBehavior, LoggingBehavior |
| `LibraryManagement.Application/Common/Specifications/` | SpecificationBase, PredicateBuilder, per-entity specs |
| `LibraryManagement.Infrastructure/Persistence/Context/ApplicationDbContext.cs` | EF Core DbContext |
| `LibraryManagement.Infrastructure/Persistence/Configurations/` | 10 Fluent API entity configurations |
| `LibraryManagement.Infrastructure/Persistence/Seed/SeedData.cs` | Database seeding (roles, admin user, branches) |
| `LibraryManagement.Infrastructure/Repositories/GenericRepository.cs` | Generic repository implementation |
| `LibraryManagement.Infrastructure/Repositories/UnitOfWork.cs` | Unit of Work implementation |
| `LibraryManagement.Infrastructure/Services/AuthService.cs` | JWT generation and validation |
| `LibraryManagement.Infrastructure/Services/TokenService.cs` | Token creation and refresh |
| `LibraryManagement.Infrastructure/Services/CurrentUserService.cs` | Current user identity from claims |
| `LibraryManagement.Infrastructure/DependencyInjection/DependencyInjection.cs` | Infrastructure service registration |
| `LibraryManagement.Api/Middleware/ExceptionHandlingMiddleware.cs` | Global exception handling |

### Frontend — Key Files

| File | Purpose |
|------|---------|
| `frontend/library-management-ui/src/app/app.routes.ts` | Angular routes (currently empty — to be populated in T-022) |
| `frontend/library-management-ui/src/app/app.component.ts` | Root component |
| `frontend/library-management-ui/src/app/app.config.ts` | Providers (HTTP client, interceptors, guards) |
| `frontend/library-management-ui/src/app/app.component.html` | Root template (currently placeholder) |

### Documentation Files

| File | Purpose |
|------|---------|
| `PROJECT.md` | This file — comprehensive project overview and frontend implementation guide |
| `requirement.txt` | Original technical assessment requirements |
| `PLAN.md` | Master plan with timeline, milestones, evaluation criteria |
| `ARCHITECTURE.md` | Architecture decisions, layer design, security, API design |
| `IMPLEMENTATION_STEPS.md` | Step-by-step implementation guide (8 phases) |
| `TASKS.md` | Granular task breakdown (36 tasks, T-001 to T-036) |

---

## 9. Current Implementation Status

| Module | Backend | Frontend | Tests |
|--------|---------|----------|-------|
| Auth (JWT + Roles) | ✅ Done | ❌ Not started | ❌ Not started |
| Users (Profile) | ✅ Done | ❌ Not started | ❌ Not started |
| Branch Management | ✅ Done | ❌ Not started | ❌ Not started |
| Book Management | ✅ Done | ❌ Not started | ❌ Not started |
| Member Management | ✅ Done | ❌ Not started | ❌ Not started |
| Borrow & Return | ✅ Done | ❌ Not started | ❌ Not started |
| Reservation Queue | ✅ Done | ❌ Not started | ❌ Not started |
| Reports | ✅ Done | ❌ Not started | ❌ Not started |
| Swagger/OpenAPI | ✅ Done | — | — |
| Exception Handling Middleware | ✅ Done | — | — |
| Serilog Logging | ✅ Done | — | — |
| Health Checks | ✅ Done | — | — |
| CORS Configuration | ✅ Done | — | — |
| Seed Data | ✅ Done | — | — |
| Angular Scaffold | — | ✅ Scaffolded | — |
| Angular UI Components | — | ❌ Not started | — |
| Unit Tests | ❌ Not started | — | ❌ Not started |
| Docker | ❌ Not started | — | — |
| CI/CD | ❌ Not started | — | — |

---

## 10. Key Configuration

### JWT Settings (from appsettings.json)
- **Secret:** From environment variable `JWT_SECRET` (min 32 chars)
- **Expiry:** 15 minutes
- **Issuer:** `LibraryManagementApi`
- **Audience:** `LibraryManagementClient`
- **Algorithm:** HMACSHA256 (symmetric key)

### Database
- **Provider:** PostgreSQL via Npgsql
- **Connection string:** From environment variable or `appsettings.json`
- **Migrations:** EF Core migrations in Infrastructure project

### CORS
- Allows `http://localhost:4200`, `http://localhost:3000`, `http://127.0.0.1:4200`
- Allows any method, any header, credentials

### Serilog
- Minimum level: Information
- Sinks: Console + rolling file (`logs/log-.txt`)
- Enriches from log context

---

## 11. Evaluation Criteria (from requirement.txt)

| Criteria | Marks | Status |
|----------|-------|--------|
| Functional Requirements | 25 | All 7 modules implemented in backend |
| Frontend Implementation | 10 | Scaffold only — needs full UI |
| Architecture & Project Structure | 15 | Onion architecture, clean separation |
| Code Quality & Maintainability | 10 | In progress |
| SOLID & Dependency Injection | 10 | DI throughout, interfaces for everything |
| Design Patterns | 10 | CQRS, Repository, Specification, Strategy, Factory, Adapter |
| Database Design | 5 | Normalized, proper relationships |
| Security | 5 | JWT + role-based auth, no secrets in repo |
| Performance | 5 | Async all I/O, efficient queries |
| Unit Testing | 5 | Not started |
| Documentation & Git Practices | 10 | In progress |
| **Total** | **100** | |

---

## 12. Assumptions & Open Questions

### Confirmed Assumptions
- Frontend: Angular 19 (standalone components)
- Backend: .NET 10
- Database: PostgreSQL
- ORM: Entity Framework Core
- CQRS/MediatR: Yes
- Validation: FluentValidation
- Logging: Serilog
- Testing: xUnit
- Email: Full SMTP integration (not just interface)
- Reports: Table-based for now
- Book copy model: Multi-copy (books have multiple physical copies per branch)
- Frontend CSS: Plain CSS (no framework)
- Reservation Queue: FIFO waitlist
- Token storage: `sessionStorage` (trade-off vs `localStorage` — sessionStorage clears on tab close, more secure)

### Open Questions (from PLAN.md)
- Exact role definitions and permissions matrix — needs confirmation
- Reservation expiry handling — background job vs manual for MVP
- Soft delete vs hard delete for books — needs decision
- Member can return own borrows or Admin/Librarian only — needs confirmation

---

## 13. Quick Start (for future sessions)

### Prerequisites
- .NET SDK 10+
- Node.js 18+
- Angular CLI 19+
- PostgreSQL 16+

### Run Backend
```bash
cd backend
dotnet restore
dotnet ef database update
dotnet run --project LibraryManagement.Api
# Swagger UI: http://localhost:5243/
```

### Run Frontend
```bash
cd frontend/library-management-ui
npm install
ng serve
# App: http://localhost:4200/
```

### Run Tests
```bash
cd backend
dotnet test
```

---

## 14. Git History Summary

| Commit | Description |
|--------|-------------|
| `29a1005` | Add authentication, controllers, and seed data |
| `46e9a0b` | imple: configs and swagger setup |
| `71a26fa` | feat: implement user management features with CurrentUserService and DbContext setup |
| `78b6653` | feat: add initial entity configurations and DbContext for library management |
| `28143cd` | feat(domain): add library management entities |

Working tree has many modified and untracked files — the project is in active development. Key untracked/new files include `UsersController.cs`, `BaseController.cs`, and database migrations.
