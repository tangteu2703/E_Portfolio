namespace E_Model.Sale
{
    /// <summary>
    /// Fake data for testing POS system
    /// </summary>
    public static class ProductSeedData
    {
        /// <summary>
        /// Get all categories
        /// </summary>
        public static List<CategoryModel> GetCategories()
        {
            return new List<CategoryModel>
            {
                new CategoryModel { Id = 1, Name = "Combo Khuyến Mãi", Icon = "bi-gift", DisplayOrder = 1 },
                new CategoryModel { Id = 2, Name = "Nước Uống", Icon = "bi-cup-straw", DisplayOrder = 2 },
                new CategoryModel { Id = 3, Name = "Bánh Ngọt", Icon = "bi-cake2", DisplayOrder = 3 },
                new CategoryModel { Id = 4, Name = "Cà Phê", Icon = "bi-cup-hot", DisplayOrder = 4 },
                new CategoryModel { Id = 5, Name = "Trà", Icon = "bi-droplet", DisplayOrder = 5 },
                new CategoryModel { Id = 6, Name = "Bánh Mì", Icon = "bi-egg-fried", DisplayOrder = 6 },
                new CategoryModel { Id = 7, Name = "Snack", Icon = "bi-basket", DisplayOrder = 7 }
            };
        }

        /// <summary>
        /// Get all products with realistic Vietnamese F&B items
        /// </summary>
        public static List<ProductModel> GetProducts()
        {
            return new List<ProductModel>
            {
                // COMBO KHUYẾN MÃI
                new ProductModel 
                { 
                    Id = 1, 
                    Code = "COMBO001", 
                    Name = "Combo Trà Sữa + Bánh Bông Lan", 
                    CategoryId = 1,
                    CategoryName = "Combo Khuyến Mãi",
                    Price = 65000, 
                    SalePrice = 45000,
                    Description = "Trà sữa trân châu + Bánh bông lan trứng muối",
                    ImageUrl = "/assets/img/sales/combo-trasua-banh.jpg",
                    Stock = 50,
                    Unit = "Set",
                    IsBestSeller = true
                },
                new ProductModel 
                { 
                    Id = 2, 
                    Code = "COMBO002", 
                    Name = "Combo Cà Phê + Bánh Mì", 
                    CategoryId = 1,
                    CategoryName = "Combo Khuyến Mãi",
                    Price = 45000, 
                    SalePrice = 35000,
                    Description = "Cà phê sữa đá + Bánh mì thịt nguội",
                    ImageUrl = "/assets/img/sales/combo-caphe-banhmi.jpg",
                    Stock = 40,
                    Unit = "Set",
                    IsBestSeller = true
                },
                new ProductModel 
                { 
                    Id = 3, 
                    Code = "COMBO003", 
                    Name = "Combo Trà Chanh + Snack", 
                    CategoryId = 1,
                    CategoryName = "Combo Khuyến Mãi",
                    Price = 50000, 
                    SalePrice = 38000,
                    Description = "Trà chanh dây + Snack khoai tây chiên",
                    ImageUrl = "/assets/img/sales/combo-trachanh-snack.jpg",
                    Stock = 35,
                    Unit = "Set"
                },

                // NƯỚC UỐNG - TRÀ
                new ProductModel 
                { 
                    Id = 4, 
                    Code = "TRA001", 
                    Name = "Trà Chanh Đào", 
                    CategoryId = 5,
                    CategoryName = "Trà",
                    Price = 35000,
                    Description = "Trà xanh pha chanh tươi với đào thái lát",
                    ImageUrl = "/assets/img/sales/tra-chanh-dao.jpg",
                    Stock = 100,
                    Unit = "Ly",
                    IsBestSeller = true
                },
                new ProductModel 
                { 
                    Id = 5, 
                    Code = "TRA002", 
                    Name = "Trà Sữa Trân Châu Đường Đen", 
                    CategoryId = 5,
                    CategoryName = "Trà",
                    Price = 45000,
                    Description = "Trà sữa với trân châu đường đen dai mềm",
                    ImageUrl = "/assets/img/sales/tra-sua-tran-chau.jpg",
                    Stock = 80,
                    Unit = "Ly",
                    IsBestSeller = true
                },
                new ProductModel 
                { 
                    Id = 6, 
                    Code = "TRA003", 
                    Name = "Trà Đào Cam Sả", 
                    CategoryId = 5,
                    CategoryName = "Trà",
                    Price = 38000,
                    Description = "Trà đào cam sả tươi mát",
                    ImageUrl = "/assets/img/sales/tra-dao-cam-sa.jpg",
                    Stock = 90,
                    Unit = "Ly"
                },
                new ProductModel 
                { 
                    Id = 7, 
                    Code = "TRA004", 
                    Name = "Trà Sữa Ô Long", 
                    CategoryId = 5,
                    CategoryName = "Trà",
                    Price = 40000,
                    Description = "Trà sữa Ô Long thơm ngon",
                    ImageUrl = "/assets/img/sales/tra-sua-olong.jpg",
                    Stock = 70,
                    Unit = "Ly"
                },

                // CÀ PHÊ
                new ProductModel 
                { 
                    Id = 8, 
                    Code = "CF001", 
                    Name = "Cà Phê Sữa Đá", 
                    CategoryId = 4,
                    CategoryName = "Cà Phê",
                    Price = 25000,
                    Description = "Cà phê phin truyền thống Việt Nam",
                    ImageUrl = "/assets/img/sales/ca-phe-sua-da.jpg",
                    Stock = 120,
                    Unit = "Ly",
                    IsBestSeller = true
                },
                new ProductModel 
                { 
                    Id = 9, 
                    Code = "CF002", 
                    Name = "Cà Phê Đen Đá", 
                    CategoryId = 4,
                    CategoryName = "Cà Phê",
                    Price = 22000,
                    Description = "Cà phê đen đá truyền thống",
                    ImageUrl = "/assets/img/sales/ca-phe-den-da.jpg",
                    Stock = 110,
                    Unit = "Ly"
                },
                new ProductModel 
                { 
                    Id = 10, 
                    Code = "CF003", 
                    Name = "Bạc Xỉu", 
                    CategoryId = 4,
                    CategoryName = "Cà Phê",
                    Price = 28000,
                    Description = "Cà phê sữa nhiều sữa ít cà phê",
                    ImageUrl = "/assets/img/sales/bac-xiu.jpg",
                    Stock = 100,
                    Unit = "Ly",
                    IsBestSeller = true
                },
                new ProductModel 
                { 
                    Id = 11, 
                    Code = "CF004", 
                    Name = "Cappuccino", 
                    CategoryId = 4,
                    CategoryName = "Cà Phê",
                    Price = 45000,
                    Description = "Cappuccino Ý với bọt sữa mịn màng",
                    ImageUrl = "/assets/img/sales/cappuccino.jpg",
                    Stock = 60,
                    Unit = "Ly"
                },

                // NƯỚC UỐNG KHÁC
                new ProductModel 
                { 
                    Id = 12, 
                    Code = "NK001", 
                    Name = "Nước Ép Cam Tươi", 
                    CategoryId = 2,
                    CategoryName = "Nước Uống",
                    Price = 30000,
                    Description = "Nước ép cam tươi 100%",
                    ImageUrl = "/assets/img/sales/nuoc-ep-cam.jpg",
                    Stock = 50,
                    Unit = "Ly"
                },
                new ProductModel 
                { 
                    Id = 13, 
                    Code = "NK002", 
                    Name = "Sinh Tố Bơ", 
                    CategoryId = 2,
                    CategoryName = "Nước Uống",
                    Price = 35000,
                    Description = "Sinh tố bơ sánh mịn béo ngậy",
                    ImageUrl = "/assets/img/sales/sinh-to-bo.jpg",
                    Stock = 45,
                    Unit = "Ly"
                },
                new ProductModel 
                { 
                    Id = 14, 
                    Code = "NK003", 
                    Name = "Matcha Đá Xay", 
                    CategoryId = 2,
                    CategoryName = "Nước Uống",
                    Price = 48000,
                    Description = "Matcha nguyên chất đá xay mát lạnh",
                    ImageUrl = "/assets/img/sales/matcha-da-xay.jpg",
                    Stock = 40,
                    Unit = "Ly"
                },

                // BÁNH NGỌT
                new ProductModel 
                { 
                    Id = 15, 
                    Code = "BANH001", 
                    Name = "Bánh Bông Lan Trứng Muối", 
                    CategoryId = 3,
                    CategoryName = "Bánh Ngọt",
                    Price = 35000,
                    Description = "Bánh bông lan phô mai trứng muối thơm ngon",
                    ImageUrl = "/assets/img/sales/banh-bong-lan.jpg",
                    Stock = 60,
                    Unit = "Cái",
                    IsBestSeller = true
                },
                new ProductModel 
                { 
                    Id = 16, 
                    Code = "BANH002", 
                    Name = "Bánh Tiramisu", 
                    CategoryId = 3,
                    CategoryName = "Bánh Ngọt",
                    Price = 45000,
                    Description = "Bánh Tiramisu Ý chuẩn vị",
                    ImageUrl = "/assets/img/sales/tiramisu.jpg",
                    Stock = 30,
                    Unit = "Miếng"
                },
                new ProductModel 
                { 
                    Id = 17, 
                    Code = "BANH003", 
                    Name = "Bánh Mousse Dâu", 
                    CategoryId = 3,
                    CategoryName = "Bánh Ngọt",
                    Price = 42000,
                    Description = "Bánh mousse dâu tây tươi",
                    ImageUrl = "/assets/img/sales/mousse-dau.jpg",
                    Stock = 25,
                    Unit = "Miếng"
                },
                new ProductModel 
                { 
                    Id = 18, 
                    Code = "BANH004", 
                    Name = "Bánh Flan Caramel", 
                    CategoryId = 3,
                    CategoryName = "Bánh Ngọt",
                    Price = 25000,
                    Description = "Bánh flan caramel mềm mịn",
                    ImageUrl = "/assets/img/sales/flan-caramel.jpg",
                    Stock = 50,
                    Unit = "Cái"
                },

                // BÁNH MÌ
                new ProductModel 
                { 
                    Id = 19, 
                    Code = "BM001", 
                    Name = "Bánh Mì Thịt Nguội", 
                    CategoryId = 6,
                    CategoryName = "Bánh Mì",
                    Price = 25000,
                    Description = "Bánh mì thịt nguội pate đầy đủ",
                    ImageUrl = "/assets/img/sales/banh-mi-thit-nguoi.jpg",
                    Stock = 40,
                    Unit = "Ổ",
                    IsBestSeller = true
                },
                new ProductModel 
                { 
                    Id = 20, 
                    Code = "BM002", 
                    Name = "Bánh Mì Xíu Mại", 
                    CategoryId = 6,
                    CategoryName = "Bánh Mì",
                    Price = 28000,
                    Description = "Bánh mì kẹp xíu mại sốt cà chua",
                    ImageUrl = "/assets/img/sales/banh-mi-xiu-mai.jpg",
                    Stock = 35,
                    Unit = "Ổ"
                },
                new ProductModel 
                { 
                    Id = 21, 
                    Code = "BM003", 
                    Name = "Bánh Mì Trứng Ốp La", 
                    CategoryId = 6,
                    CategoryName = "Bánh Mì",
                    Price = 22000,
                    Description = "Bánh mì trứng ốp la giòn thơm",
                    ImageUrl = "/assets/img/sales/banh-mi-trung.jpg",
                    Stock = 45,
                    Unit = "Ổ"
                },

                // SNACK
                new ProductModel 
                { 
                    Id = 22, 
                    Code = "SNACK001", 
                    Name = "Khoai Tây Chiên", 
                    CategoryId = 7,
                    CategoryName = "Snack",
                    Price = 20000,
                    Description = "Khoai tây chiên giòn rụm",
                    ImageUrl = "/assets/img/sales/khoai-tay-chien.jpg",
                    Stock = 80,
                    Unit = "Phần"
                },
                new ProductModel 
                { 
                    Id = 23, 
                    Code = "SNACK002", 
                    Name = "Gà Popcorn", 
                    CategoryId = 7,
                    CategoryName = "Snack",
                    Price = 35000,
                    Description = "Gà popcorn giòn tan cay nhẹ",
                    ImageUrl = "/assets/img/sales/ga-popcorn.jpg",
                    Stock = 60,
                    Unit = "Phần"
                },
                new ProductModel 
                { 
                    Id = 24, 
                    Code = "SNACK003", 
                    Name = "Phô Mai Que", 
                    CategoryId = 7,
                    CategoryName = "Snack",
                    Price = 25000,
                    Description = "Phô mai que chiên giòn bên ngoài mềm bên trong",
                    ImageUrl = "/assets/img/sales/pho-mai-que.jpg",
                    Stock = 50,
                    Unit = "Phần"
                },
                new ProductModel 
                { 
                    Id = 25, 
                    Code = "SNACK004", 
                    Name = "Nem Chua Rán", 
                    CategoryId = 7,
                    CategoryName = "Snack",
                    Price = 30000,
                    Description = "Nem chua rán giòn chua ngọt đặc trưng",
                    ImageUrl = "/assets/img/sales/nem-chua-ran.jpg",
                    Stock = 40,
                    Unit = "Phần"
                }
            };
        }

        /// <summary>
        /// Get products by category
        /// </summary>
        public static List<ProductModel> GetProductsByCategory(int categoryId)
        {
            return GetProducts().Where(p => p.CategoryId == categoryId).ToList();
        }

        /// <summary>
        /// Get best seller products
        /// </summary>
        public static List<ProductModel> GetBestSellers()
        {
            return GetProducts().Where(p => p.IsBestSeller).ToList();
        }

        /// <summary>
        /// Get products on sale
        /// </summary>
        public static List<ProductModel> GetProductsOnSale()
        {
            return GetProducts().Where(p => p.SalePrice.HasValue).ToList();
        }
    }
}

