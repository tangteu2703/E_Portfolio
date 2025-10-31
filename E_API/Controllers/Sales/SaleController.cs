using E_API.Filter;
using E_Contract.Service;
using E_Model.Request.Device;
using E_Model.Request.Kiot;
using E_Model.Response.Device;
using E_Model.Response.Kiot;
using E_Model.Sale;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_API.Controllers.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : BaseController
    {
        private readonly IServiceWrapper _serviceWrapper;
        public SaleController(IServiceWrapper serviceWrapper)
        {
            _serviceWrapper = serviceWrapper;
        }

        [HttpGet("Select-Categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = (await _serviceWrapper.KiotCategory.SelectAllAsync()).ToList();

                return OK(categories);
            }
            catch (Exception ex)
            {
                return InternalServerError($"Internal server error: {ex.Message}", ex);
            }
        }

        #region Menu

        [HttpPost("Select-Menus")]
        public async Task<IActionResult> GetMenus([FromBody] ProductModel request)
        {
            try
            {
                var products = (await _serviceWrapper.KiotMenu.SelectAllAsync()).ToList();

                var productResponses = products.Select(c => new ProductResponse
                {
                    Id = c.menu_id,
                    Code = c.menu_code,
                    Name = c.menu_name,
                    Description = c.descriptions,
                    CategoryId = c.category_id,
                    CategoryName = "",
                    Price = c.price,
                    SalePrice = c.sale_price,
                    ImageUrl = c.image_url,
                    Stock = c.stock,
                    Unit = c.unit,
                    IsActive = c.is_active,
                    IsBestSeller = c.is_bestseller,
                }).ToList();

                // fake data
                // products = ProductSeedData.GetProducts();

                if (request.CategoryId > 0)
                    productResponses = productResponses.Where(c => c.CategoryId == request.CategoryId).ToList();

                return OK(productResponses);
            }
            catch (Exception ex)
            {
                return InternalServerError($"Internal server error: {ex.Message}", ex);
            }
        }

        [HttpPost("Update-Carts")]
        public async Task<IActionResult> UpdateOrderCart([FromBody] OrderModel request)
        {
            try
            {
                // tạo session giỏ hàng nếu chưa có
                // cập nhật giỏ hàng trong session


                return OK("Cập nhật giỏ hàng thành công" );
            }
            catch (Exception ex)
            {
                return InternalServerError($"Internal server error: {ex.Message}", ex);
            }
        }

        [HttpPost("Update-Order-Payment")]
        public async Task<IActionResult> OrderPayment([FromBody] PaymentRequest request)
        {
            try
            {
                // xử lý thanh toán đơn hàng


                return OK("Thanh toán đơn hàng thành công!");
            }
            catch (Exception ex)
            {
                return InternalServerError($"Internal server error: {ex.Message}", ex);
            }
        }

        #endregion

        #region history

        [HttpPost("Select-History")]
        public async Task<IActionResult> GetOrderHistory(ProductModel request)
        {
            try
            {
                // Get products from seed data (in production: get from database)
                var products = ProductSeedData.GetProducts();

                if (request.CategoryId > 0)
                    products = products.Where(c => c.CategoryId == request.CategoryId).ToList();

                return OK(products);
            }
            catch (Exception ex)
            {
                return InternalServerError($"Internal server error: {ex.Message}", ex);
            }
        }

        [HttpPost("Update-Apply-Discount")]
        public async Task<IActionResult> ApplyDiscount(OrderModel request)
        {
            try
            {
                // apply discount code to order


                return OK();
            }
            catch (Exception ex)
            {
                return InternalServerError($"Internal server error: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
