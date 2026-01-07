# Project Instructions

## Backend

### Description

- The backend follows a DDD-inspired structure with four projects: Domain, Infrastructure, Application and WebApi.
- Target framework: .NET 10.
- Dependency Injection is used to register application and infrastructure services.
- Repository-style data access is implemented in the Infrastructure layer.
- Mapster is used for entity/DTO mapping.
- Scalar (Scalar.AspNetCore) is used to expose an OpenAPI-based UI at `/scalar`.
- Integration tests use Testcontainers (real SQL Server) instead of the InMemory provider.
- Docker Compose is used to run the backend and its SQL Server dependency.
## Backend implementation details

- **Project structure:** The backend is split into four projects following a DDD-inspired layout: `Fundo.Applications.Domain` (entities and domain types), `Fundo.Applications.Infrastructure` (EF Core, repositories, migrations), `Fundo.Applications.Application` (services, DTOs, application logic), and `Fundo.Applications.WebApi` (HTTP controllers, DI, startup).
- **Target framework:** .NET 10 (SDK-style projects). Build output and configuration files are in `backend/src`.
- **Dependency injection:** Each project exposes a `DependencyInjection` helper (`DependencyInjection.cs`) that registers services into the DI container. The WebApi project composes these registrations at startup.
- **Data access and EF Core:** The Infrastructure project uses EF Core with repository-style patterns. Migrations are included under `Migrations/`. Tests use Testcontainers for SQL Server rather than the InMemory provider to better match production behavior.
- **Mapping:** Mapster is used for mapping between entities and DTOs. Mapping configuration is typically done in the Application project or a shared mapping profile.
- **OpenAPI & Scalar:** Scalar (Scalar.AspNetCore) is used to expose an OpenAPI UI at `/scalar`. The UI will show controllers, DTOs and available endpoints. The Scalar endpoint is configured in `Fundo.Applications.WebApi`.
- **Logging & configuration:** The WebApi project uses standard `appsettings.json` and environment overrides. Logging is configured via the host builder; check `Program.cs` for providers and minimum levels.
- **Migrations on startup:** The app may apply EF Core migrations at startup via an extension (`MigrationExtension.MigrateDatabase()`). Control this behavior with environment variables or by editing that extension if you want to disable automatic migrations in certain environments.
- **Testing:** Unit tests are under `Fundo.Services.Tests/Unit` and integration tests under `Fundo.Services.Tests/Integration`. Integration tests use Testcontainers to bring up a real SQL Server instance.

Developer notes:

- **Where to look for DI registrations:** `Fundo.Applications.Application/DependencyInjection.cs` and `Fundo.Applications.Infrastructure/DependencyInjection.cs` are the canonical places to see how services and repositories are wired.
- **Entity definitions:** Domain entities live in `Fundo.Applications.Domain/Entities`. Look at `Loan.cs` and `LoanStatus.cs` for core domain types.
- **Service implementations:** Application service interfaces and implementations live in `Fundo.Applications.Application/Loans` (e.g., `ILoanService.cs`, `LoanService.cs`). These orchestrate repository calls and mapping.

### How to run (Backend)

Open a terminal and run the following from the `backend/src` folder:

```powershell
# stop and remove previous containers and volumes
docker compose down --volumes

# build and start services in the foreground (use -d to run detached)
docker compose up --build
```

Then open your browser at:

- `https://localhost:5001/scalar` — Scalar UI (OpenAPI UI)

If you prefer detached mode:

```powershell
docker compose up -d --build
```

## Frontend

### Description

- The frontend is built with Angular components.
- Axios is used for HTTP calls to the backend API.
- Orval is used to generate a TypeScript API client from the OpenAPI spec. Run `npm run generate:api` to regenerate the client when the API surface changes.

## Frontend implementation details

- **Framework & tooling:** The frontend is an Angular application (CLI-based) located in the `frontend/` folder. It uses TypeScript, SCSS, and the Angular router.
- **API client & HTTP:** API models and client code are generated with Orval into `frontend/src/api/generated.ts`. The app uses Axios for HTTP calls via a thin wrapper (`frontend/src/client/axiosInstance.ts`) that configures base URL, interceptors, and common headers.
- **App layout:** Main application bootstrap files are `main.ts` and `index.html`. Routes are defined in `frontend/src/app/app.routes.ts`. Core pages live under `frontend/src/app/` such as `loans-list`, `loans-create` and `payments`.
- **Configuration:** Environment-specific values (API base URL, feature flags) are in `frontend/src/environments/environment.ts` and `environment.prod.ts`.
- **Generating the API client:** Run `npm run generate:api` (this runs Orval) after the backend is running and exposing the OpenAPI spec at the configured URL (the default is the running WebApi service). Ensure the backend is reachable (CORS allowed) and the URL in `orval.config.ts` matches the running backend.
- **Styling:** Global styles are in `styles.scss`. Component styles live adjacent to components as SCSS files.

Developer notes:

- **Axios instance:** Customize `frontend/src/client/axiosInstance.ts` to add auth tokens, error interceptors, or different base URLs for local/debug runs.
- **Orval config:** `orval.config.ts` holds the OpenAPI endpoint and generation options. If you change backend routes, run the generate script to keep the typed client in sync.
- **Local dev:** Running `npm start` runs the Angular dev server at `http://localhost:4200/` by default. If the backend runs on HTTPS (5001), update the `proxy.conf.json` or `axiosInstance` base URL and accept self-signed certs for local development if necessary.

### How to run (Frontend)

From the `frontend` directory:

```bash
npm install
npm start
```

The frontend will typically be available at `http://localhost:4200/`.

## Notes and Troubleshooting

- If you see ephemeral high-numbered host ports when launching the Docker profile from an IDE, ensure `docker-compose.override.yml` maps explicit host ports (the repo should map `5000:5000` and `5001:5001`).
- If the backend applies EF Core migrations at startup, you can control this behavior with an environment variable or by editing `MigrationExtension.MigrateDatabase()` if you need to avoid automatic migrations in certain environments.
- If you need to inspect SQL Server logs or container status:

```powershell
docker compose ps
docker logs <container-name>
```