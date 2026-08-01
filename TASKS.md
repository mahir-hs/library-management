# Task Breakdown — Library Management System

> **Granular task list** to track progress against the 4-day timeline (Jul 28 – Aug 1).  
> Each task has a unique ID for referencing in commit messages or GitHub issues.  
> **Status markers:** `[ ]` not started, `[~]` in progress, `[x]` done.  
> All assumptions from `PLAN.md` and `ARCHITECTURE.md` apply here unless overridden.

---

## Task Index

| ID | Task | Module | Day | Est. Hours |
|----|------|--------|-----|-----------|
| T-001 | Scaffold solution & projects | Setup | 1 | 1 |
| T-002 | Add NuGet/npm packages | Setup | 1 | 1 |
| T-003 | Configure API (Program.cs, Serilog, Swagger, CORS, DI) | Setup | 1 | 2 |
| T-004 | Configure EF Core + PostgreSQL connection | Setup | 1 | 1 |
| T-005 | Seed data script (roles, default admin) | Setup | 1 | 0.5 |
| T-006 | Define all domain entities | Domain | 1 | 2 |
| T-007 | Define domain interfaces (IRepository, IUnitOfWork, etc.) | Domain | 1 | 1 |
| T-008 | Implement Specification pattern | Domain | 1 | 1 |
| T-009 | Define all DTOs (input + output) | Application | 1 | 2 |
| T-010 | Create FluentValidation validators for all input DTOs | Application | 1 | 2 |
| T-011 | Implement MediatR pipeline behaviors (validation + logging) | Application | 1 | 1 |
| T-012 | Implement Auth module (Login, Register, JWT) | Auth | 1 + 2 | 3 |
| T-013 | Branch Management (handlers + endpoints) | Branch | 2 | 2 |
| T-014 | Book Management (handlers + endpoints) | Book | 2 | 3 |
| T-015 | Member Management (handlers + endpoints) | Member | 2 | 2 |
| T-016 | Borrow Management (create borrow, business rules) | Borrow/Return | 2 | 3 |
| T-017 | Return Management (process return, update availability) | Borrow/Return | 2 | 2 |
| T-018 | Reservation Queue (FIFO, fulfillment, cancel) | Reservation | 3 | 2 |
| T-019 | Reports (borrow history, overdue, member activity) | Reports | 3 | 2 |
| T-020 | Unit Tests — Application layer (handlers + validators) | Testing | 2–4 | 4 |
| T-021 | Unit Tests — Domain layer | Testing | 4 | 2 |
| T-022 | Frontend scaffold (Angular standalone, routing, CSS) | Frontend | 3 | 2 |
| T-023 | Frontend API service layer (HttpClient, interceptors) | Frontend | 3 | 1 |
| T-024 | Frontend Auth (Login, AuthService, Guards, Logout) | Frontend | 3 | 2 |
| T-025 | Frontend Dashboard | Frontend | 3 | 1 |
| T-026 | Frontend Branch CRUD pages | Frontend | 3–4 | 2 |
| T-027 | Frontend Book CRUD pages (with search) | Frontend | 3–4 | 2 |
| T-028 | Frontend Member CRUD pages | Frontend | 3–4 | 2 |
| T-029 | Frontend Borrow/Return pages | Frontend | 4 | 2 |
| T-030 | Frontend Reservation Queue page | Frontend | 4 | 1 |
| T-031 | Frontend Reports page | Frontend | 4 | 1 |
| T-032 | Frontend responsive design & polish | Frontend | 4 | 1 |
| T-033 | Write README.md | Docs | 4 | 1 |
| T-034 | Docker + docker-compose (bonus) | DevOps | 4 | 1 |
| T-035 | GitHub Actions CI/CD (bonus) | DevOps | 4 | 0.5 |
| T-036 | Final README review + submission email | Submit | 5 | 1 |

### Task Summary by Category

| Category | Tasks | Total Hours (est.) |
|----------|-------|--------------------|
| Setup & Infrastructure | T-001 → T-005 | 5.5 |
| Domain Layer | T-006 → T-008 | 4 |
| Application Layer | T-009 → T-012 | 8 |
| Branch Module | T-013 | 2 |
| Book Module | T-014 | 3 |
| Member Module | T-015 | 2 |
| Borrow & Return Module | T-016, T-017 | 5 |
| Reservation Module | T-018 | 2 |
| Reports Module | T-019 | 2 |
| Testing | T-020, T-021 | 6 |
| Frontend | T-022 → T-032 | 16 |
| Documentation | T-033 | 1 |
| DevOps (Bonus) | T-034, T-035 | 1.5 |
| Submission | T-036 | 1 |
| **Total** | **36 tasks** | **~61 hours** |

