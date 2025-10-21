using E_Model.News;

namespace E_Model.Response.News
{
    public class NewsResponse
    {
        public int? news_id { get; set; }
        public string? user_code { get; set; }
        public string? user_name { get; set; }
        public string? user_avatar { get; set; }
        public string? title { get; set; }
        public string? contents { get; set; }
        public string? status { get; set; }
        public string? location { get; set; }
        public bool is_pinned { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        
        // Aggregated data
        public List<string>? images { get; set; }
        public List<string>? tagged_users { get; set; }
        public List<Reaction>? reactions { get; set; }
        public List<Comment>? comments { get; set; }
        public List<SharedInfo>? shares { get; set; }
        
        public class ImageItem
        {
            public int id { get; set; }
            public string? image_url { get; set; }
            public int image_order { get; set; }
        }
        
        public class Reaction
        {
            public int reaction_type { get; set; }
            public int count { get; set; }
        }
        public class Comment
        {
            public int? comment_id { get; set; }
            public int? parent_id { get; set; }
            public string? user_code { get; set; }
            public string? contents { get; set; }
            public DateTime? enter_time { get; set; }
            public List<string>? images { get; set; }
            public List<Reaction>? reactions { get; set; }
        }

        public class SharedInfo
        {
            public string? user_code { get; set; }
            public DateTime? enter_time { get; set; }
        }
    }
}

