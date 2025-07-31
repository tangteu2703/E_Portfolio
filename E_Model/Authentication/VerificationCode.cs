using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Authentication
{
    public class VerificationCode
    {
        public string? Email { get; set; }
        public string? Code { get; set; }
        public DateTime ExpireAt { get; set; }
    }

}