> Note: 61 hours over 4 days ≈ 15 hrs/day — aggressive but doable solo. If working with a team, parallelize by module. If behind schedule, drop frontend polish and bonus features first.

---

## Detailed Task Definitions

### T-001 — Scaffold solution & projects
**Day:** 1 | **Hours:** 1

**Subtasks:**
- [ ] Create root directory, initialize git repo
- [ ] Create `LibraryManagement.sln`
- [ ] `dotnet new webapi -n LibraryManagement.API`
- [ ] `dotnet new classlib -n LibraryManagement.Domain`
- [ ] `dotnet new classlib -n LibraryManagement.Application`
- [ ] `dotnet new classlib -n LibraryManagement.Infrastructure`
- [ ] `ng new LibraryManagement.Web --standalone --routing --style=css` (Angular frontend, created separately)
- [ ] Add all projects to the solution
- [ ] `dotnet build` — verify solution compiles

**Acceptance Criterion:** `dotnet sln list` shows all 5 projects; `dotnet build` succeeds with zero errors.

**Deliverable:** Solvable solution with all projects referencing each other correctly; initial git commit.

---

### T-002 — Add NuGet/npm packages
**Day:** 1 | **Hours:** 1

**Subtasks:**
- [ ] Add MediatR to Application
- [ ] Add FluentValidation to Application + API
- [ ] Add AutoMapper to Application
- [ ] Add EF Core + Npgsql to Infrastructure
- [ ] Add Serilog packages to Infrastructure + API
- [ ] Add BCrypt.Net-Next to Infrastructure
- [ ] Add JWT bearer package to API
- [ ] Add Swashbuckle to API
- [ ] npm install @angular/forms in LibraryManagement.Web (reactive forms support)

**Acceptance Criterion:** All `dotnet add package` and `npm install` commands succeed; no version conflicts.

**Deliverable:** All dependencies installed across all projects.

---

### T-003 — Configure API (Program.cs, Serilog, Swagger, CORS, DI)
**Day:** 1 | **Hours:** 2

**Subtasks:**
- [ ] Configure Serilog in `Program.cs` (console + file sinks)
- [ ] Add Swagger/OpenAPI (Swashbuckle) — document all endpoints
- [ ] Configure CORS to allow frontend origin (`http://localhost:4200`)
- [ ] Register MediatR (assembly scanning for handlers)
- [ ] Register FluentValidation (validators from Application assembly)
- [ ] Register AutoMapper (profiles from Application assembly)
- [ ] Register all application services via DI
- [ ] Add exception handling middleware (global catch → standardized error response)
- [ ] Add request logging middleware
- [ ] Configure authentication (AddAuthentication + AddJwtBearer)
- [ ] Configure authorization (AddAuthorization + role policies)

**Acceptance Criterion:** `dotnet run` starts the API; Swagger UI loads at `/swagger`; all middleware executes in correct order.

**Deliverable:** API project runs and serves Swagger documentation.

---

### T-004 — Configure EF Core + PostgreSQL
**Day:** 1 | **Hours:** 1

**Subtasks:**
- [ ] Define connection string from environment variable
- [ ] Create `ApplicationDbContext` in Infrastructure/Persistence
- [ ] Configure `OnConfiguring` or use `AddDbContext` in DI with Npgsql provider
- [ ] Configure sensitivity data logging for Development only
- [ ] Create initial migration: `dotnet ef migrations add InitialCreate`
- [ ] Apply migration: `dotnet ef database update`

**Acceptance Criterion:** Database is created in PostgreSQL; `dotnet ef database update` runs without errors.

**Deliverable:** Working EF Core connection to PostgreSQL with initial migration applied.

---

### T-005 — Seed data script
**Day:** 1 | **Hours:** 0.5

**Subtasks:**
- [ ] Write seed method in `DbContext.OnModelCreating` (or a separate `SeedData` class)
- [ ] Seed roles: Admin, Librarian, Member
- [ ] Seed default admin user (email + hashed password)
- [ ] Seed 1 default branch
- [ ] Call `context.SaveChanges()` in the seed logic
- [ ] Verify seed runs on `dotnet ef database update`

**Acceptance Criterion:** After applying migration, the database has all seed data queryable.

**Deliverable:** DB can be seeded with a single `dotnet ef database update`.

