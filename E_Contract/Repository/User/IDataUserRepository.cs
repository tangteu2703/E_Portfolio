using E_Model.Authentication;
using E_Model.Request;
using E_Model.Request.Token;
using E_Model.Request.User;
using E_Model.Response.Authentication;
using E_Model.Response.User;
using E_Model.Table_SQL.User;

namespace E_Contract.Repository.User
{
    public interface IDataUserRepository : IRepositoryBase<UserData>
    {
        Task<DataUserItemResponse> SelectAsync(string username, string password);
        Task<DataUserItemResponse> SelectByUserAsync(string email, string password, bool is_ldap = false);
        Task<DataUserResponse> SelectByUserAsync(LoginItemRequest data);
        Task<UserData> SelectUserByEmailAsync(string email);
        Task<IEnumerable<DataUserDepartmentResponse>> SelectByDepatmentAsync(int? departmentId);

        Task<IEnumerable<UserResponse>> SelectFilterAsync(UserRequest request);
    }
}