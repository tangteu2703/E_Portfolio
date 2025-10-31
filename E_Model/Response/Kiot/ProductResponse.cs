using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Response.Kiot
{
    public class ProductResponse
    {
        public int? Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; } 
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public double? Price { get; set; }
        public double? SalePrice { get; set; }
        public string? ImageUrl { get; set; }
        public int? Stock { get; set; }
        public string? Unit { get; set; }
        public bool? IsActive { get; set; } = true;
        public bool? IsBestSeller { get; set; }
    }
}
