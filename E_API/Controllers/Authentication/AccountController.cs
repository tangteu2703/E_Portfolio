using E_Common;
using E_Contract.Service;
using E_Model.Request.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_API.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly IServiceWrapper _serviceWrapper;
        private readonly JwtHelper _jwtHelper;
        public AccountController(IServiceWrapper serviceWrapper, JwtHelper jwtHelper)
        {
            _serviceWrapper = serviceWrapper;
            _jwtHelper = jwtHelper;
        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordItemRequest data)
        {
            try
            {
                var result = await _serviceWrapper.TokenService.ChangePassword(data);
                if (result)
                    return OK("Đổi mật khẩu thành công");
                else
                    return BadRequest("Đổi mật khẩu thất bại");
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = "Unexpected error: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpItemRequest data)
        {
            try
            {
                var result = await _serviceWrapper.TokenService.SignUp(data);
                if (result > 0)
                    return OK("Đăng ký thành công");
                else
                    return BadRequest("Đăng ký thất bại");
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = "Unexpected error: " + ex.Message });
            }
        }

    }
}
