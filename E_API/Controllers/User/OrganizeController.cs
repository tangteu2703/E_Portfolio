using E_API.Filter;
using E_Model.Request.User;
using E_Model.Response.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_API.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizeController : BaseController
    {

        [LoginAuthorize]
        [HttpPost("Select-Organize-Filter")]
        public async Task<IActionResult> GetOrganize(OrganizeRequest request)
        {
            try
            {
                var listData = new List<OrganizeResponse>
                {
                    new() { user_id = 1,  user_code = "NV001" ,full_name = "Nguyễn Kim Đảng", title = "Trưởng phòng IT", position = "Management", section = "Administration", dept = "IT", group = "Policy", avatar = "https://i.pravatar.cc/60?img=1", email = "ndang@company.vn", phone = "0901 555 001" },
                    new() { user_id = 2,  user_code = "NV002" ,full_name = "Nguyễn Thái Học", title = "Trưởng nhóm Hạ tầng", position = "Assistant Management", dept = "IT", section = "Infrastructure", group = "Hardware", avatar = "https://i.pravatar.cc/60?img=2", email = "nhoc@company.vn", phone = "0901 555 002", parent_id = 1 },
                    new() { user_id = 3,  user_code = "NV003" ,full_name = "Nguyễn Thị Thu Hiền", title = "Trưởng nhóm Ứng dụng", position = "Assistant Management", dept = "IT", section = "Software", group = "Application", avatar = "https://i.pravatar.cc/60?img=3", email = "nhien@company.vn", phone = "0901 555 003", parent_id = 1 },
                    
                    new() { user_id = 4,  user_code = "NV004" ,full_name = "Nguyễn Duy Thanh", title = "Supervior", position = "Senior Staff", dept = "IT", section = "Infrastructure", group = "Hardware", avatar = "https://i.pravatar.cc/60?img=4", email = "plong@company.vn", phone = "0901 555 004", parent_id = 2 },
                    new() { user_id = 5,  user_code = "NV005" ,full_name = "Hoàng Văn Lam", title = "Staff", position = "Staff", dept = "IT", section = "Infrastructure", group = "Hardware", avatar = "https://i.pravatar.cc/60?img=5", email = "thuy@company.vn", phone = "0901 555 005", parent_id = 4 },
                    new() { user_id = 6,  user_code = "NV006" ,full_name = "Trần Văn Hiếu", title = "Staff", position = "Staff", dept = "IT", section = "Infrastructure", group = "Hardware", avatar = "https://i.pravatar.cc/60?img=6", email = "thuy@company.vn", phone = "0901 555 005", parent_id = 4 },
                    new() { user_id = 7,  user_code = "NV007" ,full_name = "Đỗ Ngọc Duy", title = "Staff", position = "Staff", dept = "IT", section = "Infrastructure", group = "Hardware", avatar = "https://i.pravatar.cc/60?img=7", email = "thuy@company.vn", phone = "0901 555 005", parent_id = 4 },
                    new() { user_id = 8,  user_code = "NV008" ,full_name = "Nguyễn Phương Nam", title = "Staff", position = "Staff", dept = "IT", section = "Infrastructure", group = "Hardware", avatar = "https://i.pravatar.cc/60?img=8", email = "thuy@company.vn", phone = "0901 555 005", parent_id = 4 },

                    new() { user_id = 9,  user_code = "NV009" , full_name = "Vũ Hữu Tuấn", title = "Supervior", position = "Senior Staff", dept = "IT", section = "Application", group = "Application", avatar = "https://i.pravatar.cc/60?img=9", email = "thuy@company.vn", phone = "0901 555 005", parent_id = 3 },
                    new() { user_id = 10, user_code = "NV0010" , full_name = "Mai Tiên Tiến", title = "Staff", position = "Staff", dept = "IT", section = "Application", group = "Application", avatar = "https://i.pravatar.cc/60?img=10", email = "thuy@company.vn", phone = "0901 555 005", parent_id = 9 },
                    new() { user_id = 11, user_code = "NV0011" , full_name = "Trịnh Văn Lộc", title = "Staff", position = "Staff", dept = "IT", section = "Application", group = "Application", avatar = "https://i.pravatar.cc/60?img=11", email = "thuy@company.vn", phone = "0901 555 005", parent_id = 9 },
                    new() { user_id = 19, user_code = "NV0015" , full_name = "Nguyễn Hữu Phúc", title = "Staff", position = "Senior Staff", dept = "IT", section = "Application", group = "Application", avatar = "https://i.pravatar.cc/60?img=15", email = "thuy@company.vn", phone = "0901 555 005", parent_id = 9 },
                    new() { user_id = 19, user_code = "NV0019" , full_name = "Nguyễn Cư Sơn", title = "Staff", position = "Staff", dept = "IT", section = "Application", group = "Application", avatar = "https://i.pravatar.cc/60?img=15", email = "thuy@company.vn", phone = "0901 555 005", parent_id = 9 },

                    new() { user_id = 12, user_code = "NV001" , full_name = "Nguyễn Kim Đảng", title = "Trưởng nhóm Policy", position = "Staff", dept = "IT", section = "Policy", group = "Policy", avatar = "https://i.pravatar.cc/60?img=12", email = "thuy@company.vn", phone = "0901 555 005", parent_id = 1 },
                    new() { user_id = 13, user_code = "NV0013" , full_name = "Đào Duy Hợp", title = "Advisor", position = "Staff", dept = "IT", section = "Policy", group = "Policy", avatar = "https://i.pravatar.cc/60?img=13", email = "thuy@company.vn", phone = "0901 555 005", parent_id = 12 },
                    new() { user_id = 14, user_code = "NV0014" , full_name = "Trần Đình Khiết", title = "Staff", position = "Staff", dept = "IT", section = "Policy", group = "Policy", avatar = "https://i.pravatar.cc/60?img=14", email = "thuy@company.vn", phone = "0901 555 005", parent_id = 13 },

                    new() { user_id = 15, user_code = "NV0015" , full_name = "Nguyễn Văn Tuyên", title = "Trưởng nhóm System", position = "Supervior", dept = "IT", section = "System", group = "System", avatar = "https://i.pravatar.cc/60?img=15", email = "thuy@company.vn", phone = "0901 555 005", parent_id = 3 },
                    new() { user_id = 16, user_code = "NV0016" , full_name = "Nguyễn Ngọc Sang", title = "Staff", position = "Staff", dept = "IT", section = "System", group = "System", avatar = "https://i.pravatar.cc/60?img=16", email = "thuy@company.vn", phone = "0901 555 005", parent_id = 15 },
                    new() { user_id = 17, user_code = "NV0017" , full_name = "Nguyễn Duy Khánh", title = "Staff", position = "Staff", dept = "IT", section = "System", group = "System", avatar = "https://i.pravatar.cc/60?img=17", email = "thuy@company.vn", phone = "0901 555 005", parent_id = 15 },
                };

                await Task.Delay(100); // giả lập async
                return OK(listData);
            }
            catch (Exception ex)
            {
                // log lỗi lại nếu có ILogger
                return InternalServerError($"Internal server error: {ex.Message}", ex);
            }
        }
    }
}
