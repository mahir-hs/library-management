# Implementation Steps — Library Management System

> **Instructions:** Follow steps in order. Each step has a clear acceptance criterion before moving to the next. Steps marked with `[OPTIONAL]` are bonus features — skip them if timeline slips.
> **Before starting:** Confirm all assumptions in `PLAN.md` and `ARCHITECTURE.md`.

---

## Phase 0: Pre-Implementation Checklist

Complete ALL of these before writing any code:

- [ ] Confirm frontend framework → Angular ✅
- [ ] Confirm .NET version → .NET 10 ✅
- [ ] Confirm database → PostgreSQL ✅
- [ ] Confirm team size → Solo ✅
- [ ] Confirm exact role definitions (Admin, Librarian, Member)
- [ ] Set up local PostgreSQL instance (or Docker container)
- [ ] Create GitHub/GitLab repo with LICENSE and .gitignore
- [ ] Install required tools globally:
  - `dotnet --version` ≥ 10.0
  - `node --version` ≥ 18 (for Angular)
  - `ng version` (Angular CLI)
  - `docker --version` (optional)
- [ ] Resolve all open questions from `ARCHITECTURE.md` Section 11

---

## Phase 1: Solution Scaffold (Day 1 — ~4 hours)

### Step 1.1: Create solution and projects

```bash
# From repo root
mkdir -p src tests
cd src

dotnet new sln -n LibraryManagementSystem
dotnet new webapi -n LibraryManagement.API -nologo --no-http-restore
dotnet new classlib -n LibraryManagement.Domain -nologo --no-http-restore
dotnet new classlib -n LibraryManagement.Application -nologo --no-http-restore
dotnet new classlib -n LibraryManagement.Infrastructure -nologo --no-http-restore
# Frontend: Angular project created separately (see below)
# Do NOT use `dotnet new react` — Angular is created with `ng new`

dotnet sln add LibraryManagement.API/LibraryManagement.API.csproj
dotnet sln add LibraryManagement.Domain/LibraryManagement.Domain.csproj
dotnet sln add LibraryManagement.Application/LibraryManagement.Application.csproj
dotnet sln add LibraryManagement.Infrastructure/LibraryManagement.Infrastructure.csproj
```

**Acceptance:** `dotnet sln list` shows all 5 projects and the solution builds.

### Step 1.2: Add NuGet packages to each project

**LibraryManagement.Domain:**
```
# No external packages needed for the domain layer
```

**LibraryManagement.Application:**
```
dotnet add package MediatR
dotnet add package FluentValidation
dotnet add package AutoMapper
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions
```