---

### T-006 — Define all domain entities
**Day:** 1 | **Hours:** 2

**Subtasks:**
- [ ] Create `User.cs` — Id (Guid), Email, PasswordHash, Role (enum), CreatedAt
- [ ] Create `Branch.cs` — Id, Name, Address, Phone, IsActive
- [ ] Create `Book.cs` — Id, Title, Isbn, Author, PublicationYear, Genre, Description
- [ ] Create `BookCopy.cs` — Id, Book (FK), Branch (FK), Barcode, Status (enum), AcquiredDate
- [ ] Create `Member.cs` — Id, FirstName, LastName, Email, Phone, MembershipDate, Branch (FK)
- [ ] Create `BorrowRecord.cs` — Id, BookCopy (FK), Member (FK), BorrowedDate, DueDate, ReturnedDate, Status (enum)
- [ ] Create `Reservation.cs` — Id, Book (FK), Member (FK), Status (enum), PositionInQueue, CreatedAt
- [ ] Create enums: `Role`, `BookCopyStatus`, `BorrowStatus`, `ReservationStatus`
- [ ] Ensure none of these classes reference any framework namespace

**Acceptance Criterion:** All entities compile in the Domain project; no external framework references in Domain assembly.

**Deliverable:** All 7 entities + 4 enums in `LibraryManagement.Domain`.

---

### T-007 — Define domain interfaces
**Day:** 1 | **Hours:** 1

**Subtasks:**
- [ ] Create `IRepository<T>` — generic interface with: `GetByIdAsync`, `GetAllAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`, `ExistsAsync`
- [ ] Create `IUnitOfWork` — wraps `DbContext.SaveChangesAsync()`, exposes repositories
- [ ] Create `ICurrentUserService` — exposes `UserId`, `Role`, `IsAuthenticated`
- [ ] Create `IEmailService` — `SendEmailAsync(to, subject, body)` (for bonus email notifications)

**Acceptance Criterion:** All interfaces defined in Domain layer; compile with zero external dependencies.

**Deliverable:** Interface contracts for all infrastructure and service concerns.

---

### T-008 — Implement Specification pattern
**Day:** 1 | **Hours:** 1

**Subtasks:**
- [ ] Create `SpecificationBase<T>` abstract class in Domain
  - Properties: `Criteria` (Expression<Func<T, bool>>), `Includes` (List<Expression<Func<T, object>>>)
  - Method: `ToExpression()` returns the criteria
- [ ] Example specification: `BookByTitleSpecification : SpecificationBase<Book>`

**Acceptance Criterion:** Specification can be passed into a repository method; compiles cleanly.

**Deliverable:** Reusable query specification base class.

---

### T-009 — Define all DTOs
**Day:** 1 (continued) | **Hours:** 2

**Subtasks:**
- [ ] Auth DTOs: `LoginRequestDto`, `RegisterRequestDto`, `AuthResponseDto`, `UserDto`
- [ ] Branch DTOs: `BranchDto`, `CreateBranchDto`, `UpdateBranchDto`
- [ ] Book DTOs: `BookDto`, `CreateBookDto`, `UpdateBookDto`, `BookSearchDto`
- [ ] Member DTOs: `MemberDto`, `CreateMemberDto`, `UpdateMemberDto`
- [ ] Borrow/Return DTOs: `BorrowDto`, `CreateBorrowDto`, `ReturnDto`, `BorrowRecordDto`
- [ ] Reservation DTOs: `ReservationDto`, `CreateReservationDto`
- [ ] Report DTOs: `BorrowHistoryDto`, `OverdueReportDto`, `MemberActivityDto`
- [ ] Create `PagedResult<T>` wrapper for paginated queries

**Acceptance Criterion:** All DTOs defined; no circular references; all DTOs use primitives only (no entity references).

**Deliverable:** Complete set of DTOs in `LibraryManagement.Application/DTOs/`.

---

### T-010 — Create FluentValidation validators
**Day:** 1 (continued) | **Hours:** 2

**Subtasks:**
- [ ] `LoginRequestValidator` — email required + valid format; password required
- [ ] `RegisterRequestValidator` — email required + valid format + unique check (via service); password min length 6
- [ ] `CreateBranchValidator` — name required + max 200 chars; address required
- [ ] `CreateBookValidator` — title required; isbn required + valid format; author required
- [ ] `CreateMemberValidator` — firstName + lastName required; email required + valid format; phone required
- [ ] `CreateBorrowValidator` — memberId required; bookCopyId required; borrowDate ≤ today
- [ ] `CreateReservationValidator` — memberId + bookId required; check member doesn't already have book borrowed

