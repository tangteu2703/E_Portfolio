using E_Model.News;

namespace E_Model.Response.News
{
    public class NewsResponse
    {
        public int id { get; set; }
        public string user_code { get; set; }
        public string? user_name { get; set; }
        public string? user_avatar { get; set; }
        public string? title { get; set; }
        public string? contents { get; set; }
        public string? status { get; set; }
        public string? location { get; set; }
        public int privacy_level { get; set; }
        public bool is_pinned { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        
        // Aggregated data
        public List<ImageItem>? images { get; set; }
        public List<string>? tagged_users { get; set; }
        public List<ReactionCount>? reactions { get; set; }
        public int comment_count { get; set; }
        public int share_count { get; set; }
        
        public class ImageItem
        {
            public int id { get; set; }
            public string image_url { get; set; }
            public int image_order { get; set; }
        }
        
        public class ReactionCount
        {
            public int reaction_type { get; set; }
            public int count { get; set; }
        }
    }
}

