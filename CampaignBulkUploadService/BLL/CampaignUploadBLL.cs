using CampaignBulkUploadService.DAL;
using ProcessBulkUpload.Utilities;
using Main.Control.Core.Models;
using Main.Control.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CampaignBulkUploadService.BLL
{
    public class CampaignUploadBLL
    {
        private CampaignUploadDAL _campaignUploadDAL = null;
        private readonly EmailService _emailService;

        public CampaignUploadBLL()
        {
            _campaignUploadDAL = new CampaignUploadDAL();
            _emailService = new EmailService();
        }

        public CampaignDetails GetCampaignInitDetails()
        {
            return _campaignUploadDAL.GetCampaignInitDetails();
        }

        public void CampaignSetInProgress(CampaignDetails campaignDetails)
        {
            _campaignUploadDAL.CampaignSetInProgress(campaignDetails);
        }

        public CampaignDetails SaveCampaignDetails(CampaignDetails campaignDetails)
        {
            _campaignUploadDAL.SaveCampaignDetails(campaignDetails);
            if (!campaignDetails.IsManagerFollowUp)
            {
                //Send Bulk Upload File Completed Email to Manager
                SendEmailtoManagerBulkUploadFileCompleted(campaignDetails);
            }
            if (campaignDetails.SupportUserList != null && campaignDetails.SupportUserList.Count > 0)
            {

                AdminUser adminstratorDetail = _campaignUploadDAL.GetAdminUserById(campaignDetails.AdminUserId);


                foreach (var emailSendObjUser in campaignDetails.CampaignSupportUserDetailsList)
                {

                    AdminUser sendEmailtoSupportUserDetails = _campaignUploadDAL.GetAdminUserById(emailSendObjUser.SupportUserId);
                    MailDetails mailDetails = new MailDetails();
                    if (!campaignDetails.IsManagerFollowUp)
                    {
                        mailDetails = _campaignUploadDAL.GetMailTemplateByTemplateId(Main.Control.Service.Utilities.Constants.CampaignCreatetoSupportUser);
                    }
                    else
                    {
                        mailDetails = _campaignUploadDAL.GetMailTemplateByTemplateId(Main.Control.Service.Utilities.Constants.ManagerCampaignCreatetoSupportUser);
                    }

                    if (mailDetails != null)
                    {
                        mailDetails.MailToAddress = sendEmailtoSupportUserDetails.AdminEmailAddress;
                        //replace contents

                        //Mail Subject
                        mailDetails.MailSubject = mailDetails.MailSubject.Replace(Main.Control.Service.Utilities.Constants.CampaignName, campaignDetails.CampaignName);
                        mailDetails.MailSubject = mailDetails.MailSubject.Replace(Main.Control.Service.Utilities.Constants.AdminUserName, adminstratorDetail.AdminUserName);

                        //Mail Body
                        mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.CampaignName, campaignDetails.CampaignName);
                        mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.NoOfLeadsAssigned, emailSendObjUser.NoOfUserAssigned.ToString());
                        mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.Goals, campaignDetails.Goals);
                        string startdate = string.Empty;
                        string enddate = string.Empty;
                        if (campaignDetails.CampaignStartDate != null && campaignDetails.CampaignStartDate > DateTime.MinValue)
                        {
                            startdate = Utility.GetDateTime(campaignDetails.CampaignStartDate).ToString("MM/dd/yyyy");
                        }
                        if (campaignDetails.CampaignEndDate != null && campaignDetails.CampaignEndDate > DateTime.MinValue)
                        {
                            enddate = Utility.GetDateTime(campaignDetails.CampaignEndDate).ToString("MM/dd/yyyy");
                        }
                        string campaignPeroid = startdate + " to " + enddate;
                        mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.CampaignPeroid, campaignPeroid);
                        mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.SupportUserName, sendEmailtoSupportUserDetails.AdminUserName);
                        mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.AdminUserName, adminstratorDetail.AdminUserName);
                        mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.redirectLink, ConfigurationManager.AppSettings[Main.Control.Service.Utilities.Constants.MainControlURL].ToString());
                        _emailService.SendEmail(mailDetails);

                    }

                    }
            }
            else if(campaignDetails.IsManagerFollowUp)
            {
                if (campaignDetails.SupportUserList != null && campaignDetails.SupportUserList.Count > 0)
                {
                    AdminUser adminstratorDetail = _campaignUploadDAL.GetAdminUserById(campaignDetails.AdminUserId);
                    foreach (var emailSendObjUser in campaignDetails.CampaignSupportUserDetailsList)
                    {
                        AdminUser sendEmailtoSupportUserDetails = _campaignUploadDAL.GetAdminUserById(emailSendObjUser.SupportUserId);
                        MailDetails mailDetails = _campaignUploadDAL.GetMailTemplateByTemplateId(Main.Control.Service.Utilities.Constants.ManagerCampaignCreatetoSupportUser);
                        if (mailDetails != null)
                        {
                            mailDetails.MailToAddress = sendEmailtoSupportUserDetails.AdminEmailAddress;
                            //replace contents

                            //Mail Body
                            mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.CampaignName, campaignDetails.CampaignName);
                            mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.NoOfLeadsAssigned, emailSendObjUser.NoOfUserAssigned.ToString());
                            mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.Goals, campaignDetails.Goals);
                            string startdate = string.Empty;
                            string enddate = string.Empty;
                            if (campaignDetails.CampaignStartDate != null && campaignDetails.CampaignStartDate > DateTime.MinValue)
                            {
                                startdate = Utility.GetDateTime(campaignDetails.CampaignStartDate).ToString("MM/dd/yyyy");
                            }
                            if (campaignDetails.CampaignEndDate != null && campaignDetails.CampaignEndDate > DateTime.MinValue)
                            {
                                enddate = Utility.GetDateTime(campaignDetails.CampaignEndDate).ToString("MM/dd/yyyy");
                            }
                            string campaignPeroid = startdate + " to " + enddate;
                            mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.CampaignPeroid, campaignPeroid);
                            mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.SupportUserName, sendEmailtoSupportUserDetails.AdminUserName);
                            mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.AdminUserName, adminstratorDetail.AdminUserName);
                            mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.redirectLink, ConfigurationManager.AppSettings[Main.Control.Service.Utilities.Constants.MainControlURL].ToString());
                            _emailService.SendEmail(mailDetails);
                        }
                    }
                }
            }
            return campaignDetails;
        }

        public CampaignDetails SetSupportAdminUserList(CampaignDetails campaignDetails)
        {
            campaignDetails.SupportAdminUserList = new List<AdminUser>();
            campaignDetails.SupportUserList = new List<long>();
            campaignDetails.CampaignAssignedDetailsList = new List<CampaignAssignedDetails>();

            if (campaignDetails.CampaignSupportUserDetailsList != null && campaignDetails.CampaignSupportUserDetailsList.Count > 0)
            {
                foreach (var assinedCountobj in campaignDetails.CampaignSupportUserDetailsList)
                {
                    AdminUser adminUser = new AdminUser();
                    adminUser.AdminUserId = assinedCountobj.SupportUserId;
                    adminUser.AdminUserName = assinedCountobj.AdminUserName;
                    adminUser.TotalAssignedCount = assinedCountobj.NoOfUserAssigned ?? 0;
                    campaignDetails.SupportAdminUserList.Add(adminUser);
                    campaignDetails.SupportUserList.Add(assinedCountobj.SupportUserId);
                }
            }

            return campaignDetails;

        }

        #region Send Email to Manager Bulk Upload File Completed
        public void SendEmailtoManagerBulkUploadFileCompleted(CampaignDetails campaignDetails)
        {
            CampaignDetails mailSendCampaignDetails = _campaignUploadDAL.GetCampaignShortDetails(campaignDetails.CampaignDetailsId);
            AdminUser adminstratorDetail = _campaignUploadDAL.GetAdminUserById(mailSendCampaignDetails.AdminUserId);
            MailDetails mailDetails = _campaignUploadDAL.GetMailTemplateByTemplateId(Main.Control.Service.Utilities.Constants.FileUploadCompleted);

            if (mailDetails != null)
            {
                mailDetails.MailToAddress = adminstratorDetail.AdminEmailAddress;
                //replace contents

                //Mail Subject
                mailDetails.MailSubject = mailDetails.MailSubject.Replace(Main.Control.Service.Utilities.Constants.CampaignName, campaignDetails.CampaignName);

                //Mail Body
                mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.SupportUserName, adminstratorDetail.AdminUserName);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.AdminUserName, adminstratorDetail.AdminUserName);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.TotalRecords, campaignDetails.NoOfLeads.ToString());
                mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.SkippedRecords, "0");
                mailDetails.MailBody = mailDetails.MailBody.Replace(Main.Control.Service.Utilities.Constants.NoOfLeadsAssigned, campaignDetails.NoOfLeads.ToString());

                this._emailService.SendEmail(mailDetails);
            }
        }
        #endregion

        // Update Upload Status
        #region Update Upload Status
        public void UpdateUploadStatus(CampaignDetails campaignDetails)
        {
            _campaignUploadDAL.UpdateUploadStatus(campaignDetails);
        }
        #endregion

        #region Get Manager Follow Up Details
        public List<CampaignAssignedDetails> GetManagerFollowUpDetails()
        {
            return _campaignUploadDAL.GetManagerFollowUpDetails();
        }
        #endregion

        #region Follow Up Campaign Details
        public CampaignDetails FollowUpCampaignDetails(List<CampaignAssignedDetails> assignedDetailslist)
        {
            CampaignDetails followUpDetails = new CampaignDetails();
            long campaignDetailsId = Utility.GetLong(Utility.GetAppSettings("CampaignDetailId"));
            if (campaignDetailsId > 0)
            {
                followUpDetails = _campaignUploadDAL.GetCampaignDetailByDetailId(campaignDetailsId);
            }
            if (assignedDetailslist != null && assignedDetailslist.Any())
            {
                followUpDetails.CampaignAssignedDetailsList = assignedDetailslist;
            }
            int rowNUmber = 1;
            long SupportUserId = 0;
            int indexingSupportUserCount = 0;
            int supportUserAssignedCount = 0;

            string previousDate = DateTime.Now.Date.AddDays(-1).ToString("yyyy/MM/dd");
            
            followUpDetails.CampaignSupportUserDetailsList = new List<CampaignSupportUserDetails>();
            {
                followUpDetails.AdminUserId = Utility.GetLong(ConfigurationManager.AppSettings[Constants.AdminUserId]);
                //followUpDetails.CampaignName = "Manager Follow Up (" + previousDate + ")";
                //followUpDetails.Goals = Constants.Goals;
                followUpDetails.NoOfLeads = assignedDetailslist.Count;
                followUpDetails.ProductId = int.Parse(Constants.AdminProjectId);
                //followUpDetails.CampaignStartDate = DateTime.Now;
                //followUpDetails.CampaignEndDate = DateTime.Now.AddDays(6);
                followUpDetails.CampaignType = Constants.CampaignType;
                followUpDetails.BatchStatus = BatchUploadStatus.IN_PROGRESS.ToString();
            }
            string supportUserId = ConfigurationManager.AppSettings["SupportUserIds"];
            string[] supportUserIds = supportUserId.Split(',');
            int no_of_Users = assignedDetailslist.Count();
            if (supportUserIds.Count() > 0 && assignedDetailslist.Count() > 0)
            {
                followUpDetails.SupportUserList = new List<long>();
                var userAssigned = no_of_Users / supportUserIds.Count();
                foreach (var item in supportUserIds)
                {
                    CampaignSupportUserDetails assignedSupportDetails = new CampaignSupportUserDetails();
                    assignedSupportDetails.AdminUserId = Utility.GetLong(ConfigurationManager.AppSettings[Constants.AdminUserId]);
                    assignedSupportDetails.SupportUserId = Utility.GetLong(item);
                    assignedSupportDetails.NoOfUserAssigned = userAssigned;
                    assignedSupportDetails.NoOfPending = userAssigned;
                    followUpDetails.CampaignSupportUserDetailsList.Add(assignedSupportDetails);
                    followUpDetails.SupportUserList.Add(assignedSupportDetails.SupportUserId);
                }
                if (followUpDetails.NoOfLeads > followUpDetails.CampaignSupportUserDetailsList.Sum(m => m.NoOfUserAssigned))
                {
                    var differenceCount = followUpDetails.NoOfLeads - followUpDetails.CampaignSupportUserDetailsList.Sum(m => m.NoOfUserAssigned);
                    if (differenceCount > 0)
                    {
                        for (var index = 0; index < differenceCount; index++)
                        {
                            var supportObj = followUpDetails.CampaignSupportUserDetailsList[index];
                            if (supportObj != null)
                            {
                                supportObj.NoOfUserAssigned += 1;
                                supportObj.NoOfPending += 1;
                            }
                        }
                    }
                }
            }
            if (followUpDetails != null && followUpDetails.CampaignSupportUserDetailsList != null && followUpDetails.CampaignAssignedDetailsList != null)
            {
                supportUserAssignedCount = followUpDetails.CampaignSupportUserDetailsList[indexingSupportUserCount].NoOfUserAssigned ?? 0;
                foreach (var item in followUpDetails.CampaignAssignedDetailsList)
                {
                    SupportUserId = followUpDetails.CampaignSupportUserDetailsList[indexingSupportUserCount].SupportUserId;
                    if (supportUserAssignedCount >= rowNUmber)
                    {
                        item.SupportUserId = SupportUserId;
                    }
                    else
                    {
                        indexingSupportUserCount += 1;
                        supportUserAssignedCount += followUpDetails.CampaignSupportUserDetailsList[indexingSupportUserCount].NoOfUserAssigned ?? 0;
                        if (supportUserAssignedCount >= rowNUmber)
                        {
                            item.SupportUserId = followUpDetails.CampaignSupportUserDetailsList[indexingSupportUserCount].SupportUserId;
                        }
                    }
                    rowNUmber++;
                }
            }
            return followUpDetails;
        }
        #endregion


    }
}
