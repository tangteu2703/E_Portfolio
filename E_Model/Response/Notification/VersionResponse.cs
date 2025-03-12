using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Response.Notification
{
    public class VersionResponse
    {
        public string version { get; set; }
        public bool force_update { get; set; }
        public string min_supported_version { get; set; }
        public string latest_version { get; set; }
        public string update_url { get; set; }
        public string message { get; set; }
    }
}
