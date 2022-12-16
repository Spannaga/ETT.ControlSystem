using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
namespace Main.Control.Core.Models
{
    [Serializable()]
    [DataContract()]
    public class LeadCommunication : AdminEntityBase
    {
        public bool IsSkip { get; set; }
        [DataMember]
        public string Spoketo { get; set; }
        [DataMember]
        public string Comments { get; set; }
        [DataMember]
        public string Reason { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public string MethodeofContract { get; set; }
        public string TypeOfCall { get; set; }
        [DataMember]
        public bool DonotContactagain { get; set; }
        public bool IsFollowRequired { get; set; }  
        [DataMember]
        public DateTime? FollowupDate { get; set; }
        public TimeSpan? FollowupTime { get; set; }
        public long CampaignLeadActivityId { get; set; }
        public long CampaignAssignDetailId { get; set; }
        public long CampaignDetailId { get; set; }
        public long SupportUserId { get; set; }
        public bool IsSaveNext { get; set; }
        [DataMember]
        public string LeadStatus { get; set; }
        public string SupportUserName { get; set; }
        [DataMember]
        public DateTime CreatedDate { get; set; }
        public bool isCampaignCommunicationStatus { get; set; }
        public List<LeadCommunication> LeadCommunicationLst { get; set; }
        public long CampaignAsignedFirstcount { get; set; }
        public long CampaignAsignedLastcount { get; set; }
        public string CampaignType { get; set; }
        public string CampaignName { get; set; }
        public string OtherReason { get; set; }
        public DateTime? LogDate { get; set; }
        public string LeadName { get; set;}
        public string Phone { get; set; }
        public string StateCode { get; set; }
        public int ProductId { get; set; }
        [DataMember]
        public Guid ETTUserId { get; set; }
        [DataMember]
        public string AdminUserName { get; set; }
        public bool IsNotified { get; set; }
    }
}
