# Hướng dẫn sử dụng hệ thống POS (Point of Sale)

## Tổng quan
Hệ thống POS được thiết kế để quản lý bán hàng tại quầy (kiot), hỗ trợ:
- Tạo đơn hàng nhanh chóng
- Quản lý giỏ hàng
- Thanh toán đa phương thức
- Quản lý đơn hàng
- In hóa đơn

## Cấu trúc dự án

### Frontend (Views)
```
E_Portfolio/Views/
├── Shared/
│   ├── _Layout_Sale.cshtml          # Layout chính cho POS
│   └── _Layout_Sale.cshtml.css      # CSS tùy chỉnh (tối thiểu)
└── Sale/
    ├── Index.cshtml                  # Giao diện bán hàng
    └── Orders.cshtml                 # Quản lý đơn hàng
```

### Backend (Controllers)
```
E_Portfolio/Controllers/E_Sales/
└── SaleController.cs                 # Controller xử lý logic POS
```

### JavaScript & CSS
```
E_Portfolio/wwwroot/root/sale/
├── sale.js                           # Logic xử lý POS
└── sale.css                          # CSS tùy chỉnh (tối thiểu)
```

## Công nghệ sử dụng

### Bootstrap 5
Hệ thống sử dụng **95% Bootstrap 5 classes**, hạn chế CSS tùy chỉnh:

#### Layout Classes
- `container-fluid`: Container full width
- `row`, `col-*`: Grid system
- `d-flex`, `flex-column`: Flexbox
- `bg-white`, `bg-light`, `bg-primary`: Background colors
- `shadow-sm`, `border`: Effects

#### Components
- `card`, `card-body`: Card components
- `btn`, `btn-primary`, `btn-success`: Buttons
- `form-control`, `form-select`: Form inputs
- `navbar`, `nav-tabs`: Navigation
- `badge`, `table`: Data display

#### Utilities
- `p-*`, `m-*`: Spacing (padding, margin)
- `text-*`: Text colors
- `fw-bold`, `fs-*`: Typography
- `rounded`, `shadow`: Effects

### JavaScript Libraries
- **jQuery 3.7.1**: DOM manipulation
- **SweetAlert2**: Alerts & dialogs
- **Bootstrap 5 JS**: Interactive components

## Tính năng chính

### 1. Giao diện bán hàng (/Sale)

#### Left Panel - Danh sách sản phẩm
- Tìm kiếm sản phẩm theo tên, mã
- Lọc theo danh mục
- Hiển thị dạng grid với hình ảnh
- Hiển thị giá gốc và giá khuyến mãi
- Click vào sản phẩm để thêm vào giỏ

#### Right Panel - Giỏ hàng & Thanh toán
- Hiển thị danh sách sản phẩm trong giỏ
- Tăng/giảm số lượng
- Xóa sản phẩm
- Nhập thông tin khách hàng
- Tính tổng tiền, giảm giá
- Thanh toán (F9)
- Xóa đơn, Giữ đơn

### 2. Quản lý đơn hàng (/Sale/Orders)
- Lọc đơn hàng theo ngày, trạng thái
- Tìm kiếm theo mã đơn, khách hàng
- Xem chi tiết đơn
- In hóa đơn
- Hủy đơn

## API Endpoints

### GET /Sale
Hiển thị giao diện POS chính

### GET /Sale/Orders
Hiển thị trang quản lý đơn hàng

### GET /Sale/GetProducts
Lấy danh sách sản phẩm
```json
Response: {
  "success": true,
  "data": [...]
}
```

### GET /Sale/SearchProducts?keyword={keyword}
Tìm kiếm sản phẩm
```json
Response: {
  "success": true,
  "data": [...]
}
```

### POST /Sale/CreateOrder
Tạo đơn hàng mới
```json
Request: {
  "customer": "Tên khách hàng",
  "items": [...],
  "total": 1000000,
  "paymentMethod": "cash"
}

Response: {
  "success": true,
  "orderNumber": "HD12345678",
  "message": "Tạo đơn hàng thành công"
}
```

### GET /Sale/GetOrderHistory?page=1&pageSize=20
Lấy lịch sử đơn hàng

### GET /Sale/PrintReceipt?orderNumber={orderNumber}
In hóa đơn

### GET /Sale/GetHeldOrders
Lấy danh sách đơn hàng đang giữ

### POST /Sale/ApplyDiscount
Áp dụng mã giảm giá

## Chức năng JavaScript (sale.js)

