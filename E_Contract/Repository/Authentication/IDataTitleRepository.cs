using E_Model.Authentication;

namespace E_Contract.Repository.Authentication
{
	public interface IDataTitleRepository : IRepositoryBase<data_title>
	{
		Task<data_title> SelectByTitleNameAsync(string title);

	}
}