**Acceptance Criterion:** Each validator has ≥3 rules; unit tests confirm valid/invalid inputs.

**Deliverable:** FluentValidation validators for all input DTOs.

---

### T-011 — Implement MediatR pipeline behaviors
**Day:** 1 (continued) | **Hours:** 1

**Subtasks:**
- [ ] `ValidationBehavior<TRequest, TResponse>` — runs before handler execution, uses FluentValidation
- [ ] `LoggingBehavior<TRequest, TResponse>` — logs request type, timestamp, duration
- [ ] Register both behaviors in DI (in order: logging → validation → handler)

**Acceptance Criterion:** Invalid requests fail with 400 before handler is invoked; logs show request timing.

**Deliverable:** Two pipeline behaviors wired into MediatR.

---

### T-012 — Implement Auth module (Login, Register, JWT)
**Day:** 1 (evening) + Day 2 (morning) | **Hours:** 3

**Subtasks:**
- [ ] `TokenService` — generates JWT with claims (sub, email, role), configurable expiry, symmetric key signing
- [ ] `AuthService` — validates credentials (BCrypt password check), returns token + user DTO
- [ ] `LoginCommand` + `LoginHandler` — receives `LoginRequestDto`, calls AuthService, returns `AuthResponseDto`
- [ ] `RegisterCommand` + `RegisterHandler` — receives `RegisterRequestDto`, hashes password, persists user
- [ ] `AuthController` — `POST /api/v1/auth/login`, `POST /api/v1/auth/register`
- [ ] JWT middleware configured in Program.cs (extract token from Authorization header)
- [ ] `[Authorize]` attribute works on protected endpoints
- [ ] Role-based `[Authorize(Roles = "Admin,Librarian")]` on admin endpoints
- [ ] Test: register → login → call protected endpoint with token → success
- [ ] Test: call protected endpoint without token → 401

**Acceptance Criterion:** End-to-end auth flow works: register new user → login → access protected endpoint with bearer token → 200. Accessing protected endpoint without token → 401.

**Deliverable:** Fully working authentication system with JWT and role-based authorization.

---

### T-013 — Branch Management
**Day:** 2 (morning) | **Hours:** 2

**Subtasks:**
- [ ] **Handlers:** GetBranchesQuery, GetBranchByIdQuery, CreateBranchCommand, UpdateBranchCommand, DeleteBranchCommand
- [ ] **Controller:** `BranchesController` with all 5 endpoints
- [ ] **Business validations:**
  - Branch name unique (case-insensitive)
  - Cannot delete branch with active books or members
  - Delete requires Admin role
- [ ] **Repository methods:** GetAll with pagination, GetById, GetByName for uniqueness check
- [ ] **Mapping:** Branch entity ↔ BranchDto / CreateBranchDto / UpdateBranchDto

**Acceptance Criterion:** All 5 CRUD operations work via Swagger; uniqueness violation returns 409; attempting to delete branch with books returns 409.

**Deliverable:** Branch Management module fully functional.

---

### T-014 — Book Management
**Day:** 2 (morning) | **Hours:** 3

**Subtasks:**
- [ ] **Handlers:** GetBooksQuery (paginated, filterable), GetBookByIdQuery, SearchBooksQuery, CreateBookCommand, UpdateBookCommand, DeleteBookCommand
- [ ] **Controller:** `BooksController` with all endpoints
- [ ] **Search endpoint:** Query params for title, author, isbn, genre — partial match (ILIKE/Contains)
- [ ] **Paging:** Default 20/page, max 100. Query params: `page`, `pageSize`.
- [ ] **Business validations:**
  - ISBN unique across all books
  - Delete soft or hard — decide and document
- [ ] **Repository methods:** Search with Specification pattern (filter by title/author/isbn/genre)
- [ ] **BookCopy relationship:** Understand that Book is logical; BookCopy is physical

**Acceptance Criterion:** Search returns relevant results; pagination works (page 1, 2, etc.); creating duplicate ISBN returns 409.

**Deliverable:** Book Management module fully functional with search and pagination.

---

### T-015 — Member Management
**Day:** 2 (morning) | **Hours:** 2

**Subtasks:**
- [ ] **Handlers:** GetMembersQuery, GetMemberByIdQuery, CreateMemberCommand, UpdateMemberCommand, DeleteMemberCommand
- [ ] **Controller:** `MembersController`
- [ ] **Business validations:**
  - Email unique across all members
  - Member must belong to an existing branch (BranchId FK validation)
  - MembershipDate defaults to today
