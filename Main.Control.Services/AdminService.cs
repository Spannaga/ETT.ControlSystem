using Main.Control.Core.Models;
using Main.Control.Core.Repositories;
using Main.Control.Core.Services;
using Main.Control.Service.Utilities;
using Main.Control.Services.Utilities;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using xVal.ServerSide;

namespace Main.Control.Services
{
    public class AdminService : IAdminService
    {
        #region Declaration
        private readonly IAdminRepository _adminRepository;
        private readonly IEmailService _emailService;

        #endregion

        #region Constructor

        public AdminService()
        {
        }


        public AdminService(IAdminRepository adminRepository, IEmailService emailService)
        {
            this._adminRepository = adminRepository;
            this._emailService = emailService;
        }
        #endregion

        #region Get All Admin Users
        /// <summary>
        /// Get All Admin Users
        /// </summary>
        /// <returns></returns>
        public List<AdminUser> GetAllAdminUsers(long adminprojectId)
        {
            return this._adminRepository.GetAllAdminUsers(adminprojectId);
        }
        #endregion

        #region Get Admin User By Id
        /// <summary>
        /// Get Admin User By Id
        /// </summary>
        /// <returns></returns>
        public AdminUser GetAdminUserById(long Id)
        {
            return this._adminRepository.GetAdminUserById(Id);
        }
        #endregion

        #region Get Admin User By Email Id
        /// <summary>
        /// Get Admin User By Id
        /// </summary>
        /// <returns></returns>
        public AdminUser GetAdminUserByEmailId(string email)
        {
            return this._adminRepository.GetAdminUserByEmailId(email);
        }
        #endregion

        #region Verify User Name
        /// <summary>
        /// Verify Sub Domain
        /// </summary>
        /// <param name="subDomain"></param>
        /// <returns></returns>
        public AdminUser VerifyUserName(string subDomain, long userId)
        {
            AdminUser _user = new AdminUser();
            //check the Sub Domain availability
            if (!this._adminRepository.VerifyUserName(subDomain, userId))
            {
                _user.OperationStatus = StatusType.Failure;
            }
            else
            {
                _user.OperationStatus = StatusType.Success;
            }
            return _user;
        }
        #endregion

        #region Admin Sign In
        /// <summary>
        /// Admin Sign In
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public AdminUser AdminSignIn(AdminUser admin)
        {
            //assign password to temporary variable
            string oPassword = admin.AdminPassword;

            //Get the Admin Details
            admin = this._adminRepository.GetAdminDetailsByUserName(admin);

            if (admin != null && admin.AdminUserId > 0)
            {
                //if (admin.IsConfirmed)
                //{
                //    encrypt the entered password with the existing salt and compare 
                //    with the encrypted password from DB                  
                string encPassword = Utilities.Utilities.EncryptPassword(oPassword, admin.AdminSalt);
                if (encPassword.Equals(admin.AdminPassword))
                {
                    //assign success status
                    admin.OperationStatus = StatusType.Success;
                }
                else
                {
                    //Incorrect Password
                    admin.OperationStatus = StatusType.Failure;
                    throw new RulesException("Password", "Incorrect Password.", admin);
                }
            }
            else
            {
                //Invalid Admin details
                admin.OperationStatus = StatusType.Failure;
                throw new RulesException("UserName", "UserName or  Password is invalid.", admin);
            }

            //Return the admin datas 
            return admin;
        }
        #endregion

        #region Get Admin Roles
        /// <summary>
        /// Get Admin Roles
        /// </summary>
        /// <returns></returns>
        /// 

        public IQueryable<AdminRole> GetAllAdminRoles(string categoryid)
        {
            return this._adminRepository.GetAllAdminRoles(categoryid);
        }
        #endregion

        public List<AdminUserRole> GetAllAdminProjectRole(long userId)
        {
            return this._adminRepository.GetAllAdminProjectRole(userId);
        }

        public IQueryable<AdminCategory> GetAllAdminCategories(long? userid)
        {
            return this._adminRepository.GetAllAdminCategories(userid);
        }

        public AdminUserRole SaveProjectRole(AdminUserRole _adminuserrole)
        {
            return this._adminRepository.SaveProjectRole(_adminuserrole);
        }

        public string GetAdminRoleByProjectIdAndAdminUserId(long projectId, long adminUserId)
        {
            return this._adminRepository.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
        }

        #region Save Admin User Details
        /// <summary>
        /// Save Admin User Details
        /// </summary>
        /// <param name="Admin User"></param>
        /// <returns></returns>
        public AdminUser SaveAdminUser(AdminUser adminUser)
        {
            //Store the entered password in temporary variable
            string tmpPassword = adminUser.AdminPassword;
            bool _isNew = false;
            if (adminUser != null)
            {
                _isNew = true;
                //get the salt value
                adminUser.AdminSalt = Utilities.Utilities.CreateSalt(6);

                //Encrypt the Password and assign
                adminUser.AdminPassword = Utilities.Utilities.EncryptPassword(tmpPassword, adminUser.AdminSalt);
            }
            adminUser = this._adminRepository.SaveAdminUser(adminUser);
            //_isNew = adminUser.NotifyUser;
            if (_isNew && adminUser.NotifyUser)
            {
                string _adminRole = this._adminRepository.GetAdminRoleById(adminUser.AdminRoleId);

                //adminUser.OperationStatus = StatusType.Success;
                MailDetails maildetail = new MailDetails();

                if (maildetail != null)
                {
                    maildetail.MailToAddress = adminUser.AdminEmailAddress;
                    maildetail.MailFromAddress = ConfigurationManager.AppSettings["AdminEmail"];
                    maildetail.MailSubject = "Welcome to SpanControl";
                    maildetail.MailBody = "<table border='1' cellpadding='0' cellspacing='0'><tr><td>UserName</td><td>" + maildetail.MailToAddress + "</td></tr><tr><td>Password</td><td>" + tmpPassword + "</td></tr>";
                    maildetail.MailBody += "</table>";
                    _emailService.SendEmail(maildetail);
                }
            }
            else
            {
                if (adminUser.NotifyUser)
                {
                    string _adminRole = this._adminRepository.GetAdminRoleById(adminUser.AdminRoleId);
                    //adminUser.OperationStatus = StatusType.Success;
                    MailDetails maildetail = new MailDetails();
                    if (maildetail != null)
                    {
                        maildetail.MailToAddress = adminUser.AdminEmailAddress;
                        maildetail.MailFromAddress = ConfigurationManager.AppSettings["AdminEmail"];
                        maildetail.MailSubject = "Welcome to SpanControl";
                        maildetail.MailBody = "<table border='1' cellpadding='0' cellspacing='0'><tr><td>UserName</td><td>" + adminUser.AdminUserName + "</td></tr><tr><td>Password</td><td>Already Sent</td></tr>";
                        maildetail.MailBody += "</table>";
                        _emailService.SendEmail(maildetail);
                    }
                }

            }

            return adminUser;

        }
        #endregion

        #region Edit Savepassword

        public AdminUser SavePassword(AdminUser adminuser)
        {
            if (adminuser != null && adminuser.AdminUserId > 0)
            {

                adminuser.AdminSalt = Utilities.Utilities.CreateSalt(6);
                string temppassword = adminuser.AdminPassword;
                adminuser.AdminPassword = Utilities.Utilities.EncryptPassword(temppassword, adminuser.AdminSalt);
                adminuser = this._adminRepository.SavePassword(adminuser);
            }
            return adminuser;

        }
        #endregion

        #region Delete Admin User
        /// <summary>
        /// Delete Admin User
        /// </summary>
        /// <returns></returns>
        public bool DeleteAdminUser(long Id)
        {
            return this._adminRepository.DeleteAdminUser(Id);
        }
        #endregion

        #region Delete User project Role
        /// <summary>
        /// Delete Admin User
        /// </summary>
        /// <returns></returns>
        public bool DeleteProjectRole(long Id)
        {
            return this._adminRepository.DeleteProjectRole(Id);
        }
        #endregion

        #region Get Admin Projects
        /// <summary>
        /// Get Admin Roles
        /// </summary>
        /// <returns></returns>
        public IQueryable<AdminProject> GetAllAdminProjects(long? categoryid)
        {
            return this._adminRepository.GetAllAdminProjects(categoryid);
        }
        #endregion

        public AdminUser GetAdminDetailsByUserNameApproved(string adminUser)
        {
            return this._adminRepository.GetAdminDetailsByUserNameApproved(adminUser);
        }

        #region Get UWAdmin State
        /// <summary>
        /// Get UWAdmin State
        /// </summary>
        /// <returns></returns>
        public List<LeadLogState> GetAllAdminState(LeadLogState state)
        {
            return this._adminRepository.GetAllAdminState();
        }
        #endregion

        #region Save Activity Log

        public AdminActivityLog SaveActivityLog(AdminActivityLog adminActivityLog)
        {
            return this._adminRepository.SaveActivityLog(adminActivityLog);
        }
        #endregion

        #region   Get All Activity Log
        public List<AdminActivityLog> GetAllActivityLog()
        {
            return this._adminRepository.GetAllActivityLog();
        }
        #endregion


        #region GetAll ActivityLog ByUserID

        public List<AdminActivityLog> GetActivityLogByUserID(long Id)
        {
            return this._adminRepository.GetActivityLogByUserID(Id);
        }

        #endregion

        #region Mobile Verification Code

        #region Send Verification Code
        /// <summary>
        /// Send Verification Code
        /// </summary>
        /// <param name="mobileverification"></param>
        /// <returns></returns>
        public MobileVerification SendVerificationCode(MobileVerification mobileverification)
        {
            AdminUser _user = this._adminRepository.GetAdminUserById(mobileverification.UserId);
            mobileverification.VerificationCode = Utilities.Utilities.GetMobileVerificationCode();

            mobileverification = this._adminRepository.SendVerificationCode(mobileverification);

            bool isTextLive = Utilities.Utilities.GetBool(ConfigurationManager.AppSettings["IsTextLive"]);

            if (mobileverification.MobileverificationId > 0 && !string.IsNullOrEmpty(mobileverification.UserEmail) && _user != null && _user.AdminUserId > 0 && !string.IsNullOrEmpty(_user.VerificationCodType) && (!isTextLive || (isTextLive && _user.VerificationCodType == VerificationCodeType.EMAIL.ToString())))
            {
                MailDetails _mailDetails = new MailDetails();
                _mailDetails.UserId = mobileverification.UserId;
                _mailDetails.MailToAddress = (_user.VerificationCodType == VerificationCodeType.SMS.ToString() || (_user.VerificationCodType == VerificationCodeType.EMAIL.ToString())) && !string.IsNullOrEmpty(_user.AlternateAdminEmailAddress) ? _user.AlternateAdminEmailAddress : mobileverification.UserEmail;
                if (mobileverification.ProductName == "IPF")
                {
                    _mailDetails.MailSubject = "IPF SC Mobile Verification Code";
                }
                else
                {
                    _mailDetails.MailSubject = "Mobile Verification Code";
                }
                _mailDetails.MailBody = "Verification Code: " + mobileverification.VerificationCode;
                _emailService.SendEmail(_mailDetails);
                mobileverification.OperationStatus = "Success";
            }
            if (mobileverification.MobileverificationId > 0 && isTextLive)
            {
                mobileverification.MessageType = "VerificationCode";
                mobileverification = !Utilities.Utilities.GetBool(ConfigurationManager.AppSettings["UsePlivoToText"]) ? TwillioMessageService.SendMessage(mobileverification) : PlivoMessageService.SendPlivoMessage(mobileverification);
                mobileverification = UpdateMobileVerificationDetails(mobileverification);
            }
            return mobileverification;
        }
        #endregion

        #region Update Mobile Verification Details
        /// <summary>
        ///  Update Mobile Verification Details
        /// </summary>
        /// <param name="mobileverification"></param>
        /// <returns></returns>
        public MobileVerification UpdateMobileVerificationDetails(MobileVerification mobileverification)
        {
            return this._adminRepository.UpdateMobileVerificationDetails(mobileverification);
        }
        #endregion

        #region Verify Mobile Code
        /// <summary>
        /// Verify Mobile Code
        /// </summary>
        /// <param name="mobileverification"></param>
        /// <returns></returns>
        public bool VerifyMobileCode(MobileVerification mobileverification)
        {
            return this._adminRepository.VerifyMobileCode(mobileverification);
        }
        #endregion

        #region Get Verification Code By UniqueId
        /// <summary>
        /// Get Verification Code By UniqueId
        /// </summary>
        /// <returns></returns>
        public string GetVerificationCodeByUniqueId(string uniqueId)
        {
            return this._adminRepository.GetVerificationCodeByUniqueId(uniqueId);
        }
        #endregion

        #region Update Mobile Verification Status
        /// <summary>
        ///  Update Mobile Verification Status
        /// </summary>
        /// <param name="UniqueId"></param>
        /// <param name="SmsStatus"></param>
        /// <returns></returns>
        public bool UpdateMobileVerificationStatus(string uniqueId)
        {
            return this._adminRepository.UpdateMobileVerificationStatus(uniqueId);
        }
        #endregion

        #endregion

