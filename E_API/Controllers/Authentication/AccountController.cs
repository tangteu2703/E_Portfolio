using E_Common;
using E_Contract.Service;
using E_Model.Authentication;
using E_Model.Request.Token;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace E_API.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly IServiceWrapper _serviceWrapper;
        private readonly JwtHelper _jwtHelper;
        private readonly IMemoryCache _memoryCache;
        public AccountController(IServiceWrapper serviceWrapper, JwtHelper jwtHelper, IMemoryCache memoryCache)
        {
            _serviceWrapper = serviceWrapper;
            _jwtHelper = jwtHelper;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("Send-OTP")]
        public async Task<IActionResult> SendOTP([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Email cannot be null or empty.");

                var checkEmail = await _serviceWrapper.DataUser.SelectUserByEmailAsync(email);
                if (checkEmail == null)
                    return BadRequest("Email not found.");

                // Tạo mã 6 chữ số
                var code = new Random().Next(100000, 999999).ToString();
                var minute = 3;
                // Lưu vào MemoryCache với thời hạn 3 phút
                var cacheKey = $"forgotpwd_{email}";
                var info = new VerificationCode
                {
                    Email = email,
                    Code = code,
                    ExpireAt = DateTime.Now.AddMinutes(minute)
                };
                _memoryCache.Set(cacheKey, info, TimeSpan.FromMinutes(2));

                // Gửi email
                var sent = await MailHelper.SendOtpAsync(email, code, minute);
                if (!sent)
                    return BadRequest("Không thể gửi email.");

                return Ok(new
                {
                    countdown = minute
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = "Unexpected error: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordItemRequest data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.email) || string.IsNullOrEmpty(data.code))
                    return BadRequest("Email or code is missing.");

                if (string.IsNullOrEmpty(data.new_password))
                    return BadRequest("Password or code is missing.");

                var cacheKey = $"forgotpwd_{data.email}";
                if (!_memoryCache.TryGetValue<VerificationCode>(cacheKey, out var info))
                    return BadRequest("Verification code expired or invalid.");

                if (info.Code != data.code || info.ExpireAt < DateTime.Now)
                    return BadRequest("Invalid or expired verification code.");

                // Gọi service đổi mật khẩu
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
    }
}
