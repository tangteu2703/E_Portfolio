namespace E_Model.Authentication
{
    public class data_user : modify_info
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string full_name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string avatar { get; set; }
        public int organize_id { get; set; }
        public bool is_ldap { get; set; }

        public int title_id { get; set; }
        public int department_id { get; set; }
        public int company_id { get; set; }
        public int position_id { get; set; }

        public string card_color { get; set; }
    }
}