using E_Contract.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace E_API.Filter
{
    /// <summary>
    /// Kiểm tra quyền truy cập theo api_url + user_code + function (optional) với cache để giảm truy vấn DB.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class PermissionAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly int _functionId; // 0: chỉ cần có quyền truy cập API theo URL

        public PermissionAuthorizeAttribute(int functionId = 0)
        {
            _functionId = functionId;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var user = httpContext.User;

            if (user?.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedObjectResult(new { success = false, message = "Unauthorized." });
                return;
            }

            var userCode = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(userCode))
            {
                context.Result = new UnauthorizedObjectResult(new { success = false, message = "Invalid token (user_code missing)." });
                return;
            }

            var requestPath = httpContext.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

            var services = httpContext.RequestServices;
            var cache = services.GetService(typeof(IMemoryCache)) as IMemoryCache;
            var serviceWrapper = services.GetService(typeof(IServiceWrapper)) as IServiceWrapper;

            if (cache == null || serviceWrapper == null)
            {
                context.Result = new ObjectResult(new { success = false, message = "Authorization services unavailable." }) { StatusCode = 500 };
                return;
            }

            // Cache key per user
            var cacheKey = $"perm:{userCode}";
            if (!cache.TryGetValue(cacheKey, out List<E_Model.Response.Authentication.MenuResponse> userPerms))
            {
                var perms = await serviceWrapper.Menu.SelectMenuPermissionsByUserAsync(userCode);
                userPerms = perms?.ToList() ?? new List<E_Model.Response.Authentication.MenuResponse>();
                // Cache 10 minutes; adjust as needed
                cache.Set(cacheKey, userPerms, TimeSpan.FromMinutes(10));
            }

            // Normalize api_url for compare
            bool hasApiAccess = userPerms.Any(p =>
                !string.IsNullOrWhiteSpace(p.api_url) && requestPath.Contains(p.api_url.Trim().ToLowerInvariant()));

            if (!hasApiAccess)
            {
                context.Result = new UnauthorizedObjectResult(new { success = false, message = "Access denied for API." });
                return;
            }

            if (_functionId > 0)
            {
                var hasFunction = userPerms.Any(p =>
                    (!string.IsNullOrWhiteSpace(p.menu_url) && requestPath.Contains(p.menu_url.Trim().ToLowerInvariant()))
                    && (p.function_id == _functionId));

                if (!hasFunction)
                {
                    context.Result = new UnauthorizedObjectResult(new { success = false, message = "Access denied for function." });
                    return;
                }
            }
        }
    }
}
