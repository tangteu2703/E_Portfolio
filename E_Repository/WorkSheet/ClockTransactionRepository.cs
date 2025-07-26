using Dapper;
using E_Common;
using E_Contract.Repository.WorkSheet;
using E_Model.Response.Authentication;
using E_Model.Table_SQL.WorkSheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Repository.WorkSheet
{
    public class ClockTransactionRepository : RepositoryBase<data_transaction>, IClockTransactionRepository
    {
        public async Task<IEnumerable<data_transaction>> SelectTransactionDateAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@fromDate", fromDate);
                param.Add("@toDate", toDate);
                var result = await Connection.SelectAsync<data_transaction>("data_transaction_select_date", param, "BioStarConnection");
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
