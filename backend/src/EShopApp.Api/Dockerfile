FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the solution and restore dependencies
COPY *.sln .
COPY ["src/EShopApp.Api/EShopApp.Api.csproj", "src/EShopApp.Api/"]
COPY ["src/EShopApp.Application/EShopApp.Application.csproj", "src/EShopApp.Application/"]
COPY ["src/EShopApp.Infrastructure/EShopApp.Infrastructure.csproj", "src/EShopApp.Infrastructure/"]
COPY ["src/EShopApp.Domain/EShopApp.Domain.csproj", "src/EShopApp.Domain/"]
COPY ["tests/EShopApp.Tests/EShopApp.Tests.csproj", "tests/EShopApp.Tests/"]
COPY ["tests/EShopApp.IntegrationTests/EShopApp.IntegrationTests.csproj", "tests/EShopApp.IntegrationTests/"]
RUN dotnet restore

# Copy the rest of the application code and build the application
COPY . .
WORKDIR /src/src/EShopApp.Api
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Use the official .NET runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

# Expose ports for HTTP and HTTPS
ENV ASPNETCORE_URLS=http://+:5000;
EXPOSE 5000

# Set the entry point for the container
ENTRYPOINT ["dotnet", "EShopApp.Api.dll"]
