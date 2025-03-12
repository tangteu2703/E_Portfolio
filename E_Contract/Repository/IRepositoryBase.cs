namespace E_Contract.Repository
{
    public interface IRepositoryBase<T>
    {
        Task<IEnumerable<T>> SelectAllAsync();
        Task<IEnumerable<T>> SelectFilterAsync(T model);

        Task<T> SelectByIdAsync(int id);

        Task<int> InsertAsync(T model);

        Task<bool> UpdateAsync(T model);

        Task<bool> DeleteAsync(int id, int userId);
    }
}