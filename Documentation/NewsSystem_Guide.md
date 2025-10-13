# Hệ thống Bảng tin (News/Social Feed System)

## Tổng quan
Hệ thống bảng tin giống Facebook được xây dựng hoàn chỉnh với các tính năng:
- Đăng bài viết (text, hình ảnh, cảm xúc, check-in)
- Tương tác (like, love, haha, wow, sad, angry)
- Bình luận và phản hồi
- Chia sẻ bài viết
- Gắn thẻ người dùng
- Phân trang và tải thêm

## Cấu trúc Database

### Bảng chính

#### 1. `data_news` - Bài viết
```sql
- id: ID bài viết
- user_code: Mã người đăng
- title: Tiêu đề (nullable)
- contents: Nội dung bài viết
- status: Cảm xúc (vui vẻ, buồn, hạnh phúc...)
- location: Địa điểm check-in
- privacy_level: 1=Public, 2=Friends, 3=Private
- is_pinned: Ghim bài viết
- created_at, updated_at, is_deleted
```

#### 2. `data_news_image` - Hình ảnh bài viết
```sql
- id, news_id, image_url, image_order
```

#### 3. `data_news_tag` - Gắn thẻ người dùng
```sql
- id, news_id, user_code
```

#### 4. `data_news_reaction` - Cảm xúc bài viết
```sql
- id, news_id, user_code, reaction_type
- reaction_type: 1=Like, 2=Love, 3=Haha, 4=Wow, 5=Sad, 6=Angry
```

#### 5. `data_news_comment` - Bình luận
```sql
- id, news_id, parent_id (NULL=comment gốc), user_code, content
```

#### 6. `data_news_comment_image` - Hình ảnh bình luận
```sql
- id, comment_id, image_url
```

#### 7. `data_news_comment_reaction` - Cảm xúc bình luận
```sql
- id, comment_id, user_code, reaction_type
```

#### 8. `data_news_share` - Chia sẻ bài viết
```sql
- id, news_id, user_code, share_content
```

## Cài đặt Database

### Bước 1: Tạo bảng
Chạy script: `Database/StoredProcedures/01_CreateTables_News.sql`

### Bước 2: Tạo Stored Procedures
Chạy script: `Database/StoredProcedures/02_SP_News_CRUD.sql`

## API Endpoints

### Base URL: `/api/News`

#### 1. Lấy danh sách bài viết
```
GET /api/News/GetNewsList
Query Parameters:
  - user_code: string (optional) - Lọc theo người đăng
  - page_index: int (default: 1)
  - page_size: int (default: 10)

Response:
{
  "isSuccess": true,
  "message": "Lấy danh sách bài viết thành công",
  "data": [...]
}
```

#### 2. Tạo bài viết mới
```
POST /api/News/Create
Content-Type: multipart/form-data

Body (FormData):
  - contents: string (required)
  - status: string (optional) - Cảm xúc
  - location: string (optional)
  - privacy_level: int (default: 1)
  - images: File[] (optional)
  - tagged_users: string[] (optional)

Response:
{
  "isSuccess": true,
  "message": "Tạo bài viết thành công",
  "data": { "news_id": 123 }
}
```

#### 3. Cập nhật bài viết
```
PUT /api/News/Update/{newsId}
Content-Type: multipart/form-data

Body: Similar to Create
```

#### 4. Xóa bài viết
```
DELETE /api/News/Delete/{newsId}
```

#### 5. Thêm/Cập nhật cảm xúc
```
POST /api/News/Reaction
Content-Type: application/json

Body:
{
  "news_id": 123,
  "reaction_type": 1  // 1=Like, 2=Love, 3=Haha, 4=Wow, 5=Sad, 6=Angry
}
```

#### 6. Xóa cảm xúc
```
DELETE /api/News/Reaction/{newsId}
```

#### 7. Thêm bình luận
```
POST /api/News/Comment
Content-Type: multipart/form-data

Body:
  - news_id: int
  - content: string
  - parent_id: int (optional) - Cho reply
  - images: File[] (optional)
```

#### 8. Lấy danh sách bình luận
```
GET /api/News/Comments/{newsId}
```