- [ ] **Mapping:** Member entity ↔ MemberDto / CreateMemberDto / UpdateMemberDto

**Acceptance Criterion:** All CRUD operations work; duplicate email returns 409; creating member for non-existent branch returns 400.

**Deliverable:** Member Management module fully functional.

---

### T-016 — Borrow Management (Create Borrow)
**Day:** 2 (afternoon) | **Hours:** 3

**Subtasks:**
- [ ] **Handlers:** GetBorrowsQuery (all, admin/librarian), GetMyBorrowsQuery (member), CreateBorrowCommand, GetBorrowByIdQuery
- [ ] **Controller:** `BorrowsController`
- [ ] **Business validations (implement and test ALL):**
  1. BookCopy status must be `Available` — else 409
  2. Member must exist — else 400
  3. Member cannot have ≥ 5 active (non-returned) borrows — else 403
  4. BorrowDate must be ≤ today — else 400
  5. DueDate = BorrowDate + 14 days
  6. Create borrow record + set BookCopy status to `Borrowed`
- [ ] **Mapping:** Borrow entity ↔ BorrowDto / CreateBorrowDto

**Acceptance Criterion:** Borrowing an available book succeeds; borrowing an unavailable book returns 409; member with 5 active borrows gets 403; dueDate is exactly 14 days after borrowDate.

**Deliverable:** Borrow creation with all business rules enforced.

---

### T-017 — Return Management
**Day:** 2 (afternoon) | **Hours:** 2

**Subtasks:**
- [ ] **Handler:** ReturnBookCommand (PATCH `/api/v1/borrows/:id/return`)
- [ ] **Controller:** `BorrowsController` with `PATCH :id/return` endpoint
- [ ] **Business logic:**
  1. Find borrow record by ID
  2. Verify it's currently active (not already returned)
  3. Set `ReturnedDate = today`
  4. Set `BorrowRecord.Status = Returned`
  5. Set `BookCopy.Status = Available`
  6. Advance reservation queue for that Book (if any pending reservations exist)
- [ ] **Authorization:** Admin/Librarian can return any; Member can return own

**Acceptance Criterion:** Returning a book updates the record correctly, frees the copy, and (if applicable) advances the reservation queue. Returning an already-returned record returns 400.

**Deliverable:** Return management fully functional with reservation queue integration.

---

### T-018 — Reservation Queue
**Day:** 3 | **Hours:** 2

**Subtasks:**
- [ ] **Handlers:** GetReservationsQuery, CreateReservationCommand, CancelReservationCommand, FulfillReservationCommand
- [ ] **Controller:** `ReservationsController`
- [ ] **Business logic:**
  1. Reservation only allowed if BookCopy is not Available (book is currently borrowed)
  2. Member cannot reserve their own currently borrowed book
  3. PositionInQueue is auto-assigned (FIFO, based on CreatedAt)
  4. Cancel: only the reserving member can cancel their own pending reservation
  5. Fulfill (librarian only): when a copy is returned, the first pending reservation is assigned → notification pattern
  6. Reservation expiry after 48 hours (document as known limitation if no background job)
- [ ] **Reservation status flows:** Pending → Fulfilled (or Cancelled / Expired)

**Acceptance Criterion:** Reservations queue in FIFO order; canceling removes member from queue; fulfilling assigns copy to next in queue; member cannot reserve their own borrowed book.

**Deliverable:** Reservation Queue module fully functional.

---

### T-019 — Reports
**Day:** 3 | **Hours:** 2

**Subtasks:**
- [ ] **Handlers:**
  - `GetBorrowHistoryQuery` — parameters: fromDate, toDate, optional memberId, optional branchId
  - `GetOverdueReportQuery` — returns active borrows where DueDate < today
  - `GetMemberActivityQuery` — parameters: memberId; returns totalBorrows, activeBorrows, overdueCount, reservations
- [ ] **Controller:** `ReportsController`
- [ ] **Data joins:** BorrowRecord ← BookCopy ← Book → Author/Title; BorrowRecord ← Member; BorrowRecord ← Branch (via BookCopy)
- [ ] **Filtering:** Date range for borrow history; role-based data access (member sees only own)

**Acceptance Criterion:** All three report endpoints return correct data; date filtering works; member sees only their own data.

**Deliverable:** Reports module fully functional.

---

