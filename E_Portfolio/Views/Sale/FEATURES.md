# POS System - Chi tiết tính năng

## 🎨 Giao diện (UI/UX)

### Layout Structure
```
┌─────────────────────────────────────────────────────┐
│  Navbar (Gradient Blue-Purple)                      │
│  [Logo] POS System    [Trang chủ] [Bán hàng] [...] │
├──────────────────────┬──────────────────────────────┤
│                      │                               │
│   Product Grid       │   Cart & Checkout            │
│   (Left Panel)       │   (Right Panel)              │
│                      │                               │
│  [Search Box]        │  [🛒 Đơn hàng (0)]          │
│  [Category Filter]   │                               │
│                      │  Cart Items:                  │
│  ┌────┬────┬────┐   │  - Sản phẩm 1  [+][-][X]    │
│  │Img │Img │Img │   │  - Sản phẩm 2  [+][-][X]    │
│  │SP1 │SP2 │SP3 │   │                               │
│  │100k│200k│300k│   │  Khách hàng: [__________]    │
│  └────┴────┴────┘   │  Tổng tiền: 500,000 đ        │
│  ┌────┬────┬────┐   │  Giảm giá: 0 đ               │
│  │... │... │... │   │                               │
│  └────┴────┴────┘   │  [Thanh toán (F9)]          │
│                      │  [Xóa đơn] [Giữ đơn]        │
└──────────────────────┴──────────────────────────────┘
```

### Bootstrap 5 Classes Chính

#### Grid System
```html
<div class="container-fluid p-0">
  <div class="row g-0">
    <div class="col-lg-7 col-xl-8">Left Panel</div>
    <div class="col-lg-5 col-xl-4">Right Panel</div>
  </div>
</div>
```

#### Product Card
```html
<div class="col">
  <div class="card h-100 shadow-sm border-0">
    <div class="position-relative">
      <img class="card-img-top">
      <span class="position-absolute top-0 end-0 m-2 badge bg-danger">
        Sale
      </span>
    </div>
    <div class="card-body p-2">
      <h6 class="card-title mb-1 text-truncate small fw-bold">
        Product Name
      </h6>
      <p class="card-text text-muted small mb-1">Mã: SP0001</p>
      <p class="mb-0 fw-bold text-primary">100,000 đ</p>
    </div>
  </div>
</div>
```

#### Cart Item
```html
<div class="card mb-2 shadow-sm border-0">
  <div class="card-body p-2">
    <div class="d-flex justify-content-between align-items-start mb-2">
      <div class="flex-grow-1">
        <h6 class="mb-0 small fw-bold">Product Name</h6>
        <small class="text-muted">SP0001</small>
      </div>
      <button class="btn btn-sm btn-outline-danger border-0">
        <i class="bi bi-trash"></i>
      </button>
    </div>
    <div class="d-flex justify-content-between align-items-center">
      <div class="btn-group btn-group-sm">
        <button class="btn btn-outline-secondary">-</button>
        <button class="btn btn-outline-secondary disabled">1</button>
        <button class="btn btn-outline-secondary">+</button>
      </div>
      <span class="fw-bold text-primary">100,000 đ</span>
    </div>
  </div>
</div>
```

## 🎯 Tính năng chi tiết

### 1. Product Display

#### Grid Layout
- **Responsive columns**:
  - Mobile: 2 columns (`col-6`)
  - Tablet: 3 columns (`col-md-4`)
  - Desktop: 4-5 columns (`col-lg-3`, `col-xl-2`)
  
- **Product Info**:
  - Image với fallback
  - Product name (truncate nếu dài)
  - Product code
  - Price (với giá gốc nếu có sale)
  - Sale badge

#### Search & Filter
```javascript
// Real-time search
$('#searchProduct').on('input', function() {
  const searchTerm = $(this).val().toLowerCase();
  filterProducts(searchTerm);
});

// Filter by category
$('#categoryFilter').on('change', function() {
  const category = $(this).val();
  filterByCategory(category);
});
```

#### Tab Navigation
- Tất cả sản phẩm
- Combo khuyến mãi
- Nước uống
- (Có thể thêm tabs khác)

### 2. Shopping Cart

#### Add to Cart
```javascript
function addToCart(productId) {
  // Check if item exists
  const existingItem = cart.find(item => item.id === productId);
  
  if (existingItem) {
    existingItem.quantity++;  // Increase quantity
  } else {
    cart.push({             // Add new item
      id: productId,
      name: "...",
      code: "...",
      price: 100000,
      salePrice: 80000,     // Optional
      quantity: 1
    });
  }
  
  updateCart();
  showToast('success', 'Đã thêm vào giỏ hàng!');
}
```

#### Update Quantity
```javascript
// Increase
function increaseQuantity(index) {
  cart[index].quantity++;
  updateCart();
}

// Decrease
function decreaseQuantity(index) {
  if (cart[index].quantity > 1) {
    cart[index].quantity--;
  } else {
    removeFromCart(index);
  }
  updateCart();
}
```

