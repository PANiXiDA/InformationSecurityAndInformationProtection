
CREATE DATABASE "bank";

CREATE USER "bank" WITH PASSWORD 'password';
\c bank;

CREATE TABLE "users" (
	"usr" VARCHAR(128) PRIMARY KEY,
	"pwd" BYTEA NOT NULL,
	"parrots" INT
);

INSERT INTO "users"("usr", "pwd", "parrots") 
     VALUES ('hacker', '\x936a185caaa266bb9cbe981e9e05cb78cd732b0b3280eb944412bb6f8f8f07af', 12500), 
            ('user', '\x5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8', 1000);

GRANT SELECT,UPDATE ON "users" TO "bank";
