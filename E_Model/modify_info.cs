using System.Text.Json.Serialization;

namespace E_Model
{
    public class modify_info
    {

        [JsonIgnore]
        public DateTime? created_at { get; set; }

        [JsonIgnore]
        public string? created_by { get; set; }

        [JsonIgnore]
        public DateTime? updated_at { get; set; }

        [JsonIgnore]
        public string? updated_by { get; set; }

        [JsonIgnore]
        public bool? is_deleted { get; set; } = false;

        [JsonIgnore]
        public string? note { get; set; }

        public void SetInsertInfo(string? created_user_id)
        {
            this.is_deleted = false;
            this.created_by = created_user_id;
            this.created_at = DateTime.Now;
        }

        public void SetUpdateInfo(string? last_modified_user_id)
        {
            this.is_deleted = false;
            this.updated_by = last_modified_user_id;
            this.updated_at = DateTime.Now;
        }
    }
}