using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Contract.Repository;
using E_Contract.Service.Authentication;
using E_Model.Authentication;

namespace E_Service.Authentication
{
    public class SysRefreshTokenService : ServiceBase<sys_refresh_token>, ISysRefreshTokenService
    {
        public SysRefreshTokenService(IRepositoryWrapper repositoryWrapper): base(repositoryWrapper)
        {
            this._repositoryBase = repositoryWrapper.SysRefreshToken;
        }

        public async Task<sys_refresh_token> SelectByUser(int user_id)
        {
            return await _repositoryWrapper.SysRefreshToken.SelectByUser(user_id);
        }
    }
}
