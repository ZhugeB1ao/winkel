// Add to Cart AJAX functionality
document.addEventListener('DOMContentLoaded', function() {
    // Get all "Add to cart" buttons
    const addToCartButtons = document.querySelectorAll('.add-to-cart,.detail-add-to-cart');
    
    addToCartButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault(); // Prevent default link behavior
            
            // Don't process if already disabled
            if (this.classList.contains('disabled')) {
                return;
            }
            
            const productId = parseInt(this.getAttribute('data-product-id'));
            const originalText = this.innerHTML;
            
            // Disable button and show loading state
            this.classList.add('disabled');
            this.innerHTML = '<span>Adding...</span>';
            
            // Send AJAX request
            fetch('/Cart/AddAjax', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(productId)
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    // Show success message
                    this.innerHTML = '<span>✓ Added!</span>';
                    this.classList.add('btn-success');
                    
                    // Update cart count in navbar
                    updateCartCount(data.cartItemCount);
                    
                    // Show toast notification
                    showToast('Product added to cart successfully!', 'success');
                    
                    // Reset button after 2 seconds
                    setTimeout(() => {
                        this.innerHTML = originalText;
                        this.classList.remove('disabled');
                        this.classList.remove('btn-success');
                    }, 2000);
                } else {
                    // Show error message
                    showToast(data.message || 'Failed to add product to cart', 'error');
                    this.innerHTML = originalText;
                    this.classList.remove('disabled');
                }
            })
            .catch(error => {
                console.error('Error:', error);
                showToast('An error occurred. Please try again.', 'error');
                this.innerHTML = originalText;
                this.classList.remove('disabled');
            });
        });
    });
    
    // Remove item from cart functionality
    const removeButtons = document.querySelectorAll('.product-remove a');
    
    removeButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            
            const productId = parseInt(this.getAttribute('data-product-id'));
            const row = document.querySelector(`.cart-item-row[data-product-id="${productId}"]`);
            
            // Send AJAX request
            fetch('/Cart/RemoveItemAjax', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(productId)
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    // Fade out and remove row
                    if (row) {
                        row.style.opacity = '0';
                        row.style.transition = 'opacity 0.3s ease';
                        
                        setTimeout(() => {
                            row.remove();

                            if(data.remainingItems === 0){
                                window.location.reload();
                            }
                        }, 300);
                    }
                    
                    // Update cart count in navbar
                    updateCartCount(data.cartItemCount);
                    
                    // Show toast notification
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

// Update cart count in navbar
function updateCartCount(count) {
    const cartIcon = document.querySelector('.icon-shopping_cart');
    if (cartIcon && cartIcon.parentElement) {
        // Update the [0] text with actual count
        const cartLink = cartIcon.parentElement;
        const currentText = cartLink.innerHTML;
        const newText = currentText.replace(/\[\d+\]/, `[${count}]`);
        cartLink.innerHTML = newText;
    }
}

// Show toast notification
function showToast(message, type = 'success') {
    // Remove existing toast if any
    const existingToast = document.querySelector('.custom-toast');
    if (existingToast) {
        existingToast.remove();
    }
    
    // Create toast element
    const toast = document.createElement('div');
    toast.className = `custom-toast custom-toast-${type}`;
    toast.innerHTML = `
        <div class="custom-toast-content">
            <span class="custom-toast-icon">${type === 'success' ? '✓' : '✕'}</span>
            <span class="custom-toast-message">${message}</span>
        </div>
    `;
    
    // Add to body
    document.body.appendChild(toast);
    
    // Show toast with animation
    setTimeout(() => {
        toast.classList.add('show');
    }, 10);
    
    // Remove toast after 3 seconds
    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => {
            toast.remove();
        }, 300);
    }, 3000);
}
