using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Table_SQL.Kiot
{
    public class KiotCategory : modify_info
    {
        public int? category_id { get; set; }
        public string? category_name { get; set; }
        public string? icon { get; set; }
        public int? is_order { get; set; }
    }
}
