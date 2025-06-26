using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Response
{
    public class DataTableResponse<T>
    {
        public int Draw { get; set; }              // Số thứ tự của request, do DataTable gửi lên
        public int RecordsTotal { get; set; }      // Tổng số bản ghi trong database
        public int RecordsFiltered { get; set; }   // Tổng số bản ghi sau khi filter (nếu có filter)
        public IEnumerable<T> Data { get; set; }   // Dữ liệu thực tế trả về
    }

}
