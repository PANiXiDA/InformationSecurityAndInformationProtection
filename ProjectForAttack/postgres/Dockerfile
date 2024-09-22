FROM postgres:15

COPY fs /
RUN chmod -R 755 /docker-entrypoint-initdb.d
ENV POSTGRES_DB=postgres
ENV POSTGRES_USER=postgres
ENV POSTGRES_PASSWORD=password
RUN apt-get update && apt-get install -y coreutils
RUN apt-get update && apt-get install -y bash

