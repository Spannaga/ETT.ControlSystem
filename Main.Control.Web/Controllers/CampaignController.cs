using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Main.Control.Core.Models;
using Main.Control.Core.Services;
using Main.Control.Service.Utilities;
using Main.Control.Utilities.Infrastructure;
using Main.Control.Web.Utilities;
using ProcessBulkUpload;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using Main.Control.Web.Models;
using ClosedXML.Excel;

namespace Main.Control.Web.Controllers
{
    public class CampaignController : BaseController
    {

        #region Declaration
        private readonly IAdminService _adminService;
        private readonly ISpanControlService _mainControlService;
        private readonly BulkUploadCommon bulkUploadCommon;

        protected struct LocalConstants
        {
            public const string RememberMe = "RememberMe";
        }
        #endregion

        #region Constructor
        public CampaignController()
        {
        }
        public CampaignController(ISpanControlService mainControlService, IAdminService adminservice) // : base(mainControlService, adminservice)
        {

            this._mainControlService = mainControlService;
            this._adminService = adminservice;
            bulkUploadCommon = new BulkUploadCommon();
        }
        #endregion

        #region Campaign Index
        [HttpGet]
        public ActionResult Index(long? id)
        {
            long _adminUserId = Utility.GetAdminUserIdFromSession();
            List<AdminUserRole> adminRoleLst = this._adminService.GetAllAdminProjectRole(_adminUserId).ToList();
            long adminRoleId = 0;
            if (adminRoleLst != null && adminRoleLst.Count > 0)
            {
                if (adminRoleLst.Where(m => m.AdminRoleId == (int)AdminRoleType.Administrator).ToList().Any())
                {
                    adminRoleId = (int)AdminRoleType.Administrator;
                }
                else if (adminRoleLst.Where(m => m.AdminRoleId == (int)AdminRoleType.Manager).ToList().Any())
                {
                    adminRoleId = (int)AdminRoleType.Manager;
                }
                else if (adminRoleLst.Where(m => m.AdminRoleId == (int)AdminRoleType.Team).ToList().Any())
                {
                    adminRoleId = (int)AdminRoleType.Team;
                }
            }

            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            param.AdminUserId = _adminUserId;
            int count = this._adminService.GetCampaignSupportUserDetailsCount(param);
            ViewBag.SearchLeadCount = count;
            ViewBag.ShowTab = id ?? 1;
            ViewBag.AdminRoleId = adminRoleId;

            return View();
        }
        #endregion

        #region Campaign Dashboard
        [HttpGet]
        public ActionResult _CampaignDashboard(int? ProjectId, string CampaignStatus, int PageIndex = 0, int PageSize = 5)
        {
            int skipRecords = PageIndex * PageSize;
            int takeRecords = PageSize;
            long _adminUserId = Utility.GetAdminUserIdFromSession();
            var staticAdminProjects = this._adminService.GetStaticBizAdminProjects();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            param.AdminUserId = _adminUserId;
            param.ProjectId = ProjectId ?? 0;
            param.CampaignStatus = CampaignStatus;
            if (string.IsNullOrWhiteSpace(param.CampaignStatus))
            {
                param.CampaignStatus = "All";
            }
            param.SkipRecords = skipRecords;
            param.TakeRecords = takeRecords;
            int count = this._adminService.GetCampaignDetailsCountByAdminUserId(param);

            int maxPageSizeCount = count / PageSize;
            if (count % PageSize > 0)
            {
                maxPageSizeCount++;
            }
            ViewData["MaxPageCount"] = maxPageSizeCount;

            List<CampaignDetails> campaignDetailsList = null;
            if (param.ProjectId == 0 && string.IsNullOrWhiteSpace(param.CampaignStatus))
            {
                campaignDetailsList = this._adminService.GetCampaignDetails(param);
            }
            else
            {
                campaignDetailsList = this._adminService.GetCampaignDetailsByFilters(param);
            }

            staticAdminProjects.Insert(0, new BizAdminProjects() { ProjectId = 0, ProjectName = "All" });
            ViewBag.AdminProjects = staticAdminProjects.Select(x => new SelectListItem()
            {
                Text = x.ProjectName.ToString(),
                Value = x.ProjectId.ToString(),
                Selected = x.ProjectId == param.ProjectId
            });
            var campaignStatus = Enum.GetValues(typeof(CampaignStatus))
                                .Cast<CampaignStatus>()
                                .Select(v => v.ToString())
                                .ToList();
            campaignStatus.Insert(0, "All");
            ViewBag.CampaignStatus = campaignStatus.Select(x => new SelectListItem()
            {
                Text = x.ToString(),
                Value = x.ToString(),
                Selected = x.ToString() == param.CampaignStatus.ToString()
            });
            return PartialView(campaignDetailsList);
        }
        #endregion

        #region Create Campaign - Get
        [HttpGet]
        public ActionResult CreateCampaign(int? id, int? id2, long? id3)
        {
            int productId = id ?? 0;
            bool inValidErrorMessage = false;
            long campaignDetailsId = id3 ?? 0;
            if (id2 > 0)
            {
                inValidErrorMessage = true;
            }

            long _adminUserId = Utility.GetAdminUserIdFromSession();
            CampaignDetails campaignDetails = new CampaignDetails();
            if (campaignDetailsId == 0)
            {
                campaignDetails.ProductId = productId;
                var adminProject = this._adminService.GetProjectNameByProjectId(productId);
                if (adminProject != null)
                {
                    campaignDetails.ProductName = adminProject.ProjectName;
                }
            }
            else
            {
                campaignDetails = this._adminService.GetCampaignShortDetailsByCampaignId(campaignDetailsId);
            }


            ViewBag.SupportUsersList = this._adminService.GetAllAdminSupportUsers();
            ViewBag.inValidErrorMessage = inValidErrorMessage;
            return View(campaignDetails);
        }
        #endregion

