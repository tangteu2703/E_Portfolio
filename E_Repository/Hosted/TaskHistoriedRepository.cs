using DocumentFormat.OpenXml.Office2016.Excel;
using E_Common;
using E_Contract.Repository.Hosted;
using E_Model.Response.WorkSheet;
using E_Model.Table_SQL.Hosted;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Repository.Hosted
{
    public class TaskHistoriedRepository : RepositoryBase<TaskHistoried>, ITaskHistoriedRepository
    {
        public async Task<IEnumerable<TaskHistoried>> SelectTaskActive()
        {
            try
            {

                var result = await Connection.SelectAsync<TaskHistoried>("taskhistoried_select_active", null, "E_PortalConnection");
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
