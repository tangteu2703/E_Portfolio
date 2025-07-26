using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Table_SQL.Device
{
    public class DeviceManagement : modify_info
    {
        public int id { get; set; }
        public int type_id { get; set; }
        public string? device_code { get; set; }
        public string? device_name { get; set; }
        public string? device_config { get; set; }
        public int quantity { get; set; }
        public int status_id { get; set; }
    }
}
