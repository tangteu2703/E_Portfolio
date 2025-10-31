# Sale Models - POS System

## 📦 Models Created

### 1. **ProductModel.cs**
Model chính cho sản phẩm trong hệ thống POS

**Properties:**
- `Id`: Product ID
- `Code`: Mã sản phẩm (SP001, TRA001...)
- `Name`: Tên sản phẩm
- `Description`: Mô tả
- `CategoryId`: ID danh mục
- `CategoryName`: Tên danh mục
- `Price`: Giá gốc
- `SalePrice`: Giá khuyến mãi (nullable)
- `ImageUrl`: Link hình ảnh
- `Stock`: Số lượng tồn kho
- `Unit`: Đơn vị tính (Ly, Cái, Ổ...)
- `IsActive`: Trạng thái hoạt động
- `IsBestSeller`: Sản phẩm bán chạy
- `CreatedAt`, `ModifiedAt`: Thời gian

### 2. **CategoryModel.cs**
Model cho danh mục sản phẩm

**Properties:**
- `Id`: Category ID
- `Name`: Tên danh mục
- `Description`: Mô tả
- `Icon`: Bootstrap icon class
- `DisplayOrder`: Thứ tự hiển thị
- `IsActive`: Trạng thái

### 3. **CartItemModel.cs**
Model cho item trong giỏ hàng

**Properties:**
- `ProductId`, `ProductCode`, `ProductName`
- `UnitPrice`: Giá đơn vị
- `SalePrice`: Giá sale (nullable)
- `Quantity`: Số lượng
- `Subtotal`: Tổng tiền (computed)
- `DiscountAmount`: Tiền giảm giá (computed)

### 4. **OrderModel.cs**
Model cho đơn hàng

**Properties:**
- `Id`, `OrderNumber`
- `CustomerName`, `CustomerPhone`
- `Items`: List<CartItemModel>
- `Subtotal`, `Discount`, `Total`
- `PaymentMethod`: cash, card, transfer, momo...
- `Status`: Completed, Pending, Cancelled
- `CreatedBy`, `CreatedAt`

---

## 🗂️ Seed Data (ProductSeedData.cs)

### Categories (7 categories)
1. **Combo Khuyến Mãi** - `bi-gift`
2. **Nước Uống** - `bi-cup-straw`
3. **Bánh Ngọt** - `bi-cake2`
4. **Cà Phê** - `bi-cup-hot`
5. **Trà** - `bi-droplet`
6. **Bánh Mì** - `bi-egg-fried`
7. **Snack** - `bi-basket`

### Products (25 realistic items)

#### Combo Khuyến Mãi (3 items)
- Combo Trà Sữa + Bánh Bông Lan (65k → 45k) ⭐
- Combo Cà Phê + Bánh Mì (45k → 35k) ⭐
- Combo Trà Chanh + Snack (50k → 38k)

#### Trà (4 items)
- Trà Chanh Đào (35k) ⭐
- Trà Sữa Trân Châu Đường Đen (45k) ⭐
- Trà Đào Cam Sả (38k)
- Trà Sữa Ô Long (40k)

#### Cà Phê (4 items)
- Cà Phê Sữa Đá (25k) ⭐
- Cà Phê Đen Đá (22k)
- Bạc Xỉu (28k) ⭐
- Cappuccino (45k)

#### Nước Uống (3 items)
- Nước Ép Cam Tươi (30k)
- Sinh Tố Bơ (35k)
- Matcha Đá Xay (48k)

#### Bánh Ngọt (4 items)
- Bánh Bông Lan Trứng Muối (35k) ⭐
- Bánh Tiramisu (45k)
- Bánh Mousse Dâu (42k)
- Bánh Flan Caramel (25k)

#### Bánh Mì (3 items)
- Bánh Mì Thịt Nguội (25k) ⭐
- Bánh Mì Xíu Mại (28k)
- Bánh Mì Trứng Ốp La (22k)

#### Snack (4 items)
- Khoai Tây Chiên (20k)
- Gà Popcorn (35k)
- Phô Mai Que (25k)
- Nem Chua Rán (30k)

⭐ = Best Seller (IsBestSeller = true)

---

## 🔧 Helper Methods

### `GetProducts()`
Trả về tất cả 25 sản phẩm

