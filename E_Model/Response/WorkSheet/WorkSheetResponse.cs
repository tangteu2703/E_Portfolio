using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Response.WorkSheet
{
    public class WorkSheetResponse
    {
        public string? Factory { get; set; }
        public string? Emp_Code { get; set; }
        public string? Emp_Name { get; set; }
        public string? Photo { get; set; }
        public string? Dept_Name { get; set; }
        public DateTime? Work_Day { get; set; }
        public DateTime? DateTime_In { get; set; }
        public string? Machine_In { get; set; }
        public DateTime? DateTime_Out { get; set; }
        public string? Machine_Out { get; set; }
        public string? Shift { get; set; }
    }

}
