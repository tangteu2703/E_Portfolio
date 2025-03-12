using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Common;
using E_Contract.Repository.User;
using E_Model.Authentication;
using E_Model.Response.Authentication;

namespace E_Repository.User
{
    public class DataUserRepository : RepositoryBase<data_user>, IDataUserRepository
    {
        public async Task<DataUserItemResponse> SelectAsync(string username, string password)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@username", username);
                param.Add("@password", password);
                var result = await Connection.SelectAsync<DataUserItemResponse>("data_user_select", param);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<DataUserDepartmentResponse>> SelectByDepatmentAsync(int? departmentId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@departmentId", departmentId);
                var result = await Connection.SelectAsync<DataUserDepartmentResponse>("data_user_by_department_select", param);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DataUserItemResponse> SelectByUserAsync(string email, string password, bool is_ldap = false)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@email", email);
                param.Add("@password", password);
                param.Add("@is_ldap", is_ldap);

                var result = await Connection.SelectAsync<DataUserItemResponse>("data_user_select_by_user", param);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
