using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Response.Notification
{
    public class NotificationResponse
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string icon { get; set; }
        public DateTime timestamp { get; set; }
        public string type { get; set; }
    }
}
