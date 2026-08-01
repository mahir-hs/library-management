# Library Management System — Master Plan

> **Deadline:** 2026-08-01  
> **Source:** Technical Assessment for Software Engineer (.NET) Role (July 2026)  
> **Status:** Draft — assumptions are marked with `[ASSUMPTION]` and must be confirmed or revised.

---

## 1. Overview

Build a **Library Management System** with a RESTful Backend API and a Responsive Web Application covering 7 functional modules:

1. Authentication (JWT) & Role-based Authorization
2. Branch Management
3. Book Management
4. Member Management
5. Borrow & Return Management
6. Reservation Queue
7. Reports

The solution must demonstrate clean architecture, SOLID principles, design patterns, secure coding, async programming, testing, and solid documentation.

---

## 2. Assumptions

All items below are assumptions that need confirmation. Update this section once decisions are finalized.

| # | Assumption | Status |
|---|-----------|--------|
| A1 | **Frontend framework:** Angular | ✅ CONFIRMED |
| A2 | **Backend version:** ASP.NET Core 8.0 (minimum; .NET 9 preferred if available) | ✅ UPDATED → `.NET 10` |
| A3 | **Database:** PostgreSQL (as recommended in the assessment) | ✅ CONFIRMED |
| A4 | **Team size:** Solo developer | ✅ CONFIRMED |
| A5 | **ORM:** Entity Framework Core (as recommended) | ✅ CONFIRMED |
| A6 | **Testing framework:** xUnit (as per recommended stack) | ✅ CONFIRMED |
| A7 | **CQRS/MediatR:** Will be used (bonus feature, also supports design pattern requirement) | ✅ CONFIRMED |
| A8 | **Docker:** Will include a Dockerfile for the API project (bonus) | Optional (unchanged) |
| A9 | **CI/CD:** GitHub Actions workflow (bonus) | Optional (unchanged) |
| A10 | **Frontend styling:** Plain CSS (no framework — fully configurable, easy to swap) | ✅ CONFIRMED |
| A11 | **Email service:** Full SMTP integration (not just interface pattern) | ✅ CONFIRMED |
| A12 | **Reports:** Table-based for now (no charts yet) | ✅ CONFIRMED |
| A13 | **Book copies:** Each book can have multiple physical copies per branch | Leaned "yes" — will implement as multi-copy model (see A13 below) |
| A14 | **Reservation Queue:** Behavior TBD — see explanation below | `[NEEDS FINAL ANSWER]` |

---

## 3. Tech Stack

### Backend
- **Framework:** ASP.NET Core `.NET 10`
- **ORM:** Entity Framework Core (latest stable for .NET 10)
- **Database:** PostgreSQL 16+
- **Auth:** JWT Bearer tokens (built-in `Microsoft.AspNetCore.Authentication.JwtBearer`)
- **Validation:** FluentValidation
- **Logging:** Serilog (structured logging, console + file output)
- **Patterns:** MediatR (CQRS pipeline), Repository + Specification, Strategy, Factory
- **API Docs:** Swagger / OpenAPI via Swashbuckle
- **Testing:** xUnit + FluentAssertions + Moq (or NSubstitute)
- **Email:** SMTP integration (`IEmailService` with real implementation — `MailKit` or `SmtpClient`)

### Frontend (Angular)
- **Framework:** Angular 17+ (standalone components, Signals if adopted)
- **Routing:** Angular Router (`@angular/router`)
- **HTTP Client:** Angular HttpClient (built-in, with interceptors for JWT)
- **State Management:** Services + RxJS `BehaviorSubject` / `Signal` (no NgRx — keep it lightweight)
- **Styling:** Plain CSS (no framework — fully configurable, easy to swap later)
- **UI Components:** Custom components — no heavy UI library (Material or PrimeNG would add weight)
- **Testing:** Angular Testing Library + Jasmine (or Jest with Angular adapter)

### DevOps (Bonus)
- **Containerization:** Docker (multi-stage build for API, optional for frontend)
- **CI/CD:** GitHub Actions (build, test, Docker build on push)

---

## 4. Project Structure (Onion Architecture)

