using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Model.Table_SQL.WorkSheet
{
    public class HR_BarCode
    {
        [Required]
        public string CD_COMPANY { get; set; }

        [Required]
        public string NO_CARD { get; set; }

        [Required]
        public string DT_WORK { get; set; }

        [Required]
        public string TM_CARD { get; set; }

        [Required]
        public string CD_WCODE { get; set; }

        public string? DTS_INSERT { get; set; }

        public string? ID_INSERT { get; set; }

        public string? DTS_UPDATE { get; set; }

        public string? ID_UPDATE { get; set; }

        public string? CD_USERDEF1 { get; set; }

        public string? CD_TM { get; set; }

        public string? STATUS_ERP { get; set; }
    }
}
