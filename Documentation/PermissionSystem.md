# Hệ thống phân quyền trong dự án

## Tổng quan
Dự án sử dụng hệ thống phân quyền dựa trên **Function** để kiểm soát quyền truy cập của user vào các API endpoints. **Quyền được kiểm tra từ database** dựa trên user code từ JWT token để đảm bảo tính chính xác và dễ quản lý.

## Cấu trúc phân quyền
Mỗi user có thể được phân quyền theo dạng:
- `user_code`: Mã định danh của user
- `role_id`: ID của role (vai trò)
- `menu_id`: ID của menu/chức năng
- `function_id`: ID của chức năng cụ thể

**Cấu trúc Menu Functions:**
| menu_id | function_id | Function Name | API Endpoint | Description |
|---------|-------------|---------------|--------------|-------------|
| 1 | 1 | Tạo mới | /user-create | Tạo tài khoản |
| 1 | 2 | Cập nhật | /user-update | Cập nhật |
| 1 | 3 | Vô hiệu hóa | /user-delete | Vô hiệu hóa |
| 1 | 4 | Import excel | /user-import-excel | Import excel |
| 1 | 5 | Export excel | /user-export-excel | Export excel |

## Cách hoạt động
- **JWT Token chỉ chứa thông tin cơ bản**: user_code, email, role (không chứa quyền)
- **Kiểm tra quyền từ database**: Mỗi API call đều truy vấn database để kiểm tra quyền dựa trên user_code
- **Client tự động đính kèm token**: JavaScript apiHelper tự động thêm Bearer token vào mỗi request

## Cách sử dụng

### 1. JWT Token chỉ chứa thông tin cơ bản
Khi đăng nhập, JWT token chỉ chứa thông tin cơ bản:
```csharp
// JWT token chỉ chứa: user_code, email, role (không chứa quyền)
var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user?.user_code ?? ""),
    new Claim(ClaimTypes.Name, user?.email ?? ""),
    new Claim(ClaimTypes.Role, user?.role_name ?? ""),
};
```

### 2. Kiểm tra quyền từ Database
```csharp
// Trong TokenService - kiểm tra từ database dựa trên user_code từ JWT token
public async Task<bool> CheckCurrentUserPermissionAsync(ClaimsPrincipal user, int menuId, int functionId)
{
    var userCode = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
    return await CheckUserPermissionFromDatabaseAsync(userCode, menuId, functionId);
}

// Với chỉ function_id (không cần menu_id)
public async Task<bool> CheckCurrentUserPermissionAsync(ClaimsPrincipal user, int functionId)
{
    var userCode = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
    var permissions = await _repositoryWrapper.Menu.SelectMenuPermissionsByUserAsync(userCode);
    return permissions.Any(p => p.function_id == functionId);
}
```

### 2. Sử dụng Attribute kiểm tra quyền
```csharp
[Authorize]                    // Xác thực JWT token
[PermissionAuthorize(1)]       // Kiểm tra quyền xem thông tin user
[HttpPost("Select-Account-By-Code")]
public async Task<IActionResult> GetAccount()
{
    // Logic thực hiện nếu user có quyền
    // Quyền được kiểm tra từ JWT token (siêu nhanh!)
}
```

**⚠️ Lưu ý quan trọng:**
- `[Authorize]` phải được đặt **trước** `[PermissionAuthorize(functionId)]`
- Thứ tự thực hiện: `[Authorize]` → `[PermissionAuthorize]` → Logic nghiệp vụ
- `PermissionAuthorize` sử dụng `TypeFilterAttribute` để hỗ trợ dependency injection

**Các ví dụ sử dụng:**

