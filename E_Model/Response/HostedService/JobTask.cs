using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Response.HostedService
{
    public class JobTask
    {
        public string TaskCode { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty;
        public TimeSpan RunAt { get; set; }
        public bool Status { get; set; } = true; // true = bật, false = tắt
    }
}
