/**
 * ================================================================================
 * CART.JS - Handle shopping cart functionality using AJAX
 * ================================================================================
 * This file contains the following features:
 * 1. Add product to cart (Add to Cart)
 * 2. Update product quantity (Update Quantity)
 * 3. Remove product from cart (Remove Item)
 * 4. Utility functions: updateCartCount, updateCartTotals, showToast
 * ================================================================================
 */

/**
 * Wait for DOM to load before running code
 * - DOMContentLoaded: event triggered when HTML has finished parsing
 * - Ensures all elements exist before manipulation
 */
document.addEventListener('DOMContentLoaded', function() {
    
    /* ============================================================================
     * SECTION 1: ADD PRODUCT TO CART
     * ============================================================================
     * Handle when user clicks "Add to cart" button on Shop or Product Detail page
     */
    
    // Find all "Add to cart" buttons on the page
    // - .add-to-cart: button on Shop page (product list)
    // - .detail-add-to-cart: button on Product Detail page
    const addToCartButtons = document.querySelectorAll('.add-to-cart,.detail-add-to-cart');
    
    // Loop through each button and attach event listener
    addToCartButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            // Prevent default <a> tag behavior (page navigation)
            e.preventDefault();
            
            // Check if button is already disabled (prevent spam clicks)
            if (this.classList.contains('disabled')) {
                return; // Exit if already disabled
            }
            
            // Get productId from data-product-id attribute of button
            // parseInt: convert string "123" to number 123
            const productId = parseInt(this.getAttribute('data-product-id'));
            
            // Save original button content to restore later
            const originalText = this.innerHTML;
            
            // === SHOW LOADING STATE ===
            this.classList.add('disabled');           // Disable button
            this.innerHTML = '<span>Adding...</span>'; // Change text to "Adding..."
            
            // === SEND AJAX REQUEST TO SERVER ===
            // fetch: modern API to send HTTP requests
            fetch('/Cart/AddItemAjax', {
                method: 'POST',                        // HTTP POST method
                headers: {
                    'Content-Type': 'application/json', // Tell server: body is JSON
                },
                body: JSON.stringify(productId)        // Convert productId to JSON string
            })
            // === HANDLE RESPONSE FROM SERVER ===
            .then(response => response.json()) // Parse JSON string → JavaScript object
            .then(data => {
                // data = { success, message, cartItemCount, subtotal, total }
                
                if (data.success) {
                    // === SUCCESS ===
                    
                    // Change button to success state
                    this.innerHTML = '<span>✓ Added!</span>';
                    this.classList.add('btn-success');
                    
                    // Update cart count on navbar cart icon
                    updateCartCount(data.cartItemCount);
                    
                    // Update subtotal and total (if on Cart page)
                    updateCartTotals(data.subtotal, data.total);
                    
                    // Show toast notification
                    showToast('Product added to cart successfully!', 'success');
                    
                    // After 2 seconds, restore button to original state
                    setTimeout(() => {
                        this.innerHTML = originalText;
                        this.classList.remove('disabled');
                        this.classList.remove('btn-success');
                    }, 2000);
                } else {
                    // === FAILURE (server returned success: false) ===
                    showToast(data.message || 'Failed to add product to cart', 'error');
                    this.innerHTML = originalText;
                    this.classList.remove('disabled');
                }
            })
            .catch(error => {
                // === NETWORK ERROR or other errors ===
                console.error('Error:', error);
                showToast('An error occurred. Please try again.', 'error');
                this.innerHTML = originalText;
                this.classList.remove('disabled');
            });
        });
    });
    
    /* ============================================================================
     * SECTION 2: UPDATE PRODUCT QUANTITY
     * ============================================================================
     * Handle when user changes quantity in input on Cart page
     * Triggered when: pressing Enter or clicking outside input (blur)
     */
    
    // Find all quantity inputs in cart product rows
    // Selector: input with class "quantity" inside element with class "cart-item-row"
    const quantityInputs = document.querySelectorAll('.cart-item-row input.quantity');
    
        // Attach event listeners to each quantity input
    quantityInputs.forEach(input => {
        // Save initial value to attribute for later comparison
        // Example: <input value="2" data-original-value="2">
        input.setAttribute('data-original-value', input.value);
        
        // === EVENT: BLUR (click outside input) ===
        // When user clicks outside input → send update request
        input.addEventListener('blur', function() {
            // this = input element that triggered the event
            updateQuantityOnServer(this);
        });
        
        // === EVENT: KEYPRESS (key pressed) ===
        // When user presses Enter → trigger blur to save
        input.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                e.preventDefault();  // Prevent form submit (if any)
                this.blur();         // Trigger blur event → call updateQuantityOnServer
            }
        });
    });
    
    // Function to send quantity update request to server
    function updateQuantityOnServer(input) {
        // Find the row containing this input
        // closest(): find the nearest parent element matching the selector
        const row = input.closest('.cart-item-row');
        if (!row) return; // Exit if row not found
        
        // Get productId from row's attribute
        const productId = parseInt(row.getAttribute('data-product-id'));
        
        // Get new quantity from input
        let quantity = parseInt(input.value);
        
        // Get original value saved (to compare if changed)
        const originalValue = parseInt(input.getAttribute('data-original-value'));

        // === VALIDATE QUANTITY ===
        // If < 1 or not a number → set to 1
        if (quantity < 1 || isNaN(quantity)) {
            quantity = 1;
            input.value = 1;
        }
        
        // If > 100 → set to 100 (maximum limit)
        if (quantity > 100) {
            quantity = 100;
            input.value = 100;
        }
        
        // === CHECK IF VALUE CHANGED ===
        // If quantity unchanged → don't send request (save bandwidth)
        if (quantity === originalValue) {
            return;
        }

        // === SEND AJAX REQUEST ===
        fetch('/Cart/UpdateItemAjax', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            // Send object containing productId and quantity
            body: JSON.stringify({ productId: productId, quantity: quantity })
        })
        .then(response => response.json())
        .then(data => {
            // data = { success, itemTotal, subtotal, total, cartItemCount }
            
            if (data.success) {
                // === SUCCESS ===
                
                // Update original value = new value (for next comparison)
                input.setAttribute('data-original-value', quantity);
                
                // Update Total column for this product row
                const totalCell = row.querySelector('.total');
                if (totalCell) {
                    // toFixed(2): format number to 2 decimal places
                    totalCell.textContent = '$' + data.itemTotal.toFixed(2);
                }

                // Update Subtotal and Total for entire cart
                const subtotalEl = document.getElementById('cart-subtotal');
                const totalEl = document.getElementById('cart-total');

                if (subtotalEl) subtotalEl.textContent = '$' + data.subtotal.toFixed(2);
                if (totalEl) totalEl.textContent = '$' + data.total.toFixed(2);

                // Update cart count on navbar icon
                updateCartCount(data.cartItemCount);

                showToast('Cart updated', 'success');
            } else {
                showToast(data.message || 'Failed to update quantity', 'error');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showToast('An error occurred. Please try again.', 'error');
        });
    }

    /* ============================================================================
     * SECTION 3: REMOVE PRODUCT FROM CART
     * ============================================================================
     * Handle when user clicks X button to remove product from cart
     */
    
    // Find all remove buttons (<a> tags in product-remove column)
    const removeButtons = document.querySelectorAll('.product-remove a');
    
    removeButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault(); // Prevent page navigation
            
            // Get productId from button's attribute
            const productId = parseInt(this.getAttribute('data-product-id'));
            
            // Find corresponding product row to remove from DOM later
            const row = document.querySelector(`.cart-item-row[data-product-id="${productId}"]`);
            
            // === SEND AJAX REQUEST ===
            fetch('/Cart/RemoveItemAjax', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(productId)
            })
            .then(response => response.json())
            .then(data => {
                // data = { success, message, cartItemCount, remainingItems, subtotal, total }
                
                if (data.success) {
                    // === SUCCESS ===
                    
                    // Create fade out effect for product row
                    if (row) {
                        row.style.opacity = '0';                    // Fade out
                        row.style.transition = 'opacity 0.3s ease'; // 0.3s animation
                        
                        // After 300ms (when animation done), remove row from DOM
                        setTimeout(() => {
                            row.remove(); // Remove element from DOM

                            // If cart is empty → reload page to show "Cart is empty"
                            if(data.remainingItems === 0){
                                window.location.reload();
                            }
                        }, 300);
                    }
                    
                    // Update UI
                    updateCartCount(data.cartItemCount);
                    updateCartTotals(data.subtotal, data.total);
                    showToast('Item removed from cart', 'success');
                } else {
                    showToast(data.message || 'Failed to remove item', 'error');
                }
            })
            .catch(error => {
                console.error('Error:', error);
                showToast('An error occurred. Please try again.', 'error');
            });
        });
    });
});

