using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Authentication
{
    public class sys_ldap_error
    {
        public string error_code { get; set; }
        public string ldap_error { get; set; }
        public string dsis { get; set; }
        public string comment { get; set; }
        public string data { get; set; }
        public string version { get; set; }
    }
}