**LibraryManagement.Infrastructure:**
```
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.PgSql
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package BCrypt.Net-Next          # Password hashing
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

**LibraryManagement.API:**
```
dotnet add package Swashbuckle.AspNetCore
dotnet add package Serilog.Extensions.Hosting
dotnet add package FluentValidation.AspNetCore
```

**LibraryManagement.Web (Angular CLI):**
```bash
ng new LibraryManagement.Web --standalone --routing --style=css
cd LibraryManagement.Web
npm install @angular/material  # optional — only if Material design desired; plain CSS is the default
```

**Acceptance:** All packages install without errors; `dotnet build` succeeds.

### Step 1.3: Configure the API project (Program.cs)

Set up:
- Serilog logger
- Swagger/OpenAPI
- CORS (for Angular dev server)
- Authentication middleware (JWT)
- Authorization middleware
- MediatR (scanning for handlers)
- FluentValidation
- AutoMapper
- Database context + migrations
- Exception handling middleware
- Dependency Injection registration

**Acceptance:** `dotnet run` on API project starts without errors; Swagger UI loads at `/swagger`.

### Step 1.4: Configure Serilog

Add `serilog.json` or configure in `Program.cs`:
- Minimum level: Information
- Write to Console and to a rolling log file (`logs/log-.txt`)
- Enrich from log context (correlation ID, thread id, etc.)

**Acceptance:** Logs appear in console and in the `logs/` directory when API is running.

### Step 1.5: Set up EF Core + PostgreSQL

**Connection string** (from environment variable, not hardcoded):
```
Host=localhost;Port=5432;Database=library_db;Username=postgres;Password=yourpassword
```

**DbContext configuration** (in Infrastructure):
- Use `NpgsqlDbContextOptionsBuilder`
- Enable sensitive data logging only in Development

**Acceptance:** `dotnet ef database update` applies the initial migration against PostgreSQL.

---

## Phase 2: Domain Layer — Entities & Interfaces (Day 1 Afternoon)

### Step 2.1: Define domain entities

Create the following entity classes in `LibraryManagement.Domain/Entities/`:

| Entity | Key Fields |
|--------|-----------|
| `User` | Id, Email, PasswordHash, Role, CreatedAt |
| `Branch` | Id, Name, Address, Phone, IsActive |
| `Book` | Id, Title, Isbn, Author, PublicationYear, Genre, Description |
| `BookCopy` | Id, BookId, BranchId, Barcode, Status, AcquiredDate |
| `Member` | Id, FirstName, LastName, Email, Phone, MembershipDate, BranchId |
| `BorrowRecord` | Id, BookCopyId, MemberId, BorrowedDate, DueDate, ReturnedDate, Status |
| `Reservation` | Id, BookId, MemberId, Status, PositionInQueue, CreatedAt |

**Rules for each entity:**
- All entities have an `Id` (Guid)
- `Role` is an enum: `Admin`, `Librarian`, `Member`
- `BookCopyStatus` is an enum: `Available`, `Borrowed`, `UnderMaintenance`, `Lost`
- `BorrowStatus` is an enum: `Active`, `Returned`, `Overdue`
- `ReservationStatus` is an enum: `Pending`, `Fulfilled`, `Cancelled`, `Expired`

**Acceptance:** All entities compile; none reference any framework namespace.

### Step 2.2: Define domain interfaces

In `LibraryManagement.Domain/Interfaces/`:

- `IRepository<T>` — generic CRUD contract
- `IUnitOfWork` — transaction boundary with `SaveChangesAsync`
- `ICurrentUserService` — provides current user identity (userId, role)
- `IEmailService` — confirmed: full SMTP integration (use `MailKit` or `SmtpClient`)

**Acceptance:** All interfaces are in the Domain layer with zero external dependencies.

### Step 2.3: Define Specification pattern

Create a base `Specification<T>` class in the Domain layer.

Fields:
- `Criteria` — `Expression<Func<T, bool>>`
- `Includes` — list of `Expression<Func<T, object>>` for eager loading
- `OrderBy` / `OrderByDescending` — sorting
- `PageNumber` / `PageSize` — pagination

**Acceptance:** `Specification<Book>` can encapsulate a search query; compiles cleanly.

---

## Phase 3: Application Layer — Handlers, DTOs, Validation (Day 1 Evening)

### Step 3.1: Define DTOs

In `LibraryManagement.Application/DTOs/`, create DTOs per module:

| Folder | DTOs |
|--------|------|
| `Auth/` | `LoginRequestDto`, `RegisterRequestDto`, `AuthResponseDto`, `UserDto` |
| `Branch/` | `BranchDto`, `CreateBranchDto`, `UpdateBranchDto` |
| `Book/` | `BookDto`, `CreateBookDto`, `UpdateBookDto`, `BookSearchDto` |
| `Member/` | `MemberDto`, `CreateMemberDto`, `UpdateMemberDto` |
| `BorrowReturn/` | `BorrowDto`, `CreateBorrowDto`, `ReturnDto`, `BorrowRecordDto` |
| `Reservation/` | `ReservationDto`, `CreateReservationDto`, `ReservationQueueDto` |
| `Report/` | `BorrowHistoryDto`, `OverdueReportDto`, `MemberActivityDto` |

**Rules:**
- **Input DTOs** have validation attributes or are validated by FluentValidation (do NOT mix data annotations with FluentValidation — pick one).
- **Output DTOs** are flat projection objects.
- **No entity references in DTOs** — only primitives.

**Acceptance:** All DTOs compile; no circular references.

### Step 3.2: Create FluentValidation validators

In `LibraryManagement.Application/Validators/`, create a validator for each input DTO:

| Validator | Key Rules |
|-----------|-----------|
| `LoginRequestValidator` | Email required + valid format; Password required (min length) |
| `CreateBranchValidator` | Name required + max 200 chars; Address required |
| `CreateBookValidator` | Title required; Isbn required + valid ISBN format; Author required |
| `CreateMemberValidator` | FirstName + LastName required; Email required + valid format; Phone required |
| `CreateBorrowValidator` | MemberId valid UUID; BookCopyId valid UUID; BorrowDate ≤ today |
| `CreateReservationValidator` | MemberId + BookId required; Member cannot reserve own borrowed book |

**Acceptance:** Each validator has at least 3 rules; tests confirm valid/invalid cases.

### Step 3.3: Implement Auth Module (Core — Day 1 Evening)

**Command handlers:**
- `LoginCommand` → handler validates credentials, generates JWT
- `RegisterCommand` → handler creates user with hashed password

**Key implementation details:**
1. `TokenService` generates JWT using:
   - Symmetric key (from config)
   - Algorithm: HMACSHA256
   - Claims: `sub` (userId), `email`, `role`
   - Expiry: 15 minutes
2. `AuthService` validates password against `BCrypt.Net-Next` hash
3. `AuthController` exposes `POST /api/v1/auth/login` and `POST /api/v1/auth/register`

**Middleware/Security:**
- `JwtMiddleware` (or built-in `AddAuthentication(JwtBearerDefaults.AuthenticationScheme)`) extracts token from Authorization header, validates it
- `AddAuthorization()` with role policy: `RequireRole("Admin")`, etc.

**Acceptance:** `POST /api/v1/auth/register` creates a user; `POST /api/v1/auth/login` returns a valid JWT; calling a `[Authorize]` endpoint with the token succeeds; calling it without a token returns 401.

### Step 3.4: Implement CQRS pipeline behaviors

Configure two MediatR behaviors:
1. **ValidationBehavior** — runs FluentValidation on all `IRequest<T>` before handler executes
2. **LoggingBehavior** — logs request type, timestamp, and execution duration

**Acceptance:** Validation fails before handler is called when invalid DTO is provided; logs show request execution time.

---

## Phase 4: CRUD Module Handlers (Day 2)

### Step 4.1: Seed Data

Create an initial migration and seed:
- Roles: Admin, Librarian, Member
- Default admin user (email + password in seed data — for dev only)
- At least 1 branch

**Acceptance:** Running the app with an empty DB and the seed script populates all seed data.

### Step 4.2: Branch Management

**Handlers:**
| Handler | Handler Type | Endpoint |
|---------|-------------|----------|
| `GetBranchesQuery` | Query | GET `/api/v1/branches` |
| `GetBranchByIdQuery` | Query | GET `/api/v1/branches/:id` |
| `CreateBranchCommand` | Command | POST `/api/v1/branches` |
| `UpdateBranchCommand` | Command | PUT `/api/v1/branches/:id` |
| `DeleteBranchCommand` | Command | DELETE `/api/v1/branches/:id` |

**Business validations:**
- Branch name must be unique (case-insensitive)
- Cannot delete a branch that has active books or members
- Admin only for delete

**Acceptance:** All 5 CRUD operations work via Swagger; invalid input returns 400 with error details.

### Step 4.3: Book Management

**Handlers:**
| Handler | Handler Type | Endpoint |
|---------|-------------|----------|
| `GetBooksQuery` | Query (with pagination + optional filters) | GET `/api/v1/books` |
| `GetBookByIdQuery` | Query | GET `/api/v1/books/:id` |
| `SearchBooksQuery` | Query (title, author, isbn, genre) | GET `/api/v1/books/search?query=...` |
| `CreateBookCommand` | Command | POST `/api/v1/books` |
| `UpdateBookCommand` | Command | PUT `/api/v1/books/:id` |
| `DeleteBookCommand` | Command | DELETE `/api/v1/books/:id` |

**Business validations:**
- ISBN must be unique
- Search filters work on title, author, isbn, genre (partial match)
- Pagination: default 20 per page, max 100

**Acceptance:** Search returns relevant results; pagination works; deletion soft-deletes or hard-deletes consistently.

### Step 4.4: Member Management

**Handlers:**
| Handler | Handler Type | Endpoint |
|---------|-------------|----------|
| `GetMembersQuery` | Query | GET `/api/v1/members` |
| `GetMemberByIdQuery` | Query | GET `/api/v1/members/:id` |
| `CreateMemberCommand` | Command | POST `/api/v1/members` |
| `UpdateMemberCommand` | Command | PUT `/api/v1/members/:id` |
| `DeleteMemberCommand` | Command | DELETE `/api/v1/members/:id` |

**Business validations:**
- Email must be unique
- Member must belong to an existing branch
- Default membership date = today

### Step 4.5: Borrow Management

**Handlers:**
| Handler | Handler Type | Endpoint |
|---------|-------------|----------|
| `GetBorrowsQuery` | Query | GET `/api/v1/borrows` (Admin/Librarian) |
| `GetMyBorrowsQuery` | Query | GET `/api/v1/borrows/mine` (Member) |
| `CreateBorrowCommand` | Command | POST `/api/v1/borrows` |
| `ReturnBookCommand` | Command | PATCH `/api/v1/borrows/:id/return` |

**Business validations (critical — test thoroughly):**
1. BookCopy must be in `Available` status
2. Member must exist
3. Member cannot borrow if they have ≥ 5 active borrows
4. DueDate = BorrowedDate + 14 days (constant)
5. BorrowDate cannot be in the future
6. Returning sets `ReturnedDate = today`, status = `Returned`, BookCopy status = `Available`
7. Return must be by Admin/Librarian role (or by Member themselves for own borrows — decide)

**Acceptance:** Borrowing an available book succeeds; borrowing an already-borrowed book returns 409; member with 5 active borrows gets 403.

### Step 4.6: Reservation Management

**Handlers:**
| Handler | Handler Type | Endpoint |
|---------|-------------|----------|
| `GetReservationsQuery` | Query | GET `/api/v1/reservations` |
| `CreateReservationCommand` | Command | POST `/api/v1/reservations` |
| `CancelReservationCommand` | Command | PATCH `/api/v1/reservations/:id/cancel` |
| `FulfillReservationCommand` | Command (Librarian use) | PATCH `/api/v1/reservations/:id/fulfill` |

**Business logic:**
1. Reservation creates a spot in the queue for a book that is currently unavailable
2. `PositionInQueue` is auto-assigned (FIFO, based on CreatedAt)
3. When a book is returned, the system auto-assigns it to the next reservation (FulfillReservation)
4. Member can cancel their own pending reservation
5. Librarian can fulfill a reservation when the book copy becomes available
6. Unfulfilled reservations expire after 48 hours (handled by a background job — or manually for MVP)

**Acceptance:** Reservation queue is FIFO; returning a book advances the queue; canceling removes the member from the queue.

### Step 4.7: Reports

**Handlers:**
| Handler | Endpoint |
|---------|----------|
| `GetBorrowHistoryQuery` | GET `/api/v1/reports/borrow-history?from=...&to=...` |
| `GetOverdueReportQuery` | GET `/api/v1/reports/overdue` |
| `GetMemberActivityQuery` | GET `/api/v1/reports/member-activity/:memberId` |

**Report data:**
- Borrow history: list of borrow records with BookTitle, MemberName, BranchName, BorrowedDate, DueDate, ReturnedDate
- Overdue report: active borrows where DueDate < today; show days overdue
- Member activity: total borrows, active borrows, overdue count, reservations for a specific member

**Acceptance:** All three report endpoints return correct data; date filtering works.

---

## Phase 5: Frontend — Angular SPA (Day 3 + Day 4)

### Step 5.1: Scaffold & configure Angular app

```bash
ng new LibraryManagement.Web --standalone --routing --style=css
cd LibraryManagement.Web
npm install @angular/forms         # for reactive forms
npm install rxjs                    # already included with Angular
```

Plain CSS only — no framework dependency. All styling is in `src/styles.css` or per-component `.css` files.

**Acceptance:** `ng serve` serves the app on `http://localhost:4200`; plain CSS works.