/* ====================================================================================
 * SECTION 4: UTILITY FUNCTIONS
 * ====================================================================================
 * These functions are declared outside DOMContentLoaded so they can be called from anywhere
 */

// Update cart item count displayed on navbar cart icon
// Example: [3] → [5]
function updateCartCount(count) {
    // Find cart icon
    const cartIcon = document.querySelector('.icon-shopping_cart');
    
    if (cartIcon && cartIcon.parentElement) {
        // Get parent <a> tag containing the icon
        const cartLink = cartIcon.parentElement;
        const currentText = cartLink.innerHTML;
        
        // Use regex to replace [number] with [new count]
        // \[\d+\] : find pattern [one or more digits]
        const newText = currentText.replace(/\[\d+\]/, `[${count}]`);
        cartLink.innerHTML = newText;
    }
}

// Update subtotal and total displayed on Cart page
function updateCartTotals(subtotal, total) {
    // Find elements by id
    const subtotalEl = document.getElementById('cart-subtotal');
    const totalEl = document.getElementById('cart-total');
    
    // Update if element exists and value is valid
    if (subtotalEl && subtotal !== undefined) {
        subtotalEl.textContent = '$' + subtotal.toFixed(2);
    }
    if (totalEl && total !== undefined) {
        totalEl.textContent = '$' + total.toFixed(2);
    }
}

// Display toast notification (auto-hides after 3 seconds)
function showToast(message, type = 'success') {
    // Remove existing toast if any (only show 1 toast at a time)
    const existingToast = document.querySelector('.custom-toast');
    if (existingToast) {
        existingToast.remove();
    }
    
    // Create new toast element
    const toast = document.createElement('div');
    toast.className = `custom-toast custom-toast-${type}`;
    
    // HTML content of toast
    // Using template literal (`) to write multi-line HTML
    toast.innerHTML = `
        <div class="custom-toast-content">
            <span class="custom-toast-icon">${type === 'success' ? '✓' : '✕'}</span>
            <span class="custom-toast-message">${message}</span>
        </div>
    `;
    
    // Append toast to end of body
    document.body.appendChild(toast);
    
    // After 10ms, add 'show' class to trigger CSS animation (fade in)
    // 10ms delay allows browser to render element before animating
    setTimeout(() => {
        toast.classList.add('show');
    }, 10);
    
    // After 3 seconds, hide toast (fade out) then remove from DOM
    setTimeout(() => {
        toast.classList.remove('show'); // Trigger fade out animation
        
        // After 300ms (when animation done), remove element
        setTimeout(() => {
            toast.remove();
        }, 300);
    }, 3000);
}

