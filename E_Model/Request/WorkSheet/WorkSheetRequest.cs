using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Request.WorkSheet
{
    public class WorkSheetRequest : PaginationRequest
    {
        public string? dept_code { get; set; } = "";
        public string? shift { get; set; } = "";
        public DateTime? from_date { get; set; } = null;
        public DateTime? to_date { get; set; } = null;
    }
}
