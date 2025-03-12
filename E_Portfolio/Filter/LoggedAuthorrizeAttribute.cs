using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;

namespace E_Portfolio.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class LoggedAuthorrizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        /// <summary>
        /// Level 1 : Check điều kiện login token có hợp lệ không
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var endpoint = context.HttpContext.GetEndpoint();

            var descriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (descriptor == null) 
                return;

            var allowAnonymous = descriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).Any();
            if (allowAnonymous) 
                return;

            // Kiểm tra xác thực Xác minh User.Identity.IsAuthenticated.Nếu người dùng chưa xác thực, trả về kết quả UnauthorizedResult.
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            // Kiểm tra token hết hạn Gọi phương thức IsTokenExpired để kiểm tra thời gian hết hạn của token.Nếu hết hạn, trả về UnauthorizedResult.
            if (IsTokenExpired(user))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
        private bool IsTokenExpired(ClaimsPrincipal user)
        {
            var exp = user.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;
            if (exp != null)
            {
                var expiryDate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp)).DateTime;
                if (expiryDate < DateTime.UtcNow)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
