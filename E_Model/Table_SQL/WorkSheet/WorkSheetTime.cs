using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Table_SQL.WorkSheet
{
    public class WorkSheetTime : modify_info
    {
        public int? serial_id { get; set; }
        public string? user_code { get; set; }
        public DateTime? date_time { get; set; }
        public double? true_value { get; set; } = 0;
    }
}
