namespace E_Model.Sale
{
    /// <summary>
    /// Model for Product in POS System
    /// </summary>
    public class ProductModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string? ImageUrl { get; set; }
        public int Stock { get; set; }
        public string? Unit { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsBestSeller { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    /// <summary>
    /// Model for Product Category
    /// </summary>
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Model for Cart Item
    /// </summary>
    public class CartItemModel
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => (SalePrice ?? UnitPrice) * Quantity;
        public decimal DiscountAmount => SalePrice.HasValue ? (UnitPrice - SalePrice.Value) * Quantity : 0;
    }

    /// <summary>
    /// Model for Order
    /// </summary>
    public class OrderModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public List<CartItemModel> Items { get; set; } = new List<CartItemModel>();
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public string PaymentMethod { get; set; } = "cash";
        public string Status { get; set; } = "Completed";
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

