namespace E_Model.News
{
    public class data_news : modify_info
    {
        public int id { get; set; }
        public string user_code { get; set; }
        public string? title { get; set; }
        public string? contents { get; set; }
        public string? status { get; set; } // Cảm xúc: vui vẻ, hạnh phúc, buồn, etc.
        public string? location { get; set; } // Check-in location
        public int privacy_level { get; set; } = 1; // 1: Public, 2: Friends, 3: Private
        public bool is_pinned { get; set; } = false; // Ghim bài viết
    }
}