```
LibraryManagementSystem/
├── src/
│   ├── LibraryManagement.Domain/              # Entities, Interfaces, Value Objects
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Branch.cs
│   │   │   ├── Book.cs
│   │   │   ├── BookCopy.cs
│   │   │   ├── Member.cs
│   │   │   ├── BorrowRecord.cs
│   │   │   ├── ReturnRecord.cs
│   │   │   └── Reservation.cs
│   │   ├── Interfaces/
│   │   │   ├── IRepository.cs
│   │   │   ├── IUnitOfWork.cs
│   │   │   ├── ICurrentUserService.cs
│   │   │   └── IEmailService.cs              # Optional / bonus
│   │   ├── Enums/
│   │   │   ├── Role.cs
│   │   │   └── ReservationStatus.cs
│   │   └── ValueObjects/
│   │       ├── Money.cs
│   │       └── Address.cs
│   │
│   ├── LibraryManagement.Application/         # Use Cases, DTOs, Handlers, Validation
│   │   ├── Common/
│   │   │   ├── Mappings/                     # AutoMapper or manual profiles
│   │   │   ├── Behaviors/                    # MediatR pipeline behaviors (logging, validation)
│   │   │   └── Wrappers/                     # Result<T>, ApiResponse
│   │   ├── Contracts/
│   │   │   ├── Repositories/                 # IBranchRepository, IBookRepository, etc.
│   │   │   └── Services/                     # IAuthService, ITokenService, etc.
│   │   ├── DTOs/
│   │   │   ├── Auth/
│   │   │   ├── Branch/
│   │   │   ├── Book/
│   │   │   ├── Member/
│   │   │   ├── BorrowReturn/
│   │   │   └── Report/
│   │   ├── Features/
│   │   │   ├── Auth/                         # LoginCommand, RegisterCommand, etc.
│   │   │   ├── Branches/
│   │   │   ├── Books/
│   │   │   ├── Members/
│   │   │   ├── BorrowReturns/
│   │   │   ├── Reservations/
│   │   │   └── Reports/
│   │   ├── Validators/                       # FluentValidation rules
│   │   └── Exceptions/                       # Custom exception types
│   │
│   ├── LibraryManagement.Infrastructure/      # EF Core, Repositories, External Services
│   │   ├── Persistence/
│   │   │   ├── Data/                         # DbContext, migrations
│   │   │   ├── Configurations/               # EF EntityConfigurations (Fluent API)
│   │   │   ├── Repositories/                 # Generic repository + specific implementations
│   │   │   └── Migrations/                   # EF migrations
│   │   ├── Services/
│   │   │   ├── AuthService.cs
│   │   │   ├── TokenService.cs
│   │   │   └── EmailService.cs               # Bonus: email notifications
│   │   └── Logging/
│   │       └── SerilogConfig.cs
│   │
│   ├── LibraryManagement.API/                 # ASP.NET Core Web API (Presentation)
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── BranchesController.cs
│   │   │   ├── BooksController.cs
│   │   │   ├── MembersController.cs
│   │   │   ├── BorrowReturnsController.cs
│   │   │   ├── ReservationsController.cs
│   │   │   └── ReportsController.cs
│   │   ├── Middleware/
│   │   │   ├── ExceptionHandlingMiddleware.cs
│   │   │   ├── RequestLoggingMiddleware.cs
│   │   │   └── JwtMiddleware.cs
│   │   ├── Extensions/                       # Service registration extensions
│   │   ├── Properties/                       # launchSettings.json
│   │   └── Program.cs                        # Composition root, DI, middleware pipeline
│   │
│   └── LibraryManagement.Web/                 # Frontend (Angular)
│       ├── src/
│       │   ├── app/
│       │   │   ├── components/
│       │   │   ├── pages/
│       │   │   ├── guards/                   # Route guards (auth, role)
│       │   │   ├── interceptors/             # HTTP interceptor for JWT + error handling
│       │   │   ├── services/                 # Angular services for each API domain
│       │   │   ├── models/                   # TypeScript interfaces (DTOs)
│       │   │   ├── shared/                   # Shared components (layout, loading spinner, toast)
│       │   │   ├── app.routes.ts
│       │   │   └── app.component.ts
│       │   ├── assets/
│       │   ├── styles/
│       │   │   └── main.css                  # Plain CSS, no framework
│       │   └── main.ts
│       ├── angular.json
│       └── package.json
│
├── tests/
│   └── LibraryManagement.UnitTests/
│       ├── Domain/
│       ├── Application/
│       └── Infrastructure/
│
├── docker-compose.yml                        # DB + API
├── .gitignore
├── LICENSE
└── README.md
```

