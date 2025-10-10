namespace E_Model.Response.News
{
    public class CommentResponse
    {
        public int id { get; set; }
        public int news_id { get; set; }
        public int? parent_id { get; set; }
        public string user_code { get; set; }
        public string? user_name { get; set; }
        public string? user_avatar { get; set; }
        public string content { get; set; }
        public DateTime? created_at { get; set; }
        
        // Aggregated data
        public List<string>? images { get; set; }
        public List<ReactionCount>? reactions { get; set; }
        public int reply_count { get; set; } // Số lượng reply
        
        public class ReactionCount
        {
            public int reaction_type { get; set; }
            public int count { get; set; }
        }
    }
}

