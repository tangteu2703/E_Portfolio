using E_Model.Authentication;
using E_Model.Request.User;
using E_Model.Response.Authentication;
using E_Model.Response.User;
using E_Model.Table_SQL.Authentication;

namespace E_Contract.Service.Authentication
{
    public interface IMenuService : IServiceBase<Menu>
    {
        Task<IEnumerable<MenuResponse>> SelectMenuPermissionsByUserAsync(string userCode);
    }
}
