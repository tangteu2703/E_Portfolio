using E_Contract.Repository;
using E_Model.Authentication;

namespace E_Contract.Repository.User
{
    public interface IDataDepartmentRepository : IRepositoryBase<data_department>
    {
        Task<data_department> SelectByDepartmentNameAsync(string department);
    }
}