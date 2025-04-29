document.addEventListener("DOMContentLoaded", () => {
    const loginForm = document.getElementById('login-form');
    const productsSection = document.getElementById('products-section');
    const cartSection = document.getElementById('cart-section');
    const productsList = document.getElementById('products-list');
    const cartItems = document.getElementById('cart-items');
    const totalPrice = document.getElementById('total-price');
    const checkoutButton = document.getElementById('checkout-button');
    const clearCartButton = document.getElementById('clear-cart-button');
    const payButton = document.getElementById('pay-button');
    let token = '';
    let stripe = Stripe('pk_test_51QmeTzKXUAlVXc1HDFFazSKXTWNPRXjAWjQRwu1eOh9ArOvMSR67ijGkigGqDIpE2yHqwl3atszJFXI012qTvGFp009TmPlXdC');
    let elements = stripe.elements();
    let paymentElement;

    const useHttps = false;
    const SERVER_URL = useHttps ? 'https://localhost:7065' : 'http://localhost:5555';

    loginForm.addEventListener('submit', async (event) => {
        event.preventDefault();
        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;

        const response = await fetch(`${SERVER_URL}/api/users/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });

        const result = await response.json();
        if (response.ok) {
            token = result.accessToken;
            document.getElementById('login-result').textContent = 'Login successful!';
            loginForm.style.display = 'none';
            productsSection.style.display = 'block';
            cartSection.style.display = 'block';
            fetchProducts();
            getCart();
        } else {
            document.getElementById('login-result').textContent = result.errors[0].description;
        }
    });

    async function fetchProducts() {
        const response = await fetch(`${SERVER_URL}/api/products`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const products = await response.json();
        console.log(products);
        productsList.innerHTML = '';
        products.items.forEach(product => {
            const productDiv = document.createElement('div');
            productDiv.textContent = `${product.name} - $${product.price}`;
            const addButton = document.createElement('button');
            addButton.textContent = 'Add to Cart';
            addButton.addEventListener('click', () => addToCart(product.id));
            productDiv.appendChild(addButton);
            productsList.appendChild(productDiv);
        });
    }

    async function addToCart(productId) {
        const response = await fetch(`${SERVER_URL}/api/cart/add-to-cart`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({ productId, quantity: 1 })
        });

        if (response.ok) {
            getCart();
        }
    }

    async function getCart() {
        try {
            const response = await fetch(`${SERVER_URL}/api/cart`, {
                headers: { 'Authorization': `Bearer ${token}` }
            });
            if (!response.ok) {
                throw new Error('Failed to fetch cart');
            }
            const cart = await response.json();
            console.log(cart);
            cartItems.innerHTML = '';
            cart.cartItems.forEach(item => {
                const itemDiv = document.createElement('div');
                itemDiv.textContent = `${item.productName} - $${item.unitPrice} x ${item.quantity}`;
                cartItems.appendChild(itemDiv);
            });
            totalPrice.textContent = `Total Price: $${cart.totalPrice}`;
        } catch (error) {
            console.error('Error fetching cart:', error);
        }
    }

    clearCartButton.addEventListener('click', async () => {
        const response = await fetch(`${SERVER_URL}/api/cart/clear-cart`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.ok) {
            cartItems.innerHTML = '';
            totalPrice.textContent = 'Total Price: $0';
        } else {
            console.error('Failed to clear cart');
        }
    });

    checkoutButton.addEventListener('click', async () => {
        // Create a payment intent
        const response = await fetch(`${SERVER_URL}/api/cart/checkout`, {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        const result = await response.json();
        if (response.ok) {
            // Hide the checkout button and show the payment element
            checkoutButton.style.display = 'none';

            const { clientSecret } = result;

            // Configure the appearance to include the address element
            const appearance = {
                theme: 'stripe',
                variables: {
                    colorPrimary: '#0570de',
                    colorBackground: '#f6f9fc',
                    colorText: '#30313d',
                    colorDanger: '#df1b41',
                },
                layout: {
                    type: 'tabs',
                    defaultCollapsed: false,
                    radios: true,
                    spacedAccordionItems: true,
                },
                fields: {
                    billingDetails: {
                        address: {
                            line1: 'auto',
                            line2: 'auto',
                            city: 'auto',
                            state: 'auto',
                            postalCode: 'auto',
                            country: 'auto',
                        },
                    },
                },
            };

            // Create and mount the Payment Element with address fields
            elements = stripe.elements({ clientSecret, appearance });
            paymentElement = elements.create('payment');
            paymentElement.mount('#payment-element');

            addressElement = elements.create('address', { mode: 'shipping' });
            addressElement.mount('#address-element');

            // Show the Pay button
            payButton.style.display = 'block';

        } else {
            document.getElementById('checkout-result').textContent = 'Checkout failed.';
        }
    });

    payButton.addEventListener('click', async () => {
        // Confirm the payment intent with the Payment Element
        const { error, paymentIntent } = await stripe.confirmPayment({
            elements,
            confirmParams: {
                return_url: `http://localhost:5500/PaymentClientSide/payment-complete.html`,
            },
        });

        if (error) {
            document.getElementById('checkout-result').textContent = `Payment failed: ${error.message}`;
        } else {
            document.getElementById('checkout-result').textContent = `Payment successful! Payment Intent ID: ${paymentIntent.id}`;
            document.getElementById('payment-success').style.display = 'block'; // Show payment success message
        }
    });
});