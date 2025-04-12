# EShopApp

## Overview

EShopApp is a .NET 9.0 web application for online shopping. It provides a range of functionalities for users to browse products, manage their cart and wishlist, place orders, and leave product reviews. The application uses a layered architecture, incorporating API, Application, Domain, and Infrastructure layers.

## Features

- **Cart Management:**
  - Add items to cart (`POST /api/Cart/add-to-cart`)
    - View cart (`GET /api/Cart`)
    - Clear cart (`DELETE /api/Cart/clear-cart`)
    - Checkout (`POST /api/Cart/checkout`)
- **Category Management:**
  - Retrieve all categories (`GET /api/Categories`)
  - Get a specific category by ID (`GET /api/Categories/{categoryId}`)
  - Retrieve category tree, descendants, subcategories, and breadcrumbs.
- **Inventory Management:**
  - Retrieve all inventories (`GET /api/Inventories`)
  - Get a specific inventory by ID (`GET /api/Inventories/{inventoryId}`)
  - Get low stock inventories (`GET /api/Inventories/low-stock`)
  - Adjust inventory (`POST /api/Inventories/{inventoryId}/adjust`)
- **Order Management:**
  - Get order by ID (`GET /api/Orders/{id}`)
  - Get current user's orders (`GET /api/Orders/my`)
- **Product Management:**
  - Retrieve all products (`GET /api/Products`)
  - Get best selling products (`GET /api/Products/best-selling`)
  - Get top rated products (`GET /api/Products/top-rated`)
  - Filter products (`GET /api/Products/filter`)
  - Get product by ID (`GET /api/Products/{productId}`)
  - Update product (`PUT /api/Products/{id}`)
  - Add image to product (`POST /api/Products/{productId}/images`)
  - Set main image for product (`PUT /api/Products/{productId}/images/{imageId}/main`)
  - Delete product image (`DELETE /api/Products/{productId}/images/{imageId}`)
  - Add review to product (`POST /api/Products/{productId}/reviews`)
  - Update product review (`PUT /api/Products/{productId}/reviews/{reviewId}`)
- **User Management:**
  - Retrieve all users (`GET /api/Users`)
  - Register user (`POST /api/Users/register`)
  - Login user (`POST /api/Users/login`)
  - Refresh token (`POST /api/Users/refresh-token`)
  - Get current user's information (`GET /api/Users/me`)
  - Get user by ID (`GET /api/Users/{id}`)
  - Revoke token (`DELETE /api/Users/revoke-token`)
- **Wishlist Management:**
  - View wishlist (`GET /api/Wishlist`)
  - Add item to wishlist (`POST /api/Wishlist/add-item`)
  - Remove item from wishlist (`DELETE /api/Wishlist/remove-item/`)
  - Clear wishlist (`DELETE /api/Wishlist/clear`)
  - Check if item is in wishlist (`GET /api/Wishlist/check-item/`)
- **Payment Webhooks:**
  - Stripe webhook endpoint (`POST /api/webhooks/stripe`)

## Technologies Used

- .NET 9.0
- C#
- ASP.NET Core Web API
- Entity Framework Core
- MediatR
- Mapster
- FluentValidation
- xUnit
- Amazon S3 (for image storage)
- Stripe (for payment processing)

## Project Structure

The project is structured into the following layers:

- **EShopApp.Api:** ASP.NET Core Web API project containing controllers and API endpoints.
- **EShopApp.Application:** Contains the application logic, DTOs, and interfaces.
- **EShopApp.Domain:** Contains the core domain entities and business logic.
- **EShopApp.Infrastructure:** Implements the infrastructure concerns such as data access, services, and external integrations (like Amazon S3 and Stripe).

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Visual Studio or VS Code with C# extension
- SQL Server

### Setup

1. Clone the repository.
2. Update the `appsettings.json` file in the `EShopApp.Api` project with your database connection string.
3. Open a terminal in the `EShopApp.Api` directory.
4. Run `dotnet restore` to install the dependencies.
5. Run `dotnet build` to build the project.
6. Run `dotnet run` to start the application.

## Running Migrations

1. Ensure you have the Entity Framework Core tools installed. If not, run:

    ```bash
    dotnet tool install --global dotnet-ef
    ```

2. Open a terminal in the `EShopApp.Infrastructure` directory.
3. Run the following command to apply the migrations:

    ```bash
    dotnet ef database update -p ../EShopApp.Infrastructure -s ../EShopApp.Api
    ```

## Running Tests

1. Open a terminal in the `EShopApp.Tests` directory.
2. Run `dotnet test` to execute the unit tests.

## Contributing

Contributions are welcome! Please feel free to submit a pull request.
