using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Table_SQL.WorkSheet
{
    public class worksheet_detail : Modified
    {
        public int ID { get; set; }

        public string? Factory { get; set; }

        public string? Emp_Code { get; set; }

        public DateTime? Work_Day { get; set; }

        public string? Shift_Code { get; set; }

        public string? Day_Code { get; set; }

        public DateTime? Time_In { get; set; }

        public DateTime? Time_Out { get; set; }

        public double? Work_Hour { get; set; } = 0;

        public double? Lack_Hour { get; set; } = 0;

        public double? OT_101 { get; set; } = 0;

        public double? OT_102 { get; set; } = 0;

        public double? OT_103 { get; set; } = 0;

        public double? OT_201 { get; set; } = 0;

        public double? OT_202 { get; set; } = 0;
        public double? OT_301 { get; set; } = 0;

        public double? OT_302 { get; set; } = 0;
        public string? HR_Confirm { get; set; }
    }
}
