using E_Model.Authentication;

namespace E_Contract.Repository.Authentication
{
    public interface IDataApplicationRepository : IRepositoryBase<data_application>
    {
		Task<IEnumerable<data_application>> SelectByUserIdAsync(int user_id);
	}
}