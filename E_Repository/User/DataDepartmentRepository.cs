using Dapper;
using E_Common;
using E_Contract.Repository;
using E_Contract.Repository.User;
using E_Model.Authentication;

namespace E_Repository.User
{
    public class DataDepartmentRepository : RepositoryBase<data_department>, IDataDepartmentRepository
    {
        public async Task<data_department> SelectByDepartmentNameAsync(string department)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@department", department);
                var result = await Connection.SelectAsync<data_department>("data_department_select_by_department", param);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
