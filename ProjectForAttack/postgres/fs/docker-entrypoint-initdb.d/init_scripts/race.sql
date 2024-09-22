DROP DATABASE IF EXISTS "game";
CREATE DATABASE "game";

CREATE USER "racer" WITH PASSWORD 'racer';
\c game;

CREATE TABLE "payments" (
	"id" SERIAL PRIMARY KEY,
	"account" INT NOT NULL,
	"ammount" INT NOT NULL,
	"text" VARCHAR(512),
	"cancel" SMALLINT NOT NULL DEFAULT 0
);

CREATE TABLE "accounts" (
	"id" SERIAL PRIMARY KEY,
	"login" VARCHAR(32),
	"sault" CHAR(16),
	"password" CHAR(32),
	"money" INT NOT NULL DEFAULT 0
);

GRANT USAGE,SELECT ON SEQUENCE "payments_id_seq" TO "racer";
GRANT SELECT,UPDATE,INSERT,DELETE ON "payments" TO "racer";

GRANT USAGE,SELECT ON SEQUENCE "accounts_id_seq" TO "racer";
GRANT SELECT,UPDATE,INSERT ON "accounts" TO "racer";

INSERT INTO "accounts"("id", "login", "sault", "password", "money") VALUES
	(1, 'Nagibator3000', 'aaaaaaaaaaaaaaab', MD5('aaaaaaaaaaaaaaabNagibator3000'::bytea)::char(32), 75042);

-- INSERT INTO "payments"("id", "account", "ammount", "text") VALUES 
-- 	(1, 1, 75000, 'Заметный рост самого важного показателя! Все обзавидуются.');

