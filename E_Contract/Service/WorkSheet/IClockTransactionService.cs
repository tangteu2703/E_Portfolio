using E_Model.Table_SQL.WorkSheet;

namespace E_Contract.Service.WorkSheet
{
    public interface IClockTransactionService : IServiceBase<data_transaction>
    {
        Task<IEnumerable<data_transaction>> SelectTransactionDateAsync(DateTime fromDate, DateTime toDate);
    }
}
