using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class CampaignManagerFollowUpDetails
    {
        public int CampaignFollowUpId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public string UserEmailAddress { get; set; }
        public string Comments { get; set; }
        public int AdminUserId { get; set; }
        public string CampaignStatus { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreatedTimeStamp { get; set; }
        public System.DateTime UpdatedTimeStamp { get; set; }
    }
}
