FROM ubuntu:22.04

RUN apt-get update
RUN DEBIAN_FRONTEND=noninteractive \
    TZ=Etc/UTC \
        apt-get -y install apache2 \
                           php8.1 \
		           libapache2-mod-php8.1 \
		           php8.1-pgsql

RUN a2enmod php8.1

ENV APACHE_RUN_DIR=/var/run/apache2 \
    APACHE_RUN_USER=www-data \
    APACHE_RUN_GROUP=www-data \
    APACHE_LOG_DIR=/var/log/apache2

RUN cat /etc/php/8.1/apache2/php.ini \
  | sed 's/;extension=pdo_pgsql/extension=pdo_pgsql/' \
  > /etc/php/8.1/apache2/php.ini

COPY web /var/www/html

CMD ["apache2", "-D", "FOREGROUND"]