```csharp
// Xem danh sách user
[Authorize]
[PermissionAuthorize(1)] // Function 1: Xem danh sách
[HttpGet("SelectAll")]

// Tạo user mới
[Authorize]
[PermissionAuthorize(2)] // Function 2: Tạo mới
[HttpPost("Create-User")]

// Cập nhật user
[Authorize]
[PermissionAuthorize(3)] // Function 3: Cập nhật
[HttpPut("Update-User")]

// Xóa user
[Authorize]
[PermissionAuthorize(4)] // Function 4: Vô hiệu hóa
[HttpDelete("Delete-User")]

// Import Excel
[Authorize]
[PermissionAuthorize(5)] // Function 5: Import Excel
[HttpPost("Import-Excel")]
```

**Logic bên trong PermissionAuthorize:**
1. **TypeFilterAttribute** tự động tạo instance của `PermissionAuthorizeFilter`
2. **Dependency Injection** inject `IServiceWrapper` vào filter
3. Kiểm tra từ JWT token (nhanh)
4. Nếu không tìm thấy → Kiểm tra từ database (fallback)
5. Trả về 401 nếu không có quyền

### 3. Function ID Mapping

| Function ID | Function Name | Description | Ví dụ sử dụng |
|-------------|---------------|-------------|---------------|
| `1` | Xem | View/Display data | `[PermissionAuthorize(1)]` |
| `2` | Tạo mới | Create new records | `[PermissionAuthorize(2)]` |
| `3` | Cập nhật | Update existing data | `[PermissionAuthorize(3)]` |
| `4` | Vô hiệu hóa | Disable/Delete records | `[PermissionAuthorize(4)]` |
| `5` | Import Excel | Import data from Excel | `[PermissionAuthorize(5)]` |
| `6` | Export Excel | Export data to Excel | `[PermissionAuthorize(6)]` |

## Các thành phần đã tạo

### 1. TokenService Methods
- `CheckUserPermissionFromDatabaseAsync(string userCode, int menuId, int functionId)`: Kiểm tra quyền từ database
- `CheckCurrentUserPermissionFromToken(ClaimsPrincipal user, int functionId)`: Kiểm tra quyền từ JWT token
- `CheckCurrentUserPermissionAsync(ClaimsPrincipal user, int functionId)`: Kiểm tra quyền với fallback

### 2. Custom Attributes
- `PermissionAuthorizeAttribute`: Attribute chính để kiểm tra quyền (chỉ cần function_id)
- `CheckPermissionAttribute`: Attribute legacy (giữ tương thích ngược với menu_id + function_id)

### 3. Ví dụ sử dụng trong Controllers

#### AccountController
```csharp
[Authorize]                    // Xác thực JWT token
[PermissionAuthorize(1)]       // Kiểm tra quyền xem thông tin user
[HttpPost("Select-Account-By-Code")]
public async Task<IActionResult> GetAccount()
{
    // Chỉ user có quyền xem thông tin user mới truy cập được
}
```

#### UserController
```csharp
[Authorize]                    // Xác thực JWT token
[PermissionAuthorize(1)]       // Function xem danh sách user (1)
[HttpGet("SelectAll")]
public async Task<IActionResult> GetUserData([FromQuery] UserRequest request)

[Authorize]                    // Xác thực JWT token
[PermissionAuthorize(2)]       // Function tạo user mới (2)
[HttpPost("Create-User")]
public async Task<IActionResult> CreateUser([FromBody] UserRequest request)

[Authorize]                    // Xác thực JWT token
[PermissionAuthorize(3)]       // Function cập nhật user (3)
[HttpPut("Update-User")]
public async Task<IActionResult> UpdateUser([FromBody] UserRequest request)

[Authorize]                    // Xác thực JWT token
[PermissionAuthorize(4)]       // Function vô hiệu hóa user (4)
[HttpDelete("Delete-User")]
public async Task<IActionResult> DeleteUser(string userCode)

[Authorize]                    // Xác thực JWT token
[PermissionAuthorize(5)]       // Function import Excel (5)
[HttpPost("Import-Excel")]
public async Task<IActionResult> ImportExcel(IFormFile file)
```

## Thứ tự thực hiện kiểm tra quyền

