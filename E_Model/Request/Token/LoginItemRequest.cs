using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Request.Token
{
    public class LoginItemRequest
    {
        public string? email { get; set; }
        public string? phone_number { get; set; }
        public string password { get; set; }
        public bool is_ldap { get; set; } = false;
    }
    public class RefreshItemRequest : LoginItemRequest
    {
        public string username { get; set; }
        public string refresh_token { get; set; }
    }
    public class ChangePasswordItemRequest : LoginItemRequest
    {
        public string old_password { get; set; }
        public string new_password { get; set; }
    }
    public class SignUpItemRequest
    {
        public string username { get; set; }
        public string full_name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string title { get; set; }
    }
}
