using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace E_Portfolio.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RoleAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _requiredRole;

        public RoleAuthorizeAttribute(string requiredRole = "user")
        {
            _requiredRole = requiredRole;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var endpoint = context.HttpContext.GetEndpoint();
            var descriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

            if (descriptor == null) return;

            var allowAnonymous = descriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).Any();
            if (allowAnonymous) return;

            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = new JsonResult(new { message = "Chưa đăng nhập!", status = StatusCodes.Status401Unauthorized });
                return;
            }

            var userRole = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (_requiredRole == "salary")
            {
                if (userRole == "hr")
                {
                    // hr được phép full quyền
                    return;
                }
                var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "O";
                var userIdFromRequest = context.HttpContext.Request.Query["user_id"].ToString() ?? "X"; 
   
                if (userIdClaim != userIdFromRequest)
                {
                    // Logic khi user_id không trùng không được xem
                    context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Result = new JsonResult(new { message = "Bạn không có quyền truy cập user này", status = StatusCodes.Status403Forbidden });
                    return;
                }
            }
            else
            {
                if (userRole == "admin")
                {
                    // check admin xét full quyền
                    return;
                }
                else if (string.IsNullOrEmpty(userRole) || !userRole.Equals(_requiredRole, StringComparison.OrdinalIgnoreCase))
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Result = new JsonResult(new { message = "Bạn không đủ quyền truy cập!", status = StatusCodes.Status403Forbidden });
                    return;
                }
            }
        }

    }
}
