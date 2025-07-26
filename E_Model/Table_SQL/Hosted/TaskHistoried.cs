using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Table_SQL.Hosted
{
    public class TaskHistoried : modify_info
    {
        public int? id { get; set; }              
        public string? task_code { get; set; }    
        public string? task_name { get; set; }    
        public DateTime? task_date { get; set; }  
        public bool is_active { get; set; }       
        public bool is_send { get; set; }         
    }
}
