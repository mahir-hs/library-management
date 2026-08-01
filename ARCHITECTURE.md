# Architecture — Library Management System

> **Status:** Draft — all decisions marked with `[DECISION]` or `[ASSUMPTION]` and should be validated before implementation begins.

---

## 1. Architectural Style: Onion Architecture (Clean Architecture)

The assessment explicitly requires Clean/Onion Architecture. This means:

- **Domain layer** has zero dependencies on infrastructure or frameworks.
- **Application layer** depends only on the Domain layer.
- **Infrastructure layer** depends on the Application layer (implements interfaces defined in Domain/Application).
- **Presentation layer** (API + Frontend) depends on Application and Infrastructure via DI.

### Layer dependency direction (always inward):

```
Frontend (Presentation)
        ↓
    API (Presentation)
        ↓
  Application (Use Cases)
        ↓
    Domain (Entities, Rules)
```

Infrastructure is a peer of Application — it provides implementations for interfaces defined in Domain/Application.

---

## 2. Layer Responsibilities

### Layer 1: Domain (`LibraryManagement.Domain`)
**Zero external dependencies.**

| What | Why |
|------|-----|
| Entities (User, Book, Branch, Member, BorrowRecord, Reservation) | Core business models |
| Interfaces (IRepository, IUnitOfWork, ICurrentUserService) | Contracts that outer layers must fulfill |
| Enums (Role, ReservationStatus, BorrowStatus) | Domain vocabularies — no UI or framework references |
| Value Objects (Money, Address) | Immutable domain concepts |
| Domain Events (optional, bonus) | For domain events like `BookBorrowedEvent` |
| SpecBase / Specification pattern base class | For query encapsulation |

**Key rule:** Domain layer must NOT reference `Microsoft.EntityFrameworkCore`, `MediatR`, or any framework namespace.

### Layer 2: Application (`LibraryManagement.Application`)
**Depends on Domain only.**

| What | Why |
|------|-----|
| DTOs (Input/Output) | Decouples API shape from domain models |
| Handlers (CQRS — IRequestHandler) | Orchestrates use cases, calls repositories |
| Validators (FluentValidation) | Input validation rules |
| Mapping profiles | Maps between DTOs ↔ Domain entities |
| Behaviors (MediatR pipeline) | Cross-cutting concerns (logging, validation, transacted unit of work) |
| Result Wrapper (`Result<T>`) | Consistent API response shape |
| Custom Exceptions | Domain-specific error types |

### Layer 3: Infrastructure (`LibraryManagement.Infrastructure`)
**Depends on Application and Domain.**

| What | Why |
|------|-----|
| EfCore DbContext + Configuration | ORM setup with Fluent API |
| GenericRepository<T> / IRepository<T> | Base data access |
| BranchRepository, BookRepository, etc. | Entity-specific query methods (Specification pattern) |
| UnitOfWork (EfUnitOfWork) | Transaction boundary |
| AuthService, TokenService | JWT generation and validation |
| EmailService (interface + implementation via SMTP) | Confirmed — full SMTP email integration |
| Serilog configuration | Structured logging setup |

### Layer 4: Presentation (API + Frontend)
**Depends on Application and Infrastructure.**

| Component | Role |
|-----------|------|
| `LibraryManagement.API` | ASP.NET Core Web API — controllers, middleware, DI composition root |
| `LibraryManagement.Web` | Angular SPA — components, services, routing |

---

## 3. Technology Decisions

| Decision | Choice | Rationale | Status |
|----------|--------|-----------|--------|
| Backend framework | ASP.NET Core `.NET 10` | Confirmed | `[DECISION]` |
| ORM | Entity Framework Core (latest for .NET 10) | Recommended in assessment | `[DECISION]` |
| Database | PostgreSQL 16+ | Confirmed | `[DECISION]` |
| API pattern | RESTful resources | Standard for assessment | `[DECISION]` |
| Auth mechanism | JWT Bearer tokens | Explicitly required in assessment | `[DECISION]` |
| CQRS/MediatR | MediatR NuGet package | Supports CQRS requirement; lightweight | `[DECISION]` |
| Validation | FluentValidation | Explicitly required in assessment | `[DECISION]` |
| Logging | Serilog | Recommended in assessment | `[DECISION]` |
| Testing | xUnit + FluentAssertions + Moq | Recommended in assessment | `[DECISION]` |
| Email service | SMTP via `MailKit` or `SmtpClient` | Confirmed — full implementation | `[DECISION]` |
| Frontend framework | **Angular 17+** (Angular CLI scaffold) | Confirmed | `[DECISION]` |
| Frontend styling | **Plain CSS** — no framework, fully configurable | Confirmed | `[DECISION]` |
| Containerization | Docker | Bonus | `[ASSUMPTION]` |
| CI/CD | GitHub Actions | Bonus | `[ASSUMPTION]` |

