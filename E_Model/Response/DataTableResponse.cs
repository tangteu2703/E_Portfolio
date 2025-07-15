using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Response
{
    public class DataTableResponse<T>
    {
        public int recordsTotal { get; set; }    
        public int recordsFiltered { get; set; }
        public IEnumerable<T> listData { get; set; }
    }

}
