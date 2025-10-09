// =====================================================
// VÍ DỤ SỬ DỤNG PERMISSIONAUTHORIZE ATTRIBUTE
// =====================================================

using E_API.Filter;
using Microsoft.AspNetCore.Mvc;

namespace E_API.Controllers.Examples
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        // 1️⃣ XEM DANH SÁCH USER - Function ID: 1
        [Authorize]
        [PermissionAuthorize(1)] // Chỉ cần kiểm tra function_id = 1
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            // Logic lấy danh sách user
            return Ok(new { users = "danh sách tất cả users" });
        }

        // 2️⃣ TẠO USER MỚI - Function ID: 2
        [Authorize]
        [PermissionAuthorize(2)] // Chỉ cần kiểm tra function_id = 2
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            // Logic tạo user mới
            return Ok(new { message = "User created successfully" });
        }

        // 3️⃣ CẬP NHẬT USER - Function ID: 3
        [Authorize]
        [PermissionAuthorize(3)] // Chỉ cần kiểm tra function_id = 3
        [HttpPut("users/{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserRequest request)
        {
            // Logic cập nhật user
            return Ok(new { message = $"User {userId} updated successfully" });
        }

        // 4️⃣ VÔ HIỆU HÓA USER - Function ID: 4
        [Authorize]
        [PermissionAuthorize(4)] // Chỉ cần kiểm tra function_id = 4
        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DisableUser(string userId)
        {
            // Logic vô hiệu hóa user
            return Ok(new { message = $"User {userId} disabled successfully" });
        }

        // 5️⃣ IMPORT EXCEL - Function ID: 5
        [Authorize]
        [PermissionAuthorize(5)] // Chỉ cần kiểm tra function_id = 5
        [HttpPost("users/import-excel")]
        public async Task<IActionResult> ImportUsersFromExcel(IFormFile file)
        {
            // Logic import từ Excel
            return Ok(new { message = "Users imported successfully" });
        }

        // 6️⃣ EXPORT EXCEL - Function ID: 6
        [Authorize]
        [PermissionAuthorize(6)] // Chỉ cần kiểm tra function_id = 6
        [HttpPost("users/export-excel")]
        public async Task<IActionResult> ExportUsersToExcel()
        {
            // Logic export ra Excel
            return Ok(new { downloadUrl = "/files/users-export.xlsx" });
        }
    }

    // Ví dụ với Worksheet Management (Menu khác)
    [Route("api/[controller]")]
    [ApiController]
    public class WorkSheetController : ControllerBase
    {
        // Xem worksheet - có thể là function_id khác trong menu worksheet
        [Authorize]
        [PermissionAuthorize(10)] // Giả sử function_id = 10 cho worksheet
        [HttpGet("worksheets")]
        public async Task<IActionResult> GetWorkSheets()
        {
            return Ok(new { worksheets = "danh sách worksheets" });
        }

        // Tạo worksheet mới
        [Authorize]
        [PermissionAuthorize(11)] // function_id = 11
        [HttpPost("worksheets")]
        public async Task<IActionResult> CreateWorkSheet()
        {
            return Ok(new { message = "WorkSheet created" });
        }
    }
}

/*
📋 CẤU TRÚC PHÂN QUYỀN TRONG DATABASE:

user_code | role_id | menu_id | function_id | function_name
----------|---------|---------|-------------|--------------
NV001     | 1       | 1       | 1           | Xem danh sách user
NV001     | 1       | 1       | 2           | Tạo user mới
NV001     | 1       | 1       | 3           | Cập nhật user
NV002     | 2       | 1       | 1           | Chỉ xem danh sách user
NV002     | 2       | 2       | 10          | Xem worksheet

🔑 JWT TOKEN SẼ CHỨA:
{
  "permission": [
    "perm_1_1",  // Có quyền xem user
    "perm_1_2",  // Có quyền tạo user
    "perm_1_3"   // Có quyền cập nhật user
  ]
}

⚡ KIỂM TRA QUYỀN:
- [PermissionAuthorize(1)] → Kiểm tra có "perm_*_1" trong token không?
- [PermissionAuthorize(2)] → Kiểm tra có "perm_*_2" trong token không?
- Nếu có → Thực hiện action
- Nếu không → Trả về 401 Unauthorized

🚀 PERFORMANCE:
- Kiểm tra từ JWT token: ~1ms
- Không cần truy vấn database mỗi lần
- Scale tốt với nhiều user đồng thời
*/
