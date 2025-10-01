using Dapper;
using E_Common;
using E_Contract.Repository.Authentication;
using E_Model.Authentication;
using E_Model.Response.Authentication;
using E_Model.Table_SQL.Authentication;

namespace E_Repository.Authentication
{
    public class MenuRepository : RepositoryBase<Menu>, IMenuRepository
    {
        public async Task<IEnumerable<MenuResponse>> SelectMenuPermissionsByUserAsync(string userCode)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@user_code", userCode);

                var result = await Connection.SelectAsync<MenuResponse>("MenuPermissions_select_by_user", param);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