        #region Create Campaign - Post
        [HttpPost]
        public ActionResult CreateCampaign(FormCollection form)
        {
            long _adminUserId = Utility.GetAdminUserIdFromSession();
            var ColumnNameList = new List<string>();
            CampaignDetails campaignDetails = new CampaignDetails();
            campaignDetails.CampaignDetailsId = Utility.GetInt(form["CampaignDetailsId"]);
            campaignDetails.ProductId = Utility.GetInt(form["ProductId"]);
            campaignDetails.ProductName = form["ProductName"];
            campaignDetails.FilePath = form["FilePath"];
            campaignDetails.TechTeamStatus = form["TechTeamStatus"];
            campaignDetails.Notes = form["Notes"];
            campaignDetails.NoOfLeads = Utility.GetInt(form["NoOfLeads"]);
            campaignDetails.DemoGrapicInformation = form["DemoGrapicInformation"];


            if (Utility.GetDateTime(form["TechTeamFileUploadedTime"]) > DateTime.MinValue)
            {
                campaignDetails.TechTeamFileUploadedTime = Utility.GetDateTime(form["TechTeamFileUploadedTime"]);
            }


            if (Utility.GetBool(form["IsLead"]))
            {
                campaignDetails.CampaignType = CampaignType.LEADS.ToString();
            }
            else
            {
                campaignDetails.CampaignType = CampaignType.CLIENTS.ToString();
            }


            campaignDetails.AdminUserId = _adminUserId;
            string supportUserIds = form["SupportUserIds"];
            string[] supportUserList = supportUserIds.Split(',').ToArray();

            if (supportUserList != null && supportUserList.Length > 0)
            {
                if (campaignDetails.SupportUserList == null)
                {
                    campaignDetails.SupportUserList = new List<long>();
                }
                foreach (var supportUserId in supportUserList)
                {
                    campaignDetails.SupportUserList.Add(Utility.GetLong(supportUserId));
                }
            }

            campaignDetails.CampaignFileName = form["CampaignFileName"];

            campaignDetails.CampaignName = form["CampaignName"];

            campaignDetails.IsUploadFileAssign = Utility.GetBool(form["IsUploadFileAssign"]);


            if (form["CampaignStartDate"] != null && Utility.GetDateTime(form["CampaignStartDate"]) > DateTime.MinValue)
            {
                campaignDetails.CampaignStartDate = Utility.GetDateTime(form["CampaignStartDate"]);
            }

            if (form["CampaignEndDate"] != null && Utility.GetDateTime(form["CampaignEndDate"]) > DateTime.MinValue)
            {
                campaignDetails.CampaignEndDate = Utility.GetDateTime(form["CampaignEndDate"]);
            }

            campaignDetails.Goals = form["Goals"];

            if (!string.IsNullOrWhiteSpace(campaignDetails.TechTeamStatus))
            {
                string[] splitArray = campaignDetails.TechTeamStatus.Split(',').ToArray();
                campaignDetails.TechTeamStatus = splitArray[0];
            }

            if (!campaignDetails.IsUploadFileAssign)
            {
                if (campaignDetails.CampaignDetailsId == 0 || (campaignDetails.CampaignDetailsId > 0 && campaignDetails.TechTeamStatus == TechTeamStatus.DISCARDED_REQUEST.ToString()))
                {
                    campaignDetails.TechTeamStatus = TechTeamStatus.TECH_TEAM_NOTIFY.ToString();
                    this._adminService.SaveRequestToFileUploadCampaign(campaignDetails);
                    return RedirectToAction("RequestTechTeamCampaign");
                }
            }

            int totalRows = 0;

            if (Request.Files["campaignFile"] != null && Request.Files["campaignFile"].ContentLength > 0) // If uploaded synchronously
            {
                DataTable dt = new DataTable();
                HttpPostedFileBase hpf = Request.Files["campaignFile"] as HttpPostedFileBase;
                //do stuff with the bytes
                byte[] Imgbuf = new byte[hpf.ContentLength];
                hpf.InputStream.Read(Imgbuf, 0, hpf.ContentLength);
                string _FileName = Path.GetFileName(hpf.FileName).Replace(" ", "_").Replace(",", "_");
                campaignDetails.CampaignFileName = _FileName;
                _FileName = _FileName.Replace("(", "_").Replace(")", "");
                string _FileExtension = _FileName.Substring(_FileName.LastIndexOf('.') + 1, _FileName.Length - (_FileName.LastIndexOf('.') + 1));
                string _ReturnExtension = ReturnFileExtnSmartImp(_FileExtension);
                string _Filefolderpath = ConfigurationManager.AppSettings["AdminImportFolderPath"].ToString();
                if (!Directory.Exists(_Filefolderpath))
                {
                    Directory.CreateDirectory(_Filefolderpath);
                }

                string _Filefolderpath1 = Path.Combine(_Filefolderpath, Utility.GetAdminUserIdFromSession().ToString());

                if (!Directory.Exists(_Filefolderpath1))
                {
                    Directory.CreateDirectory(_Filefolderpath1);
                }

                _Filefolderpath1 = Path.Combine(_Filefolderpath1, "Followups");

                if (!Directory.Exists(_Filefolderpath1))
                {
                    Directory.CreateDirectory(_Filefolderpath1);
                }

                string filePathWithoutFileName = _Filefolderpath1;


                _Filefolderpath1 = Path.Combine(_Filefolderpath1, _FileName);
                string originalfilenameWithoutexten = Path.GetFileNameWithoutExtension(_Filefolderpath1);
                string exten = Path.GetExtension(_Filefolderpath1);
                if (System.IO.File.Exists(_Filefolderpath1))
                {
                    string[] fileName = Directory.GetFiles(filePathWithoutFileName).OrderByDescending(a => a).ToArray();
                    if (fileName != null && fileName.Any())
                    {
                        int renameextention = 1;
                        for (int i = 0; i < fileName.Count(); i++)
                        {
                            if (fileName.Any(a => a == _Filefolderpath1))
                            {
                                renameextention++;
                                string tempFileName1 = string.Format("{0}({1})", originalfilenameWithoutexten, renameextention);
                                string fileTemp1 = tempFileName1 + exten;
                                _Filefolderpath1 = Path.Combine(filePathWithoutFileName, fileTemp1);
                            }
                            if (!fileName.Any(a => a == _Filefolderpath1))
                            {
                                break;
                            }
                        }
                    }
                }

                hpf.SaveAs(_Filefolderpath1);
                campaignDetails.FilePath = _Filefolderpath1;


                string connString = Utility.GetImportConnectionString(_Filefolderpath1, exten);
                DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.OleDb");

                using (DbConnection connection = factory.CreateConnection())
                {
                    if (connection != null)
                    {
                        connection.ConnectionString = connString;
                        try
                        {
                            connection.Open();
                        }
                        catch (OleDbException ex)
                        {
                            if (ex.ErrorCode == -2147467259)
                            {
                                connection.ConnectionString = connString = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + _Filefolderpath1 + ";" + "Extended Properties=\"Excel 12.0;HDR=Yes;\"";
                                connection.Open();
                            }
                        }

                        using (DbCommand command = connection.CreateCommand())
                        {
                            string fileX = string.Empty;
                            string sheetName = string.Empty;
                            DataTable sheetTable = connection.GetSchema("Tables");


                            using (DbCommand command2 = connection.CreateCommand())
                            {
                                foreach (DataRow row in sheetTable.Rows)
                                {
                                    sheetName = (string)row["TABLE_NAME"];

                                    if (sheetName.EndsWith("$'") && sheetName.StartsWith("'"))
                                    {
                                        sheetName = sheetName.Remove(0, 1);
                                        sheetName = sheetName.Remove(sheetName.Length - 1, 1);
                                    }
                                    if(sheetName == "Users$")
                                    {
                                        command2.CommandText = "SELECT COUNT(*) FROM [Users$] WHERE " + Constants.T_Name + " IS NOT NULL OR " +
                                         "[" + Constants.T_Email_Address + "]" + " IS NOT NULL OR " +
                                         "[" + Constants.T_Phone_Number + "]" + " IS NOT NULL "
                                         ;
                                    }
                                    else if(sheetName == "User$")
                                    {
                                        command2.CommandText = "SELECT COUNT(*) FROM [User$] WHERE " + Constants.T_Name + " IS NOT NULL OR " +
                                         "[" + Constants.T_Phone_Number + "]" + " IS NOT NULL OR " + "[" + Constants.T_Return_Number + "]" + " IS NOT NULL ";
                                    }
                                    
                                    if (sheetName == "Users$")
                                    {
                                        totalRows = Utility.GetInt(command2.ExecuteScalar());
                                    }
                                    else if(sheetName == "User$")
                                    {
                                        totalRows = Utility.GetInt(command2.ExecuteScalar());
                                    }
                                    else if (sheetName != "Sheet2$" && sheetName != "Sheet3$")
                                    {
                                        TempData["FileUploadError"] = "Uploaded Template Sheet Name Is Invalid. Please Do Not Rename The Default Sheet Name";
                                        return RedirectToAction(ActionNames.CreateCampaign, ControllerNames.Campaign, new { id = campaignDetails.ProductId });
                                    }
                                }

                                if (totalRows > Utility.GetInt(Utility.GetAppSettings(Constants.BulkUploadCount)))
                                {
                                    campaignDetails.BatchStatus = BatchUploadStatus.INIT.ToString();
                                    campaignDetails.IsBatchStatus = true;
                                }
                                else
                                {
                                    campaignDetails.IsBatchStatus = false;
                                }
                                campaignDetails.IsBatchProcess = false;

                                if (totalRows >= 1)
                                {
                                    if(!string.IsNullOrWhiteSpace(sheetName)&& sheetName == "Users$")
                                    {
                                        command.CommandText = "SELECT * FROM [Users$] WHERE " + Constants.T_Name + " IS NOT NULL OR " +
                                                                                "[" + Constants.T_Email_Address + "]" + " IS NOT NULL OR " +
                                                                                "[" + Constants.T_Phone_Number + "]" + " IS NOT NULL"
                                                                                ;
                                    }
                                    else if(!string.IsNullOrWhiteSpace(sheetName) && sheetName == "User$")
                                    {
                                        command.CommandText = "SELECT * FROM [User$] WHERE " + Constants.T_Name + " IS NOT NULL OR " +
                                         "[" + Constants.T_Phone_Number + "]" + " IS NOT NULL OR " +
                                         "[" + Constants.T_Return_Number + "]" + " IS NOT NULL "
                                         ;
                                    }
                                   
                                    DbDataReader dr = command.ExecuteReader();
                                    dt = dr.GetSchemaTable();
                                    if (dt != null)
                                    {
                                        foreach (DataRow datarow in dt.Rows)
                                        {
                                            string strColumnName = string.Empty;
                                            strColumnName = Utility.RemoveSpaceandSpecialChars(datarow["ColumnName"].ToString());
                                            strColumnName = strColumnName.Trim();
                                            ColumnNameList.Add(strColumnName);
                                        }
                                    }

                                    if (ColumnNameList != null && ColumnNameList.Count > 0)
                                    {
                                        if(sheetName == "Users$")
                                        {
                                            if (ColumnNameList.Contains(Constants.T_Name) &&
                                            ColumnNameList.Contains(Constants.T_Email_Address) &&
                                            ColumnNameList.Contains(Constants.T_Phone_Number)
                                            )
                                            {
                                                campaignDetails.NoOfLeads = totalRows;

                                                Session["CampaignSummary" + Utility.GetAdminUserIdFromSession()] = campaignDetails;

                                                return RedirectToAction("CampaignSummary");
                                            }
                                            else
                                            {
                                                return RedirectToAction("CreateCampaign", new { id = campaignDetails.ProductId, id2 = 1 });
                                            }
                                        }
                                        else if(sheetName == "User$")
                                        {
                                            if (ColumnNameList.Contains(Constants.T_Name) &&
                                            ColumnNameList.Contains(Constants.T_Phone_Number) &&
                                            ColumnNameList.Contains(Constants.T_Return_Number)
                                            )
                                            {
                                                campaignDetails.NoOfLeads = totalRows;

                                                Session["CampaignSummary" + Utility.GetAdminUserIdFromSession()] = campaignDetails;

                                                return RedirectToAction("CampaignSummary");
                                            }
                                            else
                                            {
                                                return RedirectToAction("CreateCampaign", new { id = campaignDetails.ProductId, id2 = 1 });
                                            }
                                        }
                                        
                                    }
                                }
                                connection.Close();
                            }
                        }

                        connection.Close();
                    }
                }

            }
            else if (campaignDetails.CampaignDetailsId > 0 && !string.IsNullOrWhiteSpace(campaignDetails.FilePath) && campaignDetails.TechTeamStatus == TechTeamStatus.FILE_UPLOADED.ToString())
            {
                string exten = Path.GetExtension(campaignDetails.FilePath);
                string connString = Utility.GetImportConnectionString(campaignDetails.FilePath, exten);
                DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.OleDb");

                using (DbConnection connection = factory.CreateConnection())
                {
                    if (connection != null)
                    {
                        connection.ConnectionString = connString;
                        try
                        {
                            connection.Open();
                        }
                        catch (OleDbException ex)
                        {
                            if (ex.ErrorCode == -2147467259)
                            {
                                connection.ConnectionString = connString = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + campaignDetails.FilePath + ";" + "Extended Properties=\"Excel 12.0;HDR=Yes;\"";
                                connection.Open();
                            }
                        }

                        using (DbCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "SELECT COUNT(*) FROM [Users$] WHERE " + Constants.T_Name + " IS NOT NULL OR " +
                                        "[" + Constants.T_Email_Address + "]" + " IS NOT NULL OR " +
                                         Constants.T_EIN + " IS NOT NULL OR " +
                                        "[" + Constants.T_Phone_Number + "]" + " IS NOT NULL OR " +
                                        Constants.T_Address + " IS NOT NULL";

                            totalRows = Utility.GetInt(command.ExecuteScalar());

                            if (totalRows > Utility.GetInt(Utility.GetAppSettings(Constants.BulkUploadCount)))
                            {
                                campaignDetails.BatchStatus = BatchUploadStatus.INIT.ToString();
                                campaignDetails.IsBatchStatus = true;
                            }
                            else
                            {
                                campaignDetails.IsBatchStatus = false;
                            }
                            campaignDetails.IsBatchProcess = false;


                        }

                    }
                }



                campaignDetails.TechTeamStatus = TechTeamStatus.CAMPAIGN_CREATED.ToString();
                Session["CampaignSummary" + Utility.GetAdminUserIdFromSession()] = campaignDetails;
                return RedirectToAction("CampaignSummary");
            }

            if (totalRows == 0)
            {
                ViewBag.SupportUsersList = this._adminService.GetAllAdminSupportUsers();
                campaignDetails.CampaignErrorStatus = "No record found";
            }

            return View(campaignDetails);
        }
        #endregion

