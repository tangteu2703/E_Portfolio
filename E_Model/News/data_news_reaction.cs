namespace E_Model.News
{
    public class data_news_reaction
    {
        public int id { get; set; }
        public int news_id { get; set; }
        public string user_code { get; set; }
        public int reaction_type { get; set; } // 1:Like, 2:Love, 3:Haha, 4:Wow, 5:Sad, 6:Angry
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}

