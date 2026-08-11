CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260811164121_InitialCreate') THEN
    CREATE TABLE credits (
        "Id" uuid NOT NULL,
        "ClientName" character varying(150) NOT NULL,
        "ClientId" character varying(50) NOT NULL,
        "Amount" bigint NOT NULL,
        "InterestRate" numeric(5,2) NOT NULL,
        "TermMonths" integer NOT NULL,
        "CommercialName" character varying(150) NOT NULL,
        "RegisteredAtUtc" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_credits" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260811164121_InitialCreate') THEN
    CREATE INDEX "IX_credits_Amount" ON credits ("Amount");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260811164121_InitialCreate') THEN
    CREATE INDEX "IX_credits_ClientId" ON credits ("ClientId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260811164121_InitialCreate') THEN
    CREATE INDEX "IX_credits_CommercialName" ON credits ("CommercialName");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260811164121_InitialCreate') THEN
    CREATE INDEX "IX_credits_RegisteredAtUtc" ON credits ("RegisteredAtUtc");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260811164121_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260811164121_InitialCreate', '10.0.0');
    END IF;
END $EF$;
COMMIT;

