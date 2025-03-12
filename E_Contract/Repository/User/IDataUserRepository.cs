using E_Model.Authentication;
using E_Model.Response.Authentication;

namespace E_Contract.Repository.User
{
    public interface IDataUserRepository : IRepositoryBase<data_user>
    {
        Task<DataUserItemResponse> SelectAsync(string username, string password);
        Task<DataUserItemResponse> SelectByUserAsync(string email, string password, bool is_ldap = false);
        Task<IEnumerable<DataUserDepartmentResponse>> SelectByDepatmentAsync(int? departmentId);
    }
}