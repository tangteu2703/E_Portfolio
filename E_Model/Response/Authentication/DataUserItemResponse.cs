using E_Model.Authentication;

namespace E_Model.Response.Authentication
{
    public class DataUserItemResponse : data_user
    {
        public string organize_name { get; set; }
        public string ldap_server { get; set; }
        public int ldap_port { get; set; }
        public string ldap_dc { get; set; }
        public int ldap_setting_id { get; set; }
        public int ldap_server_id { get; set; }

        public string title { get; set; }
        public string position { get; set; }
        public string department { get; set; }
        public string organize { get; set; }
        public string company { get; set; }
        public string role_name { get; set; }
        public string? user_code { get; set; }

    }
}