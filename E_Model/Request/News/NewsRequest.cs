namespace E_Model.Request.News
{
    public class NewsRequest : PaginationRequest
    {
        public string? user_code { get; set; }
        public string? title { get; set; }
        public string? contents { get; set; }
        public string? status { get; set; }
        public string? location { get; set; }
        public int privacy_level { get; set; } = 1;
        public List<string>? tagged_users { get; set; } // Danh sách user được gắn thẻ
        public List<Microsoft.AspNetCore.Http.IFormFile>? images { get; set; } // Upload files
    }
}

