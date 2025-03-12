namespace E_Model.Authentication
{
    public class sys_ldap_server : modify_info
    {
        public int id { get; set; }
        public string ldap_server { get; set; }
        public int ldap_port { get; set; }
        public string ldap_dc { get; set; }
    }
}