### Step 5.2: API Service Layer

Create `src/app/services/api.ts` — Angular `HttpClient` wrapper (or interceptors + base URL):
- Base URL from environment variable (`environment.ts`)
- HTTP interceptor: attaches `Authorization: Bearer <token>` from `AuthService` session storage
- HTTP interceptor: 401 → redirect to `/login`; 4xx/5xx → show error via shared toast component

Create individual service modules:
- `authService.ts`
- `branchService.ts`
- `bookService.ts`
- `memberService.ts`
- `borrowService.ts`
- `reservationService.ts`
- `reportService.ts`

**Acceptance:** All service modules compile and call the right endpoints.

### Step 5.3: Auth Service, Guard & Login Component

- `AuthService` (singleton) provides `login()`, `logout()`, `isAuthenticated()`, `getRole()` — stores token in `sessionStorage`
- `auth.guard.ts` — Angular route guard that redirects to `/login` if no token
- `role.guard.ts` — Angular route guard that redirects if user doesn't have required role
- `LoginComponent` — email + password reactive form → calls auth API → stores token + user → redirect to `/`
- `Logout` button in header clears token + redirects to `/login`
- `app.routes.ts` — configures routes with guard data: `canActivate: [AuthGuard]`, `data: { roles: ['Admin', 'Librarian'] }`