### `GetCategories()`
Trả về 7 danh mục

### `GetProductsByCategory(int categoryId)`
Lọc sản phẩm theo danh mục

### `GetBestSellers()`
Lấy các sản phẩm bán chạy (7 items)

### `GetProductsOnSale()`
Lấy sản phẩm đang sale (3 combo items)

---

## 📝 Usage Examples

### In Controller

```csharp
using E_Model.Sale;

public class SaleController : Controller
{
    [HttpGet]
    public IActionResult GetProducts()
    {
        var products = ProductSeedData.GetProducts();
        return Json(new { success = true, data = products });
    }

    [HttpGet]
    public IActionResult GetCategories()
    {
        var categories = ProductSeedData.GetCategories();
        return Json(new { success = true, data = categories });
    }

    [HttpGet]
    public IActionResult GetProductsByCategory(int categoryId)
    {
        var products = ProductSeedData.GetProductsByCategory(categoryId);
        return Json(new { success = true, data = products });
    }

    [HttpGet]
    public IActionResult GetBestSellers()
    {
        var products = ProductSeedData.GetBestSellers();
        return Json(new { success = true, data = products });
    }
}
```

### In View/JavaScript

```javascript
// Load all products
async function loadProducts() {
    const response = await fetch('/Sale/GetProducts');
    const data = await response.json();
    
    if (data.success) {
        renderProducts(data.data);
    }
}

// Load categories
async function loadCategories() {
    const response = await fetch('/Sale/GetCategories');
    const data = await response.json();
    
    if (data.success) {
        renderCategoryFilter(data.data);
    }
}

// Render product card
function renderProducts(products) {
    products.forEach(product => {
        const hasSale = product.salePrice && product.salePrice > 0;
        const price = hasSale ? product.salePrice : product.price;
        
        console.log(`${product.name}: ${formatCurrency(price)}`);
        // Trà Chanh Đào: 35.000 ₫
    });
}
```

---

## 🎨 Product Code Format

| Category | Prefix | Example |
|----------|--------|---------|
| Combo | COMBO | COMBO001, COMBO002 |
| Trà | TRA | TRA001, TRA002 |
| Cà Phê | CF | CF001, CF002 |
| Nước Uống | NK | NK001, NK002 |
| Bánh Ngọt | BANH | BANH001, BANH002 |
| Bánh Mì | BM | BM001, BM002 |
| Snack | SNACK | SNACK001, SNACK002 |

---

## 💾 Database Migration (Future)

Khi cần migrate sang database thực:

```sql
CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Icon NVARCHAR(50),
    DisplayOrder INT DEFAULT 0,
    IsActive BIT DEFAULT 1
);

CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Code NVARCHAR(50) NOT NULL UNIQUE,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    CategoryId INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    SalePrice DECIMAL(18,2),
    ImageUrl NVARCHAR(500),
    Stock INT DEFAULT 0,
    Unit NVARCHAR(20),
    IsActive BIT DEFAULT 1,
    IsBestSeller BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    ModifiedAt DATETIME,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);

-- Insert seed data
INSERT INTO Categories (Name, Icon, DisplayOrder) VALUES
('Combo Khuyến Mãi', 'bi-gift', 1),
('Nước Uống', 'bi-cup-straw', 2),
... (continue)

INSERT INTO Products (Code, Name, CategoryId, Price, SalePrice, ...) VALUES
('COMBO001', 'Combo Trà Sữa + Bánh Bông Lan', 1, 65000, 45000, ...),
... (continue)
```

---

## ✅ Benefits

1. **Type Safety**: Strong typing với C# models
2. **IntelliSense**: Auto-complete trong IDE
3. **Realistic Data**: 25 sản phẩm F&B thực tế Việt Nam
4. **Easy Testing**: Seed data sẵn có
5. **Scalable**: Dễ chuyển sang database
6. **Best Practices**: Follow naming conventions

---

## 🔄 Next Steps

- [ ] Add product images (real images)
- [ ] Integrate with database
- [ ] Add more product attributes (ingredients, allergens)
- [ ] Implement inventory management
- [ ] Add product variants (size: S, M, L)
- [ ] Add product reviews/ratings
- [ ] Implement barcode support

---

**Created**: October 27, 2025  
**Version**: 1.0.0  
**Status**: ✅ Ready to use

