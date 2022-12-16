using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class UserPaymentLog
    {
        public long UserPaymentLogId { get; set; }
        public Guid UserPaymentId { get; set; }
        public long AdminUserId { get; set; }
        public string AdminUserName { get; set; }
        public string AcitivityMsg { get; set; }
        public DateTime ActivityTimeStamp { get; set; }
        public string ActivityType { get; set; }


    }
}
