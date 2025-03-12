using Dapper;
using E_Common;
using E_Contract.Repository.Authentication;
using E_Model.Authentication;

namespace E_Repository.Authentication
{
    public class DataApplicationRepository : RepositoryBase<data_application>, IDataApplicationRepository
    {
        public async Task<IEnumerable<data_application>> SelectByUserIdAsync(int user_id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@user_id", user_id);
                var result = await Connection.SelectAsync<data_application>("data_application_select_by_user", param);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
