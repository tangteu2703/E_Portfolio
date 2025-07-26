using E_Model.Response;

namespace E_Contract.Service
{
    public interface IServiceBase<T>
    {
        Task<IEnumerable<T>> SelectAllAsync(string db = "DefaultConnection");

        Task<IEnumerable<T>> SelectFilterAsync(T model, string db = "DefaultConnection");

        Task<T> SelectByIdAsync(int id, string db = "DefaultConnection");

        Task<int> InsertAsync(T model, string db = "DefaultConnection");

        Task<bool> UpdateAsync(T model, string db = "DefaultConnection");

        Task<bool> DeleteAsync(int id, int user_id, string db = "DefaultConnection");
    }
}
