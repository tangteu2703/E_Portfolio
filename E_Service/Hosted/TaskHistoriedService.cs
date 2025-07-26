using DocumentFormat.OpenXml.Office2016.Excel;
using E_Contract.Repository;
using E_Contract.Service.Hosted;
using E_Model.Table_SQL.Hosted;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Service.Hosted
{
    public class TaskHistoriedService : ServiceBase<TaskHistoried>, ITaskHistoriedService
    {
        public TaskHistoriedService(IRepositoryWrapper RepositoryWrapper) : base(RepositoryWrapper)
        {
            _repositoryBase = RepositoryWrapper.TaskHistoried;
        }

        public async Task<IEnumerable<TaskHistoried>> SelectTaskActive()
        {
            return await _repositoryWrapper.TaskHistoried.SelectTaskActive();
        }
    }
}
