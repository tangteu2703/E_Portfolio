namespace E_Model.Authentication
{
    public class data_menu : modify_info
    {
        public int id { get; set; }
        public string menu { get; set; }
        public int menu_parent_id { get; set; }
        public string icon { get; set; }
        public string menu_url { get; set; }
        public int order { get; set; }
        public int application_id { get; set; }
    }
}