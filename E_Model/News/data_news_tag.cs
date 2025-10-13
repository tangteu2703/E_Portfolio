namespace E_Model.News
{
    public class data_news_tag
    {
        public int id { get; set; }
        public int news_id { get; set; }
        public string user_code { get; set; }
        public DateTime? created_at { get; set; }
        public string? created_by { get; set; }
    }
}

