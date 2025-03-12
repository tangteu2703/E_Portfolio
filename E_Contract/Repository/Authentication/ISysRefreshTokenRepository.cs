using E_Model.Authentication;

namespace E_Contract.Repository.Authentication
{
    public interface ISysRefreshTokenRepository : IRepositoryBase<sys_refresh_token>
    {
        Task<sys_refresh_token> SelectByUser(int user_id);

        Task<sys_refresh_token> SelectByToken(string refresh_token);
    }
}