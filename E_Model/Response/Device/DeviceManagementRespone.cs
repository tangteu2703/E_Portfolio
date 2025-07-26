using E_Model.Table_SQL.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Response.Device
{
    public class DeviceManagementRespone
    {
        public int type_id { get; set; }
        public string? type_name { get; set; }
        public int? id { get; set; }
        public string? device_code { get; set; }
        public string? device_name { get; set; }
        public string? device_config { get; set; }
        public int quantity { get; set; }
        public int status_id { get; set; }
        public string? status_name { get; set; }
        public string? user_code { get; set; }
        public string? last_user_code { get; set; }
        public string? full_name { get; set; }
        public string? note { get; set; }
    }
}
