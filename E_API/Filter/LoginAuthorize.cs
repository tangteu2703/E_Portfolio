using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace E_API.Filter
{
    /// <summary>
    /// Đảm bảo request đã đăng nhập hợp lệ qua JWT (đã có UseAuthentication).
    /// Trả 401 nếu chưa xác thực.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class LoginAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                // Cố gắng authenticate theo scheme mặc định (JWT)
                var authResult = context.HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme).GetAwaiter().GetResult();
                if (authResult?.Succeeded == true && authResult.Principal != null)
                {
                    context.HttpContext.User = authResult.Principal;
                    return;
                }

                context.Result = new UnauthorizedObjectResult(new
                {
                    success = false,
                    message = "Unauthorized. Please login."
                });
                return;
            }

            // Đã đăng nhập
            return;
        }
    }
}