#### 9. Chia sẻ bài viết
```
POST /api/News/Share/{newsId}
Content-Type: application/json

Body: "Nội dung chia sẻ" (optional)
```

## Giao diện (Frontend)

### Modal Tạo bài viết
File: `E_Portfolio/Views/Portal/Index.cshtml`

Features:
- Textarea nhập nội dung
- Upload nhiều hình ảnh
- Chọn cảm xúc (Vui vẻ, Hạnh phúc, Buồn...)
- Preview hình ảnh trước khi đăng
- Gắn thẻ người dùng
- Check-in địa điểm

### Hiển thị bài viết
File: `E_Portfolio/wwwroot/root/news/newsPortal.js`

Features:
- Layout giống Facebook
- Hiển thị hình ảnh linh hoạt:
  - 1 ảnh: Full width
  - 2 ảnh: Chia đôi
  - 3 ảnh: 1 lớn + 2 nhỏ
  - 4+ ảnh: Grid 2x2 với overlay "+X"
- Nút Like, Comment, Share
- Hiển thị top 2 bình luận có lượng tương tác cao nhất
- Form nhập bình luận nhanh
- Load more posts

## Cấu trúc Code

### Models
```
E_Model/News/
├── data_news.cs
├── data_news_image.cs
├── data_news_tag.cs
├── data_news_reaction.cs
├── data_news_comment.cs
├── data_news_comment_image.cs
├── data_news_comment_reaction.cs
└── data_news_share.cs

E_Model/Request/News/
├── NewsRequest.cs
├── CommentRequest.cs
└── ReactionRequest.cs

E_Model/Response/News/
├── NewsResponse.cs
└── CommentResponse.cs
```

### Repository
```
E_Contract/Repository/News/
└── IDataNewsRepository.cs

E_Repository/News/
└── DataNewsRepository.cs
```

### Service
```
E_Contract/Service/News/
└── IDataNewsService.cs

E_Service/News/
└── DataNewsService.cs
```

### Controller
```
E_API/Controllers/News/
└── NewsController.cs
```

## Cách sử dụng

### 1. Frontend - Tạo bài viết
```javascript
// Mở modal
$('#kt_social_feeds_post_trigger').click();

// Form tự động xử lý:
// - Upload ảnh
// - Chọn cảm xúc
// - Submit qua API
```

### 2. Backend - Xử lý logic
```csharp
// Controller
var result = await _serviceWrapper.News.CreateNewsAsync(request, userId);

// Service
- Validate dữ liệu
- Upload ảnh lên server
- Gọi Repository để lưu DB

// Repository
- Insert vào data_news
- Insert images vào data_news_image
- Insert tags vào data_news_tag
```

### 3. Tích hợp Authentication
API yêu cầu Authorization header:
```javascript
headers: {
    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
}
```

## Mở rộng

### Thêm chức năng Edit bài viết
1. Thêm button "Chỉnh sửa" trong dropdown menu
2. Call API `PUT /api/News/Update/{newsId}`
3. Reload danh sách

### Thêm chức năng Report
1. Tạo bảng `data_news_report`
2. Thêm SP `data_news_report_insert`
3. Thêm method trong Service/Controller
4. Thêm button "Báo cáo" trong UI

### Real-time notifications
1. Tích hợp SignalR
2. Thông báo khi có:
   - Bình luận mới
   - Like mới
   - Mention/Tag

## Notes

- Tất cả API đều yêu cầu authentication
- Upload ảnh lưu vào `wwwroot/News/`
- Soft delete: set `is_deleted = 1`
- Pagination mặc định: 10 items/page
- Reaction: mỗi user chỉ 1 reaction/post (update nếu đã tồn tại)

## Troubleshooting

### API trả về 401 Unauthorized
- Kiểm tra token trong localStorage
- Verify token còn hạn chưa

### Không upload được ảnh
- Kiểm tra thư mục `wwwroot/News/` có quyền write
- Kiểm tra file size limit trong `appsettings.json`

### Không load được bài viết
- Kiểm tra database connection
- Verify stored procedures đã được tạo
- Check console log lỗi

## License
Internal use only - E_Portfolio Project

