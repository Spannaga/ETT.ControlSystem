using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Main.Control.Core.Models;

namespace Main.Control.Core.Repositories
{
    public interface IAdminRepository
    {
        AdminUser GetAdminDetailsByUserName(AdminUser adminUser);
        AdminUser GetAdminUserById(long Id);
        AdminUser GetAdminUserByEmailId(string email);
        IQueryable<AdminRole> GetAllAdminRoles(string categoryid);
        AdminUser SaveAdminUser(AdminUser admin);
        string GetAdminRoleById(long adminRoleId);
        string GetAdminRoleByProjectIdAndAdminUserId(long projectId, long adminUserId);
        List<AdminUser> GetAllAdminUsers(long adminprojectId);
        bool VerifyUserName(string subDomain, long UserId);
        AdminUser SavePassword(AdminUser adminuser);
        IQueryable<AdminProject> GetAllAdminProjects(long? categoryid);
        AdminUserRole SaveProjectRole(AdminUserRole _adminuserrole);
        List<AdminUserRole> GetAllAdminProjectRole(long userId);
        bool DeleteProjectRole(long Id);
        bool DeleteAdminUser(long Id);
        long GetAllAdminUserRoles(long adminUserId, long adminProducts);
        IQueryable<AdminCategory> GetAllAdminCategories(long? userid);
        AdminUser GetAdminDetailsByUserNameApproved(string adminUser);
        List<LeadLogState> GetAllAdminState();
        AdminActivityLog SaveActivityLog(AdminActivityLog adminActivityLog);
        List<AdminActivityLog> GetAllActivityLog();
        List<AdminActivityLog> GetActivityLogByUserID(long Id);
        MobileVerification SendVerificationCode(MobileVerification mobileverification);
        MobileVerification UpdateMobileVerificationDetails(MobileVerification mobileVerification);
        bool VerifyMobileCode(MobileVerification mobileverification);
        string GetVerificationCodeByUniqueId(string uniqueId);
        bool UpdateMobileVerificationStatus(string uniqueId);
        List<AdminIpAddress> GetAllIpAddressList();
        AdminIpAddress GetIpAddressDetailsById(long ipAddressId);
        bool DeleteIpAddressDetailsById(long ipAddressId, bool ismultiple);
        bool SaveIpAddressDetailsById(AdminIpAddress adminIpAddress);
        List<BizAdminProjects> GetStaticBizAdminProjects();
        bool AddAdmin(long Id);
        bool RemoveAdmin(long Id);
        List<AdminUser> GetAllAdminList();
        bool SaveActivityLogDetails(ScActivityLog scActivityLogDetails);
        List<ScActivityLog> GetActivityLogByAdminUserId(long adminUserId);
        bool IsProjectIpAlreadyExists(long ipAddressId, string projectIp, string project);

        int GetAdminActivityLogCount(JQueryDataTableParamModel param);
        List<ScActivityLog> GetAdminActivityLogList(JQueryDataTableParamModel param);
        List<AdminProject> GetAllProjects();

        AdminProject GetProjectNameByProjectId(int projectId);

        List<AdminUser> GetAdminSupportUsersByProjectId(int projectId);
        CampaignDetails CreateCampainDetails(CampaignDetails campaignDetails);

        List<CampaignDetails> GetCampaignDetails(JQueryDataTableParamModel param);

        CampaignDetails GetCampaignShortDetailsByCampaignId(long campaignId);

        bool UpdateCampaignExtendDate(CampaignDetails campaignDetails);

        bool UpdateCampaignPauseStatus(long campaignDetailsId, bool isPaused);

        bool UpdateCampaignSuspendStatus(long campaignDetailsId, bool isSuspend);


        bool DeleteCampaign(long campaignDetailsId);
        List<CampaignDetails> GetCampaignUploadRequestDetails();
        void UpdateDiscardedRequestReason(CampaignDetails campaignDetails);

        void SaveTechTeamFileUploaded(CampaignDetails campaignDetails);

        LeadDetails GetLeadDetailsbyCampainDetailIdandSupportUserId(long campaignId, long supportUserId);
        LeadDetails GetLeadDetailsForCommunicationByCampainDetailIdandSupportUserId(long campaignId, long supportUserId);


        CampaignDetails GetGoalsByCampaignDetailsId(long campaignId);

        LeadCommunication SaveCampaignCommunication(LeadCommunication leadCommunication);
        List<CampaignDetails> GetCampaignSupportUserDetails(JQueryDataTableParamModel param);

        List<State> GetAllStates();

        List<LeadDetails> GetCampaignLeadActivityListByCampaignDetailId(long campaignDetailId, long supportUserId);
        CampaignDetails GetCampaignAndSupportUserAndAssignedDetails(long campaignId, long supportUserId);

        bool UpdateLastLeadIdDuringBack(long campaignId, long supportUserId);

