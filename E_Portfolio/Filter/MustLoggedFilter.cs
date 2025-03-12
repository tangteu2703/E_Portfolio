using E_Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WDI_CMS_Common;

namespace E_Portfolio.Filter
{
    public class MustLoggedFilter : ActionFilterAttribute
    {
        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var accessToken = GetAccessToken(context.HttpContext);
                var user = context.HttpContext.User;

                if (!string.IsNullOrEmpty(accessToken))
                {
                    return;
                }
                if (IsTokenExpired(user))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
                context.Result = new UnauthorizedResult();
            }
            catch (Exception)
            {
                base.OnActionExecuting(context);
            }
        }

        private string GetAccessToken(HttpContext httpContext)
        {
            try
            {
                var request = httpContext.Request;
                var jwtToken = request.Headers[HeaderNames.Authorization]
                    .ConvertToString()
                    .Replace("Bearer ", "");
                return jwtToken;
            }
            catch (Exception ex)
            {
                ex.SaveLog("JwtTokenService/GetUserToken");
                return string.Empty;
            }
        }

        private bool IsTokenExpired(ClaimsPrincipal user)
        {
            var exp = user.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;
            if (exp != null && long.TryParse(exp, out var unixTime))
            {
                var expiryDate = DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
                return expiryDate < DateTime.UtcNow;
            }
            return false;
        }
    }
}