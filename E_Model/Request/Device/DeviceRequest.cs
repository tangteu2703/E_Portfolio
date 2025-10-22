using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Request.Device
{
    public class DeviceRequest : PaginationRequest
    {
        public string? department { get; set; } 

        public string? status { get; set; }
    }
}
