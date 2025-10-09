using E_API.Filter;
using E_Contract.Service;
using E_Model.Request;
using E_Model.Request.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_API.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IServiceWrapper _serviceWrapper;
        public UserController(IServiceWrapper serviceWrapper)
        {
            _serviceWrapper = serviceWrapper;
        }
        [Authorize]
        [PermissionAuthorize(1)] // Function xem danh sách user (1)
        [HttpGet("SelectAll")]
        public async Task<IActionResult> GetUserData([FromQuery] UserRequest request)
        {
            try
            {
                var listData = await _serviceWrapper.DataUser.SelectFilterAsync(request);

                return OK(listData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [PermissionAuthorize(1)] // Function xem chi tiết user (1)
        [HttpGet("Select-User")]
        public async Task<IActionResult> GetUserCode([FromQuery] string userCode)
        {
            try
            {
                if (string.IsNullOrEmpty(userCode))
                    return BadRequest("User code cannot be null or empty.");

                var user = await _serviceWrapper.DataUser.SelectByCodeAsync(userCode);

                return OK(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Ví dụ về quyền thêm user (function_id = 2)
        [Authorize]
        [PermissionAuthorize(2)] // Function thêm user (2)
        [HttpPost("Create-User")]
        public async Task<IActionResult> CreateUser([FromBody] UserRequest request)
        {
            try
            {
                // Logic tạo user mới sẽ được implement ở đây
                // Ví dụ này chỉ để demo cách sử dụng attribute
                return OK(new { message = "User creation functionality demo" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