        #region Find File Extension



        private string ReturnFileExtnSmartImp(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case "xls":
                    return "xls";
                case "xlsx":
                    return "xlsx";
                case "csv":
                    return "csv";
                default:
                    return null;
            }
        }
        #endregion

        #region Load Projects
        /// <summary>
        /// Load Projects
        /// </summary>
        /// <returns></returns>
        [SessionAlive(Order = 1)]
        [TaxAuthorize()]
        public ActionResult _CampaignAccessProduct()
        {
            long _adminUserId = Utility.GetAdminUserIdFromSession();
            AdminUser _adminUser = this._adminService.GetAdminUserById(_adminUserId);

            _adminUser.AdminUserRoleList = this._adminService.GetAllAdminProjectRole(_adminUserId);


            return PartialView(_adminUser);
        }
        #endregion

        #region Campaign Summary - GET
        [HttpGet]
        public ActionResult CampaignSummary()
        {
            CampaignDetails campaignDetails = new CampaignDetails();
            long _adminUserId = Utility.GetAdminUserIdFromSession();

            if (Session["CampaignSummary" + _adminUserId] != null)
            {
                campaignDetails = (CampaignDetails)Session["CampaignSummary" + _adminUserId];
                if (campaignDetails != null && campaignDetails.SupportUserList != null && campaignDetails.SupportUserList.Count > 0)
                {
                    if (campaignDetails.SupportAdminUserList == null)
                    {
                        campaignDetails.SupportAdminUserList = new List<AdminUser>();
                    }

                    var adminUserList = this._adminService.GetAllAdminSupportUsers();

                    var existsAdminUsersSupportUsersList = adminUserList.Where(m => campaignDetails.SupportUserList.Contains(m.UserId)).ToList();

                    campaignDetails.SupportAdminUserList = existsAdminUsersSupportUsersList;


                    if (campaignDetails.SupportAdminUserList != null && campaignDetails.SupportAdminUserList.Count > 0)
                    {
                        int assignedSubLevelCount = campaignDetails.NoOfLeads / campaignDetails.SupportAdminUserList.Count();

                        foreach (var assinedCountobj in campaignDetails.SupportAdminUserList)
                        {
                            assinedCountobj.TotalAssignedCount = assignedSubLevelCount;
                        }

                        if (campaignDetails.NoOfLeads > campaignDetails.SupportAdminUserList.Sum(m => m.TotalAssignedCount))
                        {
                            var differenceCount = campaignDetails.NoOfLeads - campaignDetails.SupportAdminUserList.Sum(m => m.TotalAssignedCount);
                            if (differenceCount > 0)
                            {
                                for (var index = 0; index < differenceCount; index++)
                                {
                                    var supportObj = campaignDetails.SupportAdminUserList[index];
                                    if (supportObj != null)
                                    {
                                        supportObj.TotalAssignedCount += 1;
                                    }
                                }
                            }
                        }

                    }

                }
            }
            return View(campaignDetails);
        }
        #endregion

        #region Delete Campaign
        [HttpGet]
        public ActionResult DeleteCampaignSessionValue()
        {
            Session["CampaignSummary" + Utility.GetAdminUserIdFromSession()] = null;
            return RedirectToAction("Index", new { id = Session[SessionItemKey.AdminCategoryId] });
        }
        #endregion

        #region Campaign Summary - POST
        [HttpPost]
        public ActionResult CampaignSummary(FormCollection form)
        {
            CampaignDetails campaignDetails = new CampaignDetails();

            long _adminUserId = Utility.GetAdminUserIdFromSession();

            if (Session["CampaignSummary" + _adminUserId] != null)
            {
                campaignDetails = (CampaignDetails)Session["CampaignSummary" + _adminUserId];

                campaignDetails.AdminUserId = _adminUserId;

                var adminUserList = this._adminService.GetAllAdminSupportUsers();

                var existsAdminUsersSupportUsersList = adminUserList.Where(m => campaignDetails.SupportUserList.Contains(m.UserId)).ToList();

                campaignDetails.SupportAdminUserList = existsAdminUsersSupportUsersList;

                if (campaignDetails.SupportAdminUserList != null && campaignDetails.SupportAdminUserList.Count > 0)
                {
                    int assignedSubLevelCount = campaignDetails.NoOfLeads / campaignDetails.SupportAdminUserList.Count();

                    foreach (var assinedCountobj in campaignDetails.SupportAdminUserList)
                    {
                        assinedCountobj.TotalAssignedCount = assignedSubLevelCount;
                    }

                    if (campaignDetails.NoOfLeads > campaignDetails.SupportAdminUserList.Sum(m => m.TotalAssignedCount))
                    {
                        var differenceCount = campaignDetails.NoOfLeads - campaignDetails.SupportAdminUserList.Sum(m => m.TotalAssignedCount);
                        if (differenceCount > 0)
                        {
                            for (var index = 0; index < differenceCount; index++)
                            {
                                var supportObj = campaignDetails.SupportAdminUserList[index];
                                if (supportObj != null)
                                {
                                    supportObj.TotalAssignedCount += 1;
                                }
                            }
                        }
                    }

                }

                if (campaignDetails.CampaignAssignedDetailsList == null)
                {
                    campaignDetails.CampaignAssignedDetailsList = new List<CampaignAssignedDetails>();
                }

                campaignDetails = bulkUploadCommon.CommonExcelValuesAssignedCampaignDetails(campaignDetails);


                this._adminService.CreateCampainDetails(campaignDetails);

                if (campaignDetails.CampaignAssignedDetailsList != null && campaignDetails.CampaignAssignedDetailsList.Count == 0)
                {
                    return RedirectToAction("UploadStatus", new { id = Session[SessionItemKey.AdminCategoryId] });
                }
                else
                {
                    return RedirectToAction("Index", new { id = Session[SessionItemKey.AdminCategoryId] });
                }

            }
            return RedirectToAction("Index", new { id = Session[SessionItemKey.AdminCategoryId] });
        }
        #endregion

        #region Extend Campaign Peroid - GET
        [HttpGet]
        public ActionResult _ExtendPeroid(long id)
        {
            long campaignDetailId = id;
            CampaignDetails campaignDetails = new CampaignDetails();
            campaignDetails = this._adminService.GetCampaignShortDetailsByCampaignId(campaignDetailId);
            return PartialView(campaignDetails);
        }
        #endregion

        #region Suspend Campaign - GET
        [HttpGet]
        public ActionResult _SuspendCampaign(long id)
        {
            long campaignDetailId = id;
            CampaignDetails campaignDetails = new CampaignDetails();
            campaignDetails = this._adminService.GetCampaignShortDetailsByCampaignId(campaignDetailId);
            return PartialView(campaignDetails);
        }
        #endregion

