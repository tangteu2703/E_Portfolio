namespace E_Model.Authentication
{
	public class sys_ldap_setting : modify_info
	{
		public int id { get; set; }
		public int ldap_server_id { get; set; }
		public string ldap_dn { get; set; }
		public int user_id { get; set; }
	}
}