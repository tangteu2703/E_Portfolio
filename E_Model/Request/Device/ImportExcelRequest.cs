using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Request.Device
{
    public class ImportExcelRequest
    {
        public IFormFile file { get; set; }
    }
}
