using E_Model.Authentication;
using E_Model.Response.Authentication;
using E_Model.Table_SQL.Authentication;

namespace E_Contract.Repository.Authentication
{
    public interface IMenuRepository : IRepositoryBase<Menu>
    {
        Task<IEnumerable<MenuResponse>> SelectMenuPermissionsByUserAsync(string userCode);
    }
}