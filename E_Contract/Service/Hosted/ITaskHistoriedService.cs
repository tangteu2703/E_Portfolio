using E_Model.Request.WorkSheet;
using E_Model.Response.WorkSheet;
using E_Model.Table_SQL.Hosted;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Contract.Service.Hosted
{
    public interface ITaskHistoriedService : IServiceBase<TaskHistoried>
    {
        Task<IEnumerable<TaskHistoried>> SelectTaskActive();

    }
}
