# API Pattern Summary - POS System

## ✅ Chuẩn API Calls đã áp dụng

### 📋 apiHelper Methods

```javascript
// GET Request
apiHelper.get(url, data, successCallback, errorCallback, isAddToken = true)

// POST Request  
apiHelper.post(url, data, successCallback, errorCallback, isAddToken = true, isFormData = false, isBlob = false)

// PUT Request
apiHelper.put(url, data, successCallback, errorCallback, isAddToken = true)

// DELETE Request
apiHelper.delete(url, id, successCallback, errorCallback, isAddToken = true)

// GET File
apiHelper.getFile(url, data, successCallback, errorCallback, isAddToken = true)

// POST File (with FormData)
apiHelper.postFile(url, data, successCallback, errorCallback, isAddToken = true, isBlob = false)
```

---

## 🎯 Đã implement trong sale.js

### 1. **GET - Load Products**

```javascript
await apiHelper.get('/Sale/GetProducts', {},
    function (res) {
        allProducts = Array.isArray(res.data) ? res.data : [];
        renderProducts(allProducts, '#productGrid');
    },
    function (err) {
        console.error("Lỗi tải sản phẩm:", err);
        toastr.warning('Không thể tải sản phẩm, thử lại sau.');
    });
```

**Đặc điểm**:
- ✅ URL: `/Sale/GetProducts`
- ✅ Data: `{}` (empty object cho GET)
- ✅ Success callback: Handle response
- ✅ Error callback: Log + toastr warning
- ✅ Array safety check

---

### 2. **GET - Load Categories**

```javascript
await apiHelper.get('/Sale/GetCategories', {},
    function (res) {
        var categories = Array.isArray(res.data) ? res.data : [];
        renderCategoryFilter(categories);
    },
    function (err) {
        console.error("Lỗi tải danh mục:", err);
    });
```

**Đặc điểm**:
- ✅ Empty data object `{}`
- ✅ Array safety
- ✅ Error handling

---

### 3. **GET - Filter by Category (with params)**

```javascript
await apiHelper.get('/Sale/GetProductsByCategory', { categoryId: categoryId },
    function (res) {
        var products = Array.isArray(res.data) ? res.data : [];
        renderProducts(products, '#productGrid');
    },
    function (err) {
        console.error("Lỗi lọc sản phẩm theo danh mục:", err);
        toastr.warning('Không thể lọc sản phẩm.');
    });
```

**Đặc điểm**:
- ✅ Data object: `{ categoryId: categoryId }`
- ✅ apiHelper tự động convert thành query string: `?categoryId=1`
- ✅ Toastr notification

---

### 4. **POST - Create Order**

```javascript
const orderData = {
    orderNumber: generateOrderNumber(),
    customerName: customerInfo || 'Khách lẻ',
    customerPhone: '',
    items: cart,
    subtotal: subtotal,
    discount: discount,
    total: total,
    paymentMethod: paymentMethod,
    status: 'Completed',
    createdAt: new Date().toISOString()
};

await apiHelper.post('/Sale/CreateOrder', orderData,
    function (res) {
        // Success
        const finalOrderNumber = res.orderNumber || orderData.orderNumber;
        
        Swal.fire({
            title: 'Thanh toán thành công!',
            html: `...`,
            icon: 'success'
        });

        // Clear cart
        cart = [];
        $('#customerInfo').val('');
        updateCart();
    },
    function (err) {
        console.error("Lỗi tạo đơn hàng:", err);
        toastr.error('Không thể tạo đơn hàng, vui lòng thử lại.');
    });
```

**Đặc điểm**:
- ✅ POST request với full order data
- ✅ Success: SweetAlert2 + Clear cart
- ✅ Error: toastr error
- ✅ Try-catch wrapper

---

## 📝 Pattern Comparison

### ❌ SAI - Không dùng
```javascript
// Don't use fetch
const response = await fetch('/Sale/GetProducts');
const data = await response.json();

// Don't use callApi (không tồn tại)
await apiHelper.callApi('/Sale/GetProducts', 'GET', null, ...);
```

### ✅ ĐÚNG - Dùng theo chuẩn
```javascript
// Use apiHelper.get for GET
await apiHelper.get('/Sale/GetProducts', {}, successCallback, errorCallback);

// Use apiHelper.post for POST
await apiHelper.post('/Sale/CreateOrder', orderData, successCallback, errorCallback);
```

