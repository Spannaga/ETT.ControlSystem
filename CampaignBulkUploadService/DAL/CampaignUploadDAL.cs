using Main.Control.Core.Models;
using Main.Control.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampaignBulkUploadService.DAL
{
    public class CampaignUploadDAL
    {
        private AdminRepository adminRepository = null;
        private EmailRepository emailRepository = null;
        public CampaignUploadDAL()
        {
            adminRepository = new AdminRepository();
            emailRepository = new EmailRepository();
        }
        public CampaignDetails GetCampaignInitDetails()
        {
            CampaignDetails campaignDetails = new CampaignDetails();

            using (MainControlDB_Entities _context = new MainControlDB_Entities())
            {
                var dbInitCampaignDetails = _context.Biz_Campaign_Details.Where(m => !m.Is_Deleted && m.Batch_Status == BatchUploadStatus.INIT.ToString()).OrderBy(m => m.Campaign_Details_Id).Take(1).SingleOrDefault();
                if (dbInitCampaignDetails != null)
                {

                    var _dbdynamicCampaignDetails = (from a in _context.Biz_Campaign_Details
                                                     where a.Campaign_Details_Id == dbInitCampaignDetails.Campaign_Details_Id && !a.Is_Deleted && a.Admin_Project_Id > 0
                                                     select new
                                                     {
                                                         campaignDetails = a,
                                                         campaignAssignedDetails = a.Biz_Campaign_Assigned_Details,
                                                         campaignSupportUserDetails = a.Biz_Campaign_Support_User_Details,
                                                     }).SingleOrDefault();

                    if (_dbdynamicCampaignDetails != null)
                    {
                        var _dbCampaignDetails = _dbdynamicCampaignDetails.campaignDetails;
                        var _dbCampaignSupportUserDetails = _dbdynamicCampaignDetails.campaignSupportUserDetails.Where(x => !x.Is_Deleted).ToList();

                        campaignDetails.CampaignDetailsId = _dbCampaignDetails.Campaign_Details_Id;
                        campaignDetails.AdminUserId = _dbCampaignDetails.Admin_User_Id;
                        campaignDetails.CampaignName = _dbCampaignDetails.Campaign_Name;
                        campaignDetails.CampaignFileName = _dbCampaignDetails.Campaign_File_Name;
                        campaignDetails.FilePath = _dbCampaignDetails.File_Path;
                        campaignDetails.Goals = _dbCampaignDetails.Goals;
                        campaignDetails.NoOfLeads = _dbCampaignDetails.No_Of_Leads;
                        campaignDetails.UniqueId = _dbCampaignDetails.Unique_Id;
                        campaignDetails.IsPaused = _dbCampaignDetails.Is_Paused;
                        campaignDetails.IsSuspended = _dbCampaignDetails.Is_Suspended;
                        campaignDetails.AdminProjectId = _dbCampaignDetails.Admin_Project_Id;
                        campaignDetails.CampaignStartDate = _dbCampaignDetails.Campaign_Start_Date;
                        campaignDetails.CampaignEndDate = _dbCampaignDetails.Campaign_End_Date;
                        campaignDetails.IsUploadFileAssign = _dbCampaignDetails.Is_Upload_File_Assign;
                        campaignDetails.CreateTimeStamp = _dbCampaignDetails.Created_Time_Stamp;
                        campaignDetails.ProductName = _dbCampaignDetails.Static_Biz_Admin_Project.Project_Name;
                        campaignDetails.ProductId = _dbCampaignDetails.Admin_Project_Id ?? 0;
                        campaignDetails.TechTeamStatus = _dbCampaignDetails.Tech_Team_Status;
                        campaignDetails.CampaignType = _dbCampaignDetails.Campaign_Type;
                        campaignDetails.DemoGrapicInformation = _dbCampaignDetails.DemoGrapic_Information;
                        campaignDetails.TechTeamFileUploadedTime = _dbCampaignDetails.Tech_Team_File_Uploaded_Time;
                        campaignDetails.Notes = _dbCampaignDetails.Notes;
                        campaignDetails.BatchStatus = _dbCampaignDetails.Batch_Status;
                        campaignDetails.CampaignSupportUserDetailsList = new List<CampaignSupportUserDetails>();

                        if (_dbCampaignSupportUserDetails != null && _dbCampaignSupportUserDetails.Count > 0)
                        {
                            foreach (var dbuserSupportDetails in _dbCampaignSupportUserDetails)
                            {
                                CampaignSupportUserDetails campaignSupportUserDetails = new CampaignSupportUserDetails();
                                campaignSupportUserDetails.CampaignSupportUserDetailId = dbuserSupportDetails.Campaign_Support_User_Detail_Id;
                                campaignSupportUserDetails.CampaignDetailsId = dbuserSupportDetails.Campaign_Details_Id ?? 0;
                                campaignSupportUserDetails.AdminUserId = dbuserSupportDetails.Admin_User_Id ?? 0;
                                campaignSupportUserDetails.SupportUserId = dbuserSupportDetails.Support_User_Id ?? 0;
                                campaignSupportUserDetails.NoOfUserAssigned = dbuserSupportDetails.No_Of_User_Assigned;
                                campaignSupportUserDetails.NoOfCompleted = dbuserSupportDetails.No_Of_Completed;
                                campaignSupportUserDetails.NoOfPending = dbuserSupportDetails.No_Of_Pending;
                                campaignSupportUserDetails.UserSkippedCount = dbuserSupportDetails.User_Skipped_Count;
                                campaignSupportUserDetails.IsViewed = dbuserSupportDetails.Is_Viewed;
                                campaignSupportUserDetails.AdminUserName = dbuserSupportDetails.Biz_Admin_Users1.Admin_User_Name;
                                campaignDetails.CampaignSupportUserDetailsList.Add(campaignSupportUserDetails);
                            }
                        }


                    }
                }
            }
            return campaignDetails;

        }

        public void CampaignSetInProgress(CampaignDetails campaignDetails)
        {
            using (MainControlDB_Entities _context = new MainControlDB_Entities())
            {
                var dbCampaignDetails = _context.Biz_Campaign_Details.Where(m => !m.Is_Deleted && m.Campaign_Details_Id == campaignDetails.CampaignDetailsId).SingleOrDefault();
                if (dbCampaignDetails != null)
                {
                    dbCampaignDetails.Batch_Status = BatchUploadStatus.IN_PROGRESS.ToString();
                    dbCampaignDetails.Updated_Time_Stamp = DateTime.Now;
                    _context.SaveChanges();
                }
            }
        }

        public CampaignDetails SaveCampaignDetails(CampaignDetails campaignDetails)
        {
            return adminRepository.CreateCampainDetails(campaignDetails);
        }

        public AdminUser GetAdminUserById(long adminUserId)
        {
            return adminRepository.GetAdminUserById(adminUserId);
        }

        public MailDetails GetMailTemplateByTemplateId(int p)
        {
            return emailRepository.GetMailTemplateByTemplateId(p);
        }

        public CampaignDetails GetCampaignShortDetails(long CampaignDetailsId)
        {
            return adminRepository.GetCampaignShortDetailsByCampaignId(CampaignDetailsId);
        }

        #region Update Upload Status
        public void UpdateUploadStatus(CampaignDetails campaignDetails)
        {
            using (MainControlDB_Entities _context = new MainControlDB_Entities())
            {
                var dbCampaignDetails = _context.Biz_Campaign_Details.Where(m => !m.Is_Deleted && m.Campaign_Details_Id == campaignDetails.CampaignDetailsId).SingleOrDefault();
                if (dbCampaignDetails != null)
                {
                    dbCampaignDetails.Batch_Status = BatchUploadStatus.SUCCESS.ToString();
                    dbCampaignDetails.Updated_Time_Stamp = DateTime.Now;
                    _context.SaveChanges();
                }
            }
        }
        #endregion

        #region
        public List<CampaignAssignedDetails> GetManagerFollowUpDetails()
        {
            List<CampaignAssignedDetails> ManagerFollowUpDeails = new List<CampaignAssignedDetails>();
            DateTime previousDate1 = DateTime.Now.AddDays(-1);
            //DateTime previousDate2 = previousDate1.AddDays(-1);
            
            using (MainControlDB_Entities _context = new MainControlDB_Entities())
            {
                var _dbmanagerFollowUpDetails = _context.Biz_Campaign_Manager_Follow_Up_Details.Where(q => q.Created_Time_Stamp.Day == previousDate1.Day && q.Created_Time_Stamp.Month == previousDate1.Month && q.Created_Time_Stamp.Year == previousDate1.Year && q.Campaign_Status == "INIT" && !q.Is_Deleted).ToList();
                if(_dbmanagerFollowUpDetails != null && _dbmanagerFollowUpDetails.Count>0)
                {
                    foreach(var item in _dbmanagerFollowUpDetails)
                    {
                        CampaignAssignedDetails followUpDetails = new CampaignAssignedDetails();
                        followUpDetails.AdminUserId = item.Admin_User_Id;
                        followUpDetails.CampaignFollowUpId = item.Campaign_Follow_Up_Id;
                        followUpDetails.CampaignStatus = item.Campaign_Status;
                        followUpDetails.Comments = item.Comments;
                        followUpDetails.EmailAddress = item.User_Email_Address;
                        followUpDetails.UserId = item.User_Id;
                        followUpDetails.Name = item.User_Name;
                        followUpDetails.PhoneNumber = item.User_Phone;
                        ManagerFollowUpDeails.Add(followUpDetails);
                    }
                }
            }
            return ManagerFollowUpDeails;
        }
        #endregion

        #region Get Campaign Detail By Campaign DetailId
        public CampaignDetails GetCampaignDetailByDetailId(long campaignDetailId)
        {
            CampaignDetails campaignDetail = new CampaignDetails();
            if (campaignDetailId > 0)
            {
                using (MainControlDB_Entities _context = new MainControlDB_Entities())
                {
                    Biz_Campaign_Details dbBizCampaignDetails = null;
                    dbBizCampaignDetails = _context.Biz_Campaign_Details.Where(m => m.Campaign_Details_Id == campaignDetailId).SingleOrDefault();
                    if (dbBizCampaignDetails.Campaign_Details_Id > 0)
                    {
                        campaignDetail.CampaignDetailsId = dbBizCampaignDetails.Campaign_Details_Id;
                        campaignDetail.CampaignName = dbBizCampaignDetails.Campaign_Name;
                        campaignDetail.Goals = dbBizCampaignDetails.Goals;
                        campaignDetail.CampaignStartDate = dbBizCampaignDetails.Campaign_Start_Date;
                        campaignDetail.CampaignEndDate = dbBizCampaignDetails.Campaign_End_Date;
                    }
                }
            }
            return campaignDetail;
        }
        #endregion
    }
}