### Quản lý giỏ hàng
```javascript
addToCart(productId)        // Thêm sản phẩm vào giỏ
updateCart()                // Cập nhật hiển thị giỏ hàng
increaseQuantity(index)     // Tăng số lượng
decreaseQuantity(index)     // Giảm số lượng
removeFromCart(index)       // Xóa sản phẩm
clearCart()                 // Xóa toàn bộ giỏ hàng
```

### Xử lý đơn hàng
```javascript
holdOrder()                 // Giữ đơn hàng
processPayment()            // Xử lý thanh toán
completePayment()           // Hoàn tất thanh toán
printReceipt(orderData)     // In hóa đơn
```

### Tìm kiếm & Lọc
```javascript
filterProducts(searchTerm)  // Tìm kiếm sản phẩm
filterByCategory(category)  // Lọc theo danh mục
```

### Tiện ích
```javascript
formatCurrency(amount)      // Format số tiền
showToast(type, message)    // Hiển thị thông báo
generateOrderNumber()       // Tạo mã đơn hàng
```

## Keyboard Shortcuts
- **F9**: Thanh toán nhanh
- **Escape**: Đóng dialog
- **Enter**: Xác nhận trong dialog

## Tùy chỉnh giao diện

### Màu sắc chính
```css
/* Gradient navbar */
background: linear-gradient(135deg, #0d6efd 0%, #6610f2 100%)

/* Bootstrap colors */
--bs-primary: #0d6efd
--bs-success: #198754
--bs-danger: #dc3545
--bs-warning: #ffc107
--bs-info: #0dcaf0
```

### Responsive Breakpoints
- **xs**: < 576px (Mobile)
- **sm**: ≥ 576px (Mobile landscape)
- **md**: ≥ 768px (Tablet)
- **lg**: ≥ 992px (Desktop)
- **xl**: ≥ 1200px (Large desktop)
- **xxl**: ≥ 1400px (Extra large)

## Workflow bán hàng

1. **Tìm và chọn sản phẩm**
   - Dùng thanh tìm kiếm hoặc browse danh mục
   - Click vào sản phẩm để thêm vào giỏ

2. **Chỉnh sửa giỏ hàng**
   - Điều chỉnh số lượng với nút +/-
   - Xóa sản phẩm nếu cần
   - Nhập thông tin khách hàng (tùy chọn)

3. **Thanh toán**
   - Nhấn nút "Thanh toán" hoặc phím F9
   - Chọn phương thức thanh toán
   - Nhập số tiền khách đưa
   - Xác nhận thanh toán

4. **Hoàn tất**
   - In hóa đơn (tùy chọn)
   - Giỏ hàng tự động clear
   - Sẵn sàng cho đơn tiếp theo

## Tính năng nâng cao (TODO)

### Backend Integration
- [ ] Kết nối database để lưu trữ sản phẩm
- [ ] API lấy danh sách sản phẩm thực tế
- [ ] Lưu đơn hàng vào database
- [ ] Quản lý tồn kho tự động
- [ ] Báo cáo doanh thu

### Authentication & Authorization
- [ ] Đăng nhập nhân viên
- [ ] Phân quyền (Cashier, Manager, Admin)
- [ ] Tracking người tạo đơn

### Advanced Features
- [ ] Quét mã vạch sản phẩm
- [ ] Tích điểm khách hàng
- [ ] Áp dụng voucher/coupon
- [ ] In hóa đơn nhiệt (thermal printer)
- [ ] Kết nối máy quẹt thẻ
- [ ] Thống kê theo ca, theo ngày
- [ ] Xuất file Excel báo cáo

### UI/UX Improvements
- [ ] Dark mode
- [ ] Themes tùy chỉnh
- [ ] Offline mode (PWA)
- [ ] Touch-friendly cho tablet
- [ ] Shortcuts keyboard toàn diện

## Troubleshooting

### Sản phẩm không hiển thị
- Kiểm tra API `/Sale/GetProducts`
- Kiểm tra console browser có lỗi không
- Verify Bootstrap đã load đúng

### Không thêm được vào giỏ
- Kiểm tra function `addToCart()` trong sale.js
- Kiểm tra jQuery đã load chưa
- Verify product ID hợp lệ

### Thanh toán không hoạt động
- Kiểm tra SweetAlert2 đã load
- Verify function `processPayment()`
- Kiểm tra API `/Sale/CreateOrder`

### Layout bị vỡ
- Clear cache browser
- Kiểm tra Bootstrap CSS đã load
- Verify responsive breakpoints

## Liên hệ & Hỗ trợ
- Developer: Tang DV
- Email: tangdv@example.com
- Version: 1.0.0
- Last Updated: October 2025

---

## Screenshots

### Giao diện bán hàng
![POS Interface](../path/to/screenshot.png)

### Quản lý đơn hàng
![Order Management](../path/to/orders.png)

