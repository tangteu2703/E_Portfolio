using E_Model.Authentication;

namespace E_Model.Response.Authentication
{
	public class DataUserDepartmentResponse : data_user
	{
		public string organize_name { get; set; }

		public string title_name { get; set; }
		public string position { get; set; }
		public string department_name { get; set; }
		public bool is_edit { get; set; }
		public bool is_main { get; set; }

	}
}