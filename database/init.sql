CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE "Entries" (
    "Id" uuid NOT NULL,
    "Password" text NOT NULL,
    "NumberOfAccesses" bigint NULL,
    "ValidUntil" timestamp NULL,
    "CreatedAt" timestamp NOT NULL,
    CONSTRAINT "PK_Entries" PRIMARY KEY ("Id")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20190828145559_AddedValidUntilTime', '3.1.1');

ALTER TABLE "Entries" ADD "SessionId" uuid NULL;

CREATE TABLE "Session" (
    "Id" uuid NOT NULL,
    "ExpiryTime" timestamp NOT NULL,
    CONSTRAINT "PK_Session" PRIMARY KEY ("Id")
);

CREATE INDEX "IX_Entries_SessionId" ON "Entries" ("SessionId");

ALTER TABLE "Entries" ADD CONSTRAINT "FK_Entries_Session_SessionId" FOREIGN KEY ("SessionId") REFERENCES "Session" ("Id") ON DELETE CASCADE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200121164658_Session', '3.1.1');

ALTER TABLE "Entries" DROP CONSTRAINT "FK_Entries_Session_SessionId";

ALTER TABLE "Session" DROP CONSTRAINT "PK_Session";

ALTER TABLE "Session" RENAME TO "Sessions";

ALTER TABLE "Sessions" ADD CONSTRAINT "PK_Sessions" PRIMARY KEY ("Id");

ALTER TABLE "Entries" ADD CONSTRAINT "FK_Entries_Sessions_SessionId" FOREIGN KEY ("SessionId") REFERENCES "Sessions" ("Id") ON DELETE CASCADE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200122111528_Sessions', '3.1.1');