---

## 4. Authentication & Authorization Design

### JWT Flow
1. User `POST /api/auth/login` with `{email, password}`
2. `AuthHandler` validates credentials against `User` entity in DB
3. `TokenService` generates a JWT with claims: `sub` (userId), `email`, `role`
4. Client stores token (localStorage or sessionStorage)
5. Subsequent requests include `Authorization: Bearer <token>` header
6. `JwtMiddleware` (or built-in JWT bearer) validates token on each request
7. `[Authorize]` attribute restricts endpoints to authenticated users
8. `[Authorize(Roles = "Admin,Librarian,Member")]` for role-based access

### Roles (from assessment: Role-based Authorization)
| Role | Permissions |
|------|-------------|
| Admin | Full CRUD on everything |
| Librarian | CRUD on books, members, branches; manage borrows/returns/reservations |
| Member | View books, borrow, return, reserve, view own reports |
| **(Need confirmation: exact role definitions)** | |

### Security Considerations (Assessment Requirement)
- **No secrets in repo** — use environment variables / `appsettings.Development.json` (gitignored)
- **Password hashing** — `BCrypt.Net-Next` or `Microsoft.AspNetCore.Identity` password hasher (lighter weight)
- **CORS** — configured to allow only the frontend origin
- **HTTPS enforcement** — in production profile
- **Input validation** — FluentValidation on all input DTOs (prevents over-posting, injection)
- **SQL Injection** — EF Core parameterized queries (no raw SQL unless absolutely necessary)
- **XSS** — API returns JSON only; frontend must sanitize any rendered content
- **JWT expiration** — short-lived access token (15 min), refresh token (optional, bonus)

---

## 5. Domain Model Overview (High-Level)

### Entities (without going into full field design yet — details in IMPLEMENTATION_STEPS.md)

```
User (id, email, passwordHash, role, createdAt)
  - has many: BorrowRecords
  - has many: Reservations

Branch (id, name, address, phone, isActive)
  - has many: BookCopies
  - has many: Members

Book (id, title, isbn, author, publicationYear, genre, description)
  - has many: BookCopies
  - has many: Reservations

BookCopy (id, bookId, branchId, barcode, status, acquiredDate)
  - belongs to: Book, Branch
  - has many: BorrowRecords

Member (id, firstName, lastName, email, phone, membershipDate, branchId)
  - belongs to: Branch
  - has many: BorrowRecords
  - has many: Reservations

BorrowRecord (id, bookCopyId, memberId, borrowedDate, dueDate, returnedDate, status)

Reservation (id, bookId, memberId, status, positionInQueue, createdAt)
```

### Key Relationships
- A `Book` (logical entity) can have many `BookCopy` (physical copies) at different branches.
- A `BorrowRecord` links a `BookCopy` (not a `Book`) to a `Member` — you borrow a specific copy.
- A `Reservation` is for a `Book` (logical), not a specific copy — when a copy becomes available, the first reservation in queue gets it.

### Business Rules (Need confirmation for exact values)
| Rule | Default Assumption | Status |
|------|-------------------|--------|
| Max borrow duration | 14 days | `[ASSUMPTION]` |
| Max books per member | 5 at a time | `[ASSUMPTION]` |
| Reservation expiry | Book held for 48 hours after availability | `[ASSUMPTION]` |
| Overdue threshold | DueDate < Today | `[ASSUMPTION]` |
| Duplicate reservation | Member cannot reserve same book if they already have it borrowed | `[ASSUMPTION]` |

---

## 6. Design Patterns Used

| Pattern | Where | Purpose |
|---------|-------|---------|
| **Repository** | `IRepository<T>` in Domain, implemented in Infrastructure | Abstract data access, testability |
| **Unit of Work** | `IUnitOfWork` wrapping `DbContext.SaveChanges` | Transaction consistency across multiple repositories |
| **Specification** | `Specification<T>` base class with `Criteria`, `Includes`, `OrderBy` | Encapsulate complex query logic, reusable filters |
| **CQRS (MediatR)** | Separate `IRequest` commands and `IRequest<T>` queries | Separates read from write concerns, testability |
| **Strategy** | Different notification strategies (email via SMTP, future: SMS) — email confirmed | Swappable algorithms at runtime |
| **Factory** | Create borrow records with default values (dueDate, status) | Encapsulates complex creation logic |
| **Adapter** | `TokenService` adapts JWT library output to our `IAuthService` interface | Decouples external lib from domain |