1. **Client gửi request** với Bearer token trong header
2. **[Authorize]**: Kiểm tra JWT token hợp lệ (authentication)
3. **[PermissionAuthorize(functionId)]**:
   - TypeFilterAttribute tạo instance PermissionAuthorizeFilter
   - Dependency Injection inject IServiceWrapper
   - Lấy user_code từ JWT token
   - Truy vấn database để kiểm tra quyền dựa trên user_code
4. **Logic nghiệp vụ**: Thực hiện chức năng nếu có quyền
5. **Trả response** về client

**⚠️ Thứ tự attribute rất quan trọng:**
```csharp
[Authorize]                    // Phải đặt trước - xác thực token
[PermissionAuthorize(1)]       // Kiểm tra quyền sau khi xác thực
[HttpPost("api/users")]
public async Task<IActionResult> GetUsers()
{
// Logic nghiệp vụ chỉ thực hiện nếu có quyền
}
```

## Ưu điểm của giải pháp

- **🎯 Chính xác cao**: Quyền được kiểm tra từ database nguồn gốc
- **🔒 Bảo mật tốt**: Không lưu quyền nhạy cảm trong JWT token
- **🛠️ Dễ quản lý**: Quyền được quản lý tập trung trong database
- **🔄 Linh hoạt**: Có thể thay đổi quyền mà không cần đăng nhập lại
- **📊 Transparent**: Client không biết về quyền, chỉ biết có quyền hay không

## Cách phân quyền cho user

Để phân quyền cho user, bạn cần:
1. Xác định `menu_id` và `function_id` cho chức năng cần phân quyền
2. Thêm bản ghi vào bảng phân quyền với thông tin user, role, menu, function tương ứng

Ví dụ phân quyền xem thông tin user:
```sql
INSERT INTO user_permissions (user_code, role_id, menu_id, function_id)
VALUES ('NV001', 1, 1, 1); -- User NV001 có quyền xem thông tin user
```

**⚠️ Lưu ý quan trọng**: Sau khi phân quyền, user **không cần đăng nhập lại** vì quyền được kiểm tra từ database dựa trên user_code từ JWT token hiện tại.

## Ví dụ thực tế

**Cách hoạt động:**
```
Client Request → [Authorize] → [PermissionAuthorize(1)] → Query Database → Trả kết quả
Thời gian: ~10-50ms (tùy database load và cache)
```

**Ví dụ JWT token gọn nhẹ:**
```json
{
  "sub": "NV001",
  "name": "nguyen.van.a@company.com",
  "role": "Nhân viên",
  "iat": 1704067200,
  "exp": 1704070800
}
```

**Cách kiểm tra quyền:**
```csharp
// Kiểm tra có quyền xem user không?
[Authorize]                    // Xác thực token
[PermissionAuthorize(1)]       // Kiểm tra quyền từ database dựa trên user_code

// Kiểm tra có quyền tạo user không?
[Authorize]
[PermissionAuthorize(2)]       // Kiểm tra quyền từ database dựa trên user_code

// Kiểm tra có quyền export worksheet không?
[Authorize]
[PermissionAuthorize(5)]       // Kiểm tra quyền từ database dựa trên user_code
```

**JavaScript Client tự động đính kèm token:**
```javascript
async function sendRequest({
    url,
    method = 'GET',
    data = null,
    success,
    error,
    isAddToken = true,
    isFormData = false,
    isBlob = false,
}) {
    // Tự động lấy token từ localStorage và đính kèm vào header
    const token = isAddToken ? await getApiToken(url) : localStorage.getItem("e_atoken");

    $.ajax({
        beforeSend: function (xhr) {
            if (token) {
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            }
        },
        // ... rest of config
    });
}
```

## Lưu ý
- Nếu không có quyền, hệ thống sẽ trả về HTTP 401 Unauthorized
- Thông báo lỗi sẽ hiển thị rõ ràng về quyền bị thiếu
- Có thể mở rộng để hỗ trợ nhiều quyền phức tạp hơn nếu cần
