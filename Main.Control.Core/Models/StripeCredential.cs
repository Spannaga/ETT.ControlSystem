using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class StripeCredential
    {
        public long StripeProcessorId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string StripeLoginKey { get; set; }
        public bool Is_DailyReportActive { get; set; }
        public decimal ProductsTotalMonthAmount { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedTimeStamp { get; set; }
        public DateTime CreatedTimeStamp { get; set; }
    }
}
