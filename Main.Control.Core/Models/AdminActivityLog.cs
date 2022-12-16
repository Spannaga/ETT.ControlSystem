using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class AdminActivityLog 
    {
        public long AdminUserId { get; set; }
        public long SCActivityLogId { get; set; }
        public string IpAddress { get; set; }
        public string VistedURL { get; set; }
        public long ProjectId { get; set; }
        public string Action { get; set; }
        public string AdditionalInfo1 { get; set; }
        public string AdditionalInfo2 { get; set; }
        public string AdditionalInfo3 { get; set; }


      
        public string AdminUserName { get; set; }
        public string ProjectName { get; set; }
        public DateTime UserCreatedOn { get; set; }

      


    }
}
