namespace E_Contract.Repository
{
    public interface IRepositoryBase<T>
    {
        Task<IEnumerable<T>> SelectAllAsync(string db = "DefaultConnection");
        Task<IEnumerable<T>> SelectFilterAsync(T model, string db = "DefaultConnection");

        Task<T> SelectByIdAsync(int id, string db = "DefaultConnection");
        Task<T> SelectByCodeAsync(string  code, string db = "DefaultConnection");

        Task<int> InsertAsync(T model, string db = "DefaultConnection");

        Task<bool> UpdateAsync(T model, string db = "DefaultConnection");

        Task<bool> DeleteAsync(int id, int userId, string db = "DefaultConnection");
    }
}