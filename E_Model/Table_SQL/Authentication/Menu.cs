using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Table_SQL.Authentication
{
    public class Menu : modify_info
    {
        public int? menu_id { get; set; }
        public int? parent_id { get; set; }
        public string? menu_name { get; set; }
        public string? menu_url { get; set; }
        public string? icon_url { get; set; }
        public string? descriptions { get; set; }
        public string? is_order { get; set; }
    }
}
