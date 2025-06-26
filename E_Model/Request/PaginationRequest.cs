using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Request
{
    public class PaginationRequest
    {
        public string? TextSearch { get; set; } = "";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public bool IsAscending { get; set; } = true;
      
    }
}
