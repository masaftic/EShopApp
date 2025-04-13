# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/EShopApp.Api/EShopApp.Api.csproj", "src/EShopApp.Api/"]
COPY ["src/EShopApp.Application/EShopApp.Application.csproj", "src/EShopApp.Application/"]
COPY ["src/EShopApp.Domain/EShopApp.Domain.csproj", "src/EShopApp.Domain/"]
COPY ["src/EShopApp.Infrastructure/EShopApp.Infrastructure.csproj", "src/EShopApp.Infrastructure/"]
RUN dotnet restore "src/EShopApp.Api/EShopApp.Api.csproj"
COPY . .
WORKDIR "/src/src/EShopApp.Api"
RUN dotnet build "EShopApp.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EShopApp.Api.csproj" -c Release -o /app/publish

# Use the base image to run the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EShopApp.Api.dll"]