---

## 7. API Design (RESTful)

### Base Convention
```
/api/v1/<resource>
```

### Endpoints (Subject to refinement)

| Resource | Method | Endpoint | Auth Required | Roles |
|----------|--------|----------|---------------|-------|
| Auth | POST | `/api/v1/auth/login` | No | — |
| Auth | POST | `/api/v1/auth/register` | No | — |
| Branches | GET | `/api/v1/branches` | Yes | All |
| Branches | GET | `/api/v1/branches/:id` | Yes | All |
| Branches | POST | `/api/v1/branches` | Yes | Admin, Librarian |
| Branches | PUT | `/api/v1/branches/:id` | Yes | Admin |
| Branches | DELETE | `/api/v1/branches/:id` | Yes | Admin |
| Books | GET | `/api/v1/books` | Yes | All |
| Books | GET | `/api/v1/books/:id` | Yes | All |
| Books | GET | `/api/v1/books/search` | Yes | All |
| Books | POST | `/api/v1/books` | Yes | Admin, Librarian |
| Books | PUT | `/api/v1/books/:id` | Yes | Admin, Librarian |
| Books | DELETE | `/api/v1/books/:id` | Yes | Admin |
| Members | GET | `/api/v1/members` | Yes | Admin, Librarian |
| Members | GET | `/api/v1/members/:id` | Yes | All (own) or Admin/Librarian |
| Members | POST | `/api/v1/members` | Yes | Admin, Librarian |
| Members | PUT | `/api/v1/members/:id` | Yes | Admin, Librarian |
| Members | DELETE | `/api/v1/members/:id` | Yes | Admin |
| Borrows | GET | `/api/v1/borrows` | Yes | Admin, Librarian |
| Borrows | POST | `/api/v1/borrows` | Yes | Admin, Librarian |
| Borrows | PATCH | `/api/v1/borrows/:id/return` | Yes | Admin, Librarian |
| Reservations | GET | `/api/v1/reservations` | Yes | All |
| Reservations | POST | `/api/v1/reservations` | Yes | Member |
| Reservations | PATCH | `/api/v1/reservations/:id/cancel` | Yes | Owner |
| Reports | GET | `/api/v1/reports/borrow-history` | Yes | Admin, Librarian |
| Reports | GET | `/api/v1/reports/overdue` | Yes | Admin, Librarian |
| Reports | GET | `/api/v1/reports/member-activity/:memberId` | Yes | Admin, Librarian |

> Note: Exact endpoint shapes and DTO fields will be finalized during implementation. This is the skeleton.

---

## 8. Frontend Architecture (Angular)

### Component Structure

```
src/app/
├── components/
│   ├── layout/
│   │   ├── sidebar.component.ts          # Role-based navigation
│   │   ├── header.component.ts           # User info, logout button
│   │   └── app-layout.component.ts       # Combined layout
│   ├── auth/
│   │   ├── login.component.ts
│   │   └── auth-guard.ts                 # Route guard
│   ├── dashboard/
│   │   └── dashboard.component.ts        # Summary cards/tables
│   ├── branches/
│   │   ├── branch-list.component.ts
│   │   ├── branch-form.component.ts
│   │   └── branch-detail.component.ts
│   ├── books/
│   │   ├── book-list.component.ts        # With search/filter
│   │   ├── book-form.component.ts
│   │   └── book-detail.component.ts
│   ├── members/
│   │   ├── member-list.component.ts
│   │   ├── member-form.component.ts
│   │   └── member-detail.component.ts
│   ├── borrows/
│   │   ├── borrow-list.component.ts
│   │   └── borrow-form.component.ts
│   ├── reservations/
│   │   ├── reservation-list.component.ts
│   │   └── reservation-queue.component.ts
│   └── reports/
│       ├── report-filters.component.ts
│       └── report-view.component.ts
├── pages/ (or grouped in components depending on routing style)
├── guards/
│   └── auth.guard.ts                     # Guards authenticated routes
│   └── role.guard.ts                     # Guards role-based routes
├── interceptors/
│   └── auth-interceptor.ts               # Attaches JWT to outgoing requests
│   └── error-interceptor.ts              # Handles 4xx/5xx globally
├── services/
│   ├── auth.service.ts                   # Login, register, logout
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
│   └── auth.model.ts
├── shared/
│   ├── header.component.ts
│   ├── sidebar.component.ts
│   ├── spinner.component.ts
│   └── toast.component.ts               # Toast notifications
├── app.routes.ts                         # Angular 17+ standalone routing
├── app.component.ts                      # Root component with <router-outlet>
└── app.config.ts                         # Providers: HTTP client, interceptors, guards
```

