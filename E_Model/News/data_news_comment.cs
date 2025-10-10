namespace E_Model.News
{
    public class data_news_comment : modify_info
    {
        public int id { get; set; }
        public int news_id { get; set; }
        public int? parent_id { get; set; } // NULL: comment gốc, NOT NULL: reply
        public string user_code { get; set; }
        public string content { get; set; }
    }
}

