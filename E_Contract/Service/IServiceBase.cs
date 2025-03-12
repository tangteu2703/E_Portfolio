namespace E_Contract.Service
{
    public interface IServiceBase<T>
    {
        Task<IEnumerable<T>> SelectAllAsync();
        Task<IEnumerable<T>> SelectFilterAsync(T model);

        Task<T> SelectByIdAsync(int id);

        Task<int> InsertAsync(T model);

        Task<bool> UpdateAsync(T model);

        Task<bool> DeleteAsync(int id, int user_id);
    }
}