### Routing (role-based, Angular 17+ standalone)
- **Public:** `/login`
- **Authenticated (all roles):** `/` (dashboard), `/books`, `/branches`, `/members`
- **Admin/Librarian only:** `/borrows`, `/reports`
- **Member only:** `/reservations`, `/my-borrows`
- **All routes except `/login`** redirect to `/login` if unauthenticated (handled by `auth.guard.ts`)
- **Role-based access** handled by `role.guard.ts` (redirects unauthorized users)

### API Service Pattern
Each service module encapsulates API calls using Angular's `HttpClient`. The `http-interceptor.ts` attaches the JWT token from the auth service (stored in `sessionStorage` or `localStorage`) and handles 401 (redirect to `/login`) and 4xx/5xx (show error toast via shared toast component). Services return typed `Observable<T>` responses.

---

## 9. Database Design (High-Level)

### Normalization Target: 3NF
- No repeating groups, no partial dependencies, no transitive dependencies on non-key columns.

### Entity Relationship Summary

```
User 1────∞ BorrowRecord
User 1────∞ Reservation
Branch 1────∞ BookCopy
Branch 1────∞ Member
Book 1────∞ BookCopy
Book 1────∞ Reservation
BookCopy 1────∞ BorrowRecord (one copy can have many borrow records over time)
Member 1────∞ BorrowRecord
Member 1────∞ Reservation
```

### Key Constraints
- `BookCopy` has a `status` enum: `Available`, `Borrowed`, `UnderMaintenance`, `Lost`
- `BorrowRecord.status`: `Active`, `Returned`, `Overdue`
- `Reservation.status`: `Pending`, `Fulfilled`, `Cancelled`, `Expired`
- Unique constraint on `Book.isbn` (each ISBN is one logical book)
- Unique constraint on `User.email`
- Unique constraint on `BookCopy.barcode`

> Full ERD and column definitions will be added during implementation in `IMPLEMENTATION_STEPS.md`.

---

## 10. Configuration & Environment

### Environment Variables (`.env` — gitignored)
```
DATABASE_URL=postgresql://user:password@localhost:5432/library_db
JWT_SECRET=your-secret-key-min-32-chars
JWT_EXPIRY_MINUTES=15
FRONTEND_URL=http://localhost:4200
SMTP_HOST=
SMTP_PORT=
SMTP_USER=
SMTP_PASS=
```

### `appsettings.json` structure
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=library_db;Username=postgres;Password=..."
  },
  "Jwt": {
    "Secret": "${JWT_SECRET}",
    "ExpiryMinutes": 15,
    "Issuer": "LibraryManagementApi"
  },
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel": "Information"
  }
}
```

---

## 11. Answers Received (All Resolved)

All 10 questions from the planning phase have been answered:

| # | Question | Answer |
|---|----------|--------|
| 1 | Frontend framework | **Angular** 17+ |
| 2 | .NET version | **.NET 10** |
| 3 | Database | **PostgreSQL** confirmed |
| 4 | Team size | **Solo** developer |
| 5 | Frontend CSS | **Plain CSS** (no framework, fully customizable) |
| 6 | Email service | **Full SMTP integration** (not just interface) |
| 7 | Reporting | **Table-based** for now (no charts) |
| 8 | Reservation Queue behavior | FIFO waitlist (see explanation in ARCHITECTURE.md) |
| 9 | Book copy model | **Multi-copy** — books can have multiple physical copies per branch |
| 10 | Frontend CSS config | Plain CSS kept simple and easy to swap; configurable by nature |

### Q8 — Reservation Queue (explained for the team)

When a book is currently unavailable (all copies are borrowed), a member can join a waitlist queue. When a copy is returned, the first person in the queue gets first priority to borrow it. This is a simple FIFO (first-in, first-out) system. If you need priority rules (e.g., premium members jump the queue) or additional notification channels (push, SMS), let us know and we'll adjust during implementation.
