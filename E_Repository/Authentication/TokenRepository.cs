using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Contract.Repository.Authentication;
using E_Model.Authentication;

namespace E_Repository.Authentication
{
    public class TokenRepository : RepositoryBase<data_user>, ITokenRepository
    {
    }
}
