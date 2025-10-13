namespace E_Model.Request.News
{
    public class CommentRequest
    {
        public int news_id { get; set; }
        public int? parent_id { get; set; }
        public string user_code { get; set; }
        public string content { get; set; }
        public List<Microsoft.AspNetCore.Http.IFormFile>? images { get; set; }
    }
}

