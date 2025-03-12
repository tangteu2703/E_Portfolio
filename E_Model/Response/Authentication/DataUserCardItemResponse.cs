using E_Model.Authentication;

namespace E_Model.Response.Authentication
{
    public class DataUserCardItemResponse : data_user
    {
        public string title { get; set; }
        public string position { get; set; }
        public string department { get; set; }
        public string company { get; set; }
        public string organize { get; set; }
        public IEnumerable<data_application> list_application { get; set; }
        public sys_ldap_error ldap_err { get; set; }
    }
}