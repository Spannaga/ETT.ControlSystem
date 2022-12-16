using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class LeadDetails : AdminEntityBase
    {
        public string Name { get; set; }
        public string BusinessName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string EIN { get; set; }
        public string Address { get; set; }
        public string UserType { get; set; }
        public DateTime LastFiled { get; set; }
        public long NoofTrucks { get; set; }
        public long CampaignAssignedDetailsId { get; set; }
        public string Comments { get; set; }
        public long CampaignLeadActivityId { get; set; }
        public string CampaignName { get; set; }
        public long CampaignDetailsId { get; set; }
        public long SupportUserId { get; set; }
        public int TotalAssignedLeads { get; set; }
        public int CurrentViewedLeads { get; set; }
        public long ReturnNumber { get; set; }

        public List<AdditionalContacts > AdditionalContactsList { get; set; }
        public List<ChampaignExcelValueDetails> ChampaignExcelValueDetailsList { get; set; }
    }
}
