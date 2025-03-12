using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Model.Authentication;

namespace E_Contract.Service.Authentication
{
    public interface ISysRefreshTokenService : IServiceBase<sys_refresh_token>
    {
        Task<sys_refresh_token> SelectByUser(int user_id);
    }
}
