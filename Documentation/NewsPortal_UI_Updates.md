# Cập nhật giao diện News Portal

## 📋 Tổng quan thay đổi

### 1. Sidebar bên phải đã được thiết kế lại

#### ✅ Những gì đã thay đổi:
- **Loại bỏ**: "Suggestions for you" và "Stay Connected" cũ không hợp lý
- **Thêm mới 3 widget hữu ích hơn**:

#### 🎂 Widget "Sinh nhật hôm nay"
- Hiển thị danh sách bạn bè có sinh nhật hôm nay
- Icon calendar với màu primary
- Hiển thị avatar và lời chúc mừng
- Hữu ích để tạo sự kết nối và tương tác

#### 📊 Widget "Xu hướng"  
- Hiển thị các hashtag đang trending
- Icon chart với màu danger (red)
- Hiển thị số lượng bài viết cho mỗi hashtag
- Ví dụ: #TeamBuilding2024, #ProjectSuccess, #InnovationDay, #CompanyCulture

#### 🟢 Widget "Đang hoạt động"
- Danh sách người dùng đang online
- Status badge với màu sắc:
  - 🟢 Xanh lá: Đang hoạt động
  - 🟡 Vàng: Rảnh rỗi  
  - ⚪ Xám: Offline (hiển thị thời gian cuối)
- Icon profile với màu success

### 2. Chức năng xem chi tiết bài viết (như Facebook)

#### ✅ Tính năng mới:
- **Click vào bài viết để xem chi tiết** - Click vào phần nội dung (body) của bất kỳ bài viết nào
- **Modal hiển thị đầy đủ**:
  - Header: Thông tin người đăng, thời gian, cảm xúc
  - Body: Nội dung và hình ảnh full size
  - Footer:
    - Thống kê like, comment, share
    - Nút action (Thích, Bình luận, Chia sẻ)
    - **Hiển thị TẤT CẢ bình luận** (không giới hạn 2 như feed chính)
    - Form để thêm bình luận mới

#### 🎯 UX Improvements:
- Hover vào post body sẽ thấy con trỏ chuột đổi thành pointer (chỉ tay)
- Modal có thể scroll để xem nội dung dài
- Modal width: 800px, responsive
- Close button dễ nhìn thấy ở góc trên phải

## 🔧 Technical Implementation

### Files đã cập nhật:

1. **`E_Portfolio/Views/Portal/Index.cshtml`**
   - Thêm 3 widget mới cho sidebar
   - Thêm modal `#kt_modal_view_post` để xem chi tiết

2. **`E_Portfolio/wwwroot/root/news/newsPortal.js`**
   - Thêm class `view-post-detail` cho post body
   - Thêm `data-news-id` attribute
   - Thêm hàm `viewPostDetail(newsId)` - mở modal với chi tiết
   - Thêm hàm `renderAllComments(comments, newsId)` - render tất cả comments
   - Bind click event cho `.view-post-detail` elements

### Cách sử dụng:

```javascript
// Tự động được khởi tạo khi render news
// User chỉ cần click vào nội dung bài viết

// Trong renderNews():
$('.view-post-detail').on('click', function(e) {
    e.preventDefault();
    const newsId = $(this).data('news-id');
    viewPostDetail(newsId);
});
```

## 🎨 Design Guidelines

### Sidebar Style:
- Card padding: `p-5`
- Icon size: `fs-2x`
- Title: `fs-5 fw-bold`
- User avatar: `symbol-35px`
- Status badge circle: `h-8px w-8px`
- Separator: `separator-dashed`

### Modal Style:
- Width: `mw-800px`
- Scrollable: `modal-dialog-scrollable`
- Card flush: `card-flush`
- Padding horizontal: `px-7`
- Comment background: `bg-light-primary`

## 📱 Responsive

- Sidebar collapse trên mobile (< 992px)
- Modal full width trên mobile
- Touch-friendly button sizes
- Optimized for all screen sizes

## 🚀 Next Steps (Tương lai)

1. **Kết nối API thật**:
   - Lấy danh sách sinh nhật từ database
   - Lấy trending hashtags từ analytics
   - Lấy online users từ SignalR/WebSocket

2. **Tính năng bổ sung**:
   - Realtime updates cho online status
   - Lazy load comments khi scroll
   - Rich text editor cho comment
   - Upload ảnh trong comment
   - React với nhiều loại cảm xúc (like, love, haha, wow, sad, angry)

3. **Performance**:
   - Virtual scroll cho comment list dài
   - Image lazy loading
   - Debounce cho comment input

## 🎯 User Experience Flow

```
1. User vào trang Portal
   ↓
2. Nhìn sidebar bên phải thấy:
   - Ai có sinh nhật hôm nay
   - Hashtag nào đang hot
   - Bạn bè nào đang online
   ↓
3. Đọc feed và thấy bài viết thú vị
   ↓
4. Click vào bài viết
   ↓
5. Modal mở ra hiển thị chi tiết
   - Đọc toàn bộ nội dung
   - Xem tất cả comments
   - Có thể tương tác (like, comment, share)
   ↓
6. Đóng modal hoặc tiếp tục xem feed
```

## ✨ Key Benefits

1. **Sidebar hữu ích hơn**: 
   - Thông tin có giá trị thực tế
   - Tạo engagement cao hơn
   - Personalized content

2. **Detail view như Facebook**:
   - Familiar UX pattern
   - Tập trung vào 1 bài viết
   - Xem đầy đủ thông tin
   - Không bị distract bởi feed khác

3. **Cải thiện tương tác**:
   - Dễ dàng comment nhiều hơn
   - Theo dõi discussion tốt hơn
   - UX mượt mà, tự nhiên

## 📝 Notes

- Hiện tại sidebar widgets sử dụng **fake data**, cần kết nối API để có dữ liệu thật
- Modal hiển thị tất cả comments từ fake data, trong production cần pagination
- Click event chỉ hoạt động trên post body, không ảnh hưởng đến các button actions khác

