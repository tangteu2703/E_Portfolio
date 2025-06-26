using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Request
{
    public class TimeRequest
    {
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public string? shift { get; set; }
        public string? empCode { get; set; }
    }
}
