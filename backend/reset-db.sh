#!/bin/bash

# Stop and remove the existing database container
docker-compose down

# Remove the associated volume
docker volume rm eshopapp_sqlserver_data

# Start the database container again
docker-compose up -d db