using System.Text.Json.Serialization;

namespace E_Model
{
    public class Modified
    {

        [JsonIgnore]
        public DateTime? last_modified { get; set; }
        [JsonIgnore]
        public string? last_user_id { get; set; }
        [JsonIgnore]
        public bool is_deleted { get; set; }
        public string? note { get; set; }

        public void SetModified(string user_id, bool status = false)
        {
            this.is_deleted = status;
            this.last_user_id = user_id;
            this.last_modified = DateTime.Now;
        }
    }
}