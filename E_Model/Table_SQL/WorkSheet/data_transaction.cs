using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Table_SQL.WorkSheet
{
    public class data_transaction
    {
        public int id { get; set; }
        public string emp_code { get; set; }
        public DateTime punch_time { get; set; }
        public string punch_type { get; set; } // IN, OUT, BREAK, etc.
        public string note { get; set; } 
        public string terminal_sn { get; set; } 
    }
}