#### Remove Item
```javascript
function removeFromCart(index) {
  cart.splice(index, 1);
  updateCart();
  showToast('info', 'Đã xóa sản phẩm');
}
```

#### Clear Cart
```javascript
function clearCart() {
  Swal.fire({
    title: 'Xác nhận xóa đơn hàng?',
    text: 'Bạn có chắc muốn xóa toàn bộ đơn hàng này?',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Xóa đơn',
    cancelButtonText: 'Hủy'
  }).then((result) => {
    if (result.isConfirmed) {
      cart = [];
      updateCart();
    }
  });
}
```

### 3. Calculations

#### Auto Calculate
```javascript
function updateCart() {
  subtotal = 0;
  discount = 0;
  
  cart.forEach(item => {
    const itemPrice = item.salePrice || item.price;
    const itemTotal = itemPrice * item.quantity;
    subtotal += itemTotal;
    
    if (item.salePrice) {
      discount += (item.price - item.salePrice) * item.quantity;
    }
  });
  
  total = subtotal;
  
  // Update UI
  $('#subtotal').text(formatCurrency(subtotal));
  $('#discount').text(formatCurrency(discount));
  $('#totalAmount').text(formatCurrency(total));
}
```

#### Currency Format
```javascript
function formatCurrency(amount) {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(amount);
}
// Output: 1.000.000 ₫
```

### 4. Payment Processing

#### Payment Flow
```javascript
function processPayment() {
  Swal.fire({
    title: 'Thanh toán đơn hàng',
    html: `
      <div class="text-start">
        <div class="mb-3">
          <label>Tổng tiền: ${formatCurrency(total)}</label>
        </div>
        <div class="mb-3">
          <label>Phương thức thanh toán</label>
          <select class="form-select" id="paymentMethod">
            <option value="cash">Tiền mặt</option>
            <option value="card">Thẻ</option>
            <option value="transfer">Chuyển khoản</option>
            <option value="momo">MoMo</option>
          </select>
        </div>
        <div class="mb-3">
          <label>Khách đưa</label>
          <input type="number" class="form-control" 
                 id="customerPaid" value="${total}" min="${total}">
        </div>
        <div class="mb-3">
          <label>Tiền thừa: 
            <span id="changeAmount">0 đ</span>
          </label>
        </div>
      </div>
    `,
    showCancelButton: true,
    confirmButtonText: 'Xác nhận thanh toán',
    didOpen: () => {
      // Calculate change in real-time
      $('#customerPaid').on('input', function() {
        const paid = parseFloat($(this).val()) || 0;
        const change = paid - total;
        $('#changeAmount').text(formatCurrency(Math.max(0, change)));
      });
    }
  }).then((result) => {
    if (result.isConfirmed) {
      completePayment();
    }
  });
}
```

#### Complete Payment
```javascript
function completePayment() {
  const orderData = {
    orderNumber: generateOrderNumber(),
    customer: $('#customerInfo').val() || 'Khách lẻ',
    items: cart,
    subtotal: subtotal,
    discount: discount,
    total: total,
    paymentMethod: $('#paymentMethod').val(),
    timestamp: new Date().toISOString()
  };
  
  // In production: POST to API
  // fetch('/Sale/CreateOrder', { method: 'POST', body: JSON.stringify(orderData) })
  
  Swal.fire({
    title: 'Thanh toán thành công!',
    html: `
      <i class="bi bi-check-circle-fill text-success" style="font-size: 4rem;"></i>
      <p>Mã đơn hàng: <strong>${orderData.orderNumber}</strong></p>
      <p>Tổng tiền: <strong>${formatCurrency(total)}</strong></p>
    `,
    showCancelButton: true,
    confirmButtonText: 'In hóa đơn',
    cancelButtonText: 'Đóng'
  });
  
  // Clear cart
  cart = [];
  $('#customerInfo').val('');
  updateCart();
}
```

### 5. Hold Order

```javascript
function holdOrder() {
  if (cart.length === 0) {
    showToast('warning', 'Chưa có sản phẩm để giữ đơn');
    return;
  }
  
  const orderData = {
    items: cart,
    subtotal: subtotal,
    discount: discount,
    total: total,
    timestamp: new Date().toISOString()
  };
  
  // Save to localStorage (in production: save to database)
  const heldOrders = JSON.parse(localStorage.getItem('heldOrders') || '[]');
  heldOrders.push(orderData);
  localStorage.setItem('heldOrders', JSON.stringify(heldOrders));
  
  cart = [];
  updateCart();
  showToast('success', 'Đã giữ đơn hàng');
}
```

### 6. Notifications

#### Toast (SweetAlert2)
```javascript
function showToast(type, message) {
  const Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 2000,
    timerProgressBar: true,
    didOpen: (toast) => {
      toast.addEventListener('mouseenter', Swal.stopTimer);
      toast.addEventListener('mouseleave', Swal.resumeTimer);
    }
  });
  
  Toast.fire({
    icon: type,  // success, error, warning, info
    title: message
  });
}
```

### 7. Keyboard Shortcuts

