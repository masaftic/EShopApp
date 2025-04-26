#!/bin/bash

# Pull the SQL Server Docker image

# Run the SQL Server container
docker run -e "ACCEPT_EULA=Y" \
           -e "SA_PASSWORD=Pass0Word!" \
           -p 1434:1433 \
           --name eshopapp_test_db \
           -d mcr.microsoft.com/mssql/server:2022-latest

echo "SQL Server container is running"
# echo "Server=localhost;Database=EShopApp_Tests;User Id=sa;Password=Pass0Word!;"