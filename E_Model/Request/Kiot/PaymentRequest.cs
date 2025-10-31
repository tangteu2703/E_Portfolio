using E_Model.Sale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Request.Kiot
{
    public class PaymentRequest 
    {
        public string? OrderNumber { get; set; }
        public string? OrderType { get; set; }
        public int? TableNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public List<ItemModel> Items { get; set; } = new List<ItemModel>();
        public double? Subtotal { get; set; }
        public double? Discount { get; set; }
        public double? Total { get; set; }
        public string PaymentMethod { get; set; } = "cash";
        public string Status { get; set; } = "Completed";
    }
    public class ItemModel
    {
        public int? Id { get; set; }
        public string? Code { get; set; }
        public string? name { get; set; } 
        public double? Price { get; set; }
        public double? SalePrice { get; set; }
        public int? Quantity { get; set; }
    }
}