---

## 5. Timeline (4 Days + 1 Buffer Day)

### Day 1 — Saturday, Jul 28 (Setup & Foundation)
| Time | Task | Deliverable |
|------|------|-------------|
| Morning | Create repo, scaffold solution, set up projects | Solution builds |
| Morning | Configure EF Core + PostgreSQL, create DbContext | Connection works |
| Morning | Define all Domain entities | Entities exist |
| Afternoon | Set up Repository + UnitOfWork interfaces | Generic repository works |
| Afternoon | Set up CQRS (MediatR), FluentValidation | Handlers compile |
| Afternoon | Set up Serilog + ExceptionHandling middleware | Logging works |
| Evening | Implement JWT Auth + Role-based auth (Login/Register) | Auth module working |
| Evening | Seed data script (roles, admin user) | DB seedable |

**Day 1 Milestone:** Backend API skeleton with auth, domain models, and DB connectivity is runnable via Swagger.

### Day 2 — Sunday, Jul 29 (Core CRUD Modules)
| Time | Task | Deliverable |
|------|------|-------------|
| Morning | Branch Management (CRUD) | Endpoints complete |
| Morning | Book Management (CRUD + search/filter) | Endpoints complete |
| Morning | Member Management (CRUD) | Endpoints complete |
| Afternoon | Borrow Management (Create borrow record, validation — book availability) | Business logic works |
| Afternoon | Return Management (Process return, update availability) | Business logic works |
| Evening | FluentValidation rules for all DTOs | Validation in place |
| Evening | Unit tests for Application layer (handlers + validators) | Tests pass |

**Day 2 Milestone:** All CRUD modules functional with business validation, basic test coverage on handlers.

### Day 3 — Monday, Jul 30 (Advanced Modules + Frontend)
| Time | Task | Deliverable |
|------|------|-------------|
| Morning | Reservation Queue (borrower reserves if book unavailable, FIFO logic) | Module complete |
| Morning | Reports (borrow history, overdue books, member activity) | Report endpoints working |
| Afternoon | Frontend scaffold (Angular, routing, layout) | App navigable |
| Afternoon | Auth UI (Login page, token storage, route guards) | Login works |
| Afternoon | Dashboard + role-based navigation | Navigation works |
| Evening | Branch, Book, Member CRUD pages in frontend | CRUD pages functional |

**Day 3 Milestone:** All 7 backend modules complete; frontend has auth, dashboard, and CRUD pages for main entities.

### Day 4 — Tuesday, Jul 31 (Frontend Completion + Testing + Docs)
| Time | Task | Deliverable |
|------|------|-------------|
| Morning | Borrow/Return UI pages | UI complete for borrow/return |
| Morning | Reservation Queue UI | UI complete |
| Morning | Reports UI (tables/charts) | Reports page complete |
| Afternoon | Responsive design audit + polish | Mobile-friendly |
| Afternoon | Write remaining Unit Tests + integration tests | >70% coverage target |
| Afternoon | README.md with setup instructions, assumptions, architecture overview | Docs complete |
| Afternoon | Dockerfile + docker-compose.yml (bonus) | Container buildable |
| Evening | GitHub Actions CI/CD workflow (bonus) | Pipeline configured |

**Day 4 Milestone:** Full application runnable, tested, documented, and containerized.

### Day 5 — Wednesday, Aug 1 (Buffer / Buffer Day)
- Fix any remaining bugs
- Polish UI/UX
- Final README and documentation pass
- Push to final repo state
- Email submission with summary

---

## 6. Evaluation Criteria Mapping

| Criteria | Marks | How We Address It |
|----------|-------|-------------------|
| Functional Requirements | 25 | All 7 modules implemented fully |
| Frontend Implementation | 10 | Angular SPA with all required pages |
| Architecture & Project Structure | 15 | Onion architecture, clear separation |
| Code Quality & Maintainability | 10 | Clean code, naming, comments where needed |
| SOLID & Dependency Injection | 10 | DI everywhere, interfaces for everything |
| Design Patterns | 10 | Repository, Specification, Strategy, CQRS |
| Database Design | 5 | Normalized schema, proper relationships |
| Security | 5 | JWT + role-based auth, no secrets in repo |
| Performance | 5 | Async all I/O, efficient queries |
| Unit Testing | 5 | xUnit on all Application + Domain logic |
| Documentation & Git Practices | 10 | README, commits, clean PR-like history |
| **Total** | **100** | |

