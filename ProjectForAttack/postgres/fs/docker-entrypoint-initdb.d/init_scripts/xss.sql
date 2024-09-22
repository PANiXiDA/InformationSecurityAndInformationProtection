
DROP DATABASE IF EXISTS "forum";

CREATE USER "comment_user" WITH PASSWORD 'password';
CREATE DATABASE "forum";

\c forum;

CREATE TABLE "comments" (
  "id" SERIAL PRIMARY KEY,
  "text" varchar(512)
);

INSERT INTO "comments"("text") VALUES ('Замечательный сайт, тут точно не украдут ничей пароль'); 

GRANT USAGE,SELECT ON SEQUENCE comments_id_seq TO "comment_user";
GRANT SELECT,INSERT ON "comments" TO "comment_user";

CREATE DATABASE "hacker_db";

\c hacker_db;

CREATE TABLE "stolen_identity" (
	"id" SERIAL PRIMARY KEY,
	"data" varchar(2048) NOT NULL
);

CREATE USER "hacker" WITH PASSWORD 'cool_hacker';
GRANT INSERT,UPDATE,SELECT ON "stolen_identity" TO "hacker";
GRANT USAGE,SELECT ON "stolen_identity_id_seq" TO "hacker";

