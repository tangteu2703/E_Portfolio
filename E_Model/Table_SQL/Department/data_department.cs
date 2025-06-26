using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Table_SQL.Department
{
    public class data_department : Modified
    {
        public int id { get; set; }
        public string? dept_code { get; set; }
        public string? dept_name { get; set; }
        public string? description { get; set; }
        public bool is_active { get; set; } = true;
        public int? parent_id { get; set; } 
    }
}
