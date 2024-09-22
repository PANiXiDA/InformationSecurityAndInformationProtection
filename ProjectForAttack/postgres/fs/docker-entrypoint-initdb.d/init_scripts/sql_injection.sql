
CREATE USER "auth_user" WITH PASSWORD 'password';

DROP DATABASE IF EXISTS "auth";
CREATE DATABASE "auth";

\c auth;

CREATE TABLE "users" (
  "id" SERIAL PRIMARY KEY,
  "user" VARCHAR(64),
  "password" BYTEA
);

INSERT INTO "users"("user", "password") VALUES
   ('admin', NULL);

GRANT SELECT ON "users" TO "auth_user";

