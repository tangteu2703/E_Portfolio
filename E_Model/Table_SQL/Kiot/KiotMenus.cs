using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Table_SQL.Kiot
{
    public class KiotMenus
    {
        public int? menu_id { get; set; }
        public string? menu_code { get; set; }
        public string? menu_name { get; set; }
        public string? descriptions { get; set; }
        public int? category_id { get; set; }
        public double? original_price { get; set; }
        public double? price { get; set; }
        public double? sale_price { get; set; }
        public string? image_url { get; set; }
        public int? stock { get; set; }
        public string? unit { get; set; }
        public bool? is_active { get; set; }
        public bool? is_bestseller { get; set; }
    }
}
