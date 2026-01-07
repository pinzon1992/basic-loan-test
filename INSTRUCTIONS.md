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