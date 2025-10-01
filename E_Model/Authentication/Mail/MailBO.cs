using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Authentication.Mail
{
    public class MailBO
    {
        public string? Subject { get; set; }
        public string? Content { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public string? CC { get; set; }
    }
}