**Acceptance:** Login flow works end-to-end; protected routes redirect unauthenticated users; role-based routes reject unauthorized access.

### Step 5.4: Dashboard

- Shows summary: total books, active borrows, pending reservations, overdue count
- Role-based: admin sees everything; member sees only their own

**Acceptance:** Dashboard renders correct counts based on user role.

### Step 5.5: CRUD Pages (Branch, Book, Member)

For each entity (Branch, Book, Member):
- List page with table + search bar (Debounced search input)
- Detail page showing full entity
- Create/Edit form page with Angular reactive forms validation
- Delete button with confirmation dialog

**Acceptance:** All CRUD operations work from the UI; search/filter works; responsive layout.

### Step 5.6: Borrow & Return Pages

- **Borrow page:** Select a member + a book copy (dropdown) → create borrow → show success
- **Return page:** Show active borrows → click "Return" → confirm → update record
- **My Borrows page** (Member role): View own active/returned borrow history

**Acceptance:** Borrow creates a record; Return updates it; members see their own borrows.

### Step 5.7: Reservation Queue Page

- List all reservations (admin/librarian view) or user's own reservations (member view)
- Admin: fulfill reservation button (assigns available book copy)
- Member: cancel reservation button (for own pending reservations)
- Visual queue indicator (position in queue)

