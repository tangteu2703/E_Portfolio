using E_Contract.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace E_API.Filter
{
    // Legacy filter implementation (no longer used by attribute). Kept for potential reuse.

    /// <summary>
    /// Filter implementation để kiểm tra quyền truy cập
    /// </summary>
    public class PermissionAuthorizeFilter : IAsyncActionFilter
    {
        private readonly int _functionId;
        private readonly IServiceWrapper _serviceWrapper;

        /// <summary>
        /// Constructor với dependency injection
        /// </summary>
        public PermissionAuthorizeFilter(int functionId, IServiceWrapper serviceWrapper)
        {
            _functionId = functionId;
            _serviceWrapper = serviceWrapper;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                // Kiểm tra quyền truy cập từ database dựa trên user_code từ JWT token
                // Chuyển trách nhiệm check sang PermissionAuthorizeAttribute mới (theo api_url + cache)
                // Ở đây fallback: nếu không dùng attribute mới, kiểm tra tối thiểu theo function_id qua DB
                var hasPermission = true;
                if (_functionId > 0)
                {
                    var user = context.HttpContext.User;
                    hasPermission = await _serviceWrapper.TokenService.CheckCurrentUserPermissionAsync(user, _functionId);
                }

                if (!hasPermission)
                {
                    context.Result = new UnauthorizedObjectResult(new
                    {
                        success = false,
                        message = $"Access denied. You don't have permission to access this function (Function: {_functionId})."
                    });
                    return;
                }

                // Tiếp tục thực hiện action nếu có quyền
                await next();
            }
            catch (Exception ex)
            {
                context.Result = new ObjectResult(new
                {
                    success = false,
                    message = $"Error checking permissions: {ex.Message}"
                })
                {
                    StatusCode = 500
                };
            }
        }
    }
}
