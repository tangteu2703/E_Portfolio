using E_Model.Authentication;

namespace E_Contract.Repository.Authentication
{
	public interface IDataPositionRepository : IRepositoryBase<data_position>
	{
		Task<data_position> SelectByPositionNameAsync(string position);
	}
}