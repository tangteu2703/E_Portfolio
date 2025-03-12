namespace E_Model.Authentication
{
    public class data_organize : modify_info
    {
        public int id { get; set; }
        public string organize_name { get; set; }
        public int organize_parent_id { get; set; }
        public int organize_level_id { get; set; }
    }
}