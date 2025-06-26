using E_Contract.Repository;
using E_Contract.Service.Department;
using E_Model.Table_SQL.Department;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Service.Department
{
    public class DepartmentService : ServiceBase<data_department>, IDepartmentService
    {
        public DepartmentService(IRepositoryWrapper RepositoryWrapper) : base(RepositoryWrapper)
        {
            _repositoryBase = RepositoryWrapper.Department;
        }
    }
}
