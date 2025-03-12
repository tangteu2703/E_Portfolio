using System.Text.Json.Serialization;

namespace E_Model
{
    public class modify_info
    {
        [JsonIgnore]
        public bool is_deleted { get; set; }

        [JsonIgnore]
        public DateTime last_datetime { get; set; }

        [JsonIgnore]
        public int last_user_id { get; set; }

        /// <summary>
        /// Set thông tin người thêm, thời gian thêm record này
        /// </summary>
        /// <param name="UserID"></param>
        public void SetInsertInfo(int created_user_id)
        {
            this.is_deleted = false;
            this.last_user_id = created_user_id;
            this.last_datetime = DateTime.Now;
        }

        /// <summary>
        /// Set thông tin người sửa, thời gian sửa của record này
        /// </summary>
        /// <param name="UserID"></param>
        public void SetUpdateInfo(int last_modified_user_id)
        {
            this.last_user_id = last_modified_user_id;
            this.last_datetime = DateTime.Now;
        }
    }
}