using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Model.Authentication;
using E_Model.Request.Token;
using E_Model.Response.Authentication;

namespace E_Contract.Service.Authentication
{
    public interface ITokenService : IServiceBase<data_user>
    {
        Task<UserAuthenticationItemResponse> AuthenticateAsync(string username, string password, bool is_ldap = false);
        Task<UserAuthenticationItemResponse> RefreshAsync(string username, string refresh_token);
        Task<bool> ChangePassword(ChangePasswordItemRequest request);
        Task<int> SignUp(SignUpItemRequest request);
        Task<DataUserCardItemResponse> CheckLoginAndResponseToken(DataUserItemResponse user, bool is_refresh);
        Task<IEnumerable<DataUserDepartmentResponse>> GetAllByDepartment(int? departmentId);

    }
}
