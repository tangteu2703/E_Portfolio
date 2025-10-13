namespace E_Model.News
{
    public class data_news_comment_reaction
    {
        public int id { get; set; }
        public int comment_id { get; set; }
        public string user_code { get; set; }
        public int reaction_type { get; set; } // 1:Like, 2:Love, 3:Haha
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}

