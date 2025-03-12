using E_Model.Authentication;

namespace E_Contract.Repository.Authentication
{
    public interface IDataCompanyRepository : IRepositoryBase<data_company>
    {
        Task<data_company> SelectByCompanyNameAsync(string company_name);
    }
}