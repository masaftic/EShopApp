# EShopApp

## Overview

EShopApp is a .NET 9.0 web application for online shopping. It provides a range of functionalities for users to browse products, manage their cart and wishlist, place orders, and leave product reviews. The application uses a layered architecture, incorporating API, Application, Domain, and Infrastructure layers.

## Features

The application provides comprehensive features for an e-commerce platform:

- **Cart Management:** Users can add products to their shopping cart, view the cart contents, clear the cart, and proceed to checkout.
- **Category Management:** Supports retrieving categories, including hierarchical structures like trees, descendants, subcategories, and breadcrumbs for navigation.
- **Inventory Management:** Allows administrators to view inventory levels, identify low-stock items, and adjust stock quantities.
- **Order Management:** Users can view their past orders, and administrators can retrieve specific order details.
- **Product Management:** Includes functionalities to list products (all, best-selling, top-rated), filter products based on criteria, view product details, update product information, manage product images (add, set main, delete), and manage product reviews (add, update).
- **User Management:** Provides user registration, login with token-based authentication (including refresh and revoke token mechanisms), and retrieval of user information (all users, specific user by ID, current user).
- **Wishlist Management:** Users can add items to a personal wishlist, view their wishlist, remove items, clear the wishlist, and check if a specific product is already in their wishlist.
- **Payment Integration:** Includes webhook support for payment gateways like Stripe to handle payment notifications.
- **Image Storage:** Utilizes Amazon S3 for storing product images, ensuring scalability and reliability.

For a detailed list of API endpoints, see [API_ENDPOINTS.md](./API_ENDPOINTS.md). Or you can check the Swagger UI at `http://localhost:5555/swagger` after running the application.

## Technologies Used

- .NET 9.0
- C#
- ASP.NET Core Web API
- Entity Framework Core
- MediatR
- ErrorOr
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
