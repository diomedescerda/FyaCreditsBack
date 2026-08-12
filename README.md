# Fya Credits Backend

ASP.NET Core Web API on .NET 10 with Clean Architecture, EF Core, and PostgreSQL.

## Projects

- `src/FyaCredits.Domain`: domain model and invariants.
- `src/FyaCredits.Application`: use cases and application abstractions.
- `src/FyaCredits.Infrastructure`: EF Core persistence and notification worker.
- `src/FyaCredits.WebApi`: HTTP API, JWT authentication, and OpenAPI.

## Local Setup

1. Copy `.env.example` to `.env` and replace all development placeholder values.
2. Start PostgreSQL with `docker compose up -d postgres`.
3. Apply migrations:

```bash
dotnet ef database update \
  --project src/FyaCredits.Infrastructure \
  --startup-project src/FyaCredits.WebApi
```

4. Run the API:

```bash
dotnet run --project src/FyaCredits.WebApi
```

The OpenAPI document is available at `/openapi/v1.json` in development. Health status is available at `/health`.

## Docker

The Compose API container expects `.env` values for passwords and the JWT signing key:

```bash
cp .env.example .env
```

Apply migrations deliberately against the running API/database environment before using the application. Do not commit `.env` or any real credentials.

## Email (development)

During development, credit notifications are sent to `diomedescerda@gmail.com` using Gmail SMTP. A Gmail address must be used as the sender because Gmail rejects messages that claim a Gmail `From` address but are delivered by third-party servers (DMARC alignment).

To make sending work:

1. On the Google account, enable **2-Step Verification** (Google → Security → 2-Step Verification).
2. Create an **App Password** (Google → Security → App passwords → app "Mail" → generate a 16-character password).
3. Put that password in `.env` (gitignored):
   ```env
   SMTP_HOST=smtp.gmail.com
   SMTP_PORT=587
   SMTP_ENABLE_SSL=true
   SMTP_USERNAME=diomedescerda@gmail.com
   SMTP_PASSWORD=your-gmail-app-password
   EMAIL_FROM=diomedescerda@gmail.com
   EMAIL_TO=diomedescerda@gmail.com
   ```
4. Recreate the API container so it picks up the new values:
   ```bash
   docker compose up -d api
   ```

After this, registering a credit sends the notification email. Confirm in `docker compose logs api` that no `Credit notification` failure is logged.

For the production deliverable, the recipient should be `fyasocialcapital@gmail.com` and the sender should use an authenticated domain through a dedicated ESP (SendGrid/Mailgun).

## Database artifacts

Committed SQL for creating and seeding the schema is available under `database/`:

- `database/init.sql`: idempotent EF Core migration SQL that creates the `credits` table.
- `database/seed.sql`: sample credits from the technical test annex.

Apply them to the running Compose PostgreSQL instance:

```bash
docker compose exec -T postgres psql -U postgres -d fyacredits < database/init.sql
docker compose exec -T postgres psql -U postgres -d fyacredits < database/seed.sql
```

Alternatively, from the host when the PostgreSQL port is reachable:

```bash
dotnet ef database update \
  --project src/FyaCredits.Infrastructure \
  --startup-project src/FyaCredits.WebApi
```

## API Flow

1. Request a demo JWT from `POST /api/auth/token` with a commercial name and configured demo password.
2. Send the token as `Authorization: Bearer <token>` to `POST /api/credits` and `GET /api/credits`.
3. Credit query filters and sorting are server-side query parameters. Supported sorting values are `date` and `amount`.

The commercial representative is derived from the authenticated token. Credit registration publishes an email notification to the in-process producer/consumer queue after persistence.
