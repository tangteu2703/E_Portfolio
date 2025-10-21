using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Response.User
{
    public class OrganizeResponse
    {
        public int? user_id { get; set; }
        public string? user_code { get; set; }
        public string? full_name { get; set; }
        public string? title { get; set; }
        public string? position { get; set; }
        public string? section { get; set; }
        public string? dept { get; set; }
        public string? group { get; set; }
        public string? avatar { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }

        public int? parent_id { get; set; }
    }
}
