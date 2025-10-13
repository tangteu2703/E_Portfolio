namespace E_Model.Request.News
{
    public class ReactionRequest
    {
        public int news_id { get; set; }
        public string user_code { get; set; }
        public int reaction_type { get; set; } // 1:Like, 2:Love, 3:Haha, 4:Wow, 5:Sad, 6:Angry
    }
}

