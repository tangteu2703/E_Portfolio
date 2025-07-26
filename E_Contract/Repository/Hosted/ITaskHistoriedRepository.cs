using E_Model.Table_SQL.Hosted;

namespace E_Contract.Repository.Hosted
{
    public interface ITaskHistoriedRepository : IRepositoryBase<TaskHistoried>
    {
        Task<IEnumerable<TaskHistoried>> SelectTaskActive();
    }
}
