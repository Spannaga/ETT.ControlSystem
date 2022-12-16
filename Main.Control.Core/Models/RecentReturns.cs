using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class RecentReturns
    {
        public long ReturnId { get; set; }
        public long FormId { get; set; }
        public string EIN { get; set; }
        public string BusinessName { get; set; }
        public string FUM { get; set; }
        public int FilingStatusId { get; set; }
        public string AdminUserName { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedTimeStamp { get; set; }
        public DateTime UpdatedTimeStamp { get; set; }
    }
}
