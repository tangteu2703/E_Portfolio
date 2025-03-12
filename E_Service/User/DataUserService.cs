using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Contract.Repository;
using E_Contract.Service.User;
using E_Model.Authentication;

namespace E_Service.User
{
    public class DataUserService : ServiceBase<data_user>, IDataUserService
    {
        public DataUserService(IRepositoryWrapper RepositoryWrapper) : base(RepositoryWrapper)
        {
            _repositoryBase = RepositoryWrapper.DataUser;
        }
    }
}
