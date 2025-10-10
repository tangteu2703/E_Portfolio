namespace E_Model.News
{
    public class data_news_comment_image
    {
        public int id { get; set; }
        public int comment_id { get; set; }
        public string image_url { get; set; }
        public DateTime? created_at { get; set; }
        public string? created_by { get; set; }
    }
}

