#!/bin/bash

STARTUP_PROJECT="src/EShopApp.Api/EShopApp.Api.csproj"
PROJECT="src/EShopApp.Infrastructure/EShopApp.Infrastructure.csproj"

# Run the EF Core remove migration command
dotnet ef migrations remove \
    --project "$PROJECT" \
    --startup-project "$STARTUP_PROJECT"

# Check if removal was successful
if [ $? -eq 0 ]; then
    echo "Last migration removed successfully."
else
    echo "Error removing migration."
fi
