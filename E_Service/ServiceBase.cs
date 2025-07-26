using E_Common;
using E_Contract.Repository;
using E_Contract.Service;

namespace E_Service
{
    public class ServiceBase<T> : IServiceBase<T> where T : new()
    {
        protected IRepositoryBase<T> _repositoryBase;
        protected IRepositoryWrapper _repositoryWrapper;

        public ServiceBase(IRepositoryWrapper RepositoryWrapper)
        {
            this._repositoryWrapper = RepositoryWrapper;
        }

        public async Task<IEnumerable<T>> SelectAllAsync(string db = "DefaultConnection")
        {
            return await _repositoryBase.SelectAllAsync(db);
        }

        public async Task<IEnumerable<T>> SelectFilterAsync(T model, string db = "DefaultConnection")
        {
            return await _repositoryBase.SelectFilterAsync(model, db);
        }

        public async Task<T> SelectByIdAsync(int id, string db = "DefaultConnection")
        {
            return await _repositoryBase.SelectByIdAsync(id, db);
        }
        public async Task<int> InsertAsync(T model, string db = "DefaultConnection")
        {
            return await _repositoryBase.InsertAsync(model.Map<T>(), db);
        }
        public async Task<bool> UpdateAsync(T model, string db = "DefaultConnection")
        {
            return await _repositoryBase.UpdateAsync(model.Map<T>(),db);
        }
        public async Task<bool> DeleteAsync(int id, int user_id, string db = "DefaultConnection")
        {
            return await _repositoryBase.DeleteAsync(id, user_id,db);
        }
    }
}
