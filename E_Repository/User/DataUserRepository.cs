using Dapper;
using E_Common;
using E_Contract.Repository.User;
using E_Model.Authentication;
using E_Model.Request;
using E_Model.Request.Token;
using E_Model.Request.User;
using E_Model.Request.WorkSheet;
using E_Model.Response.Authentication;
using E_Model.Response.User;
using E_Model.Response.WorkSheet;
using E_Model.Table_SQL.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Repository.User
{
    public class DataUserRepository : RepositoryBase<UserData>, IDataUserRepository
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
        public async Task<IEnumerable<UserResponse>> SelectFilterAsync(UserRequest request)
        {
            try
            {
                var param = request.ToDynamicParameters();

                var result = await Connection.SelectAsync<UserResponse>("data_user_select_filter", param, "HRMConnection");
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DataUserResponse> SelectByUserAsync(LoginItemRequest data)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@email", data.email);
                param.Add("@user_code", data.user_code);
                param.Add("@password", data.password);

                var result = await Connection.SelectAsync<DataUserResponse>("UserData_select_by_user", param);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserData> SelectUserByEmailAsync(string email)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@email", email);
                param.Add("@user_code", null);

                var result = await Connection.SelectAsync<UserData>("UserData_select_by_email", param);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
