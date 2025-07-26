using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Request.Device
{
    public class DeviceRequest : PaginationRequest
    {
        public int? type_id { get; set; } = null;

        public int? status_id { get; set; } = null;
    }
}
