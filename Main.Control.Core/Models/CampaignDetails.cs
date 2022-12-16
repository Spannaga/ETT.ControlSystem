using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class CampaignDetails : AdminEntityBase
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public long CampaignDetailsId { get; set; }
        public long CampaignAssignDetailId { get; set; }
        public string CampaignName { get; set; }
        public bool IsUploadFileAssign { get; set; }
        public string CampaignFileName { get; set; }
        public string FilePath { get; set; }
        public string Goals { get; set; }
        public int NoOfLeads { get; set; }
        public string UniqueId { get; set; }
        public long? AdminProjectId { get; set; }
        public bool IsPaused { get; set; }
        public bool IsSuspended { get; set; }
        public DateTime? CampaignStartDate { get; set; }
        public DateTime? CampaignEndDate { get; set; }
        public string TechTeamStatus { get; set; }
        public bool IsDiscardedRequest { get; set; }
        public string DiscardedReason { get; set; }
        public string Notes { get; set; }
        public string CampaignType { get; set; }
        public bool IsLead { get; set; }
        public List<long> SupportUserList { get; set; }
        public List<AdminUser> SupportAdminUserList { get; set; }
        public List<CampaignAssignedDetails> CampaignAssignedDetailsList { get; set; }
        public List<CampaignAssignedDetails> CampaignContactedDetailsList { get; set; }
        public List<CampaignAssignedDetails> CampaignSkippedDetailsList { get; set; }
        public List<CampaignSupportUserDetails> CampaignSupportUserDetailsList { get; set; }
        public CampaignSupportUserDetails CampaignSupportUserDetails { get; set; }
        public List<LeadCommunication> LeadCommunicationLst { get; set; }
        public TimeZoneInfo TimeZoneDefault { get; set; }
        public List<State> StateList { get; set; }
        public string StateCode { get; set; }
        public string TimeZone { get; set; }
        public DateTime? TechTeamFileUploadedTime { get; set; }
        public string DemoGrapicInformation { get; set; }
        public string UploaderName { get; set; }
        public int UploadedCount { get; set; }
        public string SuspendReason { get; set; }
        public DateTime SuspendDate { get; set; }
        public int PhoneCount { get; set; }
        public int EmailCount { get; set; }
        public int FollowupCount { get; set; }
        public string AdminUserName { get; set; }
        public string BatchStatus { get; set; }

        public bool IsBatchStatus { get; set; }
        public bool IsBatchProcess { get; set; }
        public bool IsManagerFollowUp { get; set; }
        public List<ChampaignExcelHeaderDetails> ChampaignExcelHeaderDetailsList { get; set; }
        public List<ChampaignExcelValueDetails> ChampaignExcelValueDetailsList { get; set; }
        public int SkippedCount { get; set; }
        public string CampaignErrorStatus { get; set; }

        public int FollowUpAdminUserId { get; set; }
        public string FollowUpComments { get; set; }
    }
}

