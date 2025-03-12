using E_Model.Authentication;

namespace E_Contract.Repository.Authentication
{
	public interface ISysLdapSettingRepository : IRepositoryBase<sys_ldap_setting>
	{
        Task<sys_ldap_server> SelectTop1LdapServerAsync();

    }
}