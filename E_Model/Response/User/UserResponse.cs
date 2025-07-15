using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Response.User
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string? Usercode { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DepartmentName { get; set; }
        public string? PositionName { get; set; }
        public bool IsActive { get; set; }
    }
}
