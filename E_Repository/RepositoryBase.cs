using Dapper;
using E_Common;
using E_Contract.Repository;

namespace E_Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        public async Task<IEnumerable<T>> SelectAllAsync()
        {
            var tableName = typeof(T).Name;
            try
            {
                return await Connection.SelectAsync<T>( $"{tableName}_select_all");
            }
            catch (Exception ex)
            {
                //ex.SaveLog(String.Format("{0}/{1}", tableName, "SelectAllAsync"));
                throw ex;
            }
        }
        public async Task<IEnumerable<T>> SelectFilterAsync(T model)
        {
            var tableName = typeof(T).Name;
            try
            {
                var param = DynamicParameterHelper.ConvertWithReturnParam(model, "id");
                return await Connection.SelectAsync<T>( $"{tableName}_select_filter", param);
            }
            catch (Exception ex)
            {
                throw; // Giữ nguyên stack trace của exception
            }
        }
        public async Task<T> SelectByIdAsync(int id)
        {
            var tableName = typeof(T).Name;
            try
            {
                var param = new DynamicParameters();
                param.Add("@id", id);
                var ressult = await Connection.SelectAsync<T>( String.Format("{0}_select_by_id", tableName), param);
                return ressult.FirstOrDefault();
            }
            catch (Exception ex)
            {
                //ex.SaveLog(String.Format("{0}/{1}", tableName, "SelectByIdAsync"));
                throw ex;
            }
        }
        public async Task<IEnumerable<T>> SelectChangedAsync(DateTime fromTime)
        {
            var tableName = typeof(T).Name;
            try
            {
                var param = new DynamicParameters();
                param.Add("@fromTime", fromTime);
                var ressult = await Connection.SelectAsync<T>( String.Format("{0}_select_changed", tableName), param);
                return ressult;
            }
            catch (Exception ex)
            {
                //ex.SaveLog(String.Format("{0}/{1}", tableName, "SelectByIdAsync"));
                throw ex;
            }
        }

        public async Task<int> InsertAsync(T model)
        {
            var tableName = typeof(T).Name;
            try
            {
                var param = DynamicParameterHelper.ConvertWithReturnParam(model, "id");
                return await Connection.ExcuteScalarAsync($"{tableName}_insert", param, "id");
            }
            catch (Exception ex)
            {
                //ex.SaveLog(String.Format("{0}/{1}", tableName, "InsertAsync"));
                throw ex;
            }
        }
        public async Task<bool> UpdateAsync(T model)
        {
            var tableName = typeof(T).Name;
            try
            {
                var param = DynamicParameterHelper.ConvertWithOutCreatitonParams(model);
                await Connection.ExecuteAsync($"{tableName}_update", param);
                return true;
            }
            catch (Exception ex)
            {
                //ex.SaveLog(String.Format("{0}/{1}", tableName, "UpdateAsync"));
                throw ex;
            }
        }
        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var tableName = typeof(T).Name;
            try
            {
                var param = new DynamicParameters();
                param.Add("@id", id);
                param.Add("@user_id", userId);
                await Connection.ExecuteAsync($"{tableName}_delete", param);
                return true;
            }
            catch (Exception ex)
            {
                //ex.SaveLog(String.Format("{0}/{1}", tableName, "DeleteAsync"));
                throw ex;
            }
        }

    }
}
