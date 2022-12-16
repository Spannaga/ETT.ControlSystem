using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class CampaignSupportUserDetails : AdminEntityBase
    {
        public long CampaignSupportUserDetailId { get; set; }
        public long CampaignDetailsId { get; set; }
        public long SupportUserId { get; set; }
        public int? NoOfUserAssigned { get; set; }
        public int? NoOfCompleted { get; set; }
        public int? NoOfPending { get; set; }
        public int? UserSkippedCount { get; set; }
        public bool IsViewed { get; set; }
        public string  AdminUserName { get; set; }
        public long LastLeadId { get; set; }
    }
}
