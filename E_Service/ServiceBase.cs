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

        public async Task<IEnumerable<T>> SelectAllAsync()
        {
            return await _repositoryBase.SelectAllAsync();
        }

        public async Task<IEnumerable<T>> SelectFilterAsync(T model)
        {
            return await _repositoryBase.SelectFilterAsync(model);
        }

        public async Task<T> SelectByIdAsync(int id)
        {
            return await _repositoryBase.SelectByIdAsync(id);
        }
        public async Task<int> InsertAsync(T model)
        {
            return await _repositoryBase.InsertAsync(model.Map<T>());
        }
        public async Task<bool> UpdateAsync(T model)
        {
            return await _repositoryBase.UpdateAsync(model.Map<T>());
        }
        public async Task<bool> DeleteAsync(int id, int user_id)
        {
            return await _repositoryBase.DeleteAsync(id, user_id);
        }
    }
}
