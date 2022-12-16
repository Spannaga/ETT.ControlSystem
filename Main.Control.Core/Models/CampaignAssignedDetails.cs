using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class CampaignAssignedDetails : AdminEntityBase
    {

        public long CampaignAssignedDetailsId { get; set; }
        public long CampaignDetailsId { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string BusinessName { get; set; }
        public string EIN { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime? SignedUpOn { get; set; }
        public string ProductName { get; set; }
        public string UserType { get; set; }
        public int NoOfTrucks { get; set; }
        public DateTime? LastFiledOn { get; set; }
        public string Subscribed { get; set; }
        public long? SupportUserId { get; set; }

        public int? LeadStatus { get; set; }

        public List<ChampaignExcelValueDetails> ChampaignExcelValueDetailsList { get; set; }

        public bool IsSkip { get; set; }

        public int CampaignFollowUpId { get; set; }
        public int UserId { get; set; }
        public string Comments { get; set; }
        public int FollowUpAdminUserId { get; set; }
        public string CampaignStatus { get; set; }
        public long ReturnNumber { get; set; }
    }
}
