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
    public class DataApplicationService : ServiceBase<data_application>, IDataApplicationService
    {
        public DataApplicationService(IRepositoryWrapper RepositoryWrapper) : base(RepositoryWrapper)
        {
            this._repositoryBase = RepositoryWrapper.DataApplication;
        }
    }
}
