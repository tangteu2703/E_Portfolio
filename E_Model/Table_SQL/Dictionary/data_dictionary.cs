using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Table_SQL.Dictionary
{
    public class data_dictionary : modify_info
    {
        public int id { get; set; }
        public string code { get; set; }
        public string vn { get; set; }
        public string en { get; set; }
        public string fr { get; set; }
        public string ja { get; set; }
        public string ko { get; set; }
        public string zh { get; set; }
    }
}
