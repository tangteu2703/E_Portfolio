using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Request.User
{
    public class OrganizeRequest : PaginationRequest
    {
        public string? user_code { get; set; }
    }
}
