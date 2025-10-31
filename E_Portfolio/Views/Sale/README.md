# POS System - Hệ thống Bán hàng

## 🚀 Quick Start

### Truy cập
- **Bán hàng**: `/Sale` hoặc `/Sale/Index`
- **Quản lý đơn hàng**: `/Sale/Orders`

### Shortcuts
- **F9**: Thanh toán nhanh
- **Search bar**: Tìm kiếm sản phẩm
- **Click sản phẩm**: Thêm vào giỏ hàng

## 📁 Files

```
Views/Sale/
├── Index.cshtml        # Giao diện POS chính
└── Orders.cshtml       # Quản lý đơn hàng

Views/Shared/
└── _Layout_Sale.cshtml # Layout POS

wwwroot/root/sale/
├── sale.js            # Logic xử lý
└── sale.css           # Styles (minimal)

Controllers/E_Sales/
└── SaleController.cs  # Backend controller
```

## 🎨 Design

- **95% Bootstrap 5** classes
- **5% Custom CSS** (animations, scrollbar)
- Responsive: Mobile, Tablet, Desktop
- Modern gradient navbar
- Card-based product display

## ✨ Features

### ✅ Implemented
- [x] Danh sách sản phẩm với grid layout
- [x] Tìm kiếm & lọc sản phẩm
- [x] Thêm/xóa/cập nhật giỏ hàng
- [x] Tính tổng tiền tự động
- [x] Hiển thị giảm giá
- [x] Form thanh toán
- [x] Giữ đơn hàng
- [x] Toast notifications
- [x] Responsive design
- [x] Keyboard shortcuts (F9)

### 🔄 TODO (Backend Integration)
- [ ] Load sản phẩm từ database
- [ ] Lưu đơn hàng vào database
- [ ] Authentication/Authorization
- [ ] In hóa đơn thực tế
- [ ] Quét mã vạch
- [ ] Báo cáo doanh thu

## 🛠️ Customization

### Màu sắc (trong _Layout_Sale.cshtml)
```css
.navbar { 
  background: linear-gradient(135deg, #0d6efd 0%, #6610f2 100%); 
}
```

### Thêm sản phẩm mẫu (trong Index.cshtml)
```csharp
@for (int i = 1; i <= 15; i++)
{
    // Edit product card here
}
```

### Custom JavaScript (trong sale.js)
```javascript
// Modify cart logic
function addToCart(productId) { ... }

// Customize payment flow
function processPayment() { ... }
```

## 📱 Responsive Grid

- **Mobile (< 768px)**: 2 columns
- **Tablet (768px - 991px)**: 3 columns
- **Desktop (992px - 1199px)**: 4 columns
- **Large Desktop (≥ 1200px)**: 5 columns

## 🎯 Main Components

### Product Card (Bootstrap classes)
```html
<div class="card h-100 shadow-sm border-0">
  <img class="card-img-top">
  <div class="card-body">
    <h6 class="card-title">...</h6>
    <p class="card-text">...</p>
  </div>
</div>
```

### Cart Item
```html
<div class="card mb-2 shadow-sm border-0">
  <div class="card-body p-2">
    <!-- Quantity controls -->
    <div class="btn-group btn-group-sm">...</div>
  </div>
</div>
```

### Payment Button
```html
<button class="btn btn-success btn-lg fw-bold">
  <i class="bi bi-credit-card"></i> Thanh toán
</button>
```

## 📊 Data Structure

### Cart Item
```javascript
{
  id: 1,
  name: "Sản phẩm 1",
  code: "SP00001",
  price: 100000,
  salePrice: 80000,  // optional
  quantity: 1
}
```

### Order
```javascript
{
  orderNumber: "HD12345678",
  customer: "Tên khách hàng",
  items: [...],
  subtotal: 1000000,
  discount: 100000,
  total: 900000,
  paymentMethod: "cash",
  timestamp: "2025-10-27T10:00:00"
}
```

## 🔧 API Integration

Connect to your backend:

```javascript
// In sale.js, replace mock data with API calls

// Load products
async function loadProducts() {
  const response = await fetch('/Sale/GetProducts');
  const data = await response.json();
  // Render products
}

// Create order
async function completePayment() {
  const response = await fetch('/Sale/CreateOrder', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(orderData)
  });
  // Handle response
}
```

## 📖 Documentation
Xem file chi tiết: [POS_System_Guide.md](../../Documentation/POS_System_Guide.md)

---
**Version**: 1.0.0  
**Author**: Tang DV  
**Date**: October 2025