        #region Extend Campaign Peroid - Post
        [HttpPost]
        public ActionResult _ExtendPeroid(FormCollection form)
        {
            CampaignDetails campaignDetails = new CampaignDetails();
            campaignDetails.CampaignDetailsId = Utility.GetLong(form["CampaignDetailsId"]);
            if (form["CampaignStartDate"] != null && Utility.GetDateTime(form["CampaignStartDate"]) > DateTime.MinValue)
            {
                campaignDetails.CampaignStartDate = Utility.GetDateTime(form["CampaignStartDate"]);
            }

            if (form["CampaignEndDate"] != null && Utility.GetDateTime(form["CampaignEndDate"]) > DateTime.MinValue)
            {
                campaignDetails.CampaignEndDate = Utility.GetDateTime(form["CampaignEndDate"]);
            }

            bool isExtendDateUpdated = this._adminService.UpdateCampaignExtendDate(campaignDetails);

            return Json(isExtendDateUpdated, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Suspend Campaign - Post
        [HttpPost]
        public ActionResult _SuspendCampaign(FormCollection form)
        {
            CampaignDetails campaignDetails = new CampaignDetails();
            campaignDetails.CampaignDetailsId = Utility.GetLong(form["CampaignDetailsId"]);
            campaignDetails.SuspendReason = form["SuspendReason"];
            bool isSuspendCampaign = this._adminService.SuspendCampaign(campaignDetails);
            return Json(isSuspendCampaign, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Update Campaign Paused Status
        [HttpGet]
        public ActionResult UpdateCampaignPauseStatus(long id, bool id2)
        {
            long campaignDetailsId = id;
            bool isPaused = id2;
            bool isPausedUpdated = this._adminService.UpdateCampaignPauseStatus(campaignDetailsId, isPaused);
            return Json(isPausedUpdated, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Update Campaign Paused Status
        [HttpGet]
        public ActionResult UpdateCampaignSuspendStatus(long id, bool id2)
        {
            long campaignDetailsId = id;
            bool isSuspend = id2;
            bool isSuspendUpdated = this._adminService.UpdateCampaignSuspendStatus(campaignDetailsId, isSuspend);
            return Json(isSuspendUpdated, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Request Tech Team Campaign
        [HttpGet]
        public ActionResult RequestTechTeamCampaign()
        {
            return View();
        }
        #endregion

        #region Delete Campaign
        [HttpGet]
        public ActionResult DisCardedCampaign(long id)
        {
            long campaignDetailsId = id;
            bool isDeleted = this._adminService.DeleteCampaign(campaignDetailsId);
            return Json(isDeleted, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Campaign Dashboard
        [HttpGet]
        public ActionResult _CampaignUploadRequest()
        {
            long _adminUserId = Utility.GetAdminUserIdFromSession();

            List<CampaignDetails> campaignDetailsList = this._adminService.GetCampaignUploadRequestDetails();

            return PartialView(campaignDetailsList);
        }
        #endregion

        #region Campain Tech Team Discarded
        [HttpGet]
        public ActionResult _DiscardedRequest(long id)
        {
            long campaignDetailId = id;
            CampaignDetails campaignDetails = new CampaignDetails();
            campaignDetails = this._adminService.GetCampaignShortDetailsByCampaignId(campaignDetailId);
            return PartialView(campaignDetails);
        }
        #endregion

        #region Campain Tech Team Discarded
        [HttpPost]
        public ActionResult _DiscardedRequest(FormCollection form)
        {
            CampaignDetails campaignDetails = new CampaignDetails();
            campaignDetails.CampaignDetailsId = Utility.GetLong(form["CampaignDetailsId"]);
            campaignDetails.DiscardedReason = form["DiscardedReason"];
            campaignDetails.TechTeamStatus = TechTeamStatus.DISCARDED_REQUEST.ToString();
            campaignDetails.IsDiscardedRequest = true;
            bool isUpdatedRequestedCancel = this._adminService.UpdateDiscardedRequestReason(campaignDetails);
            return Json(isUpdatedRequestedCancel, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region File Upload Tech Team
        [HttpGet]
        public ActionResult FileUploadTechTeam(long id, int? id2)
        {
            long campaignDetailId = id;
            bool isInvalidTemplate = (id2 ?? 0) > 0 ? true : false;
            CampaignDetails campaignDetails = new CampaignDetails();
            campaignDetails = this._adminService.GetCampaignShortDetailsByCampaignId(campaignDetailId);
            ViewBag.isInvalidTemplate = isInvalidTemplate;
            return View(campaignDetails);
        }
        #endregion

        #region Campain Tech Team Discarded
        [HttpPost]
        public ActionResult FileUploadTechTeam(FormCollection form)
        {
            long _adminUserId = Utility.GetAdminUserIdFromSession();
            var ColumnNameList = new List<string>();
            CampaignDetails campaignDetails = new CampaignDetails();
            campaignDetails.CampaignDetailsId = Utility.GetInt(form["CampaignDetailsId"]);
            campaignDetails.ProductId = Utility.GetInt(form["ProductId"]);
            campaignDetails.ProductName = form["ProductName"];
            campaignDetails.AdminUserId = _adminUserId;

            campaignDetails.CampaignName = form["CampaignName"];

            campaignDetails.IsUploadFileAssign = Utility.GetBool(form["IsUploadFileAssign"]);

            campaignDetails.DemoGrapicInformation = form["DemoGrapicInformation"];

            campaignDetails.Notes = form["Notes"];

            campaignDetails.TechTeamFileUploadedTime = DateTime.Now;
            campaignDetails.UploaderName = Utility.GetAdminUserNameFromSession();
            if (Request.Files["campaignFile"] != null && Request.Files["campaignFile"].ContentLength > 0) // If uploaded synchronously
            {
                DataTable dt = new DataTable();
                HttpPostedFileBase hpf = Request.Files["campaignFile"] as HttpPostedFileBase;
                //do stuff with the bytes
                byte[] Imgbuf = new byte[hpf.ContentLength];
                hpf.InputStream.Read(Imgbuf, 0, hpf.ContentLength);
                string _FileName = Path.GetFileName(hpf.FileName).Replace(" ", "_").Replace(",", "_");
                campaignDetails.CampaignFileName = _FileName;
                _FileName = _FileName.Replace("(", "_").Replace(")", "");
                string _FileExtension = _FileName.Substring(_FileName.LastIndexOf('.') + 1, _FileName.Length - (_FileName.LastIndexOf('.') + 1));
                string _ReturnExtension = ReturnFileExtnSmartImp(_FileExtension);
                string _Filefolderpath = ConfigurationManager.AppSettings["AdminImportFolderPath"].ToString();
                if (!Directory.Exists(_Filefolderpath))
                {
                    Directory.CreateDirectory(_Filefolderpath);
                }

                string _Filefolderpath1 = Path.Combine(_Filefolderpath, Utility.GetAdminUserIdFromSession().ToString());

                if (!Directory.Exists(_Filefolderpath1))
                {
                    Directory.CreateDirectory(_Filefolderpath1);
                }

                _Filefolderpath1 = Path.Combine(_Filefolderpath1, "Followups");

                if (!Directory.Exists(_Filefolderpath1))
                {
                    Directory.CreateDirectory(_Filefolderpath1);
                }

                string filePathWithoutFileName = _Filefolderpath1;

                _Filefolderpath1 = Path.Combine(_Filefolderpath1, _FileName);
                string originalfilenameWithoutexten = Path.GetFileNameWithoutExtension(_Filefolderpath1);
                string exten = Path.GetExtension(_Filefolderpath1);
                if (System.IO.File.Exists(_Filefolderpath1))
                {
                    string[] fileName = Directory.GetFiles(filePathWithoutFileName).OrderByDescending(a => a).ToArray();
                    if (fileName != null && fileName.Any())
                    {
                        int renameextention = 1;
                        for (int i = 0; i < fileName.Count(); i++)
                        {
                            if (fileName.Any(a => a == _Filefolderpath1))
                            {
                                renameextention++;
                                string tempFileName1 = string.Format("{0}({1})", originalfilenameWithoutexten, renameextention);
                                string fileTemp1 = tempFileName1 + exten;
                                _Filefolderpath1 = Path.Combine(filePathWithoutFileName, fileTemp1);
                            }
                            if (!fileName.Any(a => a == _Filefolderpath1))
                            {
                                break;
                            }
                        }
                    }
                }

                hpf.SaveAs(_Filefolderpath1);

                campaignDetails.FilePath = _Filefolderpath1;

                string connString = Utility.GetImportConnectionString(_Filefolderpath1, exten);
                DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.OleDb");
                int totalRows = 0;

                using (DbConnection connection = factory.CreateConnection())
                {
                    if (connection != null)
                    {
                        connection.ConnectionString = connString;
                        try
                        {
                            connection.Open();
                        }
                        catch (OleDbException ex)
                        {
                            if (ex.ErrorCode == -2147467259)
                            {
                                connection.ConnectionString = connString = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + _Filefolderpath1 + ";" + "Extended Properties=\"Excel 12.0;HDR=Yes;\"";
                                connection.Open();
                            }
                        }

                        using (DbCommand command = connection.CreateCommand())
                        {
                            string fileX = string.Empty;
                            DataTable sheetTable = connection.GetSchema("Tables");

                            using (DbCommand command2 = connection.CreateCommand())
                            {
                                foreach (DataRow row in sheetTable.Rows)
                                {
                                    string sheetName = (string)row["TABLE_NAME"];

                                    if (sheetName.EndsWith("$'") && sheetName.StartsWith("'"))
                                    {
                                        sheetName = sheetName.Remove(0, 1);
                                        sheetName = sheetName.Remove(sheetName.Length - 1, 1);
                                    }
                                    command2.CommandText = "select count(*) FROM [" + sheetName + "]";
                                    if (sheetName == "Users$")
                                    {
                                        totalRows = Utility.GetInt(command2.ExecuteScalar());
                                    }
                                }

                                if (totalRows > 1)
                                {
                                    command.CommandText = "SELECT * FROM [Users$]";
                                    DbDataReader dr = command.ExecuteReader();
                                    dt = dr.GetSchemaTable();
                                    if (dt != null)
                                    {
                                        foreach (DataRow datarow in dt.Rows)
                                        {
                                            string strColumnName = string.Empty;
                                            strColumnName = Utility.RemoveSpaceandSpecialChars(datarow["ColumnName"].ToString());
                                            strColumnName = strColumnName.Trim();
                                            ColumnNameList.Add(strColumnName);
                                        }
                                    }

                                    if (ColumnNameList != null && ColumnNameList.Count > 0)
                                    {

                                        if (ColumnNameList.Contains(Constants.T_Name) &&
                                            ColumnNameList.Contains(Constants.T_Email_Address) &&
                                            ColumnNameList.Contains(Constants.T_EIN) &&
                                            ColumnNameList.Contains(Constants.T_Phone_Number) &&
                                            ColumnNameList.Contains(Constants.T_Address)
                                            )
                                        {
                                            campaignDetails.NoOfLeads = totalRows;

                                            campaignDetails.TechTeamStatus = TechTeamStatus.FILE_UPLOADED.ToString();

                                            // Save Upload File Tech Team

                                            this._adminService.SaveTechTeamFileUploaded(campaignDetails);

                                            return RedirectToAction("Index", new { id = Session[SessionItemKey.AdminCategoryId] });
                                        }
                                        else
                                        {
                                            return RedirectToAction("FileUploadTechTeam", new { id = campaignDetails.CampaignDetailsId, id2 = 1 });
                                        }

                                    }


                                }


                                connection.Close();
                            }
                        }


                        connection.Close();
                    }
                }

            }

            return RedirectToAction("FileUploadTechTeam");


        }
        #endregion

        #region Support Dashboard - Get
        [HttpGet]
        public ActionResult _SupportDashboard(long? id, string id2, int PageIndex = 0, int PageSize = 5)
        {
            long _adminUserId = Utility.GetAdminUserIdFromSession();
            long projectId = id ?? 0;
            string listStatus = id2;
            int skipRecords = PageIndex * PageSize;
            int takeRecords = PageSize;
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            param.AdminUserId = _adminUserId;
            param.ProjectId = projectId;
            param.SkipRecords = skipRecords;
            param.TakeRecords = takeRecords;
            param.CampaignStatus = listStatus;
            int count = this._adminService.GetCampaignSupportUserDetailsCount(param);
            int maxPageSizeCount = count / PageSize;
            if (count % PageSize > 0)
            {
                maxPageSizeCount++;
            }
            ViewData["MaxPageCount"] = maxPageSizeCount;
            List<CampaignDetails> campaognDetailsList = this._adminService.GetCampaignSupportUserDetails(param);
            var staticAdminProjects = this._adminService.GetStaticBizAdminProjects();
            staticAdminProjects.Insert(0, new BizAdminProjects() { ProjectId = 0, ProjectName = "All" });
            ViewBag.Product = staticAdminProjects.Select(x => new SelectListItem()
            {
                Text = x.ProjectName.ToString(),
                Value = x.ProjectId.ToString(),
                Selected = x.ProjectId == projectId
            });

            ViewBag.CampaignStatus = id2;

            var campaignStatus = Enum.GetValues(typeof(CampaignStatus))
                                .Cast<CampaignStatus>()
                                .Select(v => v.ToString())
                                .ToList();
            campaignStatus.Insert(0, "All");
            ViewBag.Status = campaignStatus.Select(x => new SelectListItem()
            {
                Text = x.ToString(),
                Value = x.ToString(),
                Selected = x.ToString() == listStatus
            });
            return PartialView(campaognDetailsList);
        }
        #endregion

        #region _ViewGoalsByCampaignDetailsId
        [HttpGet]
        public ActionResult _ViewGoalsByCampaignDetailsId(long id)
        {
            long campaignDetailId = id;
            CampaignDetails campaignDetails = new CampaignDetails();
            campaignDetails = this._adminService.GetGoalsByCampaignDetailsId(campaignDetailId);
            return PartialView(campaignDetails);
        }
        #endregion

        #region _ViewLeadReportForSupport - Get
        [HttpGet()]
        public ActionResult _ViewLeadReportForSupport(long? id, long? id2)
        {
            long campaignDetailId = id ?? 0;
            long supportUserId = id2 ?? 0;
            List<LeadDetails> leadDetails = new List<LeadDetails>();
            if (campaignDetailId > 0 && supportUserId > 0)
            {
                string campaignName = _adminService.GetGoalsByCampaignDetailsId(campaignDetailId).CampaignName;
                if (!string.IsNullOrWhiteSpace(campaignName))
                {
                    TempData["CampaignName"] = campaignName;
                }
                leadDetails = _adminService.GetCampaignLeadActivityListByCampaignDetailId(campaignDetailId, supportUserId);
            }
            return PartialView(leadDetails);
        }
        #endregion

        #region Start With Filter - Get
        [HttpGet]
        public ActionResult _StartWithFilter(long? id, long? id2)
        {
            long campaignDetailId = id ?? 0;
            long supportUserId = id2 ?? 0;

            CampaignDetails campaignDetails = new CampaignDetails();
            campaignDetails.CampaignSupportUserDetails = new CampaignSupportUserDetails();
            if (campaignDetailId > 0 && supportUserId > 0)
            {
                campaignDetails.CampaignDetailsId = campaignDetailId;
                campaignDetails.CampaignSupportUserDetails.SupportUserId = supportUserId;
            }

            campaignDetails.StateList = _adminService.GetAllStates();

            return PartialView(campaignDetails);
        }
        #endregion

        #region ViewCampaignLeads - Get
        [HttpGet()]
        public ActionResult ViewCampaignLeads(long? id, long? id2, string id3, string id4, bool? id5)
        {
            long campaignDetailId = id ?? 0;
            long supportUserId = id2 ?? 0;
            string stateCode = id3;
            string timeZone = id4;
            bool isSkipped = id5 ?? false;

            LeadDetailsSearchOption leadDetailsSearchOption = new LeadDetailsSearchOption();
            leadDetailsSearchOption.CampaignDetailId = campaignDetailId;
            leadDetailsSearchOption.SupportUserId = supportUserId;
            leadDetailsSearchOption.Skipped = id5 ?? false;
            leadDetailsSearchOption.StateCode = id3;
            ViewData["campaignDetailId"] = campaignDetailId;
            _adminService.UpdateReassignDetailsReset(leadDetailsSearchOption);

            CampaignDetails campaignDetails = new CampaignDetails();
            campaignDetails.CampaignSupportUserDetails = new CampaignSupportUserDetails();

            if (campaignDetailId > 0 && supportUserId > 0)
            {
                string campaignName = _adminService.GetGoalsByCampaignDetailsId(campaignDetailId).CampaignName;
                _adminService.ReAssignedLeads(campaignDetailId, supportUserId);
                if (!string.IsNullOrWhiteSpace(campaignName))
                {
                    TempData["CampaignName"] = campaignName;
                }
                campaignDetails.CampaignDetailsId = campaignDetailId;
                campaignDetails.CampaignSupportUserDetails.SupportUserId = supportUserId;
            }

            if (!string.IsNullOrWhiteSpace(stateCode) && stateCode.Length == 2)
            {
                campaignDetails.StateCode = stateCode;
            }

            if (!string.IsNullOrWhiteSpace(timeZone))
            {
                if (timeZone == LeadTimeZone.AST.ToString())
                {
                    campaignDetails.TimeZone = Constants.AlaskanStandardTime;
                }
                else if (timeZone == LeadTimeZone.CST.ToString())
                {
                    campaignDetails.TimeZone = Constants.CentralStandardTime;
                }
                else if (timeZone == LeadTimeZone.EST.ToString())
                {
                    campaignDetails.TimeZone = Constants.EasternStandardTime;
                }
                else if (timeZone == LeadTimeZone.HST.ToString())
                {
                    campaignDetails.TimeZone = Constants.HawaiianStandardTime;
                }
                else if (timeZone == LeadTimeZone.MST.ToString())
                {
                    campaignDetails.TimeZone = Constants.MountainStandardTime;
                }
                else if (timeZone == LeadTimeZone.PST.ToString())
                {
                    campaignDetails.TimeZone = Constants.PacificStandardTime;
                }
            }

            ViewBag.isSkipped = isSkipped;

            return View(campaignDetails);
        }
        #endregion

        #region _LeadsDetails - Get
        [HttpGet()]
        public ActionResult _LeadsDetails(long? id, long? id2, string id3, string timeZone, bool? id4)
        {
            long campaignDetailId = id ?? 0;
            long supportUserId = id2 ?? 0;
            bool isSkippedNext = id4 ?? false;

            LeadDetails leadDetails = new LeadDetails();

            if (campaignDetailId > 0 && supportUserId > 0)
            {
                if (!string.IsNullOrWhiteSpace(id3))//Start with state
                {
                    //leadDetails = this._adminService.GetLeadDetailsListByStateId(campaignDetailId, supportUserId, id3);
                    leadDetails = this._adminService.GetLeadDetailsListByStateId(campaignDetailId, supportUserId, id3);
                }
                else if (!string.IsNullOrWhiteSpace(timeZone))//Start with timwzone
                {
                    leadDetails = this._adminService.GetLeadDetailsListByTimezone(campaignDetailId, supportUserId, timeZone);
                }
                else if (isSkippedNext)
                {
                    leadDetails = this._adminService.GetLeadDetailsBySkippedRecord(campaignDetailId, supportUserId);
                }
                else
                {
                    leadDetails = this._adminService.GetLeadDetailsbyCampainDetailIdandSupportUserId(campaignDetailId, supportUserId);
                }
                Guid userId = _adminService.GetUserIdbyEmailAddress(leadDetails.Email);
                ViewData["userId"] = userId;
            }
            return PartialView(leadDetails);
        }
        #endregion

        #region _Communication - Get
        [HttpGet]
        public ActionResult _Communication(long? id, long? id2, string id3, string timeZone, long? id4, bool? id5, long? id6)
        {
            long campaignAssignedDetailsId = id4 ?? 0;
            long campaignDetailId = id ?? 0;
            long supportUserId = id2 ?? 0;
            string stateCode = id3;
            long campaignLeadActivityId = id6 ?? 0;

            LeadDetailsSearchOption leadDetailsSearchOption = new LeadDetailsSearchOption();
            leadDetailsSearchOption.CampaignDetailId = campaignDetailId;
            leadDetailsSearchOption.SupportUserId = supportUserId;
            leadDetailsSearchOption.Skipped = id5 ?? false;
            leadDetailsSearchOption.StateCode = id3;
            long leadFirstcount = _adminService.GetCampaignDetailsFirstCount(leadDetailsSearchOption);
            long leadLastCount = _adminService.GetCampaignDetailsLastCount(leadDetailsSearchOption);
            //long leadFirstcountByStatecode = _adminService.GetCampaignDetailsFirstCountByStateCode(campaignDetailId, supportUserId, stateCode);
            //long leadLastCountByStatecode = _adminService.GetCampaignDetailsLastCountByStateCode(campaignDetailId, supportUserId, stateCode);
            LeadCommunication leadCommunication = new LeadCommunication();
            if (campaignLeadActivityId > 0)
            {
                leadCommunication = _adminService.GetCampaignLeadActivityDetails(campaignLeadActivityId);
            }
            leadCommunication.LeadCommunicationLst = new List<LeadCommunication>();
            LeadDetails leadDetails = new LeadDetails();
            CampaignDetails CampaignType = new CampaignDetails();
            CampaignType = _adminService.GetGoalsByCampaignDetailsId(campaignDetailId);
            if (campaignDetailId > 0 && supportUserId > 0)
            {
                leadCommunication.CampaignDetailId = campaignDetailId;
                leadCommunication.SupportUserId = supportUserId;
                leadDetails = this._adminService.GetLeadDetailsForCommunicationByCampainDetailIdandSupportUserId(campaignDetailId, supportUserId);
                leadCommunication.CampaignAssignDetailId = leadDetails.CampaignAssignedDetailsId;
                leadCommunication.EmailAddress = leadDetails.Email;
                leadCommunication.SupportUserId = supportUserId;
                leadCommunication.CampaignAsignedFirstcount = leadFirstcount;
                leadCommunication.CampaignAsignedLastcount = leadLastCount;

                //checking whether the user is there in ETT by emailaddress
                Guid userId = _adminService.GetUserIdbyEmailAddress(leadCommunication.EmailAddress);
                ViewData["userId"] = userId;
                //    if(userId != null && userId!= Guid.Empty)
                //    {
                //    leadCommunication.LeadCommunicationLst = this._adminService.GetCommunicationDetails(userId);
                //    ViewBag.UserId = userId;
                //}
                //else {
                leadCommunication.LeadCommunicationLst = this._adminService.GetCampaignRecentActivityList(campaignAssignedDetailsId);
                //}
                leadCommunication.CampaignType = CampaignType.CampaignType;
                leadCommunication.StateCode = stateCode;
                leadCommunication.ProductId = CampaignType.ProductId;
                //leadCommunication.CampaignAsignedFirstcountByStatecode = leadFirstcountByStatecode;
                //leadCommunication.CampaignAsignedLastcountByStatecode = leadLastCountByStatecode;
            }

            return PartialView(leadCommunication);
        }
        #endregion

        #region _Communication - Post
        [HttpPost]
        public ActionResult _Communication(FormCollection form)
        {
            LeadCommunication leadCommunication = new LeadCommunication();
            GroupMembers groupMembers = new GroupMembers();
            leadCommunication.CampaignDetailId = Utility.GetLong(form["CampaignDetailId"]);
            leadCommunication.SupportUserId = Utility.GetLong(form["SupportUserId"]);
            leadCommunication.CampaignAssignDetailId = Utility.GetLong(form["CampaignAssignDetailId"]);
            leadCommunication.EmailAddress = form["EmailAddress"];
            leadCommunication.MethodeofContract = form["hdnMethodofContract"];
            leadCommunication.IsSkip = Utility.GetBool(form["IsSkip"]);
            //if (leadCommunication.MethodeofContract != MethodOfContact.Mail.ToString())
            //{
            //    leadCommunication.TypeOfCall = form["hdnTypeofCall"];
            //}
            leadCommunication.Spoketo = form["Spoketo"];
            leadCommunication.OtherReason = form["OtherReason"];
            leadCommunication.Comments = form["Comments"];
            leadCommunication.DonotContactagain = Utility.GetBool(form["hdnDonotContactagain"]);
            leadCommunication.Reason = form["Reason"];
            leadCommunication.AdminUserName = Utility.GetAdminUserNameFromSession();
            leadCommunication.IsSaveNext = Utility.GetBool(form["IsSaveNext"]);
            leadCommunication.IsFollowRequired = Utility.GetBool(form["hdnIsFollowRequired"]);
            leadCommunication.StateCode = form["StateCode"];
            leadCommunication.ProductId = Utility.GetInt(form["ProductId"]);
            if (Utility.GetInt(form["CampaignLeadActivityId"]) > 0)
            {
                leadCommunication.CampaignLeadActivityId = Utility.GetInt(form["CampaignLeadActivityId"]);
            }

            if (form["FollowupDate"] != null && Utility.GetDateTime(form["FollowupDate"]) > DateTime.MinValue)
            {
                leadCommunication.FollowupDate = Utility.GetDateTime(form["FollowupDate"]);
            }
            if (form["FollowupTime"] != null && Utility.GetDateTime(form["FollowupTime"]) > DateTime.MinValue)
            {
                TimeSpan time;
                if (!TimeSpan.TryParse(form["FollowupTime"], out time))
                {
                    DateTime dateTime = DateTime.ParseExact(form["FollowupTime"],
                                    "hh:mm tt", CultureInfo.InvariantCulture);
                    time = dateTime.TimeOfDay;
                    leadCommunication.FollowupTime = time;
                }
            }
            //Guid userId = _adminService.GetUserIdbyEmailAddress(leadCommunication.EmailAddress);
            //if (userId != null && userId != Guid.Empty)
            //{
            //    leadCommunication.ETTUserId = userId;
            //    leadCommunication = this._adminService.SaveCommunicationDetails(leadCommunication);
            //    if (leadCommunication != null) {
            //        leadCommunication.isCampaignCommunicationStatus = true;
            //    }  
            //    //ViewBag.UserId = userId;
            //}
            //else {
            leadCommunication = this._adminService.SaveCampaignCommunication(leadCommunication);
            //}
            if (leadCommunication.ProductId == (int)Project.ExpressTruckTax && leadCommunication.DonotContactagain && leadCommunication.CampaignLeadActivityId > 0)
            {
                groupMembers = this._adminService.SaveGroupMemberByEmailAddress(leadCommunication.EmailAddress);
            }
            return Json(new { Result = leadCommunication.isCampaignCommunicationStatus, CampaignLeadActivityId = leadCommunication.CampaignLeadActivityId }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region View Support user Report
        public ActionResult _ViewSupportUserReport(long? id, long? id2, int? id3)
        {
            long adminUserId = Utility.GetLong(Utility.GetUserIdFromSession());
            long compaignId = id ?? 0;
            long supportUserId = id2 ?? 0;
            ViewData["supportUserId"] = supportUserId;
            ViewData["compaignId"] = compaignId;
            CampaignDetails compaignDetail = new CampaignDetails();
            AdminUser _adminUser = new AdminUser();
            if (adminUserId > 0)
            {
                _adminUser = this._adminService.GetAdminUserById(adminUserId);
                if (_adminUser != null)
                {
                    ViewData["AdminRoleId"] = _adminUser.AdminRoleId;
                    ViewData["AdminUserName"] = _adminUser.AdminUserName;
                }
            }
            if (compaignId > 0 && supportUserId > 0)
            {
                compaignDetail = _adminService.GetCampaignAndSupportUserAndAssignedDetails(compaignId, supportUserId);

                if (compaignDetail != null)
                {
                    string currentdate = Utility.GetDateTime(DateTime.Now).ToString("MM/dd/yyyy");
                    if (compaignDetail.CampaignEndDate != null && compaignDetail.CampaignEndDate >= DateTime.MinValue && Utility.GetDateTime(Utility.GetDateTime(compaignDetail.CampaignEndDate).ToString("MM/dd/yyyy")) < Utility.GetDateTime(currentdate))
                    {
                        ViewData["IsCampaignExpired"] = true;
                    }
                }
            }
            int productId = id3 ?? 0;
            if (productId > 0)
            {
                ViewBag.SupportUsersList = this._adminService.GetAllAdminSupportUsers();
            }
            return PartialView(compaignDetail);
        }
        #endregion

        #region _LeadDetailsBackSkip - Post
        [HttpPost]
        public ActionResult _LeadDetailsBackSkip(long id, long id2, string Callfrom)
        {
            bool isCampaignCommunicationStatus;
            if (Callfrom == Constants.Back)
            {
                isCampaignCommunicationStatus = _adminService.UpdateLastLeadIdDuringBack(id, id2);
            }
            else
            {
                isCampaignCommunicationStatus = _adminService.UpdateLastLeadIdDuringSkip(id, id2);
            }
            return Json(isCampaignCommunicationStatus, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Download Campaing Uploaded File

        public ActionResult DownloadCampaingFile()
        {
            long _adminUserId = Utility.GetAdminUserIdFromSession();
            var campaignDetails = (CampaignDetails)Session["CampaignSummary" + _adminUserId];
            if (campaignDetails != null && !string.IsNullOrWhiteSpace(campaignDetails.FilePath))
            {
                var file = System.IO.File.ReadAllBytes(campaignDetails.FilePath);
                if (file != null)
                {
                    return File(file, MimeMapping.GetMimeMapping(campaignDetails.FilePath).ToString(), Path.GetFileName(campaignDetails.FilePath));
                }
            }
            return new EmptyResult();
        }

        #endregion

        #region _PreviousActivitydetails - Get
        [HttpGet]
        public ActionResult _PreviousActivitydetails(long id)
        {
            long campaignAssignedDetailId = id;
            LeadCommunication leadCommunication = new LeadCommunication();
            leadCommunication.LeadCommunicationLst = new List<LeadCommunication>();
            leadCommunication.LeadCommunicationLst = this._adminService.GetCampaignPreviousActivityList(campaignAssignedDetailId);
            return PartialView(leadCommunication);
        }
        #endregion

        #region Assign Campaign - POST

        [HttpPost]
        public ActionResult AssignCampaign(FormCollection form)
        {
            var formDetails = form;
            bool status = false;
            CampaignDetails campaignDetails = new CampaignDetails();
            long _adminUserId = Utility.GetAdminUserIdFromSession();

            campaignDetails.AdminUserId = _adminUserId;
            campaignDetails.CampaignDetailsId = Utility.GetLong(form["hdnCampaignDetailId"]);
            long baseSupportUserId = Utility.GetLong(form["hdnBaseSupportUserId"]);
            var supportUserIds = formDetails["hdnSupportUserIds"];

            var campaignLeadActivityDetailList = this._adminService.GetCampaignLeadActivityListByCampaignDetailId(campaignDetails.CampaignDetailsId, baseSupportUserId);
            var campaignAssignedDetailsList = this._adminService.GetCampaignAssignedDetailsbyCampainDetailIdandSupportUserId(campaignDetails.CampaignDetailsId, baseSupportUserId);

            var campaignLeadCampaignAssignedDetailIds = campaignLeadActivityDetailList.Select(m => m.CampaignAssignedDetailsId).ToList();

            campaignAssignedDetailsList = campaignAssignedDetailsList.Where(m => !campaignLeadCampaignAssignedDetailIds.Contains(m.CampaignAssignedDetailsId)).ToList();

            if (campaignAssignedDetailsList != null && campaignAssignedDetailsList.Count > 0)
            {
                int supportUserIdsCount = supportUserIds.Split(',').Length;
                string[] supportUserIdsArray = supportUserIds.Split(',').ToArray();
                int assignedSubLevelCount = Utility.GetInt(formDetails["hdnTotalPendingSupportCount"]) / supportUserIdsCount;
                int differenceCount = Utility.GetInt(formDetails["hdnTotalPendingSupportCount"]) % supportUserIds.Split(',').Length;

                campaignDetails.SupportAdminUserList = new List<AdminUser>();

                if (supportUserIdsArray != null && supportUserIdsArray.Length > 0)
                {
                    foreach (var assignedsupportUserId in supportUserIdsArray)
                    {
                        AdminUser adminUser = new AdminUser();
                        adminUser.AdminUserId = Utility.GetLong(assignedsupportUserId);
                        campaignDetails.SupportAdminUserList.Add(adminUser);
                    }
                }


                if (campaignDetails.SupportAdminUserList != null && campaignDetails.SupportAdminUserList.Count > 0)
                {
                    foreach (var assinedCountobj in campaignDetails.SupportAdminUserList)
                    {
                        assinedCountobj.TotalAssignedCount = assignedSubLevelCount;
                    }

                    if (differenceCount > 0)
                    {
                        for (var index = 0; index < differenceCount; index++)
                        {
                            var supportObj = campaignDetails.SupportAdminUserList[index];
                            if (supportObj != null)
                            {
                                supportObj.TotalAssignedCount += 1;
                            }
                        }
                    }

                    int rowNUmber = 1;
                    long supportUserId = 0;
                    int indexingSupportUserCount = 0;
                    int supportUserAssignedCount = 0;
                    supportUserAssignedCount = campaignDetails.SupportAdminUserList[indexingSupportUserCount].TotalAssignedCount;

                    foreach (var dbassignedCampaignDetails in campaignAssignedDetailsList)
                    {
                        CampaignAssignedDetails campaignAssignedDetails = new CampaignAssignedDetails();
                        supportUserId = campaignDetails.SupportAdminUserList[indexingSupportUserCount].AdminUserId;
                        if (supportUserAssignedCount >= rowNUmber)
                        {
                            dbassignedCampaignDetails.SupportUserId = supportUserId;
                        }
                        else
                        {
                            indexingSupportUserCount += 1;
                            supportUserAssignedCount += campaignDetails.SupportAdminUserList[indexingSupportUserCount].TotalAssignedCount;
                            if (supportUserAssignedCount >= rowNUmber)
                            {
                                dbassignedCampaignDetails.SupportUserId = campaignDetails.SupportAdminUserList[indexingSupportUserCount].AdminUserId;
                            }
                        }

                        rowNUmber++;
                    }
                    campaignAssignedDetailsList.ForEach(x => x.AdminUserId = _adminUserId);
                    this._adminService.UpdateCampaignDetails(campaignAssignedDetailsList);
                    status = true;
                    return Json(status);
                }
            }
            return Json(status);

        }
        #endregion

        #region _LeadStatus - Post
        [HttpPost]
        public ActionResult _LeadStatus(long id, string id2)
        {
            bool isLeadStatus = false;
            if (id > 0 && !string.IsNullOrWhiteSpace(id2))
            {
                isLeadStatus = _adminService.UpdateLeadStatus(id, id2);
            }
            return Json(isLeadStatus, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region _EditBusinessName
        public JsonResult _EditBusinessName(long? id, string id2)
        {
            bool isupdated = false;
            long campaignAssignedDetailsId = id ?? 0;
            string businessname = id2;
            if (campaignAssignedDetailsId > 0 && !string.IsNullOrWhiteSpace(businessname))
            {
                isupdated = _adminService.UpdateCampaignAssignedDetailsBusinessNameByCampaignAssignedDetailsId(campaignAssignedDetailsId, businessname);
            }

            return Json(isupdated, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region _EditLeadName
        public JsonResult _EditLeadName(long? id, string id2)
        {
            bool isupdated = false;
            long campaignAssignedDetailsId = id ?? 0;
            string name = id2;
            if (campaignAssignedDetailsId > 0 && !string.IsNullOrWhiteSpace(name))
            {
                isupdated = _adminService.UpdateCampaignAssignedDetailsNameByCampaignAssignedDetailsId(campaignAssignedDetailsId, name);
            }

            return Json(isupdated, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region _EditLeadPhone
        public JsonResult _EditLeadPhone(long? id, string id2)
        {
            bool isupdated = false;
            long campaignAssignedDetailsId = id ?? 0;
            string phone = id2;
            if (campaignAssignedDetailsId > 0 && !string.IsNullOrWhiteSpace(phone))
            {
                isupdated = _adminService.UpdateCampaignAssignedDetailsPhoneByCampaignAssignedDetailsId(campaignAssignedDetailsId, phone);
            }

            return Json(isupdated, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region _EditLeadEmail
        public JsonResult _EditLeadEmail(long? id, string id2)
        {
            bool isupdated = false;
            long campaignAssignedDetailsId = id ?? 0;
            string email = id2;
            if (campaignAssignedDetailsId > 0 && !string.IsNullOrWhiteSpace(email))
            {
                isupdated = _adminService.UpdateCampaignAssignedDetailsEmailByCampaignAssignedDetailsId(campaignAssignedDetailsId, email);
            }

            return Json(isupdated, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region _EditLeadAddress
        public JsonResult _EditLeadAddress(long? id, string id2)
        {
            bool isupdated = false;
            long campaignAssignedDetailsId = id ?? 0;
            string address = id2;
            if (campaignAssignedDetailsId > 0 && !string.IsNullOrWhiteSpace(address))
            {
                isupdated = _adminService.UpdateCampaignAssignedDetailsAddressByCampaignAssignedDetailsId(campaignAssignedDetailsId, address);
            }

            return Json(isupdated, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region _EditLeadNoofTrucks
        public JsonResult _EditLeadNoofTrucks(long? id, long id2)
        {
            bool isupdated = false;
            long campaignAssignedDetailsId = id ?? 0;
            long nofTrucks = id2;
            if (campaignAssignedDetailsId > 0 && nofTrucks > 0)
            {
                isupdated = _adminService.UpdateCampaignAssignedDetailsNofTrucksByCampaignAssignedDetailsId(campaignAssignedDetailsId, nofTrucks);
            }

            return Json(isupdated, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region _EditLeadEIN
        public JsonResult _EditLeadEIN(long? id, string id2)
        {
            bool isupdated = false;
            long campaignAssignedDetailsId = id ?? 0;
            string ein = id2;
            if (campaignAssignedDetailsId > 0 && ein.Length > 0)
            {
                isupdated = _adminService.UpdateCampaignAssignedDetailsEINByCampaignAssignedDetailsId(campaignAssignedDetailsId, ein);
            }

            return Json(isupdated, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region _AddAdditionalContacts -Get
        [HttpGet()]
        public ActionResult _AddAdditionalContacts(long? id, long? id2)
        {
            long campaignAssignedDetailsId = id ?? 0;
            long additionalContactsDetailsId = id2 ?? 0;
            AdditionalContacts ContactsDetails = new AdditionalContacts();
            ContactsDetails.CampaignAssignedDetailsId = campaignAssignedDetailsId;

            if (additionalContactsDetailsId > 0)
            {
                ContactsDetails.AdditionalContactsDetailsId = additionalContactsDetailsId;
                ContactsDetails = _adminService.GetAdditionalContactDetailsByAdditionalContactsDetailsId(additionalContactsDetailsId);
            }
            return PartialView(ContactsDetails);
        }
        #endregion

        #region _AddAdditionalContacts - Post
        [HttpPost()]
        public ActionResult _AddAdditionalContacts(AdditionalContacts ContDetails)
        {
            AdditionalContacts additionalContactsDetails = new AdditionalContacts();
            bool isContactSaved = false;
            additionalContactsDetails.StatusType = StatusType.Failure;
            if (ContDetails != null && ContDetails.CampaignAssignedDetailsId > 0)
            {
                additionalContactsDetails = _adminService.SaveAdditionalContacts(ContDetails);
                if (additionalContactsDetails != null && additionalContactsDetails.StatusType == StatusType.Success)
                {
                    isContactSaved = true;
                }
            }

            return Json(isContactSaved, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Delete Additional Contact Details
        public JsonResult DeleteAdditionalContact(long? id)
        {
            long additionalContactsDetailsId = id ?? 0;
            bool isDeleted = false;
            if (additionalContactsDetailsId > 0)
            {
                isDeleted = _adminService.DeleteAdditionalContactByAdditionalContactsDetailsId(additionalContactsDetailsId);
            }
            return Json(isDeleted, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region _AdditionalContactList - Get
        public ActionResult _AdditionalContactList(long? id)
        {
            long campaignAssignedDetailsId = id ?? 0;
            LeadDetails contdetails = new LeadDetails();
            if (campaignAssignedDetailsId > 0)
            {
                contdetails = _adminService.GetAdditionalContactListByCampaignAssignedDetailsId(campaignAssignedDetailsId);
                contdetails.CampaignAssignedDetailsId = campaignAssignedDetailsId;
            }
            return PartialView(contdetails);
        }

        #endregion

        #region _Search Lead - Get
        public ActionResult _SearchLead()
        {
            return PartialView();
        }
        #endregion

        #region _View Lead List - Get
        [HttpGet]
        public ActionResult _ViewLeadList(string id, string id2)
        {
            List<LeadDetails> leadDetailsList = new List<LeadDetails>();
            string searchBy = id;
            string value = id2;
            if (searchBy == "Phone")
            {
                value = value.Replace("-", "");
                value = value.Replace("(", "");
                value = value.Replace(")", "");
                value = value.Replace(" ", "");
            }
            long supportUserId = Utility.GetAdminUserIdFromSession();
            if (supportUserId > 0 && !string.IsNullOrWhiteSpace(searchBy) && !string.IsNullOrWhiteSpace(value))
            {
                leadDetailsList = _adminService.GetAllLeadListBySupportUserId(supportUserId, searchBy, value);
            }

            return PartialView(leadDetailsList);
        }
        #endregion

        #region _FollowupDetails -Get
        [HttpGet()]
        public ActionResult _FollowupDetails(string followupStatus)
        {
            long supportUserId = Utility.GetAdminUserIdFromSession();
            LeadCommunication leadCommunication = new LeadCommunication();
            leadCommunication.LeadCommunicationLst = new List<LeadCommunication>();
            leadCommunication.LeadCommunicationLst = this._adminService.GetFollowupBySupportUserId(supportUserId, followupStatus);
            return PartialView(leadCommunication);
        }
        #endregion

        #region _Gotolead - Get
        [HttpGet]
        public ActionResult _Gotolead(long id, long id2, long id3)
        {
            bool isLeadIdUpdated;
            LeadCommunication leadCommunication = new LeadCommunication();
            leadCommunication.CampaignDetailId = id;
            leadCommunication.SupportUserId = id2;
            leadCommunication.CampaignAssignDetailId = id3;
            isLeadIdUpdated = _adminService.UpdateLastLeadIdByCampaignAssignedDetailsId(leadCommunication);
            return RedirectToAction("ViewCampaignLeads", new { id = id, id2 = id2 });

        }
        #endregion

        #region _LeadDetailsPrevoiusAndNextbyStatecode - Post
        [HttpPost]
        public ActionResult _LeadDetailsPrevoiusAndNextbyStatecode(long id, long id2, string stateCode, string callfrom, long currentLeadId, bool? id5)
        {
            bool isCampaignCommunicationStatus;
            bool isSkipped = id5 ?? false;
            LeadDetailsSearchOption leadDetailsSearchOption = new LeadDetailsSearchOption();
            leadDetailsSearchOption.CampaignDetailId = id;
            leadDetailsSearchOption.SupportUserId = id2;
            leadDetailsSearchOption.StateCode = stateCode;
            leadDetailsSearchOption.LeadId = currentLeadId;
            leadDetailsSearchOption.Skipped = id5 ?? false;

            if (callfrom == Constants.Back)
            {
                isCampaignCommunicationStatus = _adminService.UpdatePreviousLeadIdAsLastLeadIdByStatecode(leadDetailsSearchOption);
            }
            else
            {
                isCampaignCommunicationStatus = _adminService.UpdateNextLeadIdAsLastLeadIdByStatecode(leadDetailsSearchOption);
            }
            return Json(isCampaignCommunicationStatus, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region _IsStateExsits - Post
        [HttpPost]
        public ActionResult _IsStateExsits(long id, long id2, string stateCode)
        {
            bool isLeadStatus = false;
            if (id > 0 && id2 > 0 && !string.IsNullOrWhiteSpace(stateCode))
            {
                isLeadStatus = _adminService.GetCampaignDetailsCountByStateCode(id, id2, stateCode);
            }
            return Json(isLeadStatus, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Upload Status
        public ActionResult UploadStatus(long? id)
        {
            CampaignDetails campaignDetails = new CampaignDetails();
            List<LeadDetails> _leadDetailsList = new List<LeadDetails>();
            long _adminUserId = Utility.GetAdminUserIdFromSession();
            long supportUserId = 0;
            int indexingSupportUserCount = 0;
            if (Session["CampaignSummary" + _adminUserId] != null)
            {
                campaignDetails = (CampaignDetails)Session["CampaignSummary" + _adminUserId];
                supportUserId = campaignDetails.SupportUserList[indexingSupportUserCount];
                _leadDetailsList = _adminService.GetCampaignDetailsbyCampainDetailIdandSupportUserId(campaignDetails.CampaignDetailsId, supportUserId);

                campaignDetails = _adminService.GetCampaignUploadedTimeDetails(campaignDetails);

                if (_leadDetailsList != null && _leadDetailsList.Count > 0)
                {
                    ViewData["_leadDetailsListCount"] = _leadDetailsList.Count;
                }
            }

            return View(campaignDetails);
        }
        #endregion

        #region _SpancontrolActivitydetails - Get
        [HttpGet]
        public ActionResult _SpancontrolActivitydetails(string emailAddress)
        {
            Notes notes = new Notes();
            notes.commentsList = new List<Notes>();
            if (!string.IsNullOrWhiteSpace(emailAddress))
            {
                notes = this._adminService.GetUserCommentsbyEmailAddress(emailAddress);
            }
            return PartialView(notes);
        }
        #endregion

        #region Get Add Comments
        [HttpGet]
        public ActionResult _AddComments(long? id, long? id2, long? id3)
        {
            long campaignDetailId = id ?? 0;
            long supportUserId = id2 ?? 0;
            long campaignAssignedDetailId = id3 ?? 0;
            LeadCommunication leadCommunication = new LeadCommunication();
            LeadDetails leadDetails = new LeadDetails();

           // leadDetails = this._adminService.GetLeadDetailsForCommunicationByCampainDetailIdandSupportUserId(campaignDetailId, supportUserId);
            CampaignDetails CampaignType = new CampaignDetails();
            CampaignType = _adminService.GetGoalsByCampaignDetailsId(campaignDetailId);

            if (campaignDetailId > 0 && supportUserId > 0)
            {
                leadCommunication.CampaignDetailId = campaignDetailId;
                leadCommunication.SupportUserId = supportUserId;
                leadCommunication.CampaignAssignDetailId = campaignAssignedDetailId;
                leadDetails = this._adminService.GetCampaignAssignedDetails(campaignDetailId, supportUserId,campaignAssignedDetailId);
                if(leadDetails != null) { 
                leadCommunication.EmailAddress = leadDetails.Email;
                }
                leadCommunication.SupportUserId = supportUserId;
                leadCommunication.CampaignType = CampaignType.CampaignType;
                leadCommunication.ProductId = CampaignType.ProductId;
            }
            return PartialView(leadCommunication);
        }
        #endregion

        #region _Communication - Post
        [HttpPost]
        public ActionResult _AddComments(FormCollection form)
        {
            string _adminUserName = string.Empty;
            if (Session != null && Session[Main.Control.Utilities.Infrastructure.SessionItemKey.AdminDisplayName] != null)
            {
                _adminUserName = Session[Main.Control.Utilities.Infrastructure.SessionItemKey.AdminDisplayName].ToString();
            }

            LeadCommunication leadCommunication = new LeadCommunication();
            GroupMembers groupMembers = new GroupMembers();
            leadCommunication.CampaignDetailId = Utility.GetLong(form["CampaignDetailId"]);
            leadCommunication.SupportUserId = Utility.GetLong(form["SupportUserId"]);
            leadCommunication.CampaignAssignDetailId = Utility.GetLong(form["CampaignAssignDetailId"]);
            leadCommunication.EmailAddress = form["EmailAddress"];
            leadCommunication.MethodeofContract = form["hdnMethodofContract"];
            leadCommunication.IsSkip = Utility.GetBool(form["IsSkip"]);
            //if (leadCommunication.MethodeofContract != MethodOfContact.Mail.ToString())
            //{
            //    leadCommunication.TypeOfCall = form["hdnTypeofCall"];
            //}
            leadCommunication.Spoketo = form["Spoketo"];
            leadCommunication.OtherReason = form["OtherReason"];
            leadCommunication.Comments = form["Comments"];
            leadCommunication.DonotContactagain = Utility.GetBool(form["hdnDonotContactagain"]);
            leadCommunication.Reason = form["Reason"];
            leadCommunication.IsSaveNext = Utility.GetBool(form["IsSaveNext"]);
            leadCommunication.IsFollowRequired = Utility.GetBool(form["hdnIsFollowRequired"]);
            leadCommunication.StateCode = form["StateCode"];
            leadCommunication.ProductId = Utility.GetInt(form["ProductId"]);
            if (!string.IsNullOrEmpty(_adminUserName) && !string.IsNullOrEmpty(leadCommunication.Comments))
            {
                leadCommunication.Comments = leadCommunication.Comments + " commented by " + _adminUserName;
            }
            if (Utility.GetInt(form["CampaignLeadActivityId"]) > 0)
            {
                leadCommunication.CampaignLeadActivityId = Utility.GetInt(form["CampaignLeadActivityId"]);
            }

            if (form["FollowupDate"] != null && Utility.GetDateTime(form["FollowupDate"]) > DateTime.MinValue)
            {
                leadCommunication.FollowupDate = Utility.GetDateTime(form["FollowupDate"]);
            }
            if (form["FollowupTime"] != null && Utility.GetDateTime(form["FollowupTime"]) > DateTime.MinValue)
            {
                TimeSpan time;
                if (!TimeSpan.TryParse(form["FollowupTime"], out time))
                {
                    DateTime dateTime = DateTime.ParseExact(form["FollowupTime"],
                                    "hh:mm tt", CultureInfo.InvariantCulture);
                    time = dateTime.TimeOfDay;
                    leadCommunication.FollowupTime = time;
                }
            }
            leadCommunication = this._adminService.SaveCampaignCommunication(leadCommunication);
            if (leadCommunication.ProductId == (int)Project.ExpressTruckTax && leadCommunication.DonotContactagain && leadCommunication.CampaignLeadActivityId > 0)
            {
                groupMembers = this._adminService.SaveGroupMemberByEmailAddress(leadCommunication.EmailAddress);
            }
            return Json(new { Result = leadCommunication.isCampaignCommunicationStatus, CampaignLeadActivityId = leadCommunication.CampaignLeadActivityId }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region _Recent Returns
        public ActionResult _RecentReturns(Guid userId)
        {
            List<RecentReturns> recentReturns = new List<RecentReturns>();
            if (userId != null && userId!= Guid.Empty)
            {
                recentReturns = _adminService.GetRecentReturns(userId);
            }
            return PartialView(recentReturns);
        }
        #endregion

        #region
        public ActionResult GetFollowUpDetails(long id)
        {
            RecentReturns followUpDetails = new RecentReturns();
            if (id > 0)
            {
                followUpDetails = _adminService.GetFollowUpDetails(id);
            }
            return Json(followUpDetails, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region   SC Contact Group
        /// <summary>
        /// SC Contact Group
        /// </summary>
        /// <param name="emailaddress"></param>
        /// <returns></returns>
        public ActionResult _SpancontrolContactGroup(string emailaddress)
        {
            List<SpancontrolContactGroup> lstGroups = _adminService.GetContactgroupList();
            List<long> groupMembers = new List<long>();
            Guid userId = Guid.Empty;
            userId = _adminService.GetUserIdbyEmailAddress(emailaddress);
            if (userId != Guid.Empty)
            {
                groupMembers = _adminService.GetGroupMembersbyUserId(userId);
            }
            if (groupMembers!=null &&groupMembers.Count > 0)
            {
                ViewBag.IsActive = groupMembers;
            }
            ViewBag.UserId = userId;
            return PartialView(lstGroups);
        }
        #endregion

        #region Save Group member details
        /// <summary>
        /// Save group member 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="checkorunchecked"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveGroupMemberDetailsJson(long id, int checkorunchecked, Guid userid)
        {
            GroupMembers members = new GroupMembers();
            members.GroupNameId = id;
            members.CheckedOrUnchecked = checkorunchecked;
            members.UserId = userid;
            if (members.UserId != Guid.Empty)
            {
                _adminService.SaveGroupMember(members);
                if (id > 0)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                TempData["GroupMemberSuccess"] = "User not exist in ETT";
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Download report based on the option selected in the dropdown
        /// <summary>
        /// Download Report 
        /// </summary>
        /// <param name="selectedOption"></param>
        /// <returns></returns>
        public ActionResult DownloadCampaignReport(string selectedOption,long campaignDetailId)
        {
            List<GroupMembers> reports = new List<GroupMembers>();
            long adminUserId = Utility.GetAdminUserIdFromSession();
            reports = _adminService.GetReportbySelectedOption(selectedOption, adminUserId, campaignDetailId);
            string sheetName = selectedOption + " Report";
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(sheetName);
            ws.Row(1).Style.Font.Bold = true;
            ws.Cell(1, 1).Value = "Name";
            ws.Cell(1, 2).Value = "Phone Number";
            ws.Cell(1, 3).Value = "Email Address";
            if (reports != null && reports.Count > 0)
            {
                int row = 2;
                foreach (var item in reports)
                {
                    ws.Cell(row, 1).Value = item.Contact_Name;
                    ws.Cell(row, 2).Value = item.PhoneNumber;
                    ws.Cell(row, 3).Value = item.EmailAddress;
                    row++;
                }
            }
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + sheetName + ".xlsx");
            using (MemoryStream memoryStream = new MemoryStream())
            {
                wb.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                memoryStream.Close();
            }
            Response.End();

            return new EmptyResult();
        }
        #endregion

        #region _Edit Lead Return Number
        public JsonResult _EditLeadReturnNumber(long? id, long id2)
        {
            bool isupdated = false;
            long campaignAssignedDetailsId = id ?? 0;
            long returnNumber = id2;
            if (campaignAssignedDetailsId > 0 && returnNumber > 0)
            {
                isupdated = _adminService.UpdateCampaignAssignedDetailsReturnNumberByCampaignAssignedDetailsId(campaignAssignedDetailsId, returnNumber);
            }

            return Json(isupdated, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}