using E_Common;
using E_Contract.Service;
using E_Model.Authentication;
using E_Model.Request.Token;
using Microsoft.AspNetCore.Mvc;

namespace E_Portfolio.Controllers.Authentication
{
    public class AuthenController : BaseController
    {
        private readonly JwtHelper _jwtHelper;
        public AuthenController(IServiceWrapper serviceWrapper, JwtHelper jwtHelper) : base(serviceWrapper)
        {
            this._jwtHelper = jwtHelper;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginItemRequest data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.email) || string.IsNullOrWhiteSpace(data.password))
                return BadRequest("Email and password cannot be empty.");

            try
            {
                // Authenticate the user
                var user = await _serviceWrapper.TokenService.AuthenticateAsync(data.email, data.password);
                if (user != null && !string.IsNullOrEmpty(user.access_token))
                {
                    // Save the refresh token in the database
                    var refreshToken = new sys_refresh_token
                    {
                        email = user.user_info.full_name,
                        token = user.refresh_token,
                        created_by_ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
                        created_by_mac = _jwtHelper.GetMacAddressAndConvertToHS256(),
                        created_time = DateTime.Now,
                        expired_date = DateTime.UtcNow.AddDays(7),
                    };

                    //await _serviceWrapper.SysRefreshToken.InsertAsync(refreshToken);

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
                // Return a general error message to the client
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }


        [HttpPost]
        [Route("ChangePassword")]
        public async Task<ContentResult> ChangePassword([FromBody] ChangePasswordItemRequest data)
        {
            try
            {
                var result = await _serviceWrapper.TokenService.ChangePassword(data);
                if (result)
                {
                    return OK("Đổi mật khẩu thành công");
                }
                else
                {
                    return BadRequest("Đổi mật khẩu thất bại");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("SignUp")]
        public async Task<ContentResult> SignUp([FromBody] SignUpItemRequest data)
        {
            try
            {
                var result = await _serviceWrapper.TokenService.SignUp(data);
                if (result > 0)
                {
                    return OK("Đăng ký thành công");
                }
                else
                {
                    return BadRequest("Đăng ký thất bại");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<ContentResult> Refresh([FromBody] RefreshItemRequest data)
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
                    return this.OK(user);
                }
                else
                {
                    //return this.BadRequest("Đăng nhập thất bại");
                    return this.OK(null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
