# CLAUDE.md

Instructions for Claude Code working in this repository.

## Project

Helpdesk web application (ticket system) built for the university course **"Testowanie i Jakość Oprogramowania"**.
The project is graded on code quality and tests, so follow the requirements below strictly.

## Stack (decided, do not change without asking)

- C# / .NET 10, solution file `Helpdesk.slnx`
- ASP.NET Core **Razor Pages** for the UI + a small set of **API controllers** (needed for the "API documentation" section)
- Entity Framework Core + **SQLite**
- Cookie authentication with roles `User` and `Admin`
- Tests: **xUnit**, **FluentAssertions 7.2.0** (do NOT upgrade to 8.x – commercial license), **Moq**
- Integration tests: `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory<Program>`)
- Bootstrap 5 from CDN

## Repository layout

```
Helpdesk.slnx
Directory.Build.props        # net10.0, nullable, code style enforced on build
.editorconfig                # IDE0005 (unused usings) = warning, naming rules
src/Helpdesk.Web/            # the application
tests/Helpdesk.UnitTests/
tests/Helpdesk.IntegrationTests/
README.md                    # skeleton only – the author fills it in, do not rewrite it
```

## Functional requirements

- Login as **User** or **Admin**. A default admin account is seeded on startup.
- **User**: create, edit and delete **own** tickets only; sees only own tickets.
- **Admin**: create, edit, delete users; create, edit, delete **all** tickets; change ticket status.
- Ticket fields: Title, Description, Priority, Status, CreatedAt, UpdatedAt, author (UserId).
- **Priority**: `Low`, `Medium`, `High`.
- **Status**: `Open` → `InProgress` → `Completed` (only Admin changes status).
- **Lock**: once a ticket is `Completed`, the User can no longer edit or delete it (Admin still can).
- Ticket list supports **filtering** (status, priority, title search) and **sorting** (date, priority, title).
- UI texts in **Polish**; all code identifiers in **English**.

## Course requirements (mandatory)

- Minimum **10 unit tests** and **10 integration tests**.
- Every test follows **AAA** with explicit `// Arrange`, `// Act`, `// Assert` comments.
- Test naming: `MethodName_Scenario_ExpectedResult`.
- No unused usings (build warns via IDE0005 – keep the build warning-free).
- Apply SOLID and design patterns where they make sense (services behind interfaces, dependency injection, e.g. a strategy for sorting/filtering).
- No Polish identifiers, no unclear names, no huge classes or methods, testable code (business logic in services, not in PageModels or controllers).

## Planned architecture

```
src/Helpdesk.Web/
  Domain/        # entities + enums (User, Ticket, Role, TicketStatus, TicketPriority)
  Data/          # HelpdeskDbContext, migrations, DbSeeder
  Services/      # IAuthService, ITicketService, IUserService, IPasswordHasher + implementations
  Contracts/     # DTOs / request models used by API and pages
  Controllers/   # API controllers (api/tickets, api/users, api/auth)
  Pages/         # Razor Pages: Account/Login, Tickets/*, Admin/Users/*
```

Services return results that PageModels/controllers translate to HTTP/UI responses; authorization rules
(ownership, completed-ticket lock) live in services so they can be unit-tested.

## Plan (one step = one commit)

- [x] 1. Scaffold solution, test projects, editorconfig, README skeleton
- [x] 2. Domain model + DbContext + SQLite + initial migration + admin seed
- [x] 3. Authentication backend: password hashing, AuthService, cookie auth, roles + unit tests
- [x] 4. Login frontend: login/logout pages, layout navigation depending on role
- [x] 5. Tickets backend: TicketService (ownership, lock, filtering, sorting) + API + unit tests
- [x] 6. Tickets frontend: list with filters/sorting, create, edit, delete
- [x] 7. User management backend (Admin only) + API + unit tests
- [x] 8. Admin panel frontend: users CRUD, all tickets, status change
- [ ] 9. Integration tests (min. 10) with WebApplicationFactory
- [ ] 10. API documentation (Swagger/OpenAPI)
- [ ] 11. Final cleanup (unused usings, naming, small methods)

Update the checkboxes above when a step is done.

## Workflow rules

- Do **one step at a time**. After finishing a step: `dotnet build` (no warnings), `dotnet test`, then commit
  with a Conventional Commits message (`feat:`, `test:`, `chore:` …) and stop – wait for the user before the next step.
- If something important is unclear, **ask** instead of guessing. Propose (do not silently add) extra features.
- Do not fill in README sections – the author does that. You may only tell the user what to put there.

## Commands

```bash
dotnet build
dotnet test
dotnet run --project src/Helpdesk.Web            # http://localhost:5080
dotnet ef migrations add <Name> --project src/Helpdesk.Web
```