        #region GetAllIpAddressList
        /// <summary>
        /// GetAllIpAddressList
        /// </summary>
        /// <returns></returns>
        public List<AdminIpAddress> GetAllIpAddressList()
        {
            return this._adminRepository.GetAllIpAddressList();
        }
        #endregion

        #region GetIpAddressDetailsById
        /// <summary>
        /// Get Ip Address Details By Id
        /// </summary>
        /// <param name="ipAddressId"></param>
        /// <returns></returns>
        public AdminIpAddress GetIpAddressDetailsById(long ipAddressId)
        {
            return this._adminRepository.GetIpAddressDetailsById(ipAddressId);
        }
        #endregion

        #region Delete Ip Address Details By Id
        /// <summary>
        /// DeleteIpAddressDetailsById
        /// </summary>
        /// <param name="ipAddressId"></param>
        /// <returns></returns>
        public bool DeleteIpAddressDetailsById(long ipAddressId, string ipAddress, string emailAddress, string projectName, bool ismultiple, string ipName)
        {
            var isDeletedStatus = this._adminRepository.DeleteIpAddressDetailsById(ipAddressId, ismultiple);
            if (isDeletedStatus) // send sms or email to all admin user
            {
                SendSmsOrEmailToAdmin("Deleted", ipAddress, emailAddress, projectName, ipName);
            }
            return isDeletedStatus;
        }
        #endregion

        #region Save Ip Address Details By Id
        /// <summary>
        /// SaveIpAddressDetailsById
        /// </summary>
        /// <param name="adminIpAddress"></param>
        /// <returns></returns>
        public bool SaveIpAddressDetailsById(AdminIpAddress adminIpAddress)
        {
            var ipAddressId = adminIpAddress.IpAddressId;
            string emailAddress = adminIpAddress.EmailAddress;
            var isUpdateStatus = this._adminRepository.SaveIpAddressDetailsById(adminIpAddress);
            if (isUpdateStatus) // send sms or email to all Admin user
            {
                SendSmsOrEmailToAdmin(ipAddressId > 0 ? "Updated" : "Added", adminIpAddress.IpAddress, emailAddress, adminIpAddress.ProjectName, adminIpAddress.IpName);
            }
            return isUpdateStatus;
        }
        #endregion

        #region GetStaticBizAdminProjects
        /// <summary>
        /// GetStaticBizAdminProjects
        /// </summary>
        /// <returns></returns>
        public List<BizAdminProjects> GetStaticBizAdminProjects()
        {
            return this._adminRepository.GetStaticBizAdminProjects();
        }
        #endregion

        #region  Add Admin
        /// <summary>
        ///  Add Admin
        /// </summary>
        /// <returns></returns>
        public bool AddAdmin(long Id)
        {
            return this._adminRepository.AddAdmin(Id);
        }
        #endregion

        #region  Remove Admin
        /// <summary>
        /// Remove Admin
        /// </summary>
        /// <returns></returns>
        public bool RemoveAdmin(long Id)
        {
            return this._adminRepository.RemoveAdmin(Id);
        }
        #endregion


        #region SendSmsOrEmailToAdmin
        /// <summary>
        /// SendSmsOrEmailToAdmin
        /// </summary>
        /// <param name="status"></param>
        /// <param name="ipAddress"></param>
        /// <param name="emailAddress"></param>
        private void SendSmsOrEmailToAdmin(string status, string ipAddress, string emailAddress, string projectName, string iPName)
        {
            var adminList = this._adminRepository.GetAllAdminList();
            if (adminList != null && adminList.Any())
            {
                bool isTextLive = Utilities.Utilities.GetBool(ConfigurationManager.AppSettings["IsTextLive"]);
                foreach (var item in adminList)
                {
                    if (!isTextLive && !string.IsNullOrEmpty(item.AdminEmailAddress))
                    {
                        MailDetails _mailDetails = new MailDetails();
                        _mailDetails.MailToAddress = item.AdminEmailAddress;
                        _mailDetails.MailSubject = projectName + " IP Address - " + (!string.IsNullOrEmpty(status) ? status : "");
                        _mailDetails.MailBody = projectName + " - " + emailAddress + " " + (!string.IsNullOrEmpty(status) ? char.ToLower(status[0]) + status.Substring(1) : "") + " " + ipAddress + " " + iPName;
                        _emailService.SendEmail(_mailDetails);
                    }
                    else if (isTextLive)
                    {
                        if (item.PhoneNumber != ConfigurationManager.AppSettings["SkipIPPhoneNumber"])
                        {
                            if (!Utilities.Utilities.GetBool(ConfigurationManager.AppSettings["UsePlivoToText"]))
                            {
                                TwillioMessageService.SendMessageToAdmin(item.PhoneNumber, emailAddress, ipAddress, status, projectName, iPName);
                            }
                            else
                            {
                                PlivoMessageService.SendPlivoMessageToAdmin(item.PhoneNumber, emailAddress, ipAddress, status, projectName, iPName);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Save Activity Log Details
        /// <summary>
        /// Save Activity Log Details
        /// </summary>
        /// <param name="scActivityLogDetails"></param>
        /// <returns></returns>
        public bool SaveActivityLogDetails(ScActivityLog scActivityLogDetails)
        {
            return this._adminRepository.SaveActivityLogDetails(scActivityLogDetails);
        }
        #endregion

        #region Get All Activity Log By Admin User Id
        /// <summary>
        /// Gets the activity log by user identifier.
        /// </summary>
        /// <param name="adminUserId">The admin user identifier.</param>
        /// <returns></returns>
        public List<ScActivityLog> GetActivityLogByAdminUserId(long adminUserId)
        {
            return _adminRepository.GetActivityLogByAdminUserId(adminUserId);
        }
        #endregion

        #region Get Admin Activity Log Count
        /// <summary>
        /// Gets the admin activity log count.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns></returns>
        public int GetAdminActivityLogCount(JQueryDataTableParamModel param)
        {
            return _adminRepository.GetAdminActivityLogCount(param);
        }
        #endregion

        #region Get Admin Activity Log List
        /// <summary>
        /// Gets the admin activity log list.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns></returns>
        public List<ScActivityLog> GetAdminActivityLogList(JQueryDataTableParamModel param)
        {
            return _adminRepository.GetAdminActivityLogList(param);
        }
        #endregion

        #region Get All Admin projects
        /// <summary>
        /// Gets all projects.
        /// </summary>
        /// <returns></returns>
        public List<AdminProject> GetAllProjects()
        {
            return _adminRepository.GetAllProjects();
        }
        #endregion
        #region IsProjectIpAlreadyExists
        /// <summary>
        /// IsProjectIpAlreadyExists
        /// </summary>
        /// <param name="projectIp"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public bool IsProjectIpAlreadyExists(long ipAddressId, string projectIp, string project)
        {
            return _adminRepository.IsProjectIpAlreadyExists(ipAddressId, projectIp, project);
        }
        #endregion

        #region Get Project Name By Project Id
        public AdminProject GetProjectNameByProjectId(int projectId)
        {
            return _adminRepository.GetProjectNameByProjectId(projectId);
        }
        #endregion

        #region Get Admin Support Users By Project Id
        public List<AdminUser> GetAdminSupportUsersByProjectId(int projectId)
        {
            return _adminRepository.GetAdminSupportUsersByProjectId(projectId);
        }
        #endregion

        #region Create CampainDetail
        public void CreateCampainDetails(CampaignDetails campaignDetails)
        {
            _adminRepository.CreateCampainDetails(campaignDetails);

            if (campaignDetails != null && campaignDetails.IsBatchStatus == false)
            {
                if (campaignDetails.SupportUserList != null && campaignDetails.SupportUserList.Count > 0)
                {
                    AdminUser adminstratorDetail = this._adminRepository.GetAdminUserById(campaignDetails.AdminUserId);
                    foreach (var emailSendObjUser in campaignDetails.CampaignSupportUserDetailsList)
                    {
                        AdminUser sendEmailtoSupportUserDetails = this._adminRepository.GetAdminUserById(emailSendObjUser.SupportUserId);
                        MailDetails mailDetails = this._emailService.GetMailTemplateByTemplateId(Constants.CampaignCreatetoSupportUser);

                        if (mailDetails != null)
                        {
                            mailDetails.MailToAddress = sendEmailtoSupportUserDetails.AdminEmailAddress;
                            //replace contents

                            //Mail Subject
                            mailDetails.MailSubject = mailDetails.MailSubject.Replace(Constants.CampaignName, campaignDetails.CampaignName);
                            mailDetails.MailSubject = mailDetails.MailSubject.Replace(Constants.AdminUserName, adminstratorDetail.AdminUserName);

                            //Mail Body
                            mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.CampaignName, campaignDetails.CampaignName);
                            mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.NoOfLeadsAssigned, emailSendObjUser.NoOfUserAssigned.ToString());
                            mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.Goals, campaignDetails.Goals);
                            string startdate = string.Empty;
                            string enddate = string.Empty;
                            if (campaignDetails.CampaignStartDate != null && campaignDetails.CampaignStartDate > DateTime.MinValue)
                            {
                                startdate = Utilities.Utilities.GetDateTime(campaignDetails.CampaignStartDate).ToString("MM/dd/yyyy");
                            }
                            if (campaignDetails.CampaignEndDate != null && campaignDetails.CampaignEndDate > DateTime.MinValue)
                            {
                                enddate = Utilities.Utilities.GetDateTime(campaignDetails.CampaignEndDate).ToString("MM/dd/yyyy");
                            }
                            string campaignPeroid = startdate + " to " + enddate;
                            mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.CampaignPeroid, campaignPeroid);
                            mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.SupportUserName, sendEmailtoSupportUserDetails.AdminUserName);
                            mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.AdminUserName, adminstratorDetail.AdminUserName);
                            mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.redirectLink, ConfigurationManager.AppSettings[Constants.MainControlURL].ToString());
                            this._emailService.SendEmail(mailDetails);
                        }
                    }
                }
            }
            else
            {
                SendEmailtoManagerBulkUploadFileProcessing(campaignDetails);
            }
        }
        #endregion

        #region Get Campaign Details by  Admin User Id
        public List<CampaignDetails> GetCampaignDetails(JQueryDataTableParamModel param)
        {
            return this._adminRepository.GetCampaignDetails(param);
        }
        #endregion

        #region Get Campaign Short Details by  Campain Detail User Id
        public CampaignDetails GetCampaignShortDetailsByCampaignId(long campaignId)
        {
            return this._adminRepository.GetCampaignShortDetailsByCampaignId(campaignId);
        }
        #endregion

        #region  Update Campaign Extend Date
        public bool UpdateCampaignExtendDate(CampaignDetails campaignDetails)
        {
            return this._adminRepository.UpdateCampaignExtendDate(campaignDetails);
        }
        #endregion

        #region Update Campaign Pause Status
        public bool UpdateCampaignPauseStatus(long campaignDetailsId, bool isPaused)
        {
            return this._adminRepository.UpdateCampaignPauseStatus(campaignDetailsId, isPaused);
        }
        #endregion

        #region Update Campaign Suspend Status
        public bool UpdateCampaignSuspendStatus(long campaignDetailsId, bool isSuspend)
        {
            return this._adminRepository.UpdateCampaignSuspendStatus(campaignDetailsId, isSuspend);
        }
        #endregion