### T-020 — Unit Tests — Application Layer
**Day:** 2 (afternoon, ongoing) + Day 4 | **Hours:** 4

**Subtasks:**
- [ ] Test setup: xUnit project, Moq (or NSubstitute) for mocked repositories
- [ ] **Auth handler tests:**
  - Login with valid credentials → returns token
  - Login with invalid password → 401/failure
  - Login with non-existent user → 401/failure
- [ ] **Branch handler tests:**
  - GetBranches returns seed data
  - CreateBranch with duplicate name → 409
  - DeleteBranch with active books → 409
- [ ] **Book handler tests:**
  - SearchBooks by title → returns matching books
  - GetBookById with non-existent ID → 404
  - CreateBook with duplicate ISBN → 409
- [ ] **Member handler tests:**
  - CreateMember with duplicate email → 409
  - CreateMember for non-existent branch → 400
- [ ] **Borrow handler tests:**
  - Borrow available book → success
  - Borrow already-borrowed book → 409
  - Borrow when member has 5 active → 403
  - Borrow with future date → 400
- [ ] **Return handler tests:**
  - Return active borrow → success, copy becomes available, reservation advances
  - Return already-returned borrow → 400
- [ ] **Reservation handler tests:**
  - Reserve book that is borrowed → success, queue position assigned
  - Reserve own borrowed book → 400
  - Cancel reservation → removed from queue
- [ ] **Validator tests:**
  - Each validator tested with valid input (passes) and 3+ invalid inputs (fails with correct error)

**Acceptance Criterion:** All tests pass; combined Application layer coverage ≥ 80%.

**Deliverable:** Comprehensive xUnit test suite for all Application layer handlers + validators.

---

### T-021 — Unit Tests — Domain Layer
**Day:** 4 | **Hours:** 2

**Subtasks:**
- [ ] Test entity creation (set properties, verify values)
- [ ] Test that entities enforce invariants (where applicable)
- [ ] Test enum values exist and are correct
- [ ] Test Specification pattern (criteria evaluation)
- [ ] Test DTO mapping (via AutoMapper profiles or manual mapping tests)

**Acceptance Criterion:** Domain and mapping tests pass; combined overall coverage ≥ 70%.

**Deliverable:** Domain layer tests providing baseline coverage.

---

### T-022 — Frontend Scaffold (Angular Standalone + Routing)
**Day:** 3 | **Hours:** 2