**Acceptance:** Reservations can be created, fulfilled, and canceled via UI.

### Step 5.8: Reports Page

- Tabs or sections for: Borrow History, Overdue Books, Member Activity
- Date range picker for borrow history
- Table views for all reports
- Search/filter on member name for member activity report

**Acceptance:** All reports load data from API and display in tables; filters work.

### Step 5.9: Responsive Design & Polish

- Test on mobile viewport (375px width)
- Ensure navigation collapses (hamburger menu)
- Check that tables scroll horizontally on small screens
- Consistent spacing, colors, and typography
- Loading spinners on async operations
- Toast notifications for success/error

**Acceptance:** App is usable on mobile (horizontal scroll on tables is acceptable for data-dense pages).

---

## Phase 6: Testing (Ongoing — primarily Day 2 afternoon + Day 4)

### Step 6.1: Unit Tests — Domain Layer
- Test entity creation and validation
- Test enum values

### Step 6.2: Unit Tests — Application Layer
- Test handlers with mocked repositories (happy path + error cases)
- Test validators with valid/invalid inputs
- Test business rules (e.g., can't borrow when 5 books already active)

### Step 6.3: Unit Tests — Infrastructure (optional)
- Test repository queries (use in-memory database or SQLite for speed)

### Coverage Target
- Minimum **70%** line coverage on Application and Domain layers
- Every module has at least 3 test cases (happy path + 2 error cases)

**Acceptance:** `dotnet test` runs all tests and passes; coverage report shows ≥70%.

---

## Phase 7: Documentation & DevOps (Day 4)

### Step 7.1: README.md
Required content:
- Project overview (2–3 sentences)
- Features list
- Tech stack
- Prerequisites (Node, .NET SDK, PostgreSQL)
- Setup instructions (step-by-step)
- Configuration (environment variables explained)
- How to run the application
- How to run tests
- Assumptions and design decisions

### Step 7.2: Docker (Bonus — Optional)

**docker-compose.yml:**
```yaml
version: '3.8'
services:
  db:
    image: postgres:16
    environment:
      POSTGRES_DB: library_db
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
  api:
    build: ./src/LibraryManagement.API
    ports:
      - "5000:80"
    depends_on:
      - db
    environment:
      - ConnectionStrings__DefaultConnection=Host=db;Database=library_db;Username=postgres;Password=postgres
      - JWT__Secret=dev-secret-key-change-in-production
```

### Step 7.3: GitHub Actions CI/CD (Bonus — Optional)

```yaml
name: Build and Test
on: [push, pull_request]
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.x'
      - run: dotnet restore
      - run: dotnet build --no-restore
      - run: dotnet test --no-build --verbosity normal
```

### Step 7.4: Git History
- Ensure commit history is clean and meaningful
- At minimum: `feat: scaffold solution`, `feat: domain entities`, `feat: auth module`, etc.
- No merge commits in feature branches (rebase before merging)

---

## Phase 8: Final Submission (Day 5 — Buffer)

- [ ] All modules work end-to-end via Swagger
- [ ] Frontend works (auth, dashboard, CRUD, borrow/return, reservations, reports)
- [ ] All tests pass
- [ ] README is complete and accurate
- [ ] No secrets committed
- [ ] Docker build works (if attempted)
- [ ] GitHub Actions runs (if attempted)
- [ ] Email submission with summary

---

## Index of Files Created

| File | Purpose |
|------|---------|
| `PLAN.md` | Master plan with timeline and milestones |
| `ARCHITECTURE.md` | Architecture decisions and system design |
| `IMPLEMENTATION_STEPS.md` | This file — step-by-step implementation guide |
| `TASKS.md` | Granular task breakdown per module (next file) |