        #region Save Request To File Upload Campaign
        public void SaveRequestToFileUploadCampaign(CampaignDetails campaignDetails)
        {
            _adminRepository.CreateCampainDetails(campaignDetails);
            AdminUser adminstratorDetail = this._adminRepository.GetAdminUserById(campaignDetails.AdminUserId);
            MailDetails mailDetails = this._emailService.GetMailTemplateByTemplateId(Constants.RequesttoTechTeamforFileUpload);

            AdminProject adminProject = this._adminRepository.GetProjectNameByProjectId(campaignDetails.ProductId);

            if (mailDetails != null)
            {
                mailDetails.MailToAddress = adminProject.TechnicalTeamEmail;
                //replace contents

                //Mail Subject
                mailDetails.MailSubject = mailDetails.MailSubject.Replace(Constants.CampaignName, campaignDetails.CampaignName);
                mailDetails.MailSubject = mailDetails.MailSubject.Replace(Constants.AdminUserName, adminstratorDetail.AdminUserName);
                mailDetails.MailSubject = mailDetails.MailSubject.Replace(Constants.ProductName, campaignDetails.ProductName);

                //Mail Body
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.CampaignName, campaignDetails.CampaignName);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.AdminUserName, adminstratorDetail.AdminUserName);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.ProductName, campaignDetails.ProductName);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.DemographicInformation, campaignDetails.DemoGrapicInformation);

                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.redirectLink, ConfigurationManager.AppSettings[Constants.MainControlURL].ToString());

                this._emailService.SendEmail(mailDetails);
            }
        }
        #endregion

        #region Delete Campaign
        public bool DeleteCampaign(long campaignDetailsId)
        {
            return this._adminRepository.DeleteCampaign(campaignDetailsId);
        }
        #endregion

        #region Get Campaign Upload Details by  Admin User Id
        public List<CampaignDetails> GetCampaignUploadRequestDetails()
        {
            return this._adminRepository.GetCampaignUploadRequestDetails();
        }
        #endregion

        #region  Update Discarded Request Reason
        public bool UpdateDiscardedRequestReason(CampaignDetails campaignDetails)
        {
            this._adminRepository.UpdateDiscardedRequestReason(campaignDetails);
            CampaignDetails mailSendCampaignDetails = this._adminRepository.GetCampaignShortDetailsByCampaignId(campaignDetails.CampaignDetailsId);
            AdminUser adminstratorDetail = this._adminRepository.GetAdminUserById(mailSendCampaignDetails.AdminUserId);
            MailDetails mailDetails = this._emailService.GetMailTemplateByTemplateId(Constants.DiscaredCampaignRequest);


            if (mailDetails != null)
            {
                mailDetails.MailToAddress = adminstratorDetail.AdminEmailAddress;
                //replace contents

                //Mail Subject
                mailDetails.MailSubject = mailDetails.MailSubject.Replace(Constants.CampaignName, mailSendCampaignDetails.CampaignName);

                //Mail Body
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.CampaignName, mailSendCampaignDetails.CampaignName);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.AdminUserName, adminstratorDetail.AdminUserName);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.ProductName, mailSendCampaignDetails.ProductName);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.DemographicInformation, mailSendCampaignDetails.DemoGrapicInformation);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.Reason, campaignDetails.DiscardedReason);

                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.redirectLink, ConfigurationManager.AppSettings[Constants.MainControlURL].ToString());


                this._emailService.SendEmail(mailDetails);
            }


            return true;
        }
        #endregion

        #region Save Tech Team File Uploaded
        public void SaveTechTeamFileUploaded(CampaignDetails campaignDetails)
        {
            this._adminRepository.SaveTechTeamFileUploaded(campaignDetails);

            CampaignDetails mailSendCampaignDetails = this._adminRepository.GetCampaignShortDetailsByCampaignId(campaignDetails.CampaignDetailsId);
            AdminUser adminstratorDetail = this._adminRepository.GetAdminUserById(mailSendCampaignDetails.AdminUserId);

            MailDetails mailDetails = this._emailService.GetMailTemplateByTemplateId(Constants.FileUploadedTechTeam);


            if (mailDetails != null)
            {
                mailDetails.MailToAddress = adminstratorDetail.AdminEmailAddress;
                //replace contents

                //Mail Subject
                mailDetails.MailSubject = mailDetails.MailSubject.Replace(Constants.CampaignName, campaignDetails.CampaignName);

                //Mail Body
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.CampaignName, campaignDetails.CampaignName);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.AdminUserName, adminstratorDetail.AdminUserName);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.ProductName, campaignDetails.ProductName);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.DemographicInformation, campaignDetails.DemoGrapicInformation);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.Notes, campaignDetails.Notes);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.UploaderName, !string.IsNullOrWhiteSpace(campaignDetails.UploaderName) ? campaignDetails.UploaderName : string.Empty);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.redirectLink, ConfigurationManager.AppSettings[Constants.MainControlURL].ToString());
                this._emailService.SendEmail(mailDetails);
            }


        }
        #endregion

        #region Get Campaign Short Details by  Campain Detail User Id
        public LeadDetails GetLeadDetailsbyCampainDetailIdandSupportUserId(long campaignId, long supportUserId)
        {
            return this._adminRepository.GetLeadDetailsbyCampainDetailIdandSupportUserId(campaignId, supportUserId);
        }
        #endregion

        #region Get Lead Details By Skipped Record
        public LeadDetails GetLeadDetailsBySkippedRecord(long campaignId, long supportUserId)
        {
            return this._adminRepository.GetLeadDetailsBySkippedRecord(campaignId, supportUserId);
        }
        #endregion

        #region Get Campaign Short Details by  Campain Detail User Id
        public LeadDetails GetLeadDetailsForCommunicationByCampainDetailIdandSupportUserId(long campaignId, long supportUserId)
        {
            return this._adminRepository.GetLeadDetailsForCommunicationByCampainDetailIdandSupportUserId(campaignId, supportUserId);
        }
        #endregion


        #region Get Goals By Campaign Details Id
        public CampaignDetails GetGoalsByCampaignDetailsId(long campaignId)
        {
            return this._adminRepository.GetGoalsByCampaignDetailsId(campaignId);
        }
        #endregion

        #region Save Campaign Communication
        /// <summary>
        /// Save Activity Log Details
        /// </summary>
        /// <param name="scActivityLogDetails"></param>
        /// <returns></returns>
        public LeadCommunication SaveCampaignCommunication(LeadCommunication leadCommunication)
        {
            return this._adminRepository.SaveCampaignCommunication(leadCommunication);
        }
        #endregion

        #region Get Campaign Lead Activity List By Campaign Detail Id
        public List<LeadDetails> GetCampaignLeadActivityListByCampaignDetailId(long campaignDetailId, long supportUserId)
        {
            return this._adminRepository.GetCampaignLeadActivityListByCampaignDetailId(campaignDetailId, supportUserId);
        }
        #endregion

        #region Get All States
        public List<State> GetAllStates()
        {
            return this._adminRepository.GetAllStates();
        }
        #endregion

        #region Get Campaign And Support User And Assigned Details
        public CampaignDetails GetCampaignAndSupportUserAndAssignedDetails(long campaignId, long supportUserId)
        {
            return _adminRepository.GetCampaignAndSupportUserAndAssignedDetails(campaignId, supportUserId);
        }
        #endregion

        #region Update Last Lead Id During Back
        public bool UpdateLastLeadIdDuringBack(long campaignId, long supportUserId)
        {
            return _adminRepository.UpdateLastLeadIdDuringBack(campaignId, supportUserId);
        }
        #endregion

        #region Update Last Lead Id During Skip
        public bool UpdateLastLeadIdDuringSkip(long campaignId, long supportUserId)
        {
            return _adminRepository.UpdateLastLeadIdDuringSkip(campaignId, supportUserId);
        }
        #endregion

        #region Get Lead Details List By StateId
        public LeadDetails GetLeadDetailsListByStateId(long campaignId, long supportUserId, string stateCode)
        {
            return _adminRepository.GetLeadDetailsListByStateId(campaignId, supportUserId, stateCode);
        }
        #endregion

        #region Get Lead Details List By Timezone
        public LeadDetails GetLeadDetailsListByTimezone(long campaignId, long supportUserId, string timeZone)
        {
            return _adminRepository.GetLeadDetailsListByTimezone(campaignId, supportUserId, timeZone);
        }
        #endregion

        #region Get Campaign Previous Activity List
        public List<LeadCommunication> GetCampaignPreviousActivityList(long campaignAssignedDetailId)
        {
            return _adminRepository.GetCampaignPreviousActivityList(campaignAssignedDetailId);
        }
        #endregion

        #region Get Campaign Details First Count
        public long GetCampaignDetailsFirstCount(LeadDetailsSearchOption leadDetailsSearchOption)
        {
            return _adminRepository.GetCampaignDetailsFirstCount(leadDetailsSearchOption);
        }
        #endregion

        #region Get Campaign Previous Activity By Campaign Lead Activity Id
        public LeadCommunication GetCampaignPreviousActivityByCampaignLeadActivityId(long campaignLeadActivityId)
        {
            return _adminRepository.GetCampaignPreviousActivityByCampaignLeadActivityId(campaignLeadActivityId);
        }
        #endregion

        #region GetCampaignAssignedDetailsbyCampainDetailIdandSupportUserId

        public List<CampaignAssignedDetails> GetCampaignAssignedDetailsbyCampainDetailIdandSupportUserId(long campaignDetailId, long supportUserId)
        {
            return this._adminRepository.GetCampaignAssignedDetailsbyCampainDetailIdandSupportUserId(campaignDetailId, supportUserId);
        }

        #endregion

        #region UpdateCampaignDetails
        public List<CampaignAssignedDetails> UpdateCampaignDetails(List<CampaignAssignedDetails> CampaignAssignedDetailsList)
        {
            long campaignDetailsId = 0;
            if (CampaignAssignedDetailsList != null && CampaignAssignedDetailsList.Count > 0)
            {
                campaignDetailsId = CampaignAssignedDetailsList[0].CampaignDetailsId;
            }

            CampaignDetails campaignDetails = this._adminRepository.GetCampaignShortDetailsByCampaignId(campaignDetailsId);


            AdminUser adminstratorDetail = this._adminRepository.GetAdminUserById(campaignDetails.AdminUserId);

            var groupByCampaignAssignedDetails = CampaignAssignedDetailsList.GroupBy(x => x.SupportUserId)
                      .Select(y => new
                      {
                          SupportUserId = y.Key.ToString(),
                          NoOfUserAssigned = y.Select(m => m.SupportUserId).Count()
                      });

            List<CampaignAssignedDetails> dbcampaignAssignedDetailsList = new List<CampaignAssignedDetails>();
            dbcampaignAssignedDetailsList = this._adminRepository.UpdateCampaignDetails(CampaignAssignedDetailsList);

            foreach (var emailSendObjUser in groupByCampaignAssignedDetails)
            {

                AdminUser sendEmailtoSupportUserDetails = this._adminRepository.GetAdminUserById(Utilities.Utilities.GetLong(emailSendObjUser.SupportUserId));

                MailDetails mailDetails = this._emailService.GetMailTemplateByTemplateId(Constants.CampaignCreatetoSupportUser);

                if (mailDetails != null)
                {
                    mailDetails.MailToAddress = sendEmailtoSupportUserDetails.AdminEmailAddress;
                    //replace contents

                    //Mail Subject
                    mailDetails.MailSubject = mailDetails.MailSubject.Replace(Constants.CampaignName, campaignDetails.CampaignName);
                    mailDetails.MailSubject = mailDetails.MailSubject.Replace(Constants.AdminUserName, adminstratorDetail.AdminUserName);

                    //Mail Body
                    mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.CampaignName, campaignDetails.CampaignName);
                    mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.NoOfLeadsAssigned, emailSendObjUser.NoOfUserAssigned.ToString());
                    mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.Goals, campaignDetails.Goals);
                    string startdate = string.Empty;
                    string enddate = string.Empty;
                    if (campaignDetails.CampaignStartDate != null && campaignDetails.CampaignStartDate > DateTime.MinValue)
                    {
                        startdate = Utilities.Utilities.GetDateTime(campaignDetails.CampaignStartDate).ToString("MM/dd/yyyy");
                    }
                    if (campaignDetails.CampaignEndDate != null && campaignDetails.CampaignEndDate > DateTime.MinValue)
                    {
                        enddate = Utilities.Utilities.GetDateTime(campaignDetails.CampaignEndDate).ToString("MM/dd/yyyy");
                    }
                    string campaignPeroid = startdate + " to " + enddate;
                    mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.CampaignPeroid, campaignPeroid);
                    mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.SupportUserName, sendEmailtoSupportUserDetails.AdminUserName);
                    mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.AdminUserName, adminstratorDetail.AdminUserName);

                    if (campaignDetails.ProductId == (int)Project.ExpressTruckTax)
                    {
                        mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.redirectLink, ConfigurationManager.AppSettings[Constants.MainControlURL].ToString());
                    }


                    this._emailService.SendEmail(mailDetails);
                }

            }
            return dbcampaignAssignedDetailsList;
        }
        #endregion

        #region Update Lead Status
        public bool UpdateLeadStatus(long campaignLeadActivityId, string leadStatus)
        {
            return _adminRepository.UpdateLeadStatus(campaignLeadActivityId, leadStatus);
        }
        #endregion

        #region Suspend Campaign
        public bool SuspendCampaign(CampaignDetails campaignDetails)
        {
            return _adminRepository.SuspendCampaign(campaignDetails);
        }
        #endregion

        #region Update Campaign Assigned Details Business Name By Campaign Assigned Details Id
        public bool UpdateCampaignAssignedDetailsBusinessNameByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string businessName)
        {
            return _adminRepository.UpdateCampaignAssignedDetailsBusinessNameByCampaignAssignedDetailsId(campaignAssignedDetailsId, businessName);
        }
        #endregion

        #region Update Campaign Assigned Details Name By Campaign Assigned Details Id
        public bool UpdateCampaignAssignedDetailsNameByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string name)
        {
            return _adminRepository.UpdateCampaignAssignedDetailsNameByCampaignAssignedDetailsId(campaignAssignedDetailsId, name);
        }
        #endregion

        #region Update Campaign Assigned Details Email By Campaign Assigned Details Id
        public bool UpdateCampaignAssignedDetailsEmailByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string email)
        {
            return _adminRepository.UpdateCampaignAssignedDetailsEmailByCampaignAssignedDetailsId(campaignAssignedDetailsId, email);
        }
        #endregion

        #region Update Campaign Assigned Details Phone By Campaign Assigned Details Id
        public bool UpdateCampaignAssignedDetailsPhoneByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string phone)
        {
            return _adminRepository.UpdateCampaignAssignedDetailsPhoneByCampaignAssignedDetailsId(campaignAssignedDetailsId, phone);
        }
        #endregion

        #region Update Campaign Assigned Details Address By Campaign Assigned Details Id
        public bool UpdateCampaignAssignedDetailsAddressByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string address)
        {
            return _adminRepository.UpdateCampaignAssignedDetailsAddressByCampaignAssignedDetailsId(campaignAssignedDetailsId, address);
        }
        #endregion

        #region Update Campaign Assigned Details No of Trucks By Campaign Assigned Details Id
        public bool UpdateCampaignAssignedDetailsNofTrucksByCampaignAssignedDetailsId(long campaignAssignedDetailsId, long nofTrucks)
        {
            return _adminRepository.UpdateCampaignAssignedDetailsNofTrucksByCampaignAssignedDetailsId(campaignAssignedDetailsId, nofTrucks);
        }
        #endregion

        #region Update Campaign Assigned Details EIN By Campaign Assigned Details Id
        public bool UpdateCampaignAssignedDetailsEINByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string ein)
        {
            return _adminRepository.UpdateCampaignAssignedDetailsEINByCampaignAssignedDetailsId(campaignAssignedDetailsId, ein);
        }
        #endregion

        #region Get Campaign Recent Activity List
        public List<LeadCommunication> GetCampaignRecentActivityList(long campaignAssignedDetailsId)
        {
            return _adminRepository.GetCampaignRecentActivityList(campaignAssignedDetailsId);
        }
        #endregion

        #region Get All Admin Support Users
        public List<AdminUser> GetAllAdminSupportUsers()
        {
            return _adminRepository.GetAllAdminSupportUsers();
        }
        #endregion

        #region Get Campaign Details by  Admin User Id
        public List<CampaignDetails> GetCampaignDetailsByFilters(JQueryDataTableParamModel param)
        {
            return this._adminRepository.GetCampaignDetailsByFilters(param);
        }
        #endregion

        #region Get Campaign Details Count By Admin User Id

        public int GetCampaignDetailsCountByAdminUserId(JQueryDataTableParamModel param)
        {
            return this._adminRepository.GetCampaignDetailsCountByAdminUserId(param);
        }

        #endregion

        #region Save Additional Contacts
        public AdditionalContacts SaveAdditionalContacts(AdditionalContacts ContDetails)
        {
            return this._adminRepository.SaveAdditionalContacts(ContDetails);
        }
        #endregion

        #region Get Additional Contact Details By Additional Contacts Details Id
        public AdditionalContacts GetAdditionalContactDetailsByAdditionalContactsDetailsId(long additionalContactsDetailsId)
        {
            return this._adminRepository.GetAdditionalContactDetailsByAdditionalContactsDetailsId(additionalContactsDetailsId);
        }
        #endregion

        #region Delete Additional Contact By Additional Contacts Details Id
        public bool DeleteAdditionalContactByAdditionalContactsDetailsId(long additionalContactsDetailsId)
        {
            return this._adminRepository.DeleteAdditionalContactByAdditionalContactsDetailsId(additionalContactsDetailsId);
        }
        #endregion

        #region Get Campaign Details by  Admin User Id
        public List<CampaignDetails> GetCampaignSupportUserDetails(JQueryDataTableParamModel param)
        {
            return this._adminRepository.GetCampaignSupportUserDetails(param);
        }
        #endregion

        #region Get Campaign Details Last Count
        public long GetCampaignDetailsLastCount(LeadDetailsSearchOption leadDetailsSearchOption)
        {
            return _adminRepository.GetCampaignDetailsLastCount(leadDetailsSearchOption);
        }
        #endregion

        #region Get Additional Contact List By Campaign Assigned Details Id
        public LeadDetails GetAdditionalContactListByCampaignAssignedDetailsId(long campaignAssignedDetailsId)
        {
            return this._adminRepository.GetAdditionalContactListByCampaignAssignedDetailsId(campaignAssignedDetailsId);
        }
        #endregion

        #region Get Followup By Support User Id
        public List<LeadCommunication> GetFollowupBySupportUserId(long supportUserId, string followUpFilter)
        {
            return this._adminRepository.GetFollowupBySupportUserId(supportUserId, followUpFilter);
        }
        #endregion

        #region Get All Lead List By Support User Id
        public List<LeadDetails> GetAllLeadListBySupportUserId(long supportUserId, string searchBy, string value)
        {
            return this._adminRepository.GetAllLeadListBySupportUserId(supportUserId, searchBy, value);
        }
        #endregion

        #region Get Campaign Support User Details Count

        public int GetCampaignSupportUserDetailsCount(JQueryDataTableParamModel param)
        {
            return this._adminRepository.GetCampaignSupportUserDetailsCount(param);
        }
        #endregion

        #region Update Last Lead Id By Campaign Assigned Details Id
        public bool UpdateLastLeadIdByCampaignAssignedDetailsId(LeadCommunication leadCommunication)
        {
            return this._adminRepository.UpdateLastLeadIdByCampaignAssignedDetailsId(leadCommunication);
        }
        #endregion

        #region Update Previous Lead Id As Last Lead Id By Statecode
        public bool UpdatePreviousLeadIdAsLastLeadIdByStatecode(LeadDetailsSearchOption leadDetailsSearchOption)
        {
            return this._adminRepository.UpdatePreviousLeadIdAsLastLeadIdByStatecode(leadDetailsSearchOption);
        }
        #endregion

        #region Update Next Lead Id As Last Lead Id By Statecode
        public bool UpdateNextLeadIdAsLastLeadIdByStatecode(LeadDetailsSearchOption leadDetailsSearchOption)
        {
            return this._adminRepository.UpdateNextLeadIdAsLastLeadIdByStatecode(leadDetailsSearchOption);
        }
        #endregion

        //#region Get Campaign Details First Count By StateCode
        //public long GetCampaignDetailsFirstCountByStateCode(long campaignId, long supportUserId, string stateCode)
        //{
        //    return this._adminRepository.GetCampaignDetailsFirstCountByStateCode(campaignId, supportUserId, stateCode);
        //}
        //#endregion

        //#region Get Campaign Details Last Count By StateCode
        //public long GetCampaignDetailsLastCountByStateCode(long campaignId, long supportUserId, string stateCode)
        //{
        //    return this._adminRepository.GetCampaignDetailsFirstCountByStateCode(campaignId, supportUserId, stateCode);
        //}
        //#endregion

        #region Get Campaign Details Count By StateCode
        public bool GetCampaignDetailsCountByStateCode(long campaignId, long supportUserId, string stateCode)
        {
            return this._adminRepository.GetCampaignDetailsCountByStateCode(campaignId, supportUserId, stateCode);
        }
        #endregion

        #region Send Email to Manager Bulk Upload File Processing
        public void SendEmailtoManagerBulkUploadFileProcessing(CampaignDetails campaignDetails)
        {
            CampaignDetails mailSendCampaignDetails = this._adminRepository.GetCampaignShortDetailsByCampaignId(campaignDetails.CampaignDetailsId);
            AdminUser adminstratorDetail = this._adminRepository.GetAdminUserById(mailSendCampaignDetails.AdminUserId);

            MailDetails mailDetails = this._emailService.GetMailTemplateByTemplateId(Constants.FileUploadBatchProcess);


            if (mailDetails != null)
            {
                mailDetails.MailToAddress = adminstratorDetail.AdminEmailAddress;
                //replace contents

                //Mail Subject
                mailDetails.MailSubject = mailDetails.MailSubject.Replace(Constants.CampaignName, campaignDetails.CampaignName);

                //Mail Body
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.SupportUserName, adminstratorDetail.AdminUserName);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.CampaignName, campaignDetails.CampaignName);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.AdminUserName, adminstratorDetail.AdminUserName);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.ProductName, campaignDetails.ProductName);
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.NoOfLeadsAssigned, campaignDetails.NoOfLeads.ToString());
                mailDetails.MailBody = mailDetails.MailBody.Replace(Constants.FileName, campaignDetails.CampaignFileName);
                this._emailService.SendEmail(mailDetails);
            }
        }
        #endregion

        #region Get Campaign Details by  Campain Detail Id and Support User Id
        public List<LeadDetails> GetCampaignDetailsbyCampainDetailIdandSupportUserId(long campaignId, long supportUserId)
        {
            return this._adminRepository.GetCampaignDetailsbyCampainDetailIdandSupportUserId(campaignId, supportUserId);
        }
        #endregion

        #region Remove All Non Static IPs
        /// <summary>
        /// Remove All Non Static IPs
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public bool RemoveAllNonStaticIPs(string emailAddress)
        {
            bool isRemovedNonStaticIPs = this._adminRepository.RemoveAllNonStaticIPs();
            if (isRemovedNonStaticIPs)
            {
                SendSmsOrEmailToAdmin("Removed", "Non-static IPs", emailAddress, "All Projects", "");
            }
            return isRemovedNonStaticIPs;
        }
        #endregion

        #region regassign check exists Lead Id
        public void ReAssignedLeads(long campaignDetailId, long supportUserId)
        {
            this._adminRepository.ReAssignedLeads(campaignDetailId, supportUserId);
        }
        #endregion

        #region regassign check exists Lead Id
        public void UpdateReassignDetailsReset(LeadDetailsSearchOption leadDetailsSearchOption)
        {
            this._adminRepository.UpdateReassignDetailsReset(leadDetailsSearchOption);
        }
        #endregion

        #region Get Campaign Uploaded Time Details
        public CampaignDetails GetCampaignUploadedTimeDetails(CampaignDetails campaignDetails)
        {
            return this._adminRepository.GetCampaignUploadedTimeDetails(campaignDetails);
        }
        #endregion

        #region Get Campaign Lead Activity Details
        public LeadCommunication GetCampaignLeadActivityDetails(long campaignLeadActivityId)
        {
            return this._adminRepository.GetCampaignLeadActivityDetails(campaignLeadActivityId);
        }
        #endregion

        #region Save Group Member By EmailAddress
        public GroupMembers SaveGroupMemberByEmailAddress(string emailAddress)
        {
            GroupMembers groupMembers = new GroupMembers();
            groupMembers.EmailAddress = emailAddress;
            using (var reqClient = new ETTAPIClient())
            {
                //reqClient.EmailAddress = emailAddress;
                string requestTsnaUri = "User/SaveGroupMemberByEmailAddress/";
                ETTAPIRequestUtility.GenerateAuthHeader(reqClient, requestTsnaUri, "POST");
                var _settingresponse = reqClient.PostAsJsonAsync(requestTsnaUri, groupMembers).Result;
                if (_settingresponse != null && _settingresponse.IsSuccessStatusCode)
                {
                    groupMembers = _settingresponse.Content.ReadAsAsync<GroupMembers>().Result;
                }
            }
            return groupMembers;
        }
        #endregion

        #region Get User Comments by Email Address
        public Notes GetUserCommentsbyEmailAddress(string emailAddress)
        {
            Notes usercomments = new Notes();
            using (var reqClient = new ETTAPIClient())
            {
                string requestTsnaUri = "User/GetUserCommentsbyEmailAddress?emailAddress=" + Uri.EscapeDataString(emailAddress);
                ETTAPIRequestUtility.GenerateAuthHeader(reqClient, requestTsnaUri, "GET");
                var _settingresponse = reqClient.GetAsync(requestTsnaUri).Result;
                if (_settingresponse != null && _settingresponse.IsSuccessStatusCode)
                {
                    usercomments = _settingresponse.Content.ReadAsAsync<Notes>().Result;
                }
            }
            return usercomments;
        }
        #endregion
        #region
        public List<RecentReturns> GetRecentReturns(Guid userId)
        {
            return this._adminRepository.GetRecentReturns(userId);
        }
        #endregion

        #region
        public RecentReturns GetFollowUpDetails(long assignedDetailId)
        {
            return this._adminRepository.GetFollowUpDetails(assignedDetailId);
        }
        #endregion

        #region Update User Verification Code Type
        /// <summary>
        /// Update User Verification Code Type
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <param name="verificationCodeType"></param>
        /// <returns></returns>
        public bool UpdateUserVerificationCodeType(long adminUserId)
        {
            return _adminRepository.UpdateUserVerificationCodeType(adminUserId);
        }
        #endregion

        #region Get Transaction Report Details
        public List<TransactionReportDetails> GetTransactionReportDetails(TransactionReport transactionReport)
        {
            return _adminRepository.GetTransactionReportDetails(transactionReport);
        }
        #endregion

        #region Get Span Products
        public List<SpanProducts> GetSpanProducts(string connectionString)
        {
            return _adminRepository.GetSpanProducts(connectionString);
        }
        #endregion

        #region Get S3 Upload URL
        public List<TransactionReportDetails> GetS3UploadURL(string transactionId, string connectionString, int paymentProcessorType)
        {
            return _adminRepository.GetS3UploadURL(transactionId, connectionString, paymentProcessorType);
        }
        #endregion

        #region Get Span payment processors
        public List<PaymentProcessors> GetSpanPaymentProcessors(string connectionString)
        {
            return _adminRepository.GetSpanPaymentProcessors(connectionString);
        }
        #endregion

        #region Get all user payments

        public List<UserPayments> GetAllUserPayments()
        {
            return _adminRepository.GetAllUserPayments();
        }

        #endregion

        #region Get user payment by token id

        public UserPayments GetUserPaymentDetailByTokenId(Guid tokenId)
        {
            var userPayment = _adminRepository.GetUserPaymentDetailByTokenId(tokenId);
            userPayment.UserPaymentDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserPaymentDetail>>(userPayment.OrderDescription);
            return userPayment;
        }

        #endregion

        #region Save user payments

        public UserPayments SaveUserPayments(UserPayments userPayments)
        {
            return _adminRepository.SaveUserPayments(userPayments);
        }

        #endregion

        #region Get all Countries
        public List<Country> GetCountries()
        {
            return _adminRepository.GetCountries();
        }
        #endregion

        #region Prepare Payment Mail

        public PaymentTemplate GetPaymentMailHtml(UserPayments userPayments)
        {
            PaymentTemplate paymentTemplate = new PaymentTemplate();
            string totalColorCss = string.Empty;
            if (userPayments.Projectid == (int)Project.ExpressTaxFilings)
            {
                var projectDetail = _adminRepository.GetProjectNameByProjectId(userPayments.Projectid);
                var paymentTemplateDetail = _adminRepository.GetPaymentTemplateByTemplateId(Constants.PaymentTemplateIdTBS);
                paymentTemplate.MailTemplateHtml = paymentTemplateDetail.MailTemplateHtml;
                paymentTemplate.MailSubject = paymentTemplateDetail.MailSubject;
                paymentTemplate.MailTemplateHtml = paymentTemplate.MailTemplateHtml.Replace(Constants.TmplateConstPayLink, projectDetail.PaymentLink + userPayments.UserPaymentId.ToString());
                totalColorCss = "ee6c45";
            }
            if (userPayments.Projectid == (int)Project.ExpressTruckTax)
            {
                var projectDetail = _adminRepository.GetProjectNameByProjectId(userPayments.Projectid);
                var paymentTemplateDetail = _adminRepository.GetPaymentTemplateByTemplateId(Constants.PaymentTemplateIdETT);
                paymentTemplate.MailTemplateHtml = paymentTemplateDetail.MailTemplateHtml;
                paymentTemplate.MailSubject = paymentTemplateDetail.MailSubject;
                paymentTemplate.MailTemplateHtml = paymentTemplate.MailTemplateHtml.Replace(Constants.TmplateConstPayLink, projectDetail.PaymentLink + userPayments.UserPaymentId.ToString());
                totalColorCss = "0d4aa7";
            }
            string serviceDetails = string.Empty;

            foreach (var userPaymentDetail in userPayments.UserPaymentDetails)
            {
                serviceDetails += "<tr>";
                serviceDetails += "<td style=\"text-align:left;padding:5px;border:1px solid #b9b9b9;\">"+userPaymentDetail.ServiceName+"</td>";
                serviceDetails += "<td style=\"text-align:right;padding:5px;border:1px solid #b9b9b9;\">" + userPaymentDetail.Quantity + "</td>";
                serviceDetails += "<td style=\"text-align:right;padding:5px;border:1px solid #b9b9b9;\">" + userPaymentDetail.Rate.ToString("C2") + "</td>";
                serviceDetails += "<td style=\"text-align:right;padding:5px;border:1px solid #b9b9b9;\">" + userPaymentDetail.Amount.ToString("C2") + "</td>";
                serviceDetails += "</tr>";
            }
            
            serviceDetails += "<tr>";
            serviceDetails += "<td colspan=\"4\" style=\"text-align: right;padding: 15px 5px 0px;font-size: 15px;color: #333;font-weight: 600;\">Total : <b style=\"font-size: 20px;color:#"+ totalColorCss + ";\">" +userPayments.UserPaymentDetails.Sum(x=>x.Amount).ToString("C2") + "</b></td>";
            serviceDetails += "</tr>";
            paymentTemplate.MailTemplateHtml = paymentTemplate.MailTemplateHtml.Replace(Constants.TmplateConstServiceName, serviceDetails);
            paymentTemplate.MailTemplateHtml = paymentTemplate.MailTemplateHtml.Replace(Constants.TmplateConstName, userPayments.BusinessName);
            return paymentTemplate;
        }

        #endregion

        #region Send Email To Payment User

        public EmailDetailAPI SendPaymentsEmail(UserPayments userPayments)
        {
            var projectDetails = GetProjectNameByProjectId(userPayments.Projectid);
            var spanLibraryProductDetails = _adminRepository.GetAllSpanLibrProducts(userPayments.SpanLibrConnStr);
            EmailDetailAPI emailDetailAPI = new EmailDetailAPI();
            emailDetailAPI.BccAddress = Utilities.Utilities.GetEmailList(projectDetails.PaymentBccAddress);
            emailDetailAPI.CcAddress = Utilities.Utilities.GetEmailList(projectDetails.PaymentCcAddress);
            emailDetailAPI.FromAddress = userPayments.FromEmail;
            emailDetailAPI.Subject = userPayments.MailSubject;
            emailDetailAPI.IsAttachment = false;
            emailDetailAPI.MailBodyS3Path = userPayments.MailBodyS3Path;
            if (userPayments.Projectid == (int)Project.ExpressTaxFilings) emailDetailAPI.ProductCode = "TBS";
            if (userPayments.Projectid == (int)Project.ExpressTruckTax) emailDetailAPI.ProductCode = "ETT";
            emailDetailAPI.ReplyToAddress = null;
            emailDetailAPI.Subject = userPayments.MailSubject;
            emailDetailAPI.ToAddress = Utilities.Utilities.GetEmailList(userPayments.ToEmail);
            var ctProductDetail = spanLibraryProductDetails.Where(x => x.ProductCode == emailDetailAPI.ProductCode).SingleOrDefault();
            string authKey = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(
                                          ctProductDetail.AccessKey + " " +
                                          ctProductDetail.SecretKey + " " +
                                          ctProductDetail.ProductCode + " " +
                                          userPayments.ToEmail));

            using (var client = new PaymentAPIClient())
            {
                if (!string.IsNullOrWhiteSpace(authKey))
                {
                    client.DefaultRequestHeaders.Add("AuthKey", authKey);
                }
                string addTLUri = "Mail/SendEmailWithoutAttachment";
                var tlResponse = client.PostAsJsonAsync(addTLUri, emailDetailAPI).Result;
                if (tlResponse != null && tlResponse.IsSuccessStatusCode)
                {
                    var _emailDetailAPI = tlResponse.Content.ReadAsAsync<EmailDetailAPI>().Result;
                    if (!string.IsNullOrWhiteSpace(_emailDetailAPI.MessageId))
                    {
                        emailDetailAPI.MessageId = _emailDetailAPI.MessageId;
                    }
                    else
                    {
                        emailDetailAPI.FailureMessage = _emailDetailAPI.FailureMessage;
                    }
                }
                //if response from api gets failed redirect to user with error message
                else
                {
                    emailDetailAPI.FailureMessage = "Error";
                }
            }
            return emailDetailAPI;
        }

        #endregion

        #region Charge credit card

        public UserPayments ChargeCreditCardAndSave(UserPayments userPayments)
        {
            var productDetails = _adminRepository.GetAllSpanLibrProducts(Utilities.Utilities.GetAppSettings("PaymentLibConnStr"));
            var productDetail = productDetails.Where(x => x.ProductCode == userPayments.ProductCode).SingleOrDefault();
            string authKey = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(
                                          productDetail.AccessKey + " " +
                                          productDetail.SecretKey + " " +
                                          productDetail.ProductCode + " " +
                                          userPayments.ToEmail));
            var allStates = _adminRepository.GetAllStates();
            var allCountries = _adminRepository.GetCountries();
            CreditCardAPI creditCard = new CreditCardAPI
            {
                ProductCode = userPayments.ProductCode,
                AuthKey = authKey,
                CardNumber = userPayments.CardNo,
                ExpiryMonth = userPayments.ExpiryMonth,
                ExpiryYear = userPayments.ExpiryYear
            };
            creditCard.CardExpiry = userPayments.ExpiryMonth + "/" + userPayments.ExpiryYear;
            creditCard.SecurityCode = userPayments.SecurityCode;
            creditCard.NameOnCard = userPayments.NameOnCard;

            creditCard.Address1 = userPayments.Address1;
            creditCard.City = userPayments.City;
            creditCard.Zip = userPayments.PostalZipCode;
            var country = allCountries.Where(x => x.CountryId == userPayments.CountryId).SingleOrDefault();
            if (country!=null)
            {
                creditCard.CountryName = country.CountryName;
            }
            creditCard.CountryId = (short)(userPayments.CountryId==null?0: userPayments.CountryId);
            creditCard.EmailAddress = userPayments.ToEmail;
            //creditCard.UserId = CreditCardProfile.UserId;
            //creditCard.SiteName = CreditCardProfile.SiteName;
            creditCard.CardTypeStr = userPayments.CardType;
            var state = allStates.Where(x => x.StateId== userPayments.StateId).SingleOrDefault();
            if (state != null)
            {
                creditCard.StateCode = state.StateCode;
                creditCard.StateName = state.StateCode;
            }
            creditCard.ChargeAmount = userPayments.PaymentAmount;
            creditCard.TransactionId = "SCUP_" + userPayments.UserPaymentId.ToString();
            creditCard.CCInvoiceNumber=  userPayments.InvoiceNo;

            using (var client = new PaymentAPIClient())
            {
                #region API Headers
                if (!string.IsNullOrWhiteSpace(creditCard.AuthKey))
                {
                    client.DefaultRequestHeaders.Add("AuthKey", creditCard.AuthKey);
                }
                string addTLUri = "Commerce/CyberSourceCaptureAuthCard";
                var tlResponse = client.PostAsJsonAsync(addTLUri, creditCard).Result;
                #endregion

                if (tlResponse != null && tlResponse.IsSuccessStatusCode)
                {
                    creditCard = tlResponse.Content.ReadAsAsync<CreditCardAPI>().Result;

                    #region Operation Status - Success

                    if (creditCard != null && creditCard.OperationStatus == PaymentApiStatusType.Success && (!string.IsNullOrWhiteSpace(creditCard.WfAuthCode) || !string.IsNullOrWhiteSpace(creditCard.AuthCode)))
                    {
                        userPayments.OperationStatus = StatusType.Success;
                        if (!string.IsNullOrWhiteSpace(creditCard.WfAuthCode))
                        {
                            userPayments.PaymentApprovalCode = creditCard.WfAuthCode;
                        }
                        else if (!string.IsNullOrWhiteSpace(creditCard.AuthCode))
                        {
                            userPayments.PaymentApprovalCode = creditCard.AuthCode;
                        }
                        userPayments.PaymentProcessor = creditCard.PaymentProcessor;
                        userPayments.PaidTime = DateTime.Now;
                        userPayments.IsProcessed = true;
                    }
                    else
                    {
                        userPayments.OperationStatus = StatusType.Failure;
                        userPayments.IsProcessed = false;
                        if (creditCard != null && creditCard.ErrorMessages != null && creditCard.ErrorMessages.Count > 0)
                        {
                            userPayments.ErrorMessages = creditCard.ErrorMessages;
                            userPayments.ErrorMessage = string.Join(",", creditCard.ErrorMessages.Select(x => x.LongMessage).ToList());
                        }
                    }
                    #endregion
                }
                else
                {

                    #region Operation Status - Failure
                    ErrorMessage errorMessage = new ErrorMessage();
                    userPayments.OperationStatus = StatusType.Failure;
                    errorMessage.LongMessage = "You are not authorized to access this method";
                    userPayments.ErrorMessages = new List<ErrorMessage>();
                    userPayments.ErrorMessages.Add(errorMessage);
                    userPayments.ErrorMessage = "Payment Failed";
                    #endregion
                }
            }

            return userPayments;
        }

        #endregion

        #region Save admin payment log

        public void SaveUserPaymentLog(UserPaymentLog paymentLog)
        {
            _adminRepository.SaveUserPaymentLog(paymentLog);
        }

        #endregion

        #region Get admin activity log

        public List<UserPaymentLog> GetPaymentLogs(Guid paymentId, PaymentActivityType activityType)
        {
            return _adminRepository.GetPaymentLogs(paymentId, activityType);
        }

        #endregion

        #region Send payment success email
        public void SendPaymentSuccessEmail(Guid paymentId)
        {
            string spanLibrConStr = Utilities.Utilities.GetAppSettings("PaymentLibConnStr");
            var paymentDetail = GetUserPaymentDetailByTokenId(paymentId);
            var projectDetails = GetProjectNameByProjectId(paymentDetail.Projectid);
            PaymentTemplate paymentTemplate = new PaymentTemplate();
            var paymentReceiptTemplate = new PaymentTemplate();
            if (paymentDetail.Projectid == (int)Project.ExpressTruckTax)
            {
                paymentTemplate = _adminRepository.GetPaymentTemplateByTemplateId(Constants.PaymentSuccessTemplateETT);
                paymentReceiptTemplate = _adminRepository.GetPaymentTemplateByTemplateId(Constants.PaymentReceiptTemplateETT);
            }
            else if (paymentDetail.Projectid == (int)Project.ExpressTaxFilings)
            {
                paymentTemplate = _adminRepository.GetPaymentTemplateByTemplateId(Constants.PaymentSuccessTemplateTBS);
                paymentReceiptTemplate = _adminRepository.GetPaymentTemplateByTemplateId(Constants.PaymentReceiptTemplateTBS);
            }
            var spanLibraryProductDetails = _adminRepository.GetAllSpanLibrProducts(spanLibrConStr);
            EmailDetailAPI emailDetailAPI = new EmailDetailAPI();
            emailDetailAPI.BccAddress = Utilities.Utilities.GetEmailList(projectDetails.PaymentBccAddress);
            emailDetailAPI.CcAddress = Utilities.Utilities.GetEmailList(projectDetails.PaymentCcAddress);
            emailDetailAPI.FromAddress = paymentDetail.FromEmail;
            emailDetailAPI.Subject = paymentTemplate.MailSubject;
            emailDetailAPI.IsAttachment = true;
            
            if (paymentDetail.Projectid == (int)Project.ExpressTaxFilings) emailDetailAPI.ProductCode = "TBS";
            if (paymentDetail.Projectid == (int)Project.ExpressTruckTax) emailDetailAPI.ProductCode = "ETT";
            emailDetailAPI.ReplyToAddress = null;
            emailDetailAPI.ToAddress = Utilities.Utilities.GetEmailList(paymentDetail.ToEmail);
            var ctProductDetail = spanLibraryProductDetails.Where(x => x.ProductCode == emailDetailAPI.ProductCode).SingleOrDefault();
            string authKey = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(
                                          ctProductDetail.AccessKey + " " +
                                          ctProductDetail.SecretKey + " " +
                                          ctProductDetail.ProductCode + " " +
                                          paymentDetail.ToEmail));
            
            var receiptHtml = GetPaymentReceiptHtml(paymentDetail, paymentReceiptTemplate);
            Persits.PDF.PdfDocument pdfDocument = null;
            Persits.PDF.PdfManager pdfManager = new Persits.PDF.PdfManager();
            pdfManager.RegKey = Utilities.Utilities.GetPDFKey();
            pdfDocument = pdfManager.CreateDocument();
            pdfDocument.ImportFromUrl(receiptHtml.MailTemplateHtml);
            var pdfByte = pdfDocument.SaveToMemory();
            string receiptFile = paymentDetail.UserPaymentId.ToString() + ".pdf";
            string receiptS3Path = "SCPayments/Receipts/" + receiptFile;
            Utilities.Utilities.UploadImageInWebStorage(receiptS3Path, new MemoryStream(pdfByte));
            string receiptFullS3 = Utilities.Utilities.GetS3FullUrl(receiptS3Path);
            emailDetailAPI.Attachments = new List<EmailDetailAttachment>();
            emailDetailAPI.Attachments.Add(new EmailDetailAttachment
            {
                AttachmentName = receiptFile,
                AttachmentS3Path = receiptFullS3
            });
            _adminRepository.UpdateReceiptS3Path(paymentDetail.UserPaymentId, receiptFullS3);
            paymentTemplate = GetPaymentSuccessHtml(paymentDetail, paymentTemplate);
            string paymentSuccessFile = paymentDetail.UserPaymentId.ToString() + ".html";
            string paymentSuccessS3Path = "SCPayments/PaymentSuccess/" + paymentSuccessFile;
            var tmplateByte = System.Text.Encoding.UTF8.GetBytes(paymentTemplate.MailTemplateHtml);
            Utilities.Utilities.UploadImageInWebStorage(paymentSuccessS3Path, new MemoryStream(tmplateByte));
            string paymentSuccessFullS3 = Utilities.Utilities.GetS3FullUrl(paymentSuccessS3Path);
            emailDetailAPI.MailBodyS3Path = paymentSuccessFullS3;
            using (var client = new PaymentAPIClient())
            {
                if (!string.IsNullOrWhiteSpace(authKey))
                {
                    client.DefaultRequestHeaders.Add("AuthKey", authKey);
                }
                string addTLUri = "Mail/SendEmailWithAttachment";
                var tlResponse = client.PostAsJsonAsync(addTLUri, emailDetailAPI).Result;
                if (tlResponse != null && tlResponse.IsSuccessStatusCode)
                {
                    var _emailDetailAPI = tlResponse.Content.ReadAsAsync<EmailDetailAPI>().Result;
                    if (!string.IsNullOrWhiteSpace(_emailDetailAPI.MessageId))
                    {
                        emailDetailAPI.MessageId = _emailDetailAPI.MessageId;
                    }
                    else
                    {
                        emailDetailAPI.FailureMessage = _emailDetailAPI.FailureMessage;
                    }
                }
                //if response from api gets failed redirect to user with error message
                else
                {
                    emailDetailAPI.FailureMessage = "Error";
                }
            }
        }
        #endregion

        #region Send payment failure mail
        public void SendPaymentFailureEmail(Guid paymentId)
        {

        }
        #endregion

        #region Get payment success mail html

        private PaymentTemplate GetPaymentSuccessHtml(UserPayments userPayments, PaymentTemplate paymentTemplate)
        {
            paymentTemplate.MailTemplateHtml = paymentTemplate.MailTemplateHtml.Replace(Constants.TmplateConstName, userPayments.BusinessName);
            paymentTemplate.MailTemplateHtml = paymentTemplate.MailTemplateHtml.Replace(Constants.TmplateConstAmount, userPayments.PaymentAmount.ToString("C2"));
            return paymentTemplate;
        }

        #endregion

        #region Get payment success receipt mail html

        private PaymentTemplate GetPaymentReceiptHtml(UserPayments userPayments, PaymentTemplate paymentTemplate)
        {
            paymentTemplate.MailTemplateHtml = paymentTemplate.MailTemplateHtml.Replace(Constants.TmplatePaymentDate, userPayments.PaidTime.Value.ToString("MM/dd/yyyy"));
            paymentTemplate.MailTemplateHtml = paymentTemplate.MailTemplateHtml.Replace(Constants.TmplateReceiptNo, userPayments.InvoiceNo);
            paymentTemplate.MailTemplateHtml = paymentTemplate.MailTemplateHtml.Replace(Constants.TmplatePayerContactName, userPayments.BusinessName);
            paymentTemplate.MailTemplateHtml = paymentTemplate.MailTemplateHtml.Replace(Constants.TmplateAddressDetails, string.Empty);
            paymentTemplate.MailTemplateHtml = paymentTemplate.MailTemplateHtml.Replace(Constants.TmplatePhoneNo, userPayments.PhoneNo);
            paymentTemplate.MailTemplateHtml = paymentTemplate.MailTemplateHtml.Replace(Constants.TmplateEmail, userPayments.ToEmail);
            string receiptDescription = string.Empty;
            foreach (var pmntDetail in userPayments.UserPaymentDetails)
            {
                receiptDescription += "<tr>";
                receiptDescription += "<td style=\"text-align:left;\">"+ pmntDetail.ServiceName +"</td>";
                receiptDescription += "<td style=\"text-align:right;\">" + pmntDetail.Quantity + "</td>";
                receiptDescription += "<td style=\"text-align:right;\">" + pmntDetail.Rate.ToString("C2") + "</td>";
                receiptDescription += "<td style=\"text-align:right;\">" + pmntDetail.Amount.ToString("C2") + "</td>";
                receiptDescription += "</tr>";
            }
            receiptDescription += "<tr>";
            receiptDescription += "<td colspan=\"4\" style=\"text-align:right;font-weight:bold;\">Total : " + userPayments.PaymentAmount.ToString("C2") + "</td>";
            receiptDescription += "</tr>";
            paymentTemplate.MailTemplateHtml = paymentTemplate.MailTemplateHtml.Replace(Constants.TmplateReceiptDescription, receiptDescription);
            paymentTemplate.MailTemplateHtml = paymentTemplate.MailTemplateHtml.Replace(Constants.TmplateNameOnCard, userPayments.NameOnCard);
            paymentTemplate.MailTemplateHtml = paymentTemplate.MailTemplateHtml.Replace(Constants.TmplateCardNo, userPayments.CardNo);
            paymentTemplate.MailTemplateHtml = paymentTemplate.MailTemplateHtml.Replace(Constants.TmplatePaymentApprovalCode, userPayments.PaymentApprovalCode);
            return paymentTemplate;
        }

        #endregion

        #region Search Email By Product

        public List<string> SearchEmailByProduct(string emailAddress, int project)
        {
            return _adminRepository.SearchEmailByProduct(emailAddress, project);
        }

        #endregion

        #region Save Void Refund
        public void SaveVoidRefund(PaymentRefundLog voidRefundRequest, string connectionString)
        {
            _adminRepository.SaveVoidRefund(voidRefundRequest, connectionString);
        }
        #endregion

        #region Get refund details

        public List<PaymentRefundLog> GetPaymentRefundDetails(PaymentRefundLog transactionReport, string connectionString)
        {
            return _adminRepository.GetPaymentRefundDetails(transactionReport, connectionString);
        }
        #endregion

        public decimal GetRefundAmount(PaymentRefundLog transactionReport, string connectionString, string chargeBackType)
        {
            return _adminRepository.GetRefundAmount(transactionReport, connectionString,chargeBackType);
        }

        #region Void Payment API

        public WfVoidRefundResponse VoidTransaction(WfVoidRequest wfVoidRequest)
        {
            WfVoidRefundResponse wfVoidRefundResponse = new WfVoidRefundResponse();
            using (var client = new PaymentAPIClient())
            {
                string spanLibrConStr = Utilities.Utilities.GetAppSettings("PaymentLibConnStr");
                var spanLibraryProductDetails = _adminRepository.GetAllSpanLibrProducts(spanLibrConStr);
                var ctProductDetail = spanLibraryProductDetails.Where(x => x.ProductCode == wfVoidRequest.ProductCode).SingleOrDefault();
                string authKey = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(
                                      ctProductDetail.AccessKey + " " +
                                      ctProductDetail.SecretKey + " " +
                                      wfVoidRequest.ProductCode + " " +
                                      wfVoidRequest.Email));
                client.DefaultRequestHeaders.Add("AuthKey", authKey);
                string addTLUri = "Commerce/WfVoidTransaction";
                var apiResponse = client.PostAsJsonAsync(addTLUri, wfVoidRequest).Result;
                if (apiResponse != null && apiResponse.IsSuccessStatusCode)
                {
                    wfVoidRefundResponse = apiResponse.Content.ReadAsAsync<WfVoidRefundResponse>().Result;
                }
                else
                {
                    wfVoidRefundResponse.OperationStatus = PaymentApiStatusType.Failure;
                    wfVoidRefundResponse.Errors = new List<ErrorMessage>();
                    wfVoidRefundResponse.Errors.Add(new ErrorMessage()
                    {
                        ErrorCode = "ERROR",
                        LongMessage = "Api call failed"
                    });
                }

            }
            return wfVoidRefundResponse;
        }

        #endregion

        #region Refund Payment API

        public WfVoidRefundResponse RefundTransaction(WfRefundRequest wfRefundRequest)
        {
            WfVoidRefundResponse wfVoidRefundResponse = new WfVoidRefundResponse();
            using (var client = new PaymentAPIClient())
            {
                string spanLibrConStr = Utilities.Utilities.GetAppSettings("PaymentLibConnStr");
                var spanLibraryProductDetails = _adminRepository.GetAllSpanLibrProducts(spanLibrConStr);
                var ctProductDetail = spanLibraryProductDetails.Where(x => x.ProductCode == wfRefundRequest.ProductCode).SingleOrDefault();
                string authKey = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(
                                          ctProductDetail.AccessKey + " " +
                                          ctProductDetail.SecretKey + " " +
                                          wfRefundRequest.ProductCode + " " +
                                          wfRefundRequest.Email));
                client.DefaultRequestHeaders.Add("AuthKey", authKey);
                string addTLUri = "Commerce/WfRefundTransaction";
                var apiResponse = client.PostAsJsonAsync(addTLUri, wfRefundRequest).Result;
                if (apiResponse != null && apiResponse.IsSuccessStatusCode)
                {
                    wfVoidRefundResponse = apiResponse.Content.ReadAsAsync<WfVoidRefundResponse>().Result;
                }
                else
                {
                    wfVoidRefundResponse.OperationStatus = PaymentApiStatusType.Failure;
                    wfVoidRefundResponse.Errors = new List<ErrorMessage>();
                    wfVoidRefundResponse.Errors.Add(new ErrorMessage()
                    {
                        ErrorCode = "ERROR",
                        LongMessage = "Api call failed"
                    });
                }
            }
            return wfVoidRefundResponse;
        }

        #endregion

        #region Get Transaction Detail
        public TransactionReportDetails GetTransactionDetail(string transactionId, string connectionString)
        {
            return _adminRepository.GetTransactionDetail(transactionId, connectionString);
        } 
        #endregion

        #region Get Contact groups list 
        /// <summary>
        /// Get Contact groups list 
        /// </summary>
        /// <returns></returns>
        public List<SpancontrolContactGroup> GetContactgroupList()
        {
            List<SpancontrolContactGroup> groupLists = new List<SpancontrolContactGroup>();
            // SpancontrolContactGroup groupList = new SpancontrolContactGroup();
            using (var reqClient = new ETTAPIClient())
            {
                string requestTSNAuri = "/User/GetContactGroupList/";
                ETTAPIRequestUtility.GenerateAuthHeader(reqClient, requestTSNAuri, "GET");
                var response = reqClient.GetAsync(requestTSNAuri).Result;
                if (response != null && response.IsSuccessStatusCode)
                {
                    groupLists = response.Content.ReadAsAsync<List<SpancontrolContactGroup>>().Result;
                }
            }
            return groupLists;
        }
        #endregion

        #region Getting Group members By userId
        /// <summary>
        /// Get Group members by User Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<long> GetGroupMembersbyUserId(Guid userId)
        {
            List<long> groupMembers = new List<long>();
           // System.IO.File.WriteAllText(@"X:\web\user\SpanControl\www.spancontrol.com\aa.txt", "Step-1");
            using (var reqClient = new ETTAPIClient())
            {
                string requestTSNAuri = "/User/GetGroupMembersbyUserId?userId=" + userId;
                ETTAPIRequestUtility.GenerateAuthHeader(reqClient, requestTSNAuri, "GET");
                var response = reqClient.GetAsync(requestTSNAuri).Result;
             //   System.IO.File.AppendAllText(@"X:\web\user\SpanControl\www.spancontrol.com\aa.txt", "\n" +response.StatusCode.ToString());
                if (response != null && response.IsSuccessStatusCode == true)
                {
                    groupMembers = response.Content.ReadAsAsync<List<long>>().Result;
                }
            }
        //    System.IO.File.AppendAllText(@"X:\web\user\SpanControl\www.spancontrol.com\aa.txt", "\n" +"Step-3");
            return groupMembers;
        }
        #endregion

        #region Save group members
        /// <summary>
        /// Save Group members
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        public GroupMembers SaveGroupMember(GroupMembers members)
        {
            GroupMembers savedGroupmember = new GroupMembers();
            using (var reqClient = new ETTAPIClient())
            {
                //string requestTSNAuri = "/User/saveGroupmember?id= '" + id +" '& checkorunchecked='" + checkorunchecked + "'& emailaddress='" + emailaddress + "'";
                string requestTSNAuri = "/User/SaveGroupMember";
                ETTAPIRequestUtility.GenerateAuthHeader(reqClient, requestTSNAuri, "POST");
                var response = reqClient.PostAsJsonAsync(requestTSNAuri, members).Result;
                if (response != null && response.IsSuccessStatusCode == true)
                {
                    savedGroupmember = response.Content.ReadAsAsync<GroupMembers>().Result;
                }
            }
            return savedGroupmember;
        }

        #endregion

        #region Get user Id in ETT by emailaddress
        /// <summary>
        /// Get User Id by Email address
        /// </summary>
        /// <param name="emailaddress"></param>
        /// <returns></returns>
        public Guid GetUserIdbyEmailAddress(string emailaddress)
        {
            //System.IO.File.WriteAllText(@"X:\web\user\SpanControl\www.spancontrol.com\aa.txt", "Step-1 "+emailaddress );
            Guid userId = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(emailaddress))
            {
                using (var reqClient = new ETTAPIClient())
                {
                    string requestTSNAuri = "/User/GetUserIdbyEmailAddress?emailaddress=" + Uri.EscapeDataString(emailaddress);
                    ETTAPIRequestUtility.GenerateAuthHeader(reqClient, requestTSNAuri, "GET");
                    var response = reqClient.GetAsync(requestTSNAuri).Result;
                    // System.IO.File.AppendAllText(@"X:\web\user\SpanControl\www.spancontrol.com\aa.txt", "\n" + response.StatusCode.ToString());
                    if (response != null && response.IsSuccessStatusCode == true)
                    {
                        userId = response.Content.ReadAsAsync<Guid>().Result;
                    }
                    // System.IO.File.AppendAllText(@"X:\web\user\SpanControl\www.spancontrol.com\aa.txt", "\n" +"Step-3");
                }
            }
            return userId;
        }
        #endregion

        #region Get list for Campaign Download Excel report
        /// <summary>
        /// Get list for Campaign Download Excel report
        /// </summary>
        /// <param name="optionSelected"></param>
        /// <returns></returns>
        public List<GroupMembers> GetReportbySelectedOption(string optionSelected,long adminUserId,long campaignDetailId)
        {
            return _adminRepository.GetReportBySelectedOption(optionSelected, adminUserId, campaignDetailId);
        }
        #endregion

        #region Get Communication details form ETT
        /// <summary>
        ///Get Communication details form ETT 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<LeadCommunication> GetCommunicationDetails(Guid userId)
        {
            List<LeadCommunication> userdetails = new List<LeadCommunication>();
            using (var reqClient = new ETTAPIClient())
            {
                string requestTSNAuri = "/User/GetUserDetailsByUserId?userId=" + userId;
                ETTAPIRequestUtility.GenerateAuthHeader(reqClient, requestTSNAuri, "GET");
                var response = reqClient.GetAsync(requestTSNAuri).Result;
                if (response != null && response.IsSuccessStatusCode)
                {
                    userdetails = response.Content.ReadAsAsync<List<LeadCommunication>>().Result;
                }

            }
            return userdetails;
        }
        #endregion

        #region Save lead communication details in ETT
        /// <summary>
        /// Save lead communication details in ETT 
        /// </summary>
        /// <param name="leadCommunication"></param>
        /// <returns></returns>
        public LeadCommunication SaveCommunicationDetails(LeadCommunication leadCommunication)
        {
            LeadCommunication savedetails = new LeadCommunication();
            using (var reqClient = new ETTAPIClient())
            {
                string requestTsnaUri = "User/SaveCampaignDetails/";
                ETTAPIRequestUtility.GenerateAuthHeader(reqClient, requestTsnaUri, "POST");
                var _settingresponse = reqClient.PostAsJsonAsync(requestTsnaUri, leadCommunication).Result;
                if (_settingresponse != null && _settingresponse.IsSuccessStatusCode == true)
                {
                    savedetails = _settingresponse.Content.ReadAsAsync<LeadCommunication>().Result;
                }
                return savedetails;
            }
            
        }
        #endregion

        #region Get Campaign Assigned details
        /// <summary>
        /// Get Campaign Assigned details
        /// </summary>
        /// <param name="campaignDetailId"></param>
        /// <param name="supportUserId"></param>
        /// <param name="campaignAssignedDetailId"></param>
        /// <returns></returns>
        public LeadDetails GetCampaignAssignedDetails(long campaignDetailId,long supportUserId,long campaignAssignedDetailId)
        {
            return _adminRepository.GetCampaignAssignedDetails(campaignDetailId, supportUserId, campaignAssignedDetailId);
        }
        #endregion

        #region Get AdminUser Details By AdminUserId
        /// <summary>
        /// Get AdminUser Details By AdminUserId
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <returns></returns>
        public AdminUser GetAdminUserUthenticatorDetailsByAdminUserId(long adminUserId)
        {
            return _adminRepository.GetAdminUserUthenticatorDetailsByAdminUserId(adminUserId);
        }
        #endregion

        #region Update Authentication For AdminUser
        /// <summary>
        /// Update Authentication For AdminUser
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <returns></returns>
        public bool UpdateAuthenticationForAdminUser(long adminUserId)
        {
            return _adminRepository.UpdateAuthenticationForAdminUser(adminUserId);
        }
        #endregion

        #region  Reset Authentication
        /// <summary>
        /// Reset Authentication
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool ResetAuthentication(long Id)
        {
            return this._adminRepository.ResetAuthentication(Id);
        }
        #endregion

        #region _Payment Reconcilation Report
        public List<PaymentReconReport> GetPaymentBatchDetailsReport(DateTime BeginDate, DateTime EndDate, int ProductId, List<string> StripeTransactionId)
        {
            List<PaymentReconReport> PaymentReconReport = new List<PaymentReconReport>();
            var AppReconcilationPayment = _adminRepository.GetAppReconcilationPayment(BeginDate, EndDate, ProductId);
            var APIReconcilationPayment = _adminRepository.GetAPIReconcilationPayment(BeginDate, EndDate, ProductId);
            var CyberSourceReconcilationPayment = GetCyberSourceDailySPANReport(ProductId, BeginDate).strRemittanceAmtList;


            var lstStrAppTxnId = new List<String>();
            var lstStrAPITxnId = new List<String>();
            var lstCyberSourcesTxnId = new List<String>();
            if (AppReconcilationPayment != null && AppReconcilationPayment.Count > 0)
            {
                lstStrAppTxnId = AppReconcilationPayment.Select(x => x.AppTxnId.ToLower()).Distinct().ToList();
            }
            if (APIReconcilationPayment != null && APIReconcilationPayment.Count > 0)
            {
                lstStrAPITxnId = APIReconcilationPayment.Select(x => x.ApiTxnId.ToLower()).Distinct().ToList();

            }
            if (CyberSourceReconcilationPayment != null && CyberSourceReconcilationPayment.Count > 0)
            {
                lstCyberSourcesTxnId = CyberSourceReconcilationPayment.Select(x => x.CyberSourceTxnIdRefId.ToLower()).Distinct().ToList();
            }

            #region split and Avoid Duplicate TxnId


            var missingInAPIListwithApp = lstStrAppTxnId.Where(x => !lstStrAPITxnId.Contains(x)).ToList();
            var missingInAppListwithAPI = lstStrAPITxnId.Where(x => !lstStrAppTxnId.Contains(x)).ToList();

            var missingInCyberSourceListwithApp = lstStrAppTxnId.Where(x => !lstCyberSourcesTxnId.Contains(x)).ToList();
            var missingInAppListwithCyberSource = lstCyberSourcesTxnId.Where(x => !lstStrAppTxnId.Contains(x)).ToList();

            var missingInCyberSourceListwithAPI = lstStrAPITxnId.Where(x => !lstCyberSourcesTxnId.Contains(x)).ToList();
            var missingInAPIListwithCyberSource = lstCyberSourcesTxnId.Where(x => !lstStrAPITxnId.Contains(x)).ToList();

            #endregion



            var lstAppTaxnId = missingInAPIListwithApp.Concat(missingInCyberSourceListwithApp).Distinct();
            var lstAPITaxnId = missingInAppListwithAPI.Concat(missingInCyberSourceListwithAPI).Distinct();
            var lstCyberSourceTaxnId = missingInAPIListwithCyberSource.Concat(missingInAppListwithCyberSource).Distinct();

            List<string> allTransactionIds = new List<string>();
            if (lstAPITaxnId != null || lstAppTaxnId != null || lstCyberSourceTaxnId != null)
            {
                allTransactionIds.AddRange(lstAppTaxnId);
                allTransactionIds.AddRange(lstAPITaxnId);
                allTransactionIds.AddRange(lstCyberSourceTaxnId);
                if (allTransactionIds.Count > 0 && allTransactionIds != null)
                {
                    allTransactionIds = allTransactionIds.Select(e => e).Distinct().ToList();
                }
            }

            #region Missing Payments List Creation
            int serielNumber = 1;
            if (allTransactionIds != null && allTransactionIds.Count > 0)
            {
                foreach (string item in allTransactionIds)
                {
                    PaymentReconReport missingPaymentModel = new PaymentReconReport();
                    missingPaymentModel.SlNo = serielNumber;

                    missingPaymentModel.TransactionId = item;

                    missingPaymentModel.IsProduct = lstAppTaxnId.Any(e => e == item);
                    missingPaymentModel.IsApi = lstAPITaxnId.Any(e => e == item);
                    missingPaymentModel.IsCybersource = lstCyberSourceTaxnId.Any(e => e == item);
                    if (missingPaymentModel.IsProduct && AppReconcilationPayment != null)
                    {
                        missingPaymentModel.Date = AppReconcilationPayment.Where(x => x.AppTxnId.ToLower() == item.ToLower()).Select(x => x.CreatedTimeStamp).FirstOrDefault();
                        if (missingPaymentModel.IsApi && APIReconcilationPayment != null)
                        {
                            missingPaymentModel.Email = APIReconcilationPayment.Where(x => x.ApiTxnId.ToLower() == item.ToLower()).Select(x => x.Email).FirstOrDefault();
                        }
                        else
                        {
                            missingPaymentModel.Email = "-";
                        }
                        if (missingPaymentModel.IsCybersource && AppReconcilationPayment != null)
                        {
                            missingPaymentModel.Amount = CyberSourceReconcilationPayment.Where(x => x.CyberSourceTxnIdRefId == item).Select(x => x.TransactionAmount).FirstOrDefault();
                        }
                        else if (missingPaymentModel.IsApi && APIReconcilationPayment != null)
                        {
                            missingPaymentModel.Amount = APIReconcilationPayment.Where(x => x.ApiTxnId == item).Select(x => x.TransactionAmount).FirstOrDefault();
                            missingPaymentModel.ApiCallStatus = APIReconcilationPayment.Where(x => x.ApiTxnId.ToLower() == item.ToLower()).Select(x => x.ApiCallStatus).FirstOrDefault();
                        }
                        else
                        {
                            if (AppReconcilationPayment != null)
                            {
                                missingPaymentModel.Amount = AppReconcilationPayment.Where(x => x.AppTxnId == item).Select(x => x.TransactionAmount).FirstOrDefault();
                                missingPaymentModel.ApiCallStatus = "-";

                            }
                        }

                    }
                    else if (missingPaymentModel.IsApi && APIReconcilationPayment != null)
                    {
                        missingPaymentModel.Date = APIReconcilationPayment.Where(x => x.ApiTxnId.ToLower() == item.ToLower()).Select(x => x.CreatedTimeStamp).FirstOrDefault();
                        missingPaymentModel.Amount = APIReconcilationPayment.Where(x => x.ApiTxnId.ToLower() == item.ToLower()).Select(x => x.TransactionAmount).FirstOrDefault();
                        missingPaymentModel.Email = APIReconcilationPayment.Where(x => x.ApiTxnId.ToLower() == item.ToLower()).Select(x => x.Email).FirstOrDefault();
                        missingPaymentModel.ApiCallStatus = APIReconcilationPayment.Where(x => x.ApiTxnId.ToLower() == item.ToLower()).Select(x => x.ApiCallStatus).FirstOrDefault();
                    }
                    else
                    {
                        if (CyberSourceReconcilationPayment != null)
                        {
                            missingPaymentModel.Date = CyberSourceReconcilationPayment.Where(x => x.CyberSourceTxnIdRefId.ToLower() == item.ToLower()).Select(x => x.CreatedTimeStamp).FirstOrDefault();
                            missingPaymentModel.Amount = CyberSourceReconcilationPayment.Where(x => x.CyberSourceTxnIdRefId.ToLower() == item.ToLower()).Select(x => x.TransactionAmount).FirstOrDefault();
                            //Check a Response_Refund_Log table When API and TBS is false and Cs true to ChargeBackType is a Void or Refund 
                            var refundPaymentLog = _adminRepository.GetChargeBackTypeInPaymentRefundLog(missingPaymentModel.TransactionId);
                            missingPaymentModel.Email = refundPaymentLog != null && !string.IsNullOrEmpty(refundPaymentLog.Email) ? refundPaymentLog.Email : "-";
                            if (refundPaymentLog != null && refundPaymentLog.ApiCallStatus == Constants.REFUND)
                            {
                                missingPaymentModel.ApiCallStatus = Constants.Refunded;
                            }
                            else if (refundPaymentLog != null && refundPaymentLog.ApiCallStatus == Constants.VOID)
                            {
                                missingPaymentModel.ApiCallStatus = Constants.Voided;
                            }
                            else
                            {
                                missingPaymentModel.ApiCallStatus = "-";
                            }
                        }


                    }
                    if (!missingPaymentModel.IsCybersource)
                    {
                        bool isStrip = false;

                        isStrip = StripeTransactionId != null && StripeTransactionId.Any() ? StripeTransactionId.Contains(item) : false;
                        missingPaymentModel.IsStripe = isStrip ? "Yes" : "No";
                    }
                    else
                    {
                        missingPaymentModel.IsStripe = "-";
                    }
                    PaymentReconReport.Add(missingPaymentModel);
                    serielNumber++;
                }

            }
            
            #endregion

            return PaymentReconReport;
        }
        #endregion

        #region Get Cyber Source Daily SPAN Report

        public CyberSource GetCyberSourceDailySPANReport(int ProductId, DateTime BeginDate)
        {
            CyberSource cyberSource = new CyberSource();
            string merchantCode = string.Empty;
            if (ProductId == Constants.ProductIdLive || ProductId == Constants.ProductIdSprint)
            {
                SpanProductCode productCode = SpanProductCode.TBSWFG;
                if (productCode == SpanProductCode.TBSWFG) merchantCode = ConfigurationManager.AppSettings[Constants.WfgTbsMerchantCode];

            }
            else if (ProductId == Constants.EEProductIdLive || ProductId == Constants.EEProductIdSprint)
            {
                SpanProductCode productCode = SpanProductCode.EE;
                if (productCode == SpanProductCode.EE) merchantCode = ConfigurationManager.AppSettings[Constants.WfgExpressExtensionMerchantCode];

            }
            else
            {
                SpanProductCode productCode = SpanProductCode.ETTWFG;
                if (productCode == SpanProductCode.ETTWFG) merchantCode = ConfigurationManager.AppSettings[Constants.WfgETTMerchantCode];
            }
            bool _isLive = Utilities.Utilities.GetBool(ConfigurationManager.AppSettings[Constants.IsLive]);
            var dtCtDt = DateTime.Now;
            string todaysRemittanceFileName = string.Empty;
            DateTime WfgRemittanceDate = BeginDate.AddDays(1);
            string configDate = WfgRemittanceDate.ToString();
            string wfgReportDate = string.Empty;
            if (!string.IsNullOrWhiteSpace(configDate))
            {
                DateTime dtConfig = Convert.ToDateTime(configDate);
                todaysRemittanceFileName = "remittance_" + merchantCode + "_" + dtConfig.Year + dtConfig.ToString("MM") + dtConfig.ToString("dd");
                wfgReportDate = dtConfig.ToString("yyyy-MM-dd");
            }
            else
            {
                todaysRemittanceFileName = "remittance_" + merchantCode + "_" + dtCtDt.Year + dtCtDt.ToString("MM") + dtCtDt.ToString("dd");
                wfgReportDate = dtCtDt.ToString("yyyy-MM-dd");
            }
            byte[] btCsv = null;
            if (_isLive)
            {
                string s3CsvFileName = ConfigurationManager.AppSettings["WfgRemittanceLocation"] + "/" + todaysRemittanceFileName + ".csv";
                btCsv = Utilities.Utilities.DownloadS3File(s3CsvFileName);
            }
            else
            {
                btCsv = System.IO.File.ReadAllBytes(Utilities.Utilities.GetAppSettings(Constants.PaymentBatchDetailReport));
            }
            string csvContent = System.Text.Encoding.UTF8.GetString(btCsv);
            if (!string.IsNullOrWhiteSpace(csvContent))
            {
               // wfgDailyRemittance.IsFileAvailable = true;
                string[] remittanceLines = csvContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                if (remittanceLines != null && remittanceLines.Count() > 0)
                {
                    List<string> remittanceLineLst = remittanceLines.ToList();
                    remittanceLineLst.RemoveAt(0);
                    string headerLine = remittanceLineLst[0];
                    var headers = headerLine.Split(',').ToList();
                    var headerIdx = headers.IndexOf("MerchantReferenceNumber");
                    var headerIdxAmount = headers.IndexOf("Amount");
                    var headerIdxDate = headers.IndexOf("RequestDate");
                    remittanceLineLst.RemoveAt(0);
                    cyberSource.strRemittanceAmtList = new List<CyberSource>();
                    foreach (var remittanceLine in remittanceLineLst)
                    {
                        var lineValues = remittanceLine.Split(',');
                        if (lineValues[headerIdx] != null && lineValues[headerIdx] != "" && lineValues.Count() > 0)
                        {
                            // Added three parameter in List 
                            CyberSource cybersource = new CyberSource();
                            cybersource.CyberSourceTxnIdRefId = lineValues[headerIdx];
                            cybersource.TransactionAmount = Utilities.Utilities.GetDecimal2Digits(lineValues[headerIdxAmount]);
                            cybersource.CreatedTimeStamp = Utilities.Utilities.GetDateTime(lineValues[headerIdxDate]);
                            cyberSource.strRemittanceAmtList.Add(cybersource);
                        }
                    }


                }
            }
            return cyberSource;
        }
        #endregion

        #region Get Daily SPAN Report
        public AuthorizeResponse GetDailySPANReport(DateTime _tBeginDate, DateTime _tEndDate)
        {
            AuthorizeResponse authResponse = new AuthorizeResponse();
            authResponse.TransactionId = new List<string>();
            List<AuthorizePaymentReport> lstAuthPaymentReport = new List<AuthorizePaymentReport>();
            List<StripeCredential> lstStripeCredentials = _adminRepository.GetAllStripeCredentials();
            if (lstStripeCredentials != null && lstStripeCredentials.Any())
            {
                foreach (var stripeDetail in lstStripeCredentials)
                {
                    if (stripeDetail != null && !string.IsNullOrWhiteSpace(stripeDetail.StripeLoginKey) && !string.IsNullOrWhiteSpace(stripeDetail.ProductCode))
                    {
                        //string apiKey = "sk_test_niky33Z4LYrfnD1VClFj4CfQ";
                        var chargedTransactions = RunSettledBatchForStripe(stripeDetail.ProductCode, stripeDetail.StripeLoginKey, _tBeginDate, _tEndDate);
                        if (chargedTransactions.MissingPaymentTransactionId != null && chargedTransactions.MissingPaymentTransactionId.Count > 0)
                        {
                            foreach (var item in chargedTransactions.MissingPaymentTransactionId)
                            {
                                string transactionId = item.TransactionId.Replace("-", "");
                                authResponse.TransactionId.Add(transactionId);
                            }
                        }
                        if (chargedTransactions != null)
                        {
                            chargedTransactions.ProductCode = stripeDetail.ProductCode + Constants.WithStripe;
                            chargedTransactions.TransactionSettlementDate = _tBeginDate;
                            lstAuthPaymentReport.Insert(0, chargedTransactions);
                        }
                    }
                }
            }
            authResponse.BeginDate = _tBeginDate;
            authResponse.ProductBasedAmount = lstAuthPaymentReport;

            return authResponse;
        }
        #endregion

        #region Common Method to get Stripe transactions
        public AuthorizePaymentReport RunSettledBatchForStripe(string productCode, string apiKey, DateTime firstSettlementDate, DateTime lastSettlementDate)
        {
            AuthorizePaymentReport stripeChargeDetails = new AuthorizePaymentReport();
            List<PaymentReconReport> lstMissingPaymentModel = new List<PaymentReconReport>();
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                StripeList<Charge> charges = new StripeList<Charge>();
                decimal stripeTotalAmt = 0;

                var lastId = String.Empty;
                //We using do-while loop for first time, we assign to get response if more than 100 mean using while to check the again the request in Stripe API
                do
                {
                    StripeConfiguration.ApiKey = apiKey;
                    if (string.IsNullOrEmpty(lastId)) //Getting the first 100 list
                    {
                        var options = new ChargeListOptions
                        {
                            Limit = 100,
                            Created = new DateRangeOptions()
                            {
                                GreaterThanOrEqual = firstSettlementDate,
                                LessThanOrEqual = lastSettlementDate
                            }
                        };
                        var service = new ChargeService();
                        charges = service.List(
                          options
                        );
                        if (charges != null && charges.Any())
                        {
                            if (charges.Data == null)
                                return stripeChargeDetails;
                            foreach (var apiChargedDetail in charges)
                            {
                                if (apiChargedDetail != null && apiChargedDetail.Status == "succeeded")  // We need to check the Status
                                {
                                    PaymentReconReport missingpayment = new PaymentReconReport();
                                    if (apiChargedDetail.Metadata.ContainsKey("TransactionId"))
                                    {
                                        missingpayment.TransactionId = apiChargedDetail.Metadata["TransactionId"];
                                        lstMissingPaymentModel.Add(missingpayment);
                                    }
                                    decimal stripeTotal = Utilities.Utilities.GetDecimal2Digits(apiChargedDetail.Amount);
                                    stripeTotalAmt += stripeTotal;
                                }
                            }
                            if (stripeChargeDetails != null)
                            {
                                stripeChargeDetails.SettledAmount = stripeTotalAmt;
                                stripeChargeDetails.ProductCode = productCode;
                            }
                        }
                    }
                    else //For checking last Id and getting the next 100 list from the last Id
                    {
                        var options = new ChargeListOptions
                        {
                            Limit = 100,
                            StartingAfter = lastId,//Assigning the Last Id as StartingAfter 
                            Created = new DateRangeOptions()
                            {
                                GreaterThanOrEqual = firstSettlementDate,
                                LessThanOrEqual = lastSettlementDate
                            }
                        };
                        var service = new ChargeService();
                        charges = service.List(
                          options
                        );
                        if (charges != null && charges.Any())
                        {
                            if (charges.Data == null)
                                return stripeChargeDetails;
                            foreach (var apiChargedDetail in charges)
                            {
                                if (apiChargedDetail != null && apiChargedDetail.Status == "succeeded")  // We need to check the Status
                                {
                                    PaymentReconReport missingpayment = new PaymentReconReport();
                                    if (apiChargedDetail.Metadata.ContainsKey("TransactionId"))
                                    {
                                        missingpayment.TransactionId = apiChargedDetail.Metadata["TransactionId"];
                                        lstMissingPaymentModel.Add(missingpayment);
                                    }
                                    decimal stripeTotal = Utilities.Utilities.Get2DecimalPoints(apiChargedDetail.Amount);
                                    stripeTotalAmt += stripeTotal;
                                }
                            }
                            if (stripeChargeDetails != null)
                            {
                                stripeChargeDetails.SettledAmount = stripeTotalAmt;
                                stripeChargeDetails.ProductCode = productCode;
                            }
                        }
                    }
                    if (charges != null && charges.Count() > 0)
                    {
                        lastId = charges.LastOrDefault().Id;//Get the last Id from the First 100 and assign here.
                    }
                } while (charges != null && charges.Data != null && charges.Data.Count == 100);
            }
            stripeChargeDetails.MissingPaymentTransactionId = new List<PaymentReconReport>();
            if (lstMissingPaymentModel != null && lstMissingPaymentModel.Count > 0)
            {
                stripeChargeDetails.MissingPaymentTransactionId = lstMissingPaymentModel;
            }
            return stripeChargeDetails;
        }

        #endregion

        #region Update Campaign Assigned Details Return Number By Campaign Assigned Details Id
        public bool UpdateCampaignAssignedDetailsReturnNumberByCampaignAssignedDetailsId(long campaignAssignedDetailsId, long returnNumber)
        {
            return _adminRepository.UpdateCampaignAssignedDetailsReturnNumberByCampaignAssignedDetailsId(campaignAssignedDetailsId, returnNumber);
        }
        #endregion
    }
}