using E_Common;
using E_Contract.Service;
using E_Model.Authentication;
using E_Model.Request.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_API.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenController : BaseController
    {
        private readonly IServiceWrapper _serviceWrapper;
        private readonly JwtHelper _jwtHelper;
        public AuthenController(IServiceWrapper serviceWrapper, JwtHelper jwtHelper)
        {
            _serviceWrapper = serviceWrapper;
            _jwtHelper = jwtHelper;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginItemRequest data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.password))
                return BadRequest("Email and password cannot be empty.");

            try
            {
                // Authenticate the user
                var user = await _serviceWrapper.TokenService.AuthenticateAsync(data);
                if (user != null && !string.IsNullOrEmpty(user.access_token))
                {
                    // Save the refresh token in the database
                    var refreshToken = new sys_refresh_token
                    {
                        user_code = user.user_info.user_code,
                        email = user.user_info.email,
                        token = user.refresh_token,
                        created_by_ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? LogExtension.GetMachineName(),
                        created_by_mac = _jwtHelper.GetMacAddressAndConvertToHS256(),
                        created_time = DateTime.Now,
                        expired_date = DateTime.UtcNow.AddDays(7),
                    };

                    await _serviceWrapper.SysRefreshToken.InsertAsync(refreshToken);

                    // Return success response with user data
                    return OK(user);
                }
                else
                {
                    // Authentication failed
                    return BadRequest("Login failed. Please check your email and password.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = "Unexpected error: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> Refresh([FromBody] RefreshItemRequest data)
        {
            try
            {
                var user = await _serviceWrapper.TokenService.RefreshAsync(data.username, data.refresh_token);
                if (user.access_token != null)
                {
                    await _serviceWrapper.SysRefreshToken.InsertAsync(new sys_refresh_token
                    {
                        email = data.username,
                        token = user.refresh_token,
                        created_by_ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
                        created_time = DateTime.Now,
                        expired_date = DateTime.UtcNow.AddDays(7),
                    });
                    return OK(user);
                }
                else
                {
                    return BadRequest("Đăng nhập thất bại");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = "Unexpected error: " + ex.Message });
            }
        }
    }
}
