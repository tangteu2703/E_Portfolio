using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Response.Device
{
    public class DeviceRequestResponse
    {
        public string? request_id { get; set; }
        public string? request_date { get; set; }
        public string? deadline { get; set; }
        public string? requester_code { get; set; }
        public string? requester_name { get; set; }
        public string? requester_email { get; set; }
        public string? requester_position { get; set; }
        public string? requester_dept { get; set; }
        public string? receive_dept { get; set; }
        public string? title { get; set; }
        public string? content { get; set; }
        public string? status { get; set; }
        public int? step { get; set; }
    }
}
