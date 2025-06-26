using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Table_SQL.WorkSheet
{
    public class worksheet_daily
    {
        public int id { get; set; }
        public string Factory { get; set; }
        public string Emp_Code { get; set; }
        public DateTime? DateTime_In { get; set; }
        public string Machine_In { get; set; }
        public DateTime? DateTime_Out { get; set; }
        public string Machine_Out { get; set; }
        public string Shift { get; set; }
        public DateTime LastTime_Modified { get; set; }
        public string User_ID { get; set; } 
        public bool Is_Deleted { get; set; }
        public string Note { get; set; }
    }
}
