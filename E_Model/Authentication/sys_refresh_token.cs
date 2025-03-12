namespace E_Model.Authentication
{
    public class sys_refresh_token
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string email { get; set; }
        public string token { get; set; }
        public DateTime expired_date { get; set; }
        public DateTime created_time { get; set; }
        public string created_by_ip { get; set; }
        public string created_by_mac { get; set; }
        public bool is_deleted { get; set; }
    }
}