```javascript
$(document).on('keydown', function(e) {
  if (e.key === 'F9') {
    e.preventDefault();
    processPayment();
  }
  
  // Can add more shortcuts:
  // F1: Focus search
  // F2: Clear cart
  // F3: Hold order
  // Esc: Clear search
});
```

### 8. Print Receipt

```javascript
function printReceipt(orderData) {
  // Method 1: Browser print
  window.print();
  
  // Method 2: Custom print content
  const printWindow = window.open('', '_blank');
  printWindow.document.write(`
    <html>
      <head>
        <title>Hóa đơn ${orderData.orderNumber}</title>
        <style>
          body { font-family: monospace; }
          .center { text-align: center; }
          .line { border-top: 1px dashed #000; margin: 10px 0; }
        </style>
      </head>
      <body>
        <div class="center">
          <h2>HÓA ĐƠN BÁN HÀNG</h2>
          <p>Mã đơn: ${orderData.orderNumber}</p>
        </div>
        <div class="line"></div>
        <!-- Order details -->
        <div class="line"></div>
        <p>Tổng tiền: ${formatCurrency(orderData.total)}</p>
        <div class="center">
          <p>Cảm ơn quý khách!</p>
        </div>
      </body>
    </html>
  `);
  printWindow.document.close();
  printWindow.print();
}
```

## 📱 Responsive Design

### Breakpoints
```scss
// xs: < 576px (Mobile Portrait)
.col-6                  // 2 columns

// sm: >= 576px (Mobile Landscape)
.col-sm-6               // 2 columns

// md: >= 768px (Tablet)
.col-md-4               // 3 columns

// lg: >= 992px (Desktop)
.col-lg-3               // 4 columns
.col-lg-7               // Left panel
.col-lg-5               // Right panel

// xl: >= 1200px (Large Desktop)
.col-xl-2               // 5 columns
.col-xl-8               // Left panel
.col-xl-4               // Right panel
```

### Mobile Optimizations
```css
/* Stack layout on mobile */
@media (max-width: 991px) {
  .col-lg-7, .col-lg-5 {
    width: 100%;
  }
  
  /* Smaller font sizes */
  .card-title {
    font-size: 0.875rem;
  }
  
  /* Touch-friendly buttons */
  .btn {
    min-height: 44px;
    min-width: 44px;
  }
}
```

## 🎨 Styling Tips

### Custom Gradient
```css
/* Navbar gradient */
background: linear-gradient(135deg, #0d6efd 0%, #6610f2 100%);

/* Card gradient on hover */
.card:hover {
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
}
```

### Shadows
```html
<!-- Subtle shadow -->
<div class="shadow-sm">...</div>

<!-- Medium shadow -->
<div class="shadow">...</div>

<!-- Large shadow -->
<div class="shadow-lg">...</div>
```

### Hover Effects
```css
.product-card {
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.product-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15);
}
```

## 🔧 Customization Guide

### Change Theme Colors
```css
/* Primary color */
--bs-primary: #0d6efd;
--bs-primary-rgb: 13, 110, 253;

/* Success color */
--bs-success: #198754;

/* Danger color */
--bs-danger: #dc3545;
```

### Add New Product Category
```html
<!-- In Index.cshtml -->
<li class="nav-item" role="presentation">
  <button class="nav-link fw-semibold" id="newcat-tab" 
          data-bs-toggle="tab" data-bs-target="#newcat" 
          type="button" role="tab">
    <i class="bi bi-star me-1"></i> New Category
  </button>
</li>
```

### Add Payment Method
```javascript
// In processPayment() function
<select class="form-select" id="paymentMethod">
  <option value="cash">Tiền mặt</option>
  <option value="card">Thẻ</option>
  <option value="transfer">Chuyển khoản</option>
  <option value="momo">MoMo</option>
  <option value="zalopay">ZaloPay</option>        <!-- New -->
  <option value="vnpay">VNPay</option>            <!-- New -->
  <option value="shopeepay">ShopeePay</option>    <!-- New -->
</select>
```

### Custom Order Number Format
```javascript
function generateOrderNumber() {
  const now = new Date();
  const date = now.toISOString().split('T')[0].replace(/-/g, '');
  const time = now.getTime().toString().slice(-6);
  return `HD${date}${time}`;  // HD20251027123456
}
```

## 📊 Performance Tips

1. **Lazy Load Images**
```html
<img loading="lazy" src="..." alt="...">
```

2. **Debounce Search**
```javascript
let searchTimeout;
$('#searchProduct').on('input', function() {
  clearTimeout(searchTimeout);
  searchTimeout = setTimeout(() => {
    filterProducts($(this).val());
  }, 300);
});
```

3. **Virtual Scrolling** (for large product lists)
```javascript
// Use libraries like react-window or vue-virtual-scroller
```

4. **Cache Products**
```javascript
let productsCache = null;
async function loadProducts() {
  if (productsCache) return productsCache;
  const response = await fetch('/Sale/GetProducts');
  productsCache = await response.json();
  return productsCache;
}
```

---

**Tổng kết**: Hệ thống POS hoàn chỉnh với UI/UX hiện đại, sử dụng tối đa Bootstrap 5, dễ customize và ready để integrate với backend!