        bool UpdateLastLeadIdDuringSkip(long campaignId, long supportUserId);
        LeadDetails GetLeadDetailsListByStateId(long campaignId, long supportUserId, string stateCode);

        List<LeadCommunication> GetCampaignPreviousActivityList(long campaignAssignedDetailId);
        LeadDetails GetLeadDetailsListByTimezone(long campaignId, long supportUserId, string timeZone);

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
        LeadDetails GetAdditionalContactListByCampaignAssignedDetailsId(long campaignAssignedDetailsId);
        List<LeadDetails> GetAllLeadListBySupportUserId(long supportUserId, string searchBy, string value);
        List<LeadCommunication> GetFollowupBySupportUserId(long supportUserId, string followUpFilter);

        bool UpdateLastLeadIdByCampaignAssignedDetailsId(LeadCommunication leadCommunication);

        int GetCampaignSupportUserDetailsCount(JQueryDataTableParamModel param);
        bool UpdatePreviousLeadIdAsLastLeadIdByStatecode(LeadDetailsSearchOption leadDetailsSearchOption);
        bool UpdateNextLeadIdAsLastLeadIdByStatecode(LeadDetailsSearchOption leadDetailsSearchOption);
        bool GetCampaignDetailsCountByStateCode(long campaignId, long supportUserId, string stateCode);
        List<LeadDetails> GetCampaignDetailsbyCampainDetailIdandSupportUserId(long campaignId, long supportUserId);
        bool RemoveAllNonStaticIPs();

        bool UpdateCampaignAssignedDetailsEINByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string ein);

        LeadDetails GetLeadDetailsBySkippedRecord(long campaignId, long supportUserId);

        void ReAssignedLeads(long campaignDetailId, long supportUserId);

        void UpdateReassignDetailsReset(LeadDetailsSearchOption leadDetailsSearchOption);

        CampaignDetails GetCampaignUploadedTimeDetails(CampaignDetails campaignDetails);

        LeadCommunication GetCampaignLeadActivityDetails(long campaignLeadActivityId);
        List<RecentReturns> GetRecentReturns(Guid assignedDetailId);
        RecentReturns GetFollowUpDetails(long assignedDetailId);
        bool UpdateUserVerificationCodeType(long adminUserId);
        List<TransactionReportDetails> GetTransactionReportDetails(TransactionReport transactionReport);
        List<SpanProducts> GetSpanProducts(string connectionString);
        List<TransactionReportDetails> GetS3UploadURL(string transactionId, string connectionString, int paymentProcessorType);
        List<PaymentProcessors> GetSpanPaymentProcessors(string connectionString);
        List<UserPayments> GetAllUserPayments();
        UserPayments GetUserPaymentDetailByTokenId(Guid tokenId);
        UserPayments SaveUserPayments(UserPayments userPayments);
        List<Country> GetCountries();
        PaymentTemplate GetPaymentTemplateByTemplateId(int templateId);
        List<SpanLibraryProductDetails> GetAllSpanLibrProducts(string spanLibConnStr);
        void SaveUserPaymentLog(UserPaymentLog paymentLog);
        List<UserPaymentLog> GetPaymentLogs(Guid paymentId, PaymentActivityType activityType);
        void UpdateReceiptS3Path(Guid userPaymentId, string receiptS3Path);
        List<string> SearchEmailByProduct(string emailAddress, int project);
        void SaveVoidRefund(PaymentRefundLog voidRefundRequest, string connectionString);
        List<PaymentRefundLog> GetPaymentRefundDetails(PaymentRefundLog transactionReport, string connectionString);
        decimal GetRefundAmount(PaymentRefundLog transactionReport, string connectionString, string chargeBackType);
        TransactionReportDetails GetTransactionDetail(string transactionId, string connectionString);
        List<GroupMembers> GetReportBySelectedOption(string optionSelected,long adminUserId,long campaignDetailId);
        LeadDetails GetCampaignAssignedDetails(long campaignDetailId, long supportUserId, long campaignAssignedDetailId);
        AdminUser GetAdminUserUthenticatorDetailsByAdminUserId(long adminUserId);
        bool UpdateAuthenticationForAdminUser(long adminUserId);
        bool ResetAuthentication(long Id);
        List<ReturnCountReport> GetAppReconcilationPayment(DateTime BeginDate, DateTime EndDate, int ProductId);
        List<ReturnCountReport> GetAPIReconcilationPayment(DateTime BeginDate, DateTime EndDate, int ProductId);
        ReturnCountReport GetChargeBackTypeInPaymentRefundLog(string TransactionRefId);
        List<StripeCredential> GetAllStripeCredentials();
        bool UpdateCampaignAssignedDetailsReturnNumberByCampaignAssignedDetailsId(long campaignAssignedDetailsId, long returnNumber);
    }
}
