using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using E_Model.Authentication;
using E_Model.Request.Token;
using E_Model.Response.Authentication;

namespace E_Contract.Service.Authentication
{
    public interface ITokenService : IServiceBase<data_user>
    {
        Task<UserAuthenticationItemResponse> AuthenticateAsync(LoginItemRequest data);
        Task<UserAuthenticationItemResponse> RefreshAsync(string username, string refresh_token);
        Task<bool> ChangePassword(ChangePasswordItemRequest request);
        Task<int> SignUp(SignUpItemRequest request);
        Task<DataUserCardItemResponse> CheckLoginAndResponseToken(DataUserItemResponse user, bool is_refresh);

        /// <summary>
        /// Kiểm tra quyền truy cập của user cho menu và function cụ thể từ database
        /// </summary>
        Task<bool> CheckUserPermissionFromDatabaseAsync(string userCode, int menuId, int functionId);


        /// <summary>
        /// Kiểm tra quyền truy cập của user hiện tại từ JWT token (có fallback về database nếu cần) - với menu_id và function_id
        /// </summary>
        Task<bool> CheckCurrentUserPermissionAsync(ClaimsPrincipal user, int menuId, int functionId);

        /// <summary>
        /// Kiểm tra quyền truy cập của user hiện tại từ JWT token (có fallback về database nếu cần) - chỉ với function_id
        /// </summary>
        Task<bool> CheckCurrentUserPermissionAsync(ClaimsPrincipal user, int functionId);

    }
}