---

## 7. Bonus Features (Priority Order)

| Priority | Bonus Feature | Effort | Value |
|----------|--------------|--------|-------|
| P0 | MediatR/CQRS | Already planned | Core requirement |
| P0 | Email notifications (SMTP) — CONFIRMED | Medium | High |
| P1 | Docker + docker-compose | Medium | High |
| P2 | GitHub Actions CI/CD | Low | High |
| P3 | Domain Events (borrow overdue notification pattern) | Medium | Medium |
| P4 | Excel/PDF Export for reports | Medium | Medium |
| P5 | Redis caching for book lookups | High | Low |

---

## 8. Risks & Mitigations

| Risk | Mitigation |
|------|-----------|
| Running out of time on frontend | Prioritize backend completeness — frontend can be simpler but functional |
| EF Core migration issues with PostgreSQL | Test DB connection on Day 1 morning |
| Auth not working end-to-end | Test login flow on Day 1 evening with Swagger |
| Test coverage too low | Write tests alongside handlers, not after |
| Angular routing misconfiguration | Test auth-guard flow on Day 1 evening |
| Email service SMTP connection issues | Implement interface + stub first; real SMTP on Day 4 if time permits |

---

## 9. Git Strategy

- **Main branch:** `main` — stable, deployable state
- **Development branch:** `develop` — integration branch
- **Feature branches:** `feat/auth`, `feat/branches`, `feat/books`, etc.
- **Commit convention:** Conventional Commits (`feat:, fix:, docs:, test:, refactor:`)
- **No secrets in repo:** `.env` files gitignored, all config via environment variables

---

## 10. Answers Received

All 10 questions have been answered by the team lead:

| # | Question | Answer |
|---|----------|--------|
| 1 | Frontend framework | **Angular** |
| 2 | .NET version | **.NET 10** |
| 3 | Database | **PostgreSQL** confirmed |
| 4 | Team size | **Solo** developer |
| 5 | Frontend CSS | **Plain CSS** (no framework, fully configurable) |
| 6 | Email service | **Yes** — full SMTP integration (not just interface) |
| 7 | Reporting | **Table-based** for now; charts are optional |
| 8 | Reservation Queue behavior | ⚠️ **See explanation below — needs team confirmation** |
| 9 | Book copy model | **"Maybe yes"** → leaning toward multi-copy model (books have multiple physical copies per branch) |
| 10 | Frontend CSS config | **"Up to you, maybe configurable"** → plain CSS is the most configurable approach — no framework lock-in |

### Q8 — Reservation Queue Behavior (Needs Confirmation)

**What it is:** When a book is currently unavailable (all copies borrowed), a member can "reserve" their place in a queue. When a copy is returned, the first person in the queue gets first access. This is a waitlist/FIFO system.

**Default implementation (if no preference):** Simple FIFO queue — first reservation in line gets the book when it's returned. Email notification to the member when their reservation is fulfilled (bonus, since email is confirmed).

**Please confirm:** Is this sufficient, or do you need priority rules (e.g., priority for certain member tiers), notification channels (email only, or also push/in-app), or a maximum queue length?

## 11. Notes

- The PDF title has a typo ("Assesment" instead of "Assessment") — not relevant to the build.
- The assessment emphasizes **quality over quantity** — a well-architected subset beats a buggy full implementation.
- All passwords, API keys, and connection strings must be configured via environment variables or `appsettings.json` with gitignored override files.
- **Frontend is Angular** — use `ng new` or manual Angular CLI scaffold, not React/Vite.

---

## 11. Notes

- The PDF title has a typo ("Assesment" instead of "Assessment") — not relevant to the build.
- The assessment emphasizes **quality over quantity** — a well-architected subset beats a buggy full implementation.
- All passwords, API keys, and connection strings must be configured via environment variables or `appsettings.json` with gitignored override files.
