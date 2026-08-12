-- Seed the Comercial role and the default Ana Comercial user.
-- The password for this user matches the SEED_PASSWORD default: FyaDev123!

INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp")
SELECT '019ff64a-16cb-7244-950f-2de10ddd9244', 'Comercial', 'COMERCIAL', '6f2b8f1a-4d7c-4a2e-b3c9-0f2d1a9c5b41'
WHERE NOT EXISTS (SELECT 1 FROM "AspNetRoles" WHERE "Name" = 'Comercial');

INSERT INTO "AspNetUsers" (
    "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail",
    "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp",
    "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount", "FullName")
SELECT '019ff64a-17e1-7ff4-8ec7-b5400203bf67',
       'ana.comercial@fyasocialcapital.com',
       'ANA.COMERCIAL@FYASOCIALCAPITAL.COM',
       'ana.comercial@fyasocialcapital.com',
       'ANA.COMERCIAL@FYASOCIALCAPITAL.COM',
       true,
       'AQAAAAIAAYagAAAAEN/XKj9mqHPKQTjoTOeqhGMi29eSm3WONhDekJ5UxBle5yelqAYdhzcf1UIWPlHQLw==',
       'CUPWT3A2NHBCASZDARUFV3JN2EQBROA2',
       '2cb1b82c-a658-4ae7-ac63-88082336f93e',
       false, false, true, 0, 'Ana Comercial'
WHERE NOT EXISTS (SELECT 1 FROM "AspNetUsers" WHERE "Email" = 'ana.comercial@fyasocialcapital.com');

INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
SELECT '019ff64a-17e1-7ff4-8ec7-b5400203bf67', '019ff64a-16cb-7244-950f-2de10ddd9244'
WHERE NOT EXISTS (
    SELECT 1 FROM "AspNetUserRoles"
    WHERE "UserId" = '019ff64a-17e1-7ff4-8ec7-b5400203bf67'
      AND "RoleId" = '019ff64a-16cb-7244-950f-2de10ddd9244'
);
