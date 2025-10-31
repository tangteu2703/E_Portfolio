using E_Contract.Repository;
using E_Contract.Service.Kiot;
using E_Model.Table_SQL.Kiot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Service.Kiot
{
    public class KiotCategoryService : ServiceBase<KiotCategory>, IKiotCategoryService
    {
        public KiotCategoryService(IRepositoryWrapper RepositoryWrapper) : base(RepositoryWrapper)
        {
            _repositoryBase = RepositoryWrapper.KiotCategory;
        }
    }
}
