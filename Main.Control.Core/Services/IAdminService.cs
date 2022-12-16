using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Main.Control.Core.Models;

namespace Main.Control.Core.Services
{
    public interface IAdminService
    {
        AdminUser AdminSignIn(AdminUser admin);
        AdminUser GetAdminUserById(long Id);
        AdminUser GetAdminUserByEmailId(string email);
        IQueryable<AdminRole> GetAllAdminRoles(string categoryid);
        AdminUser SaveAdminUser(AdminUser admin);
        List<AdminUser> GetAllAdminUsers(long adminprojectId);
        AdminUser VerifyUserName(string subDomain, long userId);
        AdminUser SavePassword(AdminUser adminUser);
        IQueryable<AdminProject> GetAllAdminProjects(long? categoryid);
        AdminUserRole SaveProjectRole(AdminUserRole _adminuserrole);
        List<AdminUserRole> GetAllAdminProjectRole(long userId);
        bool DeleteProjectRole(long Id);
        bool DeleteAdminUser(long Id);
        IQueryable<AdminCategory> GetAllAdminCategories(long? userid);
        string GetAdminRoleByProjectIdAndAdminUserId(long projectId, long adminUserId);
        AdminUser GetAdminDetailsByUserNameApproved(string adminUser);
        List<LeadLogState> GetAllAdminState(LeadLogState state);
        AdminActivityLog SaveActivityLog(AdminActivityLog adminActivityLog);
        List<AdminActivityLog> GetAllActivityLog();
        List<AdminActivityLog> GetActivityLogByUserID(long Id);
        MobileVerification SendVerificationCode(MobileVerification mobileverification);
        bool VerifyMobileCode(MobileVerification mobileverification);
        string GetVerificationCodeByUniqueId(string uniqueId);
        bool UpdateMobileVerificationStatus(string uniqueId);
        List<AdminIpAddress> GetAllIpAddressList();
        AdminIpAddress GetIpAddressDetailsById(long ipAddressId);
        bool DeleteIpAddressDetailsById(long ipAddressId, string ipAddress, string emailAddress, string projectName, bool ismultiple, string ipName);
        bool SaveIpAddressDetailsById(AdminIpAddress adminIpAddress);
        List<BizAdminProjects> GetStaticBizAdminProjects();
        bool AddAdmin(long Id);
        bool RemoveAdmin(long Id);
        bool SaveActivityLogDetails(ScActivityLog scActivityLogDetails);
        List<ScActivityLog> GetActivityLogByAdminUserId(long adminUserId);
        bool IsProjectIpAlreadyExists(long ipAddressId, string projectIp, string project);
        int GetAdminActivityLogCount(JQueryDataTableParamModel param);
        List<ScActivityLog> GetAdminActivityLogList(JQueryDataTableParamModel param);
        List<AdminProject> GetAllProjects();
        AdminProject GetProjectNameByProjectId(int projectId);
        List<AdminUser> GetAdminSupportUsersByProjectId(int projectId);
        void CreateCampainDetails(CampaignDetails campaignDetails);
        List<CampaignDetails> GetCampaignDetails(JQueryDataTableParamModel param);

        CampaignDetails GetCampaignShortDetailsByCampaignId(long campaignId);
        bool UpdateCampaignExtendDate(CampaignDetails campaignDetails);

        bool UpdateCampaignPauseStatus(long campaignDetailsId, bool isPaused);

        bool UpdateCampaignSuspendStatus(long campaignDetailsId, bool isSuspend);

        void SaveRequestToFileUploadCampaign(CampaignDetails campaignDetails);

        bool DeleteCampaign(long campaignDetailsId);

        List<CampaignDetails> GetCampaignUploadRequestDetails();

        bool UpdateDiscardedRequestReason(CampaignDetails campaignDetails);

        void SaveTechTeamFileUploaded(CampaignDetails campaignDetails);

        LeadDetails GetLeadDetailsbyCampainDetailIdandSupportUserId(long campaignId, long supportUserId);
        LeadDetails GetLeadDetailsForCommunicationByCampainDetailIdandSupportUserId(long campaignId, long supportUserId);

