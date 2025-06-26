using E_Contract.Repository;
using E_Contract.Service.WorkSheet;
using E_Model.Table_SQL.WorkSheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Service.WorkSheet
{
    public class ClockTransactionService : ServiceBase<data_transaction>, IClockTransactionService
    {
        public ClockTransactionService(IRepositoryWrapper RepositoryWrapper) : base(RepositoryWrapper)
        {
            _repositoryBase = RepositoryWrapper.ClockTransaction;
        }
        public async Task<IEnumerable<data_transaction>> SelectTransactionDateAsync(DateTime fromDate, DateTime toDate)
        {
            return await _repositoryWrapper.ClockTransaction.SelectTransactionDateAsync(fromDate, toDate);
        }
    }
}
