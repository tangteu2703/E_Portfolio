using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Common;
using E_Contract.Repository.Authentication;
using E_Model.Authentication;

namespace E_Repository.Authentication
{
    public class SysRefreshTokenRepository : RepositoryBase<sys_refresh_token>, ISysRefreshTokenRepository
    {
        public async Task<sys_refresh_token> SelectByToken(string refresh_token)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@refresh_token", refresh_token);
                var result = await Connection.SelectAsync<sys_refresh_token>("sys_refresh_token_select_by_token", param);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<sys_refresh_token> SelectByUser(int user_id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@user_id", user_id);
                var result = await Connection.SelectAsync<sys_refresh_token>("sys_refresh_token_select_by_user", param);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