**Subtasks:**
- [ ] Initialize Angular standalone project with routing and plain CSS: `ng new LibraryManagement.Web --standalone --routing --style=css`
- [ ] Install `@angular/forms` (reactive forms support)
- [ ] Set up router with routes: `/login` (public), `/` (dashboard), `/branches`, `/books`, `/members`, `/borrows`, `/reservations`, `/reports`
- [ ] Create `AppLayoutComponent` with sidebar navigation + header + `<router-outlet>` for page rendering
- [ ] Create `AuthGuard` (redirect to `/login` if no token in sessionStorage)
- [ ] Create `RoleGuard` (redirect if user doesn't have required role)
- [ ] Create `LoginComponent` (email + password reactive form → call auth API → store token + user in sessionStorage → redirect to `/`)
- [ ] Configure `HttpClient` instance (`api.ts`) with request interceptor (attach `Authorization: Bearer <token>`) and response interceptor (401 → redirect to login, 4xx/5xx → show error toast)

**Acceptance Criterion:** `ng serve` serves the app; login page works; protected routes redirect unauthenticated users; role-based routes reject unauthorized access.

**Deliverable:** Angular app scaffolded with routing, layout component, login page, and HTTP client configured.

---

### T-023 — Frontend Auth Service & API Services
**Day:** 3 | **Hours:** 1

**Subtasks:**
- [ ] Create `authService.ts` (singleton Angular service) — provides `login()`, `logout()`, `isAuthenticated()`, `getRole()` — stores token in `sessionStorage`
- [ ] Token stored in sessionStorage (document trade-off vs localStorage)
- [ ] Auth service consumed by `AuthGuard` and `RoleGuard` for role-based route access
- [ ] Create `authService.ts` — `login(email, password)`, `register(data)` (API calls)
- [ ] Create `branchService.ts`, `bookService.ts`, `memberService.ts`, `borrowService.ts`, `reservationService.ts`, `reportService.ts`
- [ ] Each service module wraps Angular `HttpClient` calls with proper error handling

**Acceptance Criterion:** Auth service works across routes; all service modules call correct API endpoints via HttpClient.

**Deliverable:** Angular auth service and HTTP API service layer complete.

---

### T-024 — Frontend Dashboard
**Day:** 3 | **Hours:** 1

**Subtasks:**
- [ ] Fetch summary data from API (total books, active borrows, pending reservations, overdue count)
- [ ] Display as summary cards on dashboard
- [ ] Role-based filtering (admin sees all; member sees only their own)
- [ ] Link summary cards to detailed pages

**Acceptance Criterion:** Dashboard renders correct counts; counts change based on role.

**Deliverable:** Working dashboard page with summary metrics.

---

### T-025 — Frontend Branch CRUD Pages
**Day:** 3 | **Hours:** 2

**Subtasks:**
- [ ] `BranchListPage` — table of branches with search bar and pagination
- [ ] `BranchDetailPage` — display full branch details
- [ ] `BranchFormPage` — create/edit form with validation
- [ ] Delete button with confirmation dialog
- [ ] Loading states and error handling (toast notifications)

**Acceptance Criterion:** All CRUD operations work from the UI; search filters branches; delete requires confirmation.

**Deliverable:** Branch Management pages in frontend.

---

### T-026 — Frontend Book CRUD Pages (with Search)
**Day:** 3–4 | **Hours:** 2

**Subtasks:**
- [ ] `BookListPage` — table with debounced search bar (title, author, isbn, genre), pagination
- [ ] `BookDetailPage` — display book details + available copies
- [ ] `BookFormPage` — create/edit form with validation
- [ ] Search filters work (debounced input, API query params)
- [ ] Pagination controls (page numbers)

**Acceptance Criterion:** Search returns results as you type; pagination switches pages; create/edit works.

**Deliverable:** Book Management pages with search and pagination in frontend.

---

### T-027 — Frontend Member CRUD Pages
**Day:** 3–4 | **Hours:** 2

**Subtasks:**
- [ ] `MemberListPage` — table with search, pagination
- [ ] `MemberDetailPage` — display member details + their borrow history summary
- [ ] `MemberFormPage` — create/edit form with validation
- [ ] Branch selector (dropdown populated from API)

**Acceptance Criterion:** All CRUD operations work; member creation validates email uniqueness and branch existence.

**Deliverable:** Member Management pages in frontend.

---

### T-028 — Frontend Borrow/Return Pages
**Day:** 4 | **Hours:** 2

**Subtasks:**
- [ ] `BorrowListPage` — list active borrows (admin/librarian view) or own borrows (member view)
- [ ] `BorrowFormPage` — select member + book copy → create borrow; show validation errors
- [ ] `ReturnPage` or return action in `BorrowListPage` — click Return → confirmation → success
- [ ] Error handling: show why borrow failed (book unavailable, limit reached, etc.)

**Acceptance Criterion:** Borrow creates a record visible on the borrow list; Return updates status and makes copy available again.

**Deliverable:** Borrow/Return UI pages functional.

---

### T-029 — Frontend Reservation Queue Page
**Day:** 4 | **Hours:** 1

**Subtasks:**
- [ ] `ReservationListPage` — shows all reservations (admin/librarian) or member's own reservations
- [ ] Fulfill button for admin/librarian on pending reservations
- [ ] Cancel button for member on own pending reservations
- [ ] Show queue position / order

**Acceptance Criterion:** Reservations can be created, fulfilled, and canceled from the UI; queue position is visible.

**Deliverable:** Reservation Queue page in frontend.

---

### T-030 — Frontend Reports Page
**Day:** 4 | **Hours:** 1

**Subtasks:**
- [ ] `ReportsPage` — tabs or sections for Borrow History, Overdue Books, Member Activity
- [ ] Borrow History: date range picker + table of results
- [ ] Overdue Books: table showing book title, member name, days overdue
- [ ] Member Activity: member selector/input + summary stats table
- [ ] All tables are responsive (horizontal scroll on mobile)

**Acceptance Criterion:** All three reports load data correctly; date filtering works; member sees only their own activity.

**Deliverable:** Reports page with all three report types.

---

### T-031 — Frontend Responsive Design & Polish
**Day:** 4 | **Hours:** 1

**Subtasks:**
- [ ] Test on mobile viewport (375px)
- [ ] Sidebar collapses to hamburger menu
- [ ] Data tables scroll horizontally on small screens
- [ ] Toast notifications for success/error on all operations
- [ ] Loading spinners on async operations (API calls)
- [ ] Consistent spacing, colors, and typography across all pages
- [ ] Form error messages visible and clear
- [ ] Login page centered with clean design

**Acceptance Criterion:** App is usable on mobile; no layout breakage; all interactions provide visual feedback.

**Deliverable:** Polished, responsive frontend.

---

### T-032 — Write README.md
**Day:** 4 | **Hours:** 1

**Subtasks:**
- [ ] Project overview (2–3 sentences)
- [ ] Features list (all 7 modules + bonus features attempted)
- [ ] Tech stack
- [ ] Prerequisites (Node, .NET SDK 10+, PostgreSQL)
- [ ] Setup instructions (step-by-step: clone, DB setup, connection string, run migrations, run API, run frontend)
- [ ] Configuration section (environment variables explained)
- [ ] How to run the application
- [ ] How to run tests
- [ ] Assumptions and design decisions
- [ ] Screenshots (optional — can skip if short on time)

**Acceptance Criterion:** A new developer could follow README and run the application from scratch.

**Deliverable:** Complete README.md.

---

### T-033 — Docker + docker-compose (Bonus)
**Day:** 4 | **Hours:** 1 | **[OPTIONAL]**

**Subtasks:**
- [ ] Write `Dockerfile` for API project (multi-stage build: build + runtime)
- [ ] Write `docker-compose.yml` with API + PostgreSQL services
- [ ] Verify `docker-compose up` starts both services and API is accessible
- [ ] Verify API responds to HTTP requests (e.g., `/swagger` or `/api/v1/health`)

**Acceptance Criterion:** `docker-compose up` creates both containers; API is reachable.

**Deliverable:** Docker build artifacts.

---

### T-034 — GitHub Actions CI/CD (Bonus)
**Day:** 4 | **Hours:** 0.5 | **[OPTIONAL]**

**Subtasks:**
- [ ] Create `.github/workflows/ci.yml`
- [ ] Workflow triggers on push and pull_request
- [ ] Steps: checkout → setup dotnet → restore → build → test
- [ ] Push triggers to repo and verify workflow runs

**Acceptance Criterion:** Push to repo triggers GitHub Actions run; all tests pass in CI.

**Deliverable:** CI pipeline configured.

---

### T-035 — Final Submission
**Day:** 5 (Buffer / Aug 1) | **Hours:** 1

**Subtasks:**
- [ ] Verify all modules work end-to-end via Swagger
- [ ] Run full test suite one final time: `dotnet test` — all pass
- [ ] Final README review (accurate, no outdated steps)
- [ ] Verify no secrets/API keys in repo (grep for `password`, `secret`, `Bearer`, connection strings)
- [ ] Clean up git history if needed (squash minor commits)
- [ ] Email submission with: full name, position, repo URL, implementation summary

**Acceptance Criterion:** All acceptance criteria in `PLAN.md` Section 10 (Evaluation Criteria) are met.

**Deliverable:** Final submission email sent.

---

## Dependency Order (What Must Come First)

```
T-001 → T-002 → T-003 → T-004 → T-005
                                            ↓
T-006 ──────────────────────────────────────┘
    ↓
T-007 → T-008
    ↓ (feeds into application layer)
T-009 → T-010 → T-011 → [T-012 is first user-facing feature]
    ↓
T-013 → T-014 → T-015 (all CRUD modules — can be partially parallelized)
    ↓
T-016 → T-017 (Borrow & Return depend on Book, Member, Branch)
    ↓
T-018     (Reservation depends on Borrow + Book)
    ↓
T-019     (Reports depend on BorrowRecord + all other modules)

Parallel with backend (start Day 3):
T-022 → T-023 → T-024 → T-025 → T-026 → T-027 → T-028 → T-029 → T-030 → T-031

Testing (run alongside development):
T-020 (alongside T-012 → T-019) → T-021 (Day 4)

Documentation (Day 4):
T-032 → T-033 → T-034 → T-035 → T-036
```

---

## Priority Order When Behind Schedule

If the timeline slips, drop in this order:

1. **Last to drop:** Auth, Branch/Book/Member CRUD, Borrow/Return — these are core evaluation criteria
2. **Second to drop:** Reservation Queue, Reports — these are important but can be simplified
3. **Third to drop:** Frontend polish, responsive design — backend completeness is weighted more (25 vs 10 for frontend)
4. **First to drop:** Bonus features (Docker, CI/CD, email, Excel export, Redis, Domain Events)

> **Key insight:** The assessment values functional requirements (25 marks) over everything else. Having all 7 modules working with clean backend architecture beats having a pretty frontend with missing modules.
