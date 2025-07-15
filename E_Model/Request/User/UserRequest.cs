using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Request.User
{
    public class UserRequest : PaginationRequest
    {
        public string? Emp_Code { get; set; }
        public string? Dept_code { get; set; }

    }
}
