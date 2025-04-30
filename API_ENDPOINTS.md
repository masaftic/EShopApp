# EShopApp API Endpoints

This document lists the available API endpoints for the EShopApp application.

## Cart Management

- Add items to cart: `POST /api/Cart/add-to-cart`
- View cart: `GET /api/Cart`
- Clear cart: `DELETE /api/Cart/clear-cart`
- Update Item in cart: `PUT /api/Cart/update-item`
- Remove item from cart: `DELETE /api/Cart/remove-item/{productId}`
- Checkout: `POST /api/Cart/checkout`

## Category Management

- Retrieve all categories: `GET /api/Categories`
- Get a specific category by ID: `GET /api/Categories/{categoryId}`
- Retrieve category tree: `GET /api/Categories/tree`
- Retrieve category descendants: `GET /api/Categories/{categoryId}/descendants`
- Retrieve category subcategories: `GET /api/Categories/{categoryId}/subcategories`
- Retrieve category breadcrumbs: `GET /api/Categories/{categoryId}/breadcrumbs`

## Inventory Management

- Retrieve all inventories: `GET /api/Inventories`
- Get a specific inventory by ID: `GET /api/Inventories/{inventoryId}`
- Get low stock inventories: `GET /api/Inventories/low-stock`
- Add new inventory: `POST /api/Inventories`
- Update inventory: `PUT /api/Inventories/{inventoryId}`
- Adjust inventory: `POST /api/Inventories/{inventoryId}/adjust`

## Order Management

- Get order by ID: `GET /api/Orders/{id}`
- Get current user's orders: `GET /api/Orders/my`
- Get all orders: `GET /api/Orders`
- Get all orders by user ID: `GET /api/Orders/user/{userId}`
- Update order status: `PUT /api/Orders/{id}`
- Cancel order: `DELETE /api/Orders/{id}`

## Product Management

- Retrieve all products: `GET /api/Products`
- Get best selling products: `GET /api/Products/best-selling`
- Get top rated products: `GET /api/Products/top-rated`
- Filter products: `GET /api/Products/filter`
- Get product by ID: `GET /api/Products/{productId}`
- Update product: `PUT /api/Products/{id}`
- Add image to product: `POST /api/Products/{productId}/images`
- Set main image for product: `PUT /api/Products/{productId}/images/{imageId}/main`
- Delete product image: `DELETE /api/Products/{productId}/images/{imageId}`
- Add review to product: `POST /api/Products/{productId}/reviews`
- Update product review: `PUT /api/Products/{productId}/reviews/{reviewId}`

## User Management

- Retrieve all users: `GET /api/Users`
- Register user: `POST /api/Users/register`
- Login user: `POST /api/Users/login`
- Refresh token: `POST /api/Users/refresh-token`
- Get current user's information: `GET /api/Users/me`
- Get user by ID: `GET /api/Users/{id}`
- Revoke token: `DELETE /api/Users/revoke-token`
- Delete user: `DELETE /api/Users/{id}`

## Wishlist Management

- View wishlist: `GET /api/Wishlist`
- Add item to wishlist: `POST /api/Wishlist/add-item`
- Remove item from wishlist: `DELETE /api/Wishlist/remove-item/{productId}`
- Clear wishlist: `DELETE /api/Wishlist/clear`
- Check if item is in wishlist: `GET /api/Wishlist/check-item/{productId}`

## Payment Webhooks

- Stripe webhook endpoint: `POST /api/webhooks/stripe`
