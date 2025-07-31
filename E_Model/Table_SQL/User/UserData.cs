using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Table_SQL.User
{
    public class UserData : modify_info
    {
        public int user_id { get; set; }
        public string? email { get; set; }
        public string? phone_number { get; set; }
        public string? password { get; set; }
        public string? full_name { get; set; }
        public DateTime? birth_date { get; set; }
        public bool? gender { get; set; }
        public string? address { get; set; }
        public string? avatar_url { get; set; }
        public string? user_code { get; set; }
        public int? department_id { get; set; }
        public int? title_id { get; set; }
        public int? position_id { get; set; }
        public string? card_color { get; set; }
        public bool? is_ldap { get; set; }
    }
}
