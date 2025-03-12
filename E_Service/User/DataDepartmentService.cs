using E_Contract.Repository;
using E_Contract.Service.User;
using E_Model.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Service.User
{
    public class DataDepartmentService : ServiceBase<data_department>, IDataDepartmentService
    {
        public DataDepartmentService(IRepositoryWrapper RepositoryWrapper) : base(RepositoryWrapper)
        {
        }
    }
}
