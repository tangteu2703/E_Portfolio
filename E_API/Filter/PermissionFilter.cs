using E_Contract.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace E_API.Filter
{
    /// <summary>
    /// Custom attribute để kiểm tra quyền truy cập function (chỉ cần function_id)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PermissionAuthorizeAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Khởi tạo attribute với function cần kiểm tra
        /// </summary>
        /// <param name="functionId">ID của function (1: tạo mới, 2: cập nhật, 3: vô hiệu hóa, 4: import, 5: export)</param>
        public PermissionAuthorizeAttribute(int functionId) : base(typeof(PermissionAuthorizeFilter))
        {
            Arguments = new object[] { functionId };
        }
    }

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
                var hasPermission = await _serviceWrapper.TokenService.CheckCurrentUserPermissionAsync(
                    context.HttpContext.User, _functionId);

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
