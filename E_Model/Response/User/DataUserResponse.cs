using E_Model.Table_SQL.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Response.User
{
    public class DataUserResponse : UserData
    {
        public string? user_code { get; set; }
        public string? card_color { get; set; }
        public bool? is_ldap { get; set; }
        public string? role_name { get; set; }
    }
}
