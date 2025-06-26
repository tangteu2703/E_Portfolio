using E_Model.Table_SQL.WorkSheet;

namespace E_Contract.Repository.WorkSheet
{
    public interface IClockTransactionRepository : IRepositoryBase<data_transaction>
    {
        Task<IEnumerable<data_transaction>> SelectTransactionDateAsync(DateTime fromDate, DateTime toDate);
    }
}
