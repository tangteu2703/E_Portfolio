namespace E_Model.News
{
    public class data_news_image
    {
        public int id { get; set; }
        public int news_id { get; set; }
        public string image_url { get; set; }
        public int image_order { get; set; } = 0;
        public DateTime? created_at { get; set; }
        public string? created_by { get; set; }
        public bool is_deleted { get; set; } = false;
    }
}