---

## 🔧 Controller Side (C#)

### GET Endpoints

```csharp
[HttpGet]
public IActionResult GetProducts()
{
    var products = ProductSeedData.GetProducts();
    return Json(new { success = true, data = products });
}

[HttpGet]
public IActionResult GetProductsByCategory(int categoryId)
{
    var products = ProductSeedData.GetProductsByCategory(categoryId);
    return Json(new { success = true, data = products });
}
```

### POST Endpoint

```csharp
[HttpPost]
public IActionResult CreateOrder([FromBody] OrderModel orderData)
{
    try
    {
        var orderNumber = $"HD{DateTime.Now.Ticks.ToString().Substring(10)}";
        
        _logger.LogInformation($"Order created: {orderNumber}");

        return Json(new { 
            success = true, 
            orderNumber = orderNumber, 
            message = "Tạo đơn hàng thành công" 
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating order");
        return Json(new { 
            success = false, 
            message = "Có lỗi xảy ra khi tạo đơn hàng" 
        });
    }
}
```

---

## 📊 Response Format (Standard)

### Success Response
```json
{
    "success": true,
    "data": [...] // hoặc object
}
```

### Error Response
```json
{
    "success": false,
    "message": "Error message here"
}
```

### POST Success
```json
{
    "success": true,
    "orderNumber": "HD12345678",
    "message": "Tạo đơn hàng thành công"
}
```

---

## 🎨 Client-side Handling

### Success Callback Pattern
```javascript
function (res) {
    // 1. Check array safety
    var data = Array.isArray(res.data) ? res.data : [];
    
    // 2. Process data
    renderProducts(data, '#productGrid');
    
    // 3. Show success notification (optional)
    toastr.success('Thành công!');
}
```

### Error Callback Pattern
```javascript
function (err) {
    // 1. Log to console
    console.error("Lỗi:", err);
    
    // 2. Show user-friendly message
    toastr.warning('Không thể tải dữ liệu, thử lại sau.');
}
```

---

## ✅ Checklist

### API Calls
- [x] `apiHelper.get` cho GET requests
- [x] `apiHelper.post` cho POST requests
- [x] Empty object `{}` cho GET không có params
- [x] Object `{ key: value }` cho GET có params
- [x] Full data object cho POST

### Error Handling
- [x] Success callback
- [x] Error callback
- [x] Array.isArray() check
- [x] toastr notifications
- [x] console.error logging
- [x] try-catch wrapper

### Response Handling
- [x] Check `res.data`
- [x] Array safety
- [x] Fallback values
- [x] Update UI
- [x] Clear state

---

## 🚀 Complete Example Flow

```javascript
// 1. Page Load
$(document).ready(function () {
    loadProducts();     // GET all products
    loadCategories();   // GET categories
});

// 2. Filter Products
async function filterByCategory(categoryId) {
    await apiHelper.get(
        '/Sale/GetProductsByCategory', 
        { categoryId: categoryId },
        function (res) {
            var products = Array.isArray(res.data) ? res.data : [];
            renderProducts(products, '#productGrid');
        },
        function (err) {
            console.error("Lỗi:", err);
            toastr.warning('Không thể lọc sản phẩm.');
        }
    );
}

// 3. Create Order
async function completePayment() {
    const orderData = {
        orderNumber: generateOrderNumber(),
        customerName: 'Khách hàng A',
        items: cart,
        total: total
    };

    await apiHelper.post(
        '/Sale/CreateOrder',
        orderData,
        function (res) {
            Swal.fire('Thành công!', 'Đơn hàng đã được tạo', 'success');
            cart = [];
            updateCart();
        },
        function (err) {
            console.error("Lỗi:", err);
            toastr.error('Không thể tạo đơn hàng.');
        }
    );
}
```

---

## 🎯 Best Practices

1. **Always use apiHelper methods** (get, post, put, delete)
2. **Empty object `{}` for GET without params**
3. **Object with params for GET with params**
4. **Full data object for POST**
5. **Array safety check** in success callback
6. **toastr for notifications** (not SweetAlert2 toast)
7. **console.error for logging**
8. **try-catch for async functions**
9. **Clear state after success**
10. **User-friendly error messages**

---

**Version**: 3.0.0  
**Status**: ✅ Standardized & Production Ready  
**Updated**: October 27, 2025

