using E_Model.Authentication;

namespace E_Model.Response.Authentication
{
	public class UserAuthenticationItemResponse
	{
		public string access_token { get; set; }
		public string refresh_token { get; set; }
		public DataUserCardItemResponse user_info { get; set; }
		public IEnumerable<data_application>list_application { get; set; }
	}
}