        CampaignDetails GetGoalsByCampaignDetailsId(long campaignDetailId);
        LeadCommunication SaveCampaignCommunication(LeadCommunication leadCommunication);

        List<State> GetAllStates();

        List<LeadDetails> GetCampaignLeadActivityListByCampaignDetailId(long campaignDetailId, long supportUserId);
        CampaignDetails GetCampaignAndSupportUserAndAssignedDetails(long campaignId, long supportUserId);

        bool UpdateLastLeadIdDuringBack(long campaignId, long supportUserId);

        bool UpdateLastLeadIdDuringSkip(long campaignId, long supportUserId);
        LeadDetails GetLeadDetailsListByStateId(long campaignId, long supportUserId, string stateCode);

        LeadDetails GetLeadDetailsListByTimezone(long campaignId, long supportUserId, string timeZone);

        List<LeadCommunication> GetCampaignPreviousActivityList(long campaignAssignedDetailId);

        long GetCampaignDetailsFirstCount(LeadDetailsSearchOption leadDetailsSearchOption);

        LeadCommunication GetCampaignPreviousActivityByCampaignLeadActivityId(long campaignLeadActivityId);

        bool UpdateLeadStatus(long campaignLeadActivityId, string leadStatus);
        bool SuspendCampaign(CampaignDetails campaignDetails);
        List<CampaignAssignedDetails> GetCampaignAssignedDetailsbyCampainDetailIdandSupportUserId(long campaignDetailId, long supportUserId);
        List<CampaignAssignedDetails> UpdateCampaignDetails(List<CampaignAssignedDetails> CampaignAssignedDetailsList);
        bool UpdateCampaignAssignedDetailsBusinessNameByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string businessName);
        bool UpdateCampaignAssignedDetailsNameByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string name);
        bool UpdateCampaignAssignedDetailsEmailByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string email);
        bool UpdateCampaignAssignedDetailsPhoneByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string phone);
        bool UpdateCampaignAssignedDetailsAddressByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string address);
        bool UpdateCampaignAssignedDetailsNofTrucksByCampaignAssignedDetailsId(long campaignAssignedDetailsId, long NofTrucks);

        //int GetCampaignDetailsCountByAdminUserId(long AdminUserId);

        List<LeadCommunication> GetCampaignRecentActivityList(long campaignAssignedDetailsId);

        List<AdminUser> GetAllAdminSupportUsers();

        List<CampaignDetails> GetCampaignDetailsByFilters(JQueryDataTableParamModel param);
        int GetCampaignDetailsCountByAdminUserId(JQueryDataTableParamModel param);

        AdditionalContacts SaveAdditionalContacts(AdditionalContacts ContDetails);

        AdditionalContacts GetAdditionalContactDetailsByAdditionalContactsDetailsId(long additionalContactsDetailsId);

        bool DeleteAdditionalContactByAdditionalContactsDetailsId(long additionalContactsDetailsId);
        long GetCampaignDetailsLastCount(LeadDetailsSearchOption leadDetailsSearchOption);


        List<CampaignDetails> GetCampaignSupportUserDetails(JQueryDataTableParamModel param);

        LeadDetails GetAdditionalContactListByCampaignAssignedDetailsId(long campaignAssignedDetailsId);

        int GetCampaignSupportUserDetailsCount(JQueryDataTableParamModel param);
        List<LeadCommunication> GetFollowupBySupportUserId(long supportUserId, string followUpFilter);

        List<LeadDetails> GetAllLeadListBySupportUserId(long supportUserId, string searchBy, string value);
        bool UpdateLastLeadIdByCampaignAssignedDetailsId(LeadCommunication leadCommunication);
        bool UpdatePreviousLeadIdAsLastLeadIdByStatecode(LeadDetailsSearchOption leadDetailsSearchOption);
        bool UpdateNextLeadIdAsLastLeadIdByStatecode(LeadDetailsSearchOption leadDetailsSearchOption);
        bool GetCampaignDetailsCountByStateCode(long campaignId, long supportUserId, string stateCode);
        List<LeadDetails> GetCampaignDetailsbyCampainDetailIdandSupportUserId(long campaignId, long supportUserId);
        bool RemoveAllNonStaticIPs(string emailAddress);

        bool UpdateCampaignAssignedDetailsEINByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string ein);

        LeadDetails GetLeadDetailsBySkippedRecord(long campaignDetailId, long supportUserId);

        void ReAssignedLeads(long campaignDetailId, long supportUserId);

        void UpdateReassignDetailsReset(LeadDetailsSearchOption leadDetailsSearchOption);

        CampaignDetails GetCampaignUploadedTimeDetails(CampaignDetails campaignDetails);

        LeadCommunication GetCampaignLeadActivityDetails(long campaignLeadActivityId);
        GroupMembers SaveGroupMemberByEmailAddress(string emailAddress);

        Notes GetUserCommentsbyEmailAddress(string emailAddress);
        List<RecentReturns> GetRecentReturns(Guid userId);
        RecentReturns GetFollowUpDetails(long assigneddetailId);
        bool UpdateUserVerificationCodeType(long adminUserId);
        List<TransactionReportDetails> GetTransactionReportDetails(TransactionReport transactionReport);
        List<SpanProducts> GetSpanProducts(string connectionString);
        List<TransactionReportDetails> GetS3UploadURL(string transactionId, string connectionString,int paymentProcessorType);
        List<PaymentProcessors> GetSpanPaymentProcessors(string connectionString);
        List<UserPayments> GetAllUserPayments();
        UserPayments GetUserPaymentDetailByTokenId(Guid tokenId);
        UserPayments SaveUserPayments(UserPayments userPayments);
        List<Country> GetCountries();
        PaymentTemplate GetPaymentMailHtml(UserPayments userPayments);
        EmailDetailAPI SendPaymentsEmail(UserPayments userPayments);
        UserPayments ChargeCreditCardAndSave(UserPayments userPayments);
        void SaveUserPaymentLog(UserPaymentLog paymentLog);
        List<UserPaymentLog> GetPaymentLogs(Guid paymentId, PaymentActivityType activityType);
        void SendPaymentFailureEmail(Guid paymentId);
        void SendPaymentSuccessEmail(Guid paymentId);
        List<string> SearchEmailByProduct(string emailAddress, int project);
        void SaveVoidRefund(PaymentRefundLog voidRefundRequest, string connectionString);
        List<PaymentRefundLog> GetPaymentRefundDetails(PaymentRefundLog transactionReport, string connectionString);
        decimal GetRefundAmount(PaymentRefundLog transactionReport, string connectionString, string chargeBackType);
        WfVoidRefundResponse VoidTransaction(WfVoidRequest wfVoidRequest);
        WfVoidRefundResponse RefundTransaction(WfRefundRequest wfRefundRequest);
        TransactionReportDetails GetTransactionDetail(string transactionId, string connectionString);
        List<SpancontrolContactGroup> GetContactgroupList();
        List<long> GetGroupMembersbyUserId(Guid userId);
        GroupMembers SaveGroupMember(GroupMembers members);
        List<GroupMembers> GetReportbySelectedOption(string selectedOption,long adminUserId,long campaignDetailId);
        Guid GetUserIdbyEmailAddress(string emailaddress);
        List<LeadCommunication> GetCommunicationDetails(Guid userId);
        LeadCommunication SaveCommunicationDetails(LeadCommunication leadCommunication);
        LeadDetails GetCampaignAssignedDetails(long campaignDetailId, long supportUserId, long campaignAssignedDetailId);
        AdminUser GetAdminUserUthenticatorDetailsByAdminUserId(long adminUserId);
        bool UpdateAuthenticationForAdminUser(long adminUserId);
        bool ResetAuthentication(long Id);
        List<PaymentReconReport> GetPaymentBatchDetailsReport(DateTime BeginDate, DateTime EndDate, int ProductId,List<string> TransactionId);
        AuthorizeResponse GetDailySPANReport(DateTime BeginDate, DateTime EndDate);
        bool UpdateCampaignAssignedDetailsReturnNumberByCampaignAssignedDetailsId(long campaignAssignedDetailsId, long returnNumber);
    }
}
