using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class ChampaignExcelValueDetails
    {
        public long CampaignExcelValuesId { get; set; }
        public long CampaignExcelHeaderId { get; set; }
        public long CampaignAssignedDetailsId { get; set; }
        public string ExcelHeaderValue { get; set; }
        public string ExcelHeaderName { get; set; }
    }
}
