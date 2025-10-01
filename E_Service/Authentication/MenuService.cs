using DocumentFormat.OpenXml.EMMA;
using E_Contract.Repository;
using E_Contract.Service.Authentication;
using E_Model.Response.Authentication;
using E_Model.Table_SQL.Authentication;

namespace E_Service.Authentication
{
    public class MenuService : ServiceBase<Menu>, IMenuService
    {
        public MenuService(IRepositoryWrapper RepositoryWrapper) : base(RepositoryWrapper)
        {
            this._repositoryBase = RepositoryWrapper.Menu;
        }

        public async Task<IEnumerable<MenuResponse>> SelectMenuPermissionsByUserAsync(string userCode)
        {
            return await _repositoryWrapper.Menu.SelectMenuPermissionsByUserAsync(userCode);
        }
    }
}
