using System;
using System.Configuration;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Main.Control.Core.Infrastructure;
using System.IO;
using System.Data.Common;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Main.Control.Core.Models;
using Main.Control.Web.Utilities;
using Main.Control.Web.ViewModels;
using Main.Control.Core.Services;
using Main.Control.Utilities.Infrastructure;
using System.Collections.Generic;
using Main.Control.Web.Models;
using System.Web.Security;
using System.Web;
using Main.Control.Service.Utilities;
using System.Net;
using Newtonsoft.Json;
using Google.Authenticator;

namespace Main.Control.Web.Controllers
{
    [TraceFilter()]
    [RemoteRequireHttps()]
    [SessionAlive()]
    public class AdminController : BaseController
    {
        #region Declaration
        private readonly IAdminService _adminService;
        private readonly ISpanControlService _mainControlService;
        protected struct LocalConstants
        {
            public const string RememberMe = "RememberMe";
        }
        #endregion

        #region Constructor
        public AdminController()
        {

        }
        public AdminController(ISpanControlService mainControlService, IAdminService adminservice) // : base(mainControlService, adminservice)
        {

            this._mainControlService = mainControlService;
            this._adminService = adminservice;

        }
        #endregion

        #region Sign In
        /// <summary>
        /// SignIn
        /// </summary>
        /// <returns></returns>
        public ActionResult SignIn()
        {
            //logon
            if (Request.QueryString["product"] != null)
            {
                ViewData["ProductName"] = Request.QueryString["product"];
                Session["ReferenceApplication"] = Request.QueryString["product"];
            }
            //accountlogin
            if (FormsAuthentication.EnableCrossAppRedirects)
            {
                string text = Request.QueryString[FormsAuthentication.FormsCookieName];
                long _userId = default(long);
                FormsAuthenticationTicket formsAuthenticationTicket;
                if (Request.QueryString["product"] != null)
                {
                    ViewData["ProductName"] = Request.QueryString["product"];
                    Session["ReferenceApplication"] = Request.QueryString["product"];
                }
                string _lang = null;
                if (Request.QueryString["lang"] != null)
                {
                    _lang = Request.QueryString["lang"];
                }
                if (!string.IsNullOrWhiteSpace(text))
                {
                    formsAuthenticationTicket = FormsAuthentication.Decrypt(text);
                    string _userData = formsAuthenticationTicket.UserData;

                    if (!string.IsNullOrWhiteSpace(_userData))
                    {
                        _userId = Utility.GetLong(_userData);
                        AdminUser _adminUser = _adminService.GetAdminUserById(_userId);
                        //If sign in is success, then add session variables
                        if (_adminUser != null && _adminUser.AdminUserId > 0)
                        {
                            //Assign the user details to session
                            Utility.AddToSession(_adminUser, false);
                        }
                        ////set forms auth cookie
                    }
                }
                else if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                    formsAuthenticationTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    string _userData = formsAuthenticationTicket.UserData;

                    if (!string.IsNullOrWhiteSpace(_userData))
                    {
                        _userId = Utility.GetLong(_userData);
                        AdminUser _adminUser = _adminService.GetAdminUserById(_userId);
                        if (_adminUser != null && _adminUser.AdminUserId > 0)
                        {
                            //Assign the user details to session
                            Utility.AddToSession(_adminUser, false);
                        }
                    }
                }
                long.TryParse(Session[SessionItemKey.AdminUserId] != null ? Session[SessionItemKey.AdminUserId].ToString() : string.Empty, out _userId);

                if (_userId > 0)
                {
                    string _productName = Request.QueryString["product"];
                    if (string.IsNullOrWhiteSpace(_productName))
                    {
                        return RedirectToAction(ActionNames.MainMenu, ControllerNames.Admin);
                    }
                    else
                    {
                        if (_productName.ToLower() == Products.Sales.ToString().ToLower())
                        {
                            return RedirectToAction(ActionNames.Sales, ControllerNames.Admin);
                        }
                        else
                        {
                            return RedirectToAction(ActionNames.Tax, ControllerNames.Admin, new { appName = _productName });
                        }
                    }
                }
                else
                {
                    return View();
                }
            }
            else if (Session != null && Session["OAuthEmail"] == null)
            {
                Session.Abandon();
            }
            return View();
        }
        #endregion

        #region Sign In - Post
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SignIn(SignInUI signInUI, FormCollection form)
        {
            //logon usercontrol
            string _username = signInUI.UserName;
            if (ModelState.IsValid)
            {
                AdminUser _admin = new AdminUser();
                string _productName = Request.QueryString["product"];
                if (string.IsNullOrWhiteSpace(_productName) && !string.IsNullOrWhiteSpace(form["ProductName"].ToString()))
                {
                    _productName = form["ProductName"].ToString();
                }
                try
                {
                    //fill Admin object and get the details

                    //assigning admin details 
                    if (!string.IsNullOrWhiteSpace(signInUI.UserName))
                    {
                        _admin.AdminUserName = signInUI.UserName;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid Data");
                        //  return View(model);
                        return RedirectToAction(ActionNames.SignIn, ControllerNames.Admin, new { product = _productName });
                    }
                    _admin.AdminPassword = signInUI.Password;

                    //Get The Admin Details based on the User Id
                    AdminUser _adminDetails = _adminService.AdminSignIn(_admin);

                    //If sign in is success, then add session variables
                    if (_adminDetails != null && _adminDetails.OperationStatus == StatusType.Success && _adminDetails.AdminUserId > 0)
                    {
                        //add to cookie, If Remember me check box is checked, else clear the cookie
                        bool rememberPwd = form[LocalConstants.RememberMe].Contains(Constants.True.ToLower());
                        //Assign the user details to session
                        Utility.AddToSession(_adminDetails, rememberPwd);
                        //set forms auth cookie
                        FormsAuthentication.SetAuthCookie(_admin.AdminUserName, false);
                        return RedirectToApplication(_productName);
                    }
                }
                catch
                {
                    if (_username != null && !string.IsNullOrWhiteSpace(_username))
                    {
                        AdminUser adminUser = _adminService.GetAdminDetailsByUserNameApproved(_username);
                        if (!string.IsNullOrWhiteSpace(adminUser.AdminUserName) && adminUser.IsApproved == false)
                        {
                            TempData["Invalid"] = "Your account has been blocked";
                        }
                        else if (!string.IsNullOrWhiteSpace(adminUser.AdminUserName))
                        {
                            TempData["Invalid"] = "Invalid Username or Password";
                        }
                        else if (string.IsNullOrWhiteSpace(adminUser.AdminUserName))
                        {
                            TempData["Invalid"] = "Invalid Username or Password";
                        }
                    }
                }
            }
            //redirect to SignIn.

            return (ActionResult)View(ActionNames.SignIn, signInUI);
        }
        #endregion

        #region Google Callback
        public ActionResult GoogleCallback()
        {
            string _productName = Request.QueryString["product"];

            Session["OAuthType"] = null;
            Session["OAuthEmail"] = null;
            Session["OAuthId"] = null;
            Session["OAuthName"] = null;

            string url = "";
            oAuthGoogle oAuth = new oAuthGoogle();

            if (Request["code"] == null)
            {

                TempData["OAuthError"] = true;
                return RedirectToAction(ActionNames.SignIn, ControllerNames.Admin);
            }
            else
            {
                oAuth.AccessTokenGet(Request["code"]);

                if (oAuth.Token.Length > 0)
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    url = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + oAuth.Token;
                    string json = oAuth.WebRequest(oAuthGoogle.Method.GET, url, String.Empty);

                    if (!string.IsNullOrEmpty(json))
                    {
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        GoogleResponseJson _googleResponse = (GoogleResponseJson)serializer.Deserialize(json, typeof(GoogleResponseJson));

                        //Get The Admin Details based on the User Id
                        AdminUser _adminDetails = _adminService.GetAdminUserByEmailId(_googleResponse.email);

                        if (_adminDetails != null && _adminDetails.AdminUserId > 0 && Utility.GetBool(Utility.GetAppSettings(Constants.IsAuthenticatorEnabled)) == true && _adminDetails.VerificationCodType == VerificationCodeType.AUTHENTICATOR.ToString())
                        {
                            Utility.AddMobileVerifyUserId(_adminDetails.AdminUserId, _adminDetails.PhoneNumber, _adminDetails.AdminEmailAddress);
                            return RedirectToAction("TwoFactorAuthentication", ControllerNames.Admin);
                        }
                        else if (_adminDetails != null && _adminDetails.AdminUserId > 0 && !string.IsNullOrEmpty(_adminDetails.PhoneNumber) && _adminDetails.IsApproved == true)
                        {
                            //send verification code to user's mobile number
                            MobileVerification mobileVerification = new MobileVerification();
                            mobileVerification.MobileNumber = _adminDetails.PhoneNumber;
                            mobileVerification.UserEmail = _adminDetails.AdminEmailAddress;
                            mobileVerification.UserId = _adminDetails.AdminUserId;
                            mobileVerification = _adminService.SendVerificationCode(mobileVerification);

                            if (mobileVerification != null && mobileVerification.OperationStatus == "Success")
                            {
                                //Add user id to session
                                Utility.AddMobileVerifyUserId(_adminDetails.AdminUserId, _adminDetails.PhoneNumber, _adminDetails.AdminEmailAddress);
                                return RedirectToAction("MobileVerification", "Admin", new { productName = _productName });
                            }
                            else
                            {
                                TempData["MobileVerificationMessage"] = "Mobile Number is invalid";
                                return RedirectToAction(ActionNames.SignIn, ControllerNames.Admin);
                            }
                        }
                        else if (string.IsNullOrEmpty(_adminDetails.PhoneNumber) && _adminDetails.IsApproved == true)
                        {
                            TempData["MobileVerificationMessage"] = "Mobile Number is invalid";
                            return RedirectToAction(ActionNames.SignIn, ControllerNames.Admin);
                        }
                        else
                        {
                            Session["OAuthType"] = "Google";
                            Session["OAuthEmail"] = _googleResponse.email;
                            Session["OAuthId"] = _googleResponse.id;
                            Session["OAuthName"] = _googleResponse.name;

                            TempData["MobileVerificationMessage"] = "You are not authorized to access this site. Please contact Admin";

                            return RedirectToAction(ActionNames.SignIn, ControllerNames.Admin);
                        }
                    }
                }
            }
            return new EmptyResult();
        }
        #endregion

        #region Google Callback
        public ActionResult MicrosoftCallBack()
        {
            string _productName = Request.QueryString["product"];

            Session["OAuthType"] = null;
            Session["OAuthEmail"] = null;
            Session["OAuthId"] = null;
            Session["OAuthName"] = null;

            oAuthMicrosoft oAuth = new oAuthMicrosoft();

            if (Request["code"] == null)
            {

                TempData["OAuthError"] = true;
                return RedirectToAction(ActionNames.SignIn, ControllerNames.Admin);
            }
            else
            {
                oAuth.AccessTokenGet(Request["code"]);

                if (oAuth.Token.Length > 0)
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    var _microsoftResponse = oAuth.GraphApiCall(oAuth.Token);

                    if (_microsoftResponse !=null)
                    {
                        //Get The Admin Details based on the User Id
                        AdminUser _adminDetails = _adminService.GetAdminUserByEmailId(_microsoftResponse.userPrincipalName);

                        if (_adminDetails != null && _adminDetails.AdminUserId > 0 && Utility.GetBool(Utility.GetAppSettings(Constants.IsAuthenticatorEnabled)) == true && _adminDetails.VerificationCodType == VerificationCodeType.AUTHENTICATOR.ToString())
                        {
                            Utility.AddMobileVerifyUserId(_adminDetails.AdminUserId, _adminDetails.PhoneNumber, _adminDetails.AdminEmailAddress);
                            return RedirectToAction("TwoFactorAuthentication", ControllerNames.Admin);
                        }
                        else if (_adminDetails != null && _adminDetails.AdminUserId > 0 && !string.IsNullOrEmpty(_adminDetails.PhoneNumber) && _adminDetails.IsApproved == true)
                        {
                            //send verification code to user's mobile number
                            MobileVerification mobileVerification = new MobileVerification();
                            mobileVerification.MobileNumber = _adminDetails.PhoneNumber;
                            mobileVerification.UserEmail = _adminDetails.AdminEmailAddress;
                            mobileVerification.UserId = _adminDetails.AdminUserId;
                            mobileVerification = _adminService.SendVerificationCode(mobileVerification);

                            if (mobileVerification != null && mobileVerification.OperationStatus == "Success")
                            {
                                //Add user id to session
                                Utility.AddMobileVerifyUserId(_adminDetails.AdminUserId, _adminDetails.PhoneNumber, _adminDetails.AdminEmailAddress);
                                return RedirectToAction("MobileVerification", "Admin", new { productName = _productName });
                            }
                            else
                            {
                                TempData["MobileVerificationMessage"] = "Mobile Number is invalid";
                                return RedirectToAction(ActionNames.SignIn, ControllerNames.Admin);
                            }
                        }
                        else if (string.IsNullOrEmpty(_adminDetails.PhoneNumber) && _adminDetails.IsApproved == true)
                        {
                            TempData["MobileVerificationMessage"] = "Mobile Number is invalid";
                            return RedirectToAction(ActionNames.SignIn, ControllerNames.Admin);
                        }
                        else
                        {
                            Session["OAuthType"] = "Microsoft";
                            Session["OAuthEmail"] = _microsoftResponse.userPrincipalName;
                            Session["OAuthId"] = _microsoftResponse.id;
                            Session["OAuthName"] = _microsoftResponse.userPrincipalName;

                            TempData["MobileVerificationMessage"] = "You are not authorized to access this site. Please contact Admin";

                            return RedirectToAction(ActionNames.SignIn, ControllerNames.Admin);
                        }
                    }
                }
            }
            return new EmptyResult();
        }
        #endregion

        public ActionResult SignOut()
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["product"]))
            {
                TempData["appName"] = Request.QueryString["product"];
            }

            if (Session != null)
            {
                if (Session["LogOffApp"] != null)
                {
                    TempData["appName"] = Session["LogOffApp"];
                }
                if (Session["LoggedInApps"] == null)
                {
                    FormsAuthentication.SignOut();
                    Session.Abandon();
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(Request.QueryString["product"]))
                    {
                        Session["LogOffApp"] = Request.QueryString["product"];
                    }
                    //log off sales
                    if (Session["LoggedInApps"] != null && Session["LoggedInApps"].ToString().Contains(Products.Sales.ToString()))
                    {
                        Session["LoggedInApps"] = Session["LoggedInApps"].ToString().Replace(Products.Sales.ToString(), "");
                        Response.Redirect(ConfigurationManager.AppSettings["SalesLogOffUrl"]);
                        return new EmptyResult();
                    }
                    else if (Session["LoggedInApps"] != null && Session["LoggedInApps"].ToString().Contains(Products.Tax.ToString()))
                    {
                        Session["LoggedInApps"] = Session["LoggedInApps"].ToString().Replace(Products.Tax.ToString(), "");
                        Response.Redirect(ConfigurationManager.AppSettings["TaxLogOffUrl"]);
                        return new EmptyResult();
                    }
                    else
                    {
                        FormsAuthentication.SignOut();
                        Session.Abandon();
                    }
                }
            }
            FormsAuthentication.SignOut();
            Session.Abandon();
            AppCookies.Remove(SessionItemKey.AdminUserName);
            AppCookies.Remove(SessionItemKey.CookieSession);
            Session.Contents.Abandon();
            Session.Contents.RemoveAll();
            return View();

        }

        [TaxAuthorize()]
        [SessionAlive(Order = 1)]
        public ActionResult CreateAdminUser()
        {
            return View();
        }
        [TaxAuthorize()]
        public ActionResult AddLead()
        {
            return View();
        }
        [TaxAuthorize()]
        [SessionAlive(Order = 1)]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CreateProjectRole(long? id, long? id2, int? id3)
        {
            AdminUserRoleUI _adminUserRoleUI = new AdminUserRoleUI();
            ViewData["AdminUserName"] = string.Empty;
            long adminUserId = id ?? 0;
            long adminUserRoleId = id2 ?? 0;
            _adminUserRoleUI.AdminUserId = adminUserId;
            AdminUser _adminUser = new AdminUser();
            if (id > 0)
            {
                _adminUser = this._adminService.GetAdminUserById(id ?? 0);
                ViewData["AdminUserName"] = _adminUser.AdminFirstName + " " + _adminUser.AdminLastName;
            }
            _adminUserRoleUI.AdminUserRoleList = this._adminService.GetAllAdminProjectRole(adminUserId).ToList();
            _adminUserRoleUI.AdminCategoryList = this._adminService.GetAllAdminCategories(adminUserId).ToList();

            if (id2 > 0)
            {
                foreach (var item in _adminUserRoleUI.AdminUserRoleList)
                {
                    if (id2 == item.AdminUserRoleId)
                    {
                        _adminUserRoleUI.ProjectId = item.AdminProjectId;
                        _adminUserRoleUI.Roles = item.AdminRoleId;
                        _adminUserRoleUI.AdminCategoryId = item.AdminCategoryId;
                        _adminUserRoleUI.AdminUserRoleId = item.AdminUserRoleId;
                        _adminUserRoleUI.AdminProjectList = _adminService.GetAllAdminProjects(item.AdminCategoryId).ToList();
                    }
                }
            }

            int msgCode = id3 ?? 0;
            if (msgCode == 1)
            {
                TempData["SuccessMsg"] = "Role Added Successfully.";
            }
            else if (msgCode == 2)
            {
                TempData["SuccessMsg"] = "Role Updated Successfully.";
            }
            else if (msgCode == 3)
            {
                TempData["SuccessMsg"] = "Role deleted successfully.";
            }
            else
            {
                TempData["SuccessMsg"] = null;
            }

            return View(_adminUserRoleUI);
        }


        [TaxAuthorize()]
        public JsonResult ProjectList(long Id, long? Id2, bool? Id3)
        {
            long _id2 = Id2 ?? 0;
            bool isUpdate = Id3 ?? false;
            var assignlist = this._adminService.GetAllAdminProjectRole(_id2).Where(o => o.AdminUserId == _id2).ToList();
            var unassignlist = _adminService.GetAllAdminProjects(Id);
            if (!isUpdate)
            {
                unassignlist = _adminService.GetAllAdminProjects(Id).ToList().Where(p => !assignlist.Any(pm => p.AdminProjectId == pm.AdminProjectId)).AsQueryable();
            }

            return Json(new SelectList(unassignlist.ToArray(), "AdminProjectId", "ProjectName"), JsonRequestBehavior.AllowGet);
        }
        [TaxAuthorize()]
        public JsonResult RoleList(long Id)
        {
            var list = _adminService.GetAllAdminRoles(Id.ToString());


            return Json(new SelectList(list.ToArray(), "AdminRoleId", "Role"), JsonRequestBehavior.AllowGet);
        }

        [TaxAuthorize()]
        public ActionResult AddLeadLog()
        {
            return PartialView();
        }

        //public ActionResult CreateUser()
        //{
        //    return View();
        //}

        #region Create User
        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [SessionAlive(Order = 1)]
        [TaxAuthorize()]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CreateUser(long? Id)
        {
            AdminUser _adminUser = new AdminUser();
            _adminUser.IsNewUser = true;
            if (Id > 0)
            {
                _adminUser = this._adminService.GetAdminUserById(Id ?? 0);
                _adminUser.IsNewUser = false;
            }
            return View(_adminUser);
        }
        #endregion

        #region Create User
        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="adminUser"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [TaxAuthorize()]
        [SessionAlive(Order = 1)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateUser(AdminUser adminUser, FormCollection form)
        {
            adminUser.CreatedUserId = Utility.GetAdminUserIdFromSession();

            adminUser.ProjectType = "Tax";


            if (form["IsApproved"] != null && form["IsApproved"].Contains("true"))
            {
                adminUser.IsApproved = true;
            }
            else
            {
                adminUser.IsApproved = false;
            }

            if (form["NotifyUser"] != null && form["NotifyUser"].Contains("true"))
            {
                adminUser.NotifyUser = true;
            }

            if (adminUser.AdminUserId == 0)
            {
                adminUser.AdminPassword = form["AdminPassword"];
            }
            else
            {
                adminUser.AdminPassword = form["RAdminPassword"];
            }


            adminUser = _adminService.SaveAdminUser(adminUser);
            if (adminUser.OperationStatus == StatusType.Success)
            {
                TempData["Update"] = "User Created successfully";
                if (adminUser.Is_Existing)
                {
                    TempData["Update"] = "User details has been Updated successfully";
                }
                else
                {
                    TempData["Update"] = "User Created successfully";
                }
                return Redirect(Url.RouteUrl(new { Controller = ControllerNames.Admin, action = ActionNames.Index }));
            }
            return View();
        }
        #endregion

        #region Activity Log- Get
        [TaxAuthorize()]
        public ActionResult ActivityLog()
        {
            List<AdminActivityLog> _adminUserslist = this._adminService.GetAllActivityLog();
            return View(_adminUserslist);
        }
        #endregion

        #region View Activity Log
        [HttpGet]
        [TaxAuthorize()]
        public ActionResult _ViewActivityLog(long? id)
        {
            AdminActivityLog _ActivityLog = new AdminActivityLog();
            ViewData["AdminUserName"] = string.Empty;
            long adminUserId = id ?? 0;
            _ActivityLog.AdminUserId = adminUserId;
            List<AdminActivityLog> _activity = new List<AdminActivityLog>();
            if (id > 0)
            {
                _activity = this._adminService.GetActivityLogByUserID(id ?? 0);
                //var a = this._adminService.GetActivityLogByUserID(id ?? 0).GroupBy(t => t.ProjectId);
                AdminUser _adminUser = new AdminUser();
                _adminUser = this._adminService.GetAdminUserById(id ?? 0);
                ViewData["AdminUserName"] = _adminUser.AdminFirstName + " " + _adminUser.AdminLastName;

            }

            return PartialView(_activity);
        }
        #endregion

        #region Go to ProjectID
        [TaxAuthorize()]
        [HttpGet]
        public ActionResult GotoProduct(long Id)
        {
            long adminUserId = Utility.GetLong(Utility.GetAdminUserIdFromSession());
            long projectId = Id;
            string url = string.Empty;
            DateTime today = DateTime.Now;
            DateTime expires = DateTime.Now.AddMinutes(10);
            string encrypted = "";
            AdminUser taxUser = _mainControlService.GetAdminUserById(adminUserId);

            if (taxUser != null)
            {
                FormsAuthentication.RedirectFromLoginPage(taxUser.AdminUserName, true);
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, taxUser.AdminUserName, today, expires, true, adminUserId.ToString());
                encrypted = FormsAuthentication.Encrypt(ticket);
            }

            if (projectId == (long)Project.ExpressTruckTax)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("ETTBaseURL") + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.ExpressIFTA)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("IFTABaseURL") + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.ExpressTaxFilings)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("ETFBaseURL") + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.ExpressExtension)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("ExtensionBaseURL") + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.Express990)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("E990BaseURL") + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.TruckLogics)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("ETLBaseURL") + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }

            else if (projectId == (long)Project.ExpressTruckTaxEfileAdmin)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("ETTefileBaseURL") + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }

            else if (projectId == (long)Project.ExtensionEfileAdmin)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("EETefileBaseURL") + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }

            else if (projectId == (long)Project.E990EfileAdmin)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("E990efileBaseURL") + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }

            else if (projectId == (long)Project.ETFEfileAdmin)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("ETFefileBaseURL") + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.ETFEfileAdmin2016)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("ETFefile2016BaseURL") + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.ACAEfileAdmin)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("ACAEfileBaseURL") + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.TSNAAdmin)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("TSNABaseURL") + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.StayTaxExempt)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("STEBaseURL") + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.ACAwise)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("ACAWBaseURL") + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.MefServices)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("MefWSBaseURL") + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.ToolsServices)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("EIFWSBaseURL") + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.ACAwiseEfileAdmin)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("ACAwiseEfileBaseURL") + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.ExpressTruckTaxEfileAdminMobile)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("ETTefileMobileBaseURL") + "/Account/LogOn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.UnitWise)
            {
                url = Biz("unitwise");
            }
            else if (projectId == (long)Project.SpanPlan)
            {
                url = Biz("inspherio");
            }
            else if (projectId == (long)Project.PayWow)
            {
                //string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                //Session[SessionItemKey.AdminRole] = _adminRole;
                //Session[SessionItemKey.ProjectID] = projectId;
                encrypted = Utility.PWEncryptPlainTextToCipherText(adminUserId.ToString());
                url = Utility.GetAppSettings("PayWowBaseURL") + "/user/login?" + FormsAuthentication.FormsCookieName + "=" + HttpUtility.UrlEncode(encrypted);
            }
            else if (projectId == (long)Project.PayWowCompliance)
            {
                //string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                //Session[SessionItemKey.AdminRole] = _adminRole;
                //Session[SessionItemKey.ProjectID] = projectId;
                encrypted = Utility.PWComplianceEncryptPlainTextToCipherText(adminUserId.ToString());
                url = Utility.GetAppSettings("PayWowComplianceBaseURL") + "/user/login?" + FormsAuthentication.FormsCookieName + "=" + HttpUtility.UrlEncode(encrypted);
            }
            else if (projectId == (long)Project.ExpressEfile)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("ExpressEfileBaseURL") + "/Admin/Signin?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.TaxBanditsPEO)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("TaxBanditsPEOBaseURL") + "/Admin/Signin?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.AWTools)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("AWToolsBaseURL") + "/User/Signin?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.ETTSUPPORTADMIN)
            {
                url = Utility.GetAppSettings("ETTSUPPORTADMINBASEURL") + "/Admin/Signin?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.EESUPPORTADMIN)
            {
                url = Utility.GetAppSettings("EESUPPORTADMINBASEURL") + "/Admin/Signin?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.SERVICECONTROLCENTER)
            {
                url = Utility.GetAppSettings("ServiceControlCeneterBaseUrl") + "/Admin/Signin?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.PayWowT_AND_A)
            {
                encrypted = Utility.EncryptPlainTextToCipherText(adminUserId.ToString());
                url = Utility.GetAppSettings("PayWowTAndABaseURL") + "/admin/signin?" + FormsAuthentication.FormsCookieName + "=" + HttpUtility.UrlEncode(encrypted);
            }
            else if (projectId == (long)Project.PayWowSymmetryCompliance)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("PayWowSymmetryComplianceBaseURL") + "/user/login?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.ExpressPaystubGenerator)
            {
                encrypted = Utility.EPSEncryptPlainTextToCipherText(adminUserId.ToString());
                url = Utility.GetAppSettings("ExpressPaystubGeneratorBaseURL") + "?" + FormsAuthentication.FormsCookieName + "=" + HttpUtility.UrlEncode(encrypted);
            }
            else if (projectId == (long)Project.EPSSupportAdmin)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("ExpressPaystubGeneratorBaseSupportURL") + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.EXPRESSNOTIFY)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("ExpressNotifyBaseUrl") + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.EXPRESSSUPPORTADMIN)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("ExpressSupportBaseUrl") + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.EXPRESSEFILEADMIN)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("EEFefileBaseURL") + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            else if (projectId == (long)Project.ExpressEFileAdmin1)
            {
                encrypted = Utility.EEFEncryptPlainTextToCipherText(adminUserId.ToString());
                url = Utility.GetAppSettings("ExpressEFileAdmin") + "?" + FormsAuthentication.FormsCookieName + "=" + HttpUtility.UrlEncode(encrypted);
            }
            //else if (projectId == (long)Project.ExpressEFileAngular)
            //{
            //    encrypted = Utility.EncryptPlainTextToCipherTextBySecurityKey(adminUserId.ToString(), Utility.GetAppSettings("ExpressEfileAngularEncryptionKey"));
            //    url = Utility.GetAppSettings("ExpressEfileAngularUrl") + "?" + FormsAuthentication.FormsCookieName + "=" + HttpUtility.UrlEncode(encrypted);
            //}
            else if (projectId == (long)Project.TaxBandits2)
            {
                encrypted = Utility.EncryptPlainTextToCipherTextBySecurityKey(adminUserId.ToString(), Utility.GetAppSettings("TaxBandits2SessionEncryptionKey"));
                url = Utility.GetAppSettings("TaxBandits2") + "/user/login" + "?" + FormsAuthentication.FormsCookieName + "=" + HttpUtility.UrlEncode(encrypted);
            }
            else if (projectId == (long)Project.ExpressPayments)
            {
                encrypted = Utility.EncryptPlainTextToCipherTextBySecurityKey(adminUserId.ToString(), Utility.GetAppSettings("ExpressPaymentSessionEncryptionKey"));
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("PaymentsBaseURL") + "/ExpressPayments/PaymentReport?" + FormsAuthentication.FormsCookieName + "=" + HttpUtility.UrlEncode(encrypted);
            }
            else if (projectId == (long)Project.ExpressTruckTaxTestEfileAdmin)
            {
                string _adminRole = _mainControlService.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
                Session[SessionItemKey.AdminRole] = _adminRole;
                Session[SessionItemKey.ProjectID] = projectId;
                url = Utility.GetAppSettings("ETTBPefileBaseURL") + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted;
            }
            return Redirect(url);
        }
        #endregion

        #region Admin User List - Get
        [SessionAlive(Order = 1)]
        [TaxAuthorize()]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index(long? id, long? id2)
        {
            long adminprojectId = id ?? 0;
            List<AdminUser> _adminUserslist = this._adminService.GetAllAdminUsers(adminprojectId);
            List<AdminProject> ProjectList = _adminService.GetAllAdminProjects(2).ToList();
            List<AdminCategory> AdminCategoryList = _adminService.GetAllAdminCategories(1).ToList();
            if (id != null)
            {
                ViewData["ProjectType"] = id;
            }
            else
            {
                ViewData["ProjectType"] = 0;
            }
            if (ProjectList != null && ProjectList.Count() > 0)
            {
                ViewData["AdminProjectList"] = new SelectList(ProjectList, "AdminProjectId", "ProjectName");
            }

            if (AdminCategoryList != null && AdminCategoryList.Count() > 0)
            {
                ViewData["AdminCategoryList"] = new SelectList(AdminCategoryList, "AdminCategoryId", "AdminCategoryName", id2 ?? 0);
            }
            return View(_adminUserslist);
        }
        #endregion

        #region Main Menu - Get
        [SessionAlive(Order = 1)]
        [TaxAuthorize()]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult MainMenu()
        {
            long _adminUserId = Utility.GetAdminUserIdFromSession();
            if (_adminUserId > 0)
            {
                AdminUser _adminUser = this._adminService.GetAdminUserById(_adminUserId);
                return View(_adminUser);
            }
            else
            {
                return RedirectToAction(ActionNames.SignOut, ControllerNames.Admin);
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
        public ActionResult _Projects()
        {
            long adminUserId = Utility.GetAdminUserIdFromSession();
            AdminUser adminUser = this._adminService.GetAdminUserById(adminUserId);
            adminUser.AdminUserRoleList = this._adminService.GetAllAdminProjectRole(adminUserId);
            return PartialView(adminUser);
        }
        #endregion
        [TaxAuthorize()]
        public JsonResult SaveProjectRole(long projectid, long roleid, long adminUserId, long? adminUserRoleId)
        {
            AdminUserRole _roleDetails = new AdminUserRole();
            if (projectid > 0 && roleid > 0)
            {

                _roleDetails.AdminUserId = adminUserId;
                _roleDetails.AdminUserRoleId = adminUserRoleId ?? 0;
                _roleDetails.AdminProjectId = projectid;
                _roleDetails.AdminRoleId = roleid;

                _roleDetails = this._adminService.SaveProjectRole(_roleDetails);

                if (_roleDetails != null && _roleDetails.Status == "Success")
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
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult IsUserNameAvailable(string str, long? UserId)
        {
            AdminUser _user = new AdminUser();
            if (!string.IsNullOrEmpty(str))
            {
                //verfy users subdomain
                _user = this._adminService.VerifyUserName(str, UserId ?? 0);
                if (_user != null && _user.OperationStatus == StatusType.Success)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        #region Delete Admin User
        /// <summary>
        /// Delete Admin User
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [SessionAlive(Order = 1)]
        [TaxAuthorize()]
        public JsonResult DeleteUser(long aId)
        {
            bool _isDeleted = false;
            if (aId > 0)
            {
                _isDeleted = this._adminService.DeleteAdminUser(aId);
            }
            //TempData["SuccessMsg"] = "User deleted successfully";
            return Json(_isDeleted, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Delete projectRole
        [SessionAlive(Order = 1)]
        [TaxAuthorize()]
        public JsonResult DeleteProjectRole(long aId)
        {
            bool _isDeleted = false;
            if (aId > 0)
            {
                _isDeleted = this._adminService.DeleteProjectRole(aId);
            }
            //TempData["SuccessMsg"] = "Role deleted successfully";
            return Json(_isDeleted, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Edit save password

        public JsonResult SavePassword(string str, long UserId)
        {
            AdminUser _user = new AdminUser();
            if (!string.IsNullOrEmpty(str))
            {
                _user.AdminUserId = UserId;
                _user.AdminPassword = str;
                _user = this._adminService.SavePassword(_user);
            }
            return Json(_user, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region View Admin Roles
        [HttpGet]
        [TaxAuthorize()]
        public ActionResult _ViewAdminProjectRoles(int? id, int? id2)
        {

            AdminUserRoleUI _adminUserRoleUI = new AdminUserRoleUI();
            ViewData["AdminUserName"] = string.Empty;
            long adminUserId = id ?? 0;
            long adminUserRoleId = id2 ?? 0;
            _adminUserRoleUI.AdminUserId = adminUserId;
            AdminUser _adminUser = new AdminUser();
            if (id > 0)
            {
                _adminUser = this._adminService.GetAdminUserById(id ?? 0);
                ViewData["AdminUserName"] = _adminUser.AdminFirstName + " " + _adminUser.AdminLastName;
            }


            _adminUserRoleUI.AdminUserRoleList = this._adminService.GetAllAdminProjectRole(adminUserId).ToList();
            //_adminUserRoleUI.AdminRoleList = _adminService.GetAllAdminRoles("tax").ToList();
            _adminUserRoleUI.AdminCategoryList = this._adminService.GetAllAdminCategories(adminUserId).ToList();


            if (id2 > 0)
            {
                foreach (var item in _adminUserRoleUI.AdminUserRoleList)
                {
                    if (id2 == item.AdminUserRoleId)
                    {
                        _adminUserRoleUI.ProjectId = item.AdminProjectId;
                        _adminUserRoleUI.Roles = item.AdminRoleId;

                        _adminUserRoleUI.AdminProjectList = _adminService.GetAllAdminProjects(0).ToList();

                    }
                }
            }

            return PartialView(_adminUserRoleUI);
        }
        #endregion

        #region Go to Sales Products
        [TaxAuthorize()]
        public void Sales()
        {
            string _lang = null;
            if (Request.QueryString["lang"] != null)
            {
                _lang = Request.QueryString["lang"];
            }
            long _userId = Utility.GetUserIdFromSession();
            DateTime _today = DateTime.Now;
            DateTime _expires = DateTime.Now.AddMinutes(10);

            AdminUser _adminUser = _adminService.GetAdminUserById(_userId);
            if (_adminUser != null)
            {
                FormsAuthentication.RedirectFromLoginPage(_adminUser.AdminUserName, true);
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, _adminUser.AdminUserName, _today, _expires, true, _userId.ToString());
                string encrypted = FormsAuthentication.Encrypt(ticket);
                Response.Redirect(ConfigurationManager.AppSettings["SalesUrl"] + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                if (Session["LoggedInApps"] != null)
                {
                    Session["LoggedInApps"] += "," + Products.Sales.ToString();
                }
                else
                {
                    Session["LoggedInApps"] += Products.Sales.ToString();
                }
            }
            return;// null;
        }
        #endregion

        #region Go to Tax Products
        [TaxAuthorize()]
        public void Tax(string appName)
        {
            string _lang = null;
            if (Request.QueryString["lang"] != null)
            {
                _lang = Request.QueryString["lang"];
            }
            long _userId = Utility.GetUserIdFromSession();
            DateTime _today = DateTime.Now;
            DateTime _expires = DateTime.Now.AddMinutes(10);

            AdminUser _adminUser = _adminService.GetAdminUserById(_userId);
            if (_adminUser != null)
            {
                FormsAuthentication.RedirectFromLoginPage(_adminUser.AdminUserName, true);
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, _adminUser.AdminUserName, _today, _expires, true, _userId.ToString());
                string encrypted = FormsAuthentication.Encrypt(ticket);

                if (!string.IsNullOrEmpty(appName))
                {
                    if (appName == "ett")
                    {
                        ViewData["ProductName"] = "Express Truck Tax";
                        Response.Redirect(ConfigurationManager.AppSettings["ETTBaseURL"] + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "etf")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["ETFBaseURL"] + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "e990")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["E990BaseURL"] + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "ifta")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["IFTABaseURL"] + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "extn")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["ExtensionBaseURL"] + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "etl")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["ETLBaseURL"] + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "efileett")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["ETTefileBaseURL"] + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "efileextn")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["EETefileBaseURL"] + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "efile990")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["E990efileBaseURL"] + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "efileetf")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["ETFefileBaseURL"] + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "efileetf2016")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["ETFefile2016BaseURL"] + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "tsna")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["TSNABaseURL"] + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "ste")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["STEBaseURL"] + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "efileaca")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["ACAEfileBaseURL"] + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "acaw")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["ACAWBaseURL"] + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "efileacawise")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["ACAwiseEfileBaseURL"] + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "efileettm")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["ETTefileMobileBaseURL"] + "/Account/Logon?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "paywow")
                    {
                        encrypted = Utility.PWEncryptPlainTextToCipherText(_userId.ToString());
                        Response.Redirect(ConfigurationManager.AppSettings["PayWowBaseURL"] + "/user/login?" + FormsAuthentication.FormsCookieName + "=" + HttpUtility.UrlEncode(encrypted) + "&&lang=" + _lang);
                    }
                    else if (appName == "paywowcompliance")
                    {
                        encrypted = Utility.PWComplianceEncryptPlainTextToCipherText(_userId.ToString());
                        Response.Redirect(ConfigurationManager.AppSettings["PayWowComplianceBaseURL"] + "/user/login?" + FormsAuthentication.FormsCookieName + "=" + HttpUtility.UrlEncode(encrypted) + "&&lang=" + _lang);
                    }
                    else if (appName == "paywowsymmetrycompliance")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["PayWowSymmetryComplianceBaseURL"] + "/user/login?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "eef")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["ExpressEfileBaseURL"] + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else if (appName == "expresspaystubgenerator")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["ExpressPaystubGeneratorBaseURL"] + "/admin/signin?" + FormsAuthentication.FormsCookieName + "=" + HttpUtility.UrlEncode(encrypted) + "&&lang=" + _lang);
                    }
                    else if (appName == "epssupportadmin")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["ExpressPaystubGeneratorBaseSupportURL"] + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }

                    else if (appName == "eesupportadmin")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["EESUPPORTADMINBASEURL"] + "/admin/signin?" + FormsAuthentication.FormsCookieName + "=" + HttpUtility.UrlEncode(encrypted) + "&&lang=" + _lang);
                    }
                    else if (appName == "expresspayments")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["PaymentsBaseUrl"] + "/ExpressPayments/PaymentReport?" + FormsAuthentication.FormsCookieName + "=" + HttpUtility.UrlEncode(encrypted) + "&&lang=" + _lang);
                    }
                    else if (appName == "testefileett")
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["ETTBPefileBaseURL"] + "/Home/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                    else
                    {
                        Response.Redirect(ConfigurationManager.AppSettings["TaxUrl"] + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                    }
                }
                else
                {
                    Response.Redirect(ConfigurationManager.AppSettings["TaxUrl"] + FormsAuthentication.FormsCookieName + "=" + encrypted + "&&lang=" + _lang);
                }
                if (Session["LoggedInApps"] != null)
                {
                    Session["LoggedInApps"] += "," + Products.Tax.ToString();
                }
                else
                {
                    Session["LoggedInApps"] += Products.Tax.ToString();
                }
            }
            return;// null;
        }
        #endregion

        #region Show Product
        public ActionResult _ShowProduct()
        {
            string appName = null;
            ViewData["ImageName"] = "";
            if (Request.QueryString["product"] != null)
            {
                appName = Request.QueryString["product"];
            }


            if (!string.IsNullOrEmpty(appName))
            {
                if (appName == "biz")
                {
                    ViewData["ImageName"] = "uwLogo.gif";
                }
                else if (appName == "sp")
                {
                    ViewData["ImageName"] = "spLogo.gif";
                }
                else if (appName == "ett")
                {
                    ViewData["ImageName"] = "ETT.png";
                }
                else if (appName == "etf")
                {
                    ViewData["ImageName"] = "ETF.png";
                }
                else if (appName == "e990")
                {
                    ViewData["ImageName"] = "E990.png";
                }
                else if (appName == "ifta")
                {
                    ViewData["ImageName"] = "IFTA.png";
                }
                else if (appName == "extn")
                {
                    ViewData["ImageName"] = "EET.png";
                }
                else if (appName == "etl")
                {
                    ViewData["ImageName"] = "ETL.png";
                }
                else if (appName == "efileett")
                {
                    ViewData["ImageName"] = "ETTefile.jpg";
                }
                else if (appName == "efileextn")
                {
                    ViewData["ImageName"] = "EETefile.jpg";
                }
                else if (appName == "efile990")
                {
                    ViewData["ImageName"] = "E990efile.jpg";
                }
                else if (appName == "efileetf")
                {
                    ViewData["ImageName"] = "ETFefile.jpg";
                }
                else if (appName == "tsna")
                {
                    ViewData["ImageName"] = "TSNA.png";
                }
                else if (appName == "ste")
                {
                    ViewData["ImageName"] = "STE.jpg";
                }
                else if (appName == "efileaca")
                {
                    ViewData["ImageName"] = "ACAefile.jpg";
                }
                else if (appName == "acaw")
                {
                    ViewData["ImageName"] = "ACAw.png";
                }




            }
            return PartialView();
        }
        #endregion

        #region Go to Biz Products
        public string Biz(string appname)
        {
            string lang = string.Empty;
            string url = string.Empty;
            if (Request.QueryString["lang"] != null)
            {
                lang = Request.QueryString["lang"];
            }
            long userId = Utility.GetUserIdFromSession();
            DateTime today = DateTime.Now;
            DateTime expires = DateTime.Now.AddMinutes(10);

            AdminUser adminUser = _adminService.GetAdminUserById(userId);

            if (adminUser != null)
            {
                FormsAuthentication.RedirectFromLoginPage(adminUser.AdminUserName, true);
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, adminUser.AdminUserName, today, expires, true, userId.ToString());
                string encrypted = FormsAuthentication.Encrypt(ticket);
                if (Session["LoggedInApps"] != null)
                {
                    Session["LoggedInApps"] += "," + Products.Biz.ToString();
                }
                else
                {
                    Session["LoggedInApps"] += Products.Biz.ToString();
                }

                url = ConfigurationManager.AppSettings["BizUrl"] + "/Admin/SignIn?" + FormsAuthentication.FormsCookieName + "=" + encrypted + "&lang=" + lang + "&appname=" + appname;
            }
            else
            {
                url = ConfigurationManager.AppSettings["BizUrl"] + "/Admin/SignIn";
            }
            return url;
        }
        #endregion

        #region Redirect to Application
        [TaxAuthorize()]
        private ActionResult RedirectToApplication(string appName)
        {
            if (string.IsNullOrEmpty(appName) && Session["ReferenceApplication"] != null && !string.IsNullOrEmpty(Session["ReferenceApplication"].ToString()))
            {
                appName = Session["ReferenceApplication"].ToString();
                Session["ReferenceApplication"] = null;
            }
            if (!string.IsNullOrWhiteSpace(appName))
            {
                long adminUserId = Utility.GetLong(Utility.GetUserIdFromSession());
                if (appName == "sales")
                {
                    return RedirectToAction("Sales", ControllerNames.Admin);
                }
                else if (appName == "biz")
                {
                    return RedirectToAction("Biz", ControllerNames.Admin);
                }
                else
                {
                    return RedirectToAction("Tax", ControllerNames.Admin, new { appName = appName });
                }
            }
            return RedirectToAction(ActionNames.MainMenu, ControllerNames.Admin);
        }
        #endregion

        #region Mobile Verification

        #region Mobile Verification Get Method
        /// <summary>
        /// Mobile Verification Get Method
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MobileVerification(string productName)
        {
            var userid = Utility.GetMobileVerifyUserIdFromSession();
            if (userid > 0)
            {
                ViewData["ProductName"] = productName ?? "";
                return View();
            }
            else
            {
                return RedirectToAction(ActionNames.SignIn, ControllerNames.Admin);
            }

        }
        #endregion

        #region Mobile Verification - Partial View  Get Method
        /// <summary>
        /// Mobile Verification - Partial View  Get Method
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult _MobileVerification()
        {
            return PartialView();
        }
        #endregion

        #region Send Verification Code
        /// <summary>
        /// Send Verification Code
        /// </summary>
        /// <param name="mobileVerification"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SendVerificationCode(MobileVerification mobileVerification)
        {
            MobileVerification _mobileVerification = new MobileVerification();
            mobileVerification.UserId = Utility.GetMobileVerifyUserIdFromSession();
            mobileVerification.MobileNumber = Utility.GetMobileNumberFromSession();
            mobileVerification.UserEmail = Utility.GetMobileEmailFromSession();
            _mobileVerification = _adminService.SendVerificationCode(mobileVerification);

            return Json(_mobileVerification, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Verify Code
        /// <summary>
        /// Verify Code
        /// </summary>
        /// <param name="mobileVerification"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult VerifyCode(MobileVerification mobileVerification)
        {
            bool _isVerified = false;
            var userid = Utility.GetMobileVerifyUserIdFromSession();
            if (userid > 0)
            {

                mobileVerification.UserId = Utility.GetMobileVerifyUserIdFromSession();
                _isVerified = _adminService.VerifyMobileCode(mobileVerification);

                if (!_isVerified)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var userId = Utility.GetMobileVerifyUserIdFromSession();

                    AdminUser _adminDetails = _adminService.GetAdminUserById(userId);
                    Utility.AddToSession(_adminDetails, false);

                    //set forms auth cookie
                    FormsAuthentication.SetAuthCookie(_adminDetails.AdminUserName, false);


                    string redirectURL = "";

                    string appName = mobileVerification.ProductName;

                    if (string.IsNullOrEmpty(appName) && Session["ReferenceApplication"] != null && !string.IsNullOrEmpty(Session["ReferenceApplication"].ToString()))
                    {
                        appName = Session["ReferenceApplication"].ToString();
                        Session["ReferenceApplication"] = null;
                    }
                    if (!string.IsNullOrWhiteSpace(appName) && _adminDetails.NotifyUser == true)
                    {
                        long adminUserId = Utility.GetLong(Utility.GetUserIdFromSession());
                        if (appName == "sales")
                        {

                            redirectURL = "/Admin/Sales";
                        }
                        else if (appName == "biz")
                        {
                            redirectURL = "/Admin/Biz";
                        }
                        else
                        {
                            redirectURL = "/Admin/Tax?appName=" + appName;
                        }
                    }
                    else
                    {
                        redirectURL = "/Admin/MainMenu";
                    }

                    return Json(redirectURL, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction(ActionNames.SignIn, ControllerNames.Admin);
            }

        }
        #endregion

        #region Get Verification Code By UniqueId
        /// <summary>
        /// Get Verification Code By UniqueId 
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public JsonResult GetVerificationCodeByUniqueId(string uniqueId)
        {
            string verificationCode = string.Empty;
            verificationCode = _adminService.GetVerificationCodeByUniqueId(uniqueId);
            return Json(verificationCode, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region Add Admin
        /// <summary>
        /// Add Admin
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [SessionAlive(Order = 1)]
        [TaxAuthorize()]
        public JsonResult AddAdmin(long aId)
        {
            bool _updateStatus = false;
            if (aId > 0)
            {
                _updateStatus = this._adminService.AddAdmin(aId);
            }
            return Json(_updateStatus, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Remove Admin
        /// <summary>
        /// Remove Admin
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [SessionAlive(Order = 1)]
        [TaxAuthorize()]
        public JsonResult RemoveAdmin(long aId)
        {
            bool _updateStatus = false;
            if (aId > 0)
            {
                _updateStatus = this._adminService.RemoveAdmin(aId);
            }
            return Json(_updateStatus, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region No Access -- Get Method
        /// <summary>
        /// NoAccess
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult NoAccess()
        {
            return View();
        }
        #endregion

        #region View Activity Log
        /// <summary>
        /// View Activity Log
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [TaxAuthorize()]
        public ActionResult _AdminLogs(long? id)
        {
            List<ScActivityLog> adminLogList = new List<ScActivityLog>();
            long adminUserId = id ?? 0;
            if (adminUserId > 0)
            {
                adminLogList = _adminService.GetActivityLogByAdminUserId(adminUserId);
                AdminUser _adminUser = new AdminUser();
                _adminUser = this._adminService.GetAdminUserById(id ?? 0);
                ViewData["AdminUserName"] = _adminUser.AdminFirstName + " " + _adminUser.AdminLastName;
            }
            return PartialView(adminLogList);
        }
        #endregion

        #region Remove Project Cache
        /// <summary>
        /// RemoveProjectCache
        /// </summary>
        /// <returns></returns>
        [TaxAuthorize()]
        public void RemoveProjectCache(string key)
        {
            CacheBlock cacheBlock = new CacheBlock();
            if (cacheBlock.Contains(key))
            {
                cacheBlock.Remove(key);
            }
        }
        #endregion

        #region View Activity Log
        /// <summary>
        /// Activities the log.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TaxAuthorize()]
        public ActionResult AdminActivityLog()
        {
            List<AdminUser> adminUserList = new List<AdminUser>();
            List<AdminProject> adminProjectList = new List<AdminProject>();
            adminUserList = _adminService.GetAllAdminUsers(0);
            adminProjectList = _adminService.GetAllProjects();
            if (adminUserList != null && adminUserList.Count > 0)
            {
                ViewBag.AdminUserDropDown = adminUserList.OrderBy(x => x.AdminFirstName).Select(v => new SelectListItem { Text = v.AdminFirstName + " " + v.AdminLastName, Value = v.AdminUserId.ToString() }).ToList();
            }
            if (adminProjectList != null && adminProjectList.Count > 0)
            {
                ViewBag.AdminProjectDropDown = adminProjectList.OrderBy(x => x.ProjectName).Select(v => new SelectListItem { Text = v.ProjectName, Value = v.AdminProjectId.ToString() }).ToList();
            }
            return View();
        }
        #endregion

        #region Get Admin Log by Json
        [HttpGet]
        [TaxAuthorize()]
        public JsonResult GetAdminActivityLogByJson(JQueryDataTableParamModel param)
        {
            List<ScActivityLog> adminLogList = new List<ScActivityLog>();
            Nullable<int> sortColumnIndex = Utility.GetInt(Request["iSortCol_0"]) > 0 ? Utility.GetInt(Request["iSortCol_0"]) : (param.SortColumnIndex == 5 ? default(Nullable<int>) : Utility.GetInt(Request["iSortCol_0"]));
            string sortDirection = Request["sSortDir_0"];// asc or des
            param.SortColumnIndex = sortColumnIndex;
            param.sortDirection = sortDirection;
            int recordsCount = _adminService.GetAdminActivityLogCount(param);

            if (recordsCount > 0)
            {
                adminLogList = _adminService.GetAdminActivityLogList(param);
            }

            var result = (from c in adminLogList
                          select new[] {
                             c.IPAddress, //0
                             c.Activity,//1
                            c.Memo,//2
                            c.ProjectName,//3
                            c.CreateTimeStamp+" (EST)",//4
                            c.ActivityLogId.ToString(),//5
                             });

            return Json(new { sEcho = param.sEcho, iTotalRecords = recordsCount, iTotalDisplayRecords = recordsCount, aaData = result }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Update User Verification Code Type
        /// <summary>
        /// Update User Verification Code Type
        /// </summary>
        /// <param name="aId"></param>
        /// <returns></returns>
        [SessionAlive(Order = 1)]
        [TaxAuthorize()]
        public JsonResult UpdateUserVerificationCodeType(long aId)
        {
            bool _updateStatus = false;
            if (aId > 0)
            {
                _updateStatus = this._adminService.UpdateUserVerificationCodeType(aId);
            }
            return Json(_updateStatus, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Payment Transaction Report
        [TaxAuthorize()]
        public ActionResult PaymentTransactionReport()
        {
            long _adminUserId = Utility.GetAdminUserIdFromSession();
            var adminRole = Convert.ToString(Session[SessionItemKey.AdminRole]);
            var productId = Utility.GetInt(Session[SessionItemKey.ProductId]);
            var paymentProcessors = Utility.GetInt(Session[SessionItemKey.PaymentProcessorid]);
            var beginDate = Utility.GetDateTime(Session[SessionItemKey.FromDate]);
            var endDate = Utility.GetDateTime(Session[SessionItemKey.ToDate]);
            var transactionType = Utility.GetSringValue(Session[SessionItemKey.TransactionType]);
            bool isPaymentAccessible = false;
            var adminEmailAddress = Convert.ToString(Session[SessionItemKey.AdminEmailAddress]);
            if (!string.IsNullOrWhiteSpace(adminEmailAddress))
            {
                string paymentsAccessEmails = Utility.GetAppSettings(Constants.PaymentsAccessEmails);
                if (!string.IsNullOrWhiteSpace(paymentsAccessEmails) && paymentsAccessEmails.ToLower().Contains(adminEmailAddress.ToLower()))
                {
                    isPaymentAccessible = true;
                }

            }
            if (adminRole == AdminRoleType.Administrator.ToString() || isPaymentAccessible)
            {
                if (_adminUserId > 0)
                {
                    string connectionString = Utility.GetAppSettings("PaymentLibConnStr");
                    var spanProducts = _adminService.GetSpanProducts(connectionString);
                    var spanPaymentProcessors = _adminService.GetSpanPaymentProcessors(connectionString);

                    //Add ALL category in Drop down
                    SpanProducts productALL = new SpanProducts();
                    productALL.ProductId = 100;
                    productALL.ProductName = "ALL";
                    spanProducts.Insert((spanProducts.Count), productALL);

                    ViewData["SpanProducts"] = spanProducts;
                    ViewData["SpanPaymentProcessors"] = spanPaymentProcessors;
                    ViewData["BeginDate"] = beginDate != DateTime.MinValue ? beginDate.ToString("MM/dd/yyyy") : DateTime.Now.ToString("MM/dd/yyyy");
                    ViewData["EndDate"] = endDate != DateTime.MinValue ? endDate.ToString("MM/dd/yyyy") : DateTime.Now.ToString("MM/dd/yyyy");
                    if (!string.IsNullOrWhiteSpace(transactionType))
                    {
                        ViewData["TransactionType"] = transactionType;
                    }
                    if (paymentProcessors > 0)
                    {
                        ViewData["PaymentProcessors"] = paymentProcessors;
                    }
                    if (productId > 0)
                    {
                        ViewData["ProductName"] = productId;
                    }
                    return View();
                }
                else
                {
                    return RedirectToAction(ActionNames.SignOut, ControllerNames.Admin);
                }
            }
            else
            {
                return RedirectToAction(ActionNames.MainMenu, ControllerNames.Admin);
            }



        }
        #endregion

        #region _ReturnCreatedReports
        [TaxAuthorize()]
        public ActionResult _TransactionReport(short id, int id1, DateTime id2, DateTime id3, string transactionType)
        {
            Session[SessionItemKey.ProductId] = null; //Ticket no - 4581 issue product code change issue fixed
            Session[SessionItemKey.PaymentProcessorid] = null;
            Session[SessionItemKey.FromDate] = null;
            Session[SessionItemKey.ToDate] = null;
            Session[SessionItemKey.TransactionType] = null;
            string connectionString = Utility.GetAppSettings("PaymentLibConnStr");
            TransactionReport transactionReport = new TransactionReport();
            transactionReport.ConnectionString = connectionString;
            transactionReport.ProductId = id;
            transactionReport.PaymentProcessorid = id1;
            transactionReport.FromDate = id2 != DateTime.MinValue ? id2 : DateTime.Now;
            transactionReport.ToDate = id3 != DateTime.MinValue ? id3 : DateTime.Now;
            transactionReport.TransactionType = transactionType;
            transactionReport.ToDate = transactionReport.ToDate.AddDays(1);
            TempData["PaymentProcessorid"] = id1;
            TempData["FromDate"] = id2;
            TempData["ToDate"] = id3;
            TempData["TransactionType"] = transactionType;
            
            List<TransactionReportDetails> transactionReportDetails = new List<TransactionReportDetails>();
            List<TransactionReportDetails> logFileText = new List<TransactionReportDetails>();
            transactionReportDetails = _adminService.GetTransactionReportDetails(transactionReport);
            if (transactionReportDetails != null)
            {
                foreach (var item in transactionReportDetails)
                {
                    if (item != null && item.ApiCallStatus == "Failure")
                    {
                        var btArray = Utility.DownloadS3File(item.ResponseS3FilePath);
                        string logTxt = System.Text.Encoding.UTF8.GetString(btArray);
                        var requestResponseLogDetails = JsonConvert.DeserializeObject<RequestResponseLogDetails>(logTxt);
                        if (requestResponseLogDetails != null)
                        {
                            APIResponse deserialize = JsonConvert.DeserializeObject<APIResponse>(requestResponseLogDetails.ApiResponse);
                            if (deserialize != null)
                            {
                                item.GateWayErrorsList = deserialize.GatewayErrors;
                            }
                        }
                    }
                }
            }
            PaymentRefundLog refundAmountReq = new PaymentRefundLog();
            refundAmountReq.BeginDate = transactionReport.FromDate;
            refundAmountReq.EndDate = transactionReport.ToDate;
            refundAmountReq.SpanProductId = transactionReport.ProductId;
            decimal refundAmount = _adminService.GetRefundAmount(refundAmountReq, connectionString, "REFUND");
            decimal voidAmount = _adminService.GetRefundAmount(refundAmountReq, connectionString, "VOID");
            ViewData["RefundAmount"] = refundAmount;
            ViewData["VoidAmount"] = voidAmount;
            return PartialView(transactionReportDetails);
        }
        #endregion

        #region _AmazonS3Request
        [TaxAuthorize()]
        public ActionResult _AmazonS3Request(string id, int id2)
        {
            string connectionString = Utility.GetAppSettings("PaymentLibConnStr");
            var s3UploadUrl = _adminService.GetS3UploadURL(id, connectionString, id2);
            List<TransactionReportDetails> logFileText = new List<TransactionReportDetails>();
            foreach (var item in s3UploadUrl)
            {
                //using (WebClient wc = new WebClient())
                //{
                var btArray = Utility.DownloadS3File(item.ResponseS3FilePath);
                string logTxt = System.Text.Encoding.UTF8.GetString(btArray);
                TransactionReportDetails logDetails = new TransactionReportDetails();
                logDetails.RequestResponseLogDetails = JsonConvert.DeserializeObject<RequestResponseLogDetails>(logTxt);
                logDetails.TransactionLogText = logTxt;
                logDetails.PaymentProcessorType = item.PaymentProcessorType;
                logFileText.Add(logDetails);
                //}
            }
            return PartialView(logFileText);
        }
        #endregion


        #region Create refund request
        [HttpGet]
        [TaxAuthorize()]
        public ActionResult _VoidRefundRequest(string id)
        {
            PaymentRefundLog refundRequest = new PaymentRefundLog();
            string connectionString = Utility.GetAppSettings("PaymentLibConnStr");
            if (!string.IsNullOrWhiteSpace(id))
            {
                var transactionDetail = _adminService.GetTransactionDetail(id, connectionString);
                if (transactionDetail != null && !string.IsNullOrWhiteSpace(transactionDetail.TransactionReferenceId))
                {
                    refundRequest.TransactionReferenceId = transactionDetail.TransactionReferenceId;
                    refundRequest.PaidAmount = transactionDetail.OrderAmount;
                    refundRequest.RefundAmount = transactionDetail.OrderAmount;
                    refundRequest.PaymentProcessorId = transactionDetail.PaymentProcessorType;
                    refundRequest.SpanProductId = transactionDetail.ProductId;
                    refundRequest.EmailAddress = transactionDetail.EmailAddress;
                    refundRequest.PaidDate = transactionDetail.PaymentDate;
                    refundRequest.VoidRefundDate = DateTime.Today; // Ticket no-4581 Refund date issue for show current date
                }
            }
            var spanProducts = _adminService.GetSpanProducts(connectionString);
            var spanPaymentProcessors = _adminService.GetSpanPaymentProcessors(connectionString);
            ViewData["SpanProducts"] = spanProducts;
            ViewData["SpanPaymentProcessors"] = spanPaymentProcessors;
            return PartialView(refundRequest);
        }
        #endregion

        #region Refund request - POST
        [HttpPost]
        [TaxAuthorize()]
        public ActionResult VoidRefundRequest(PaymentRefundLog refundRequest)
        {
            string connectionString = Utility.GetAppSettings("PaymentLibConnStr");
            var adminUserId = Utility.GetAdminUserIdFromSession();
            var admiNEmail = _adminService.GetAdminUserById(adminUserId);
            refundRequest.AdminUserName = Utility.GetAdminUserIdFromSession() + "#" + admiNEmail.UserName + "#" + admiNEmail.EmailAddress;
            var spanProducts = _adminService.GetSpanProducts(connectionString);
            var paymentProcessors = Utility.GetInt(TempData["PaymentProcessorid"]);
            var beginDate = Utility.GetDateTime(TempData["FromDate"]);
            var endDate = Utility.GetDateTime(TempData["ToDate"]);
            var transactionType = Utility.GetSringValue(TempData["TransactionType"]);
            if (refundRequest.IsApiRefund && refundRequest.PaymentProcessorId == 5)
            {
                if (refundRequest.ChargeBackType == "VOID")
                {
                    WfVoidRequest wfVoidRequest = new WfVoidRequest();
                    wfVoidRequest.AdminUserName = refundRequest.AdminUserName;
                    wfVoidRequest.Comments = refundRequest.Comments;
                    wfVoidRequest.Email = refundRequest.EmailAddress;
                    wfVoidRequest.PaidDate = refundRequest.PaidDate;
                    wfVoidRequest.ProductCode = spanProducts.Where(x => x.ProductId == refundRequest.SpanProductId).SingleOrDefault().ProductName;
                    wfVoidRequest.TransactionReferenceId = refundRequest.TransactionReferenceId;
                    wfVoidRequest.VoidAmount = refundRequest.PaidAmount;
                    var apiResp = _adminService.VoidTransaction(wfVoidRequest);
                    if (apiResp.OperationStatus == PaymentApiStatusType.Success)
                    {
                        Session[SessionItemKey.ProductId] = refundRequest.SpanProductId;
                        Session[SessionItemKey.PaymentProcessorid] = paymentProcessors;
                        Session[SessionItemKey.FromDate] = beginDate;
                        Session[SessionItemKey.ToDate] = endDate;
                        Session[SessionItemKey.TransactionType] = transactionType;
                        return Json("SUCCESS", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        string error = string.Empty;
                        if (apiResp.Errors != null && apiResp.Errors.Count > 0)
                        {
                            error = apiResp.Errors[0].ErrorCode + "-" + apiResp.Errors[0].LongMessage;
                        }
                        return Json(error, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (refundRequest.ChargeBackType == "REFUND")
                {
                    WfRefundRequest wfRefundRequest = new WfRefundRequest();
                    wfRefundRequest.PaidAmount = refundRequest.PaidAmount;
                    wfRefundRequest.PaidDate = refundRequest.PaidDate;
                    wfRefundRequest.ProductCode = spanProducts.Where(x => x.ProductId == refundRequest.SpanProductId).SingleOrDefault().ProductName;
                    wfRefundRequest.RefundAmount = refundRequest.RefundAmount;
                    wfRefundRequest.TransactionReferenceId = refundRequest.TransactionReferenceId;
                    wfRefundRequest.AdminUserName = refundRequest.AdminUserName;
                    wfRefundRequest.Comments = refundRequest.Comments;
                    wfRefundRequest.Email = refundRequest.EmailAddress;
                    var apiResp = _adminService.RefundTransaction(wfRefundRequest);
                    if (apiResp.OperationStatus == PaymentApiStatusType.Success)
                    {
                        Session[SessionItemKey.ProductId] = refundRequest.SpanProductId;
                        Session[SessionItemKey.PaymentProcessorid] = paymentProcessors;
                        Session[SessionItemKey.FromDate] = beginDate;
                        Session[SessionItemKey.ToDate] = endDate;
                        Session[SessionItemKey.TransactionType] = transactionType;
                        return Json("SUCCESS", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        string error = string.Empty;
                        if (apiResp.Errors != null && apiResp.Errors.Count > 0)
                        {
                            error = apiResp.Errors[0].ErrorCode + "-" + apiResp.Errors[0].LongMessage;
                        }
                        return Json(error, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            _adminService.SaveVoidRefund(refundRequest, connectionString);
            Session[SessionItemKey.ProductId] = refundRequest.SpanProductId;
            Session[SessionItemKey.PaymentProcessorid] = paymentProcessors;
            Session[SessionItemKey.FromDate] = beginDate;
            Session[SessionItemKey.ToDate] = endDate;
            Session[SessionItemKey.TransactionType] = transactionType;
            return Json("SUCCESS", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Payment Refunds
        [TaxAuthorize()]
        public ActionResult PaymentRefunds()
        {
            long _adminUserId = Utility.GetAdminUserIdFromSession();
            var adminRole = Convert.ToString(Session[SessionItemKey.AdminRole]);
            var adminEmailAddress = Convert.ToString(Session[SessionItemKey.AdminEmailAddress]);
            bool isPaymentAccessible = false;
            if (!string.IsNullOrWhiteSpace(adminEmailAddress))
            {
                string paymentsAccessEmails = Utility.GetAppSettings(Constants.PaymentsAccessEmails);
                if (!string.IsNullOrWhiteSpace(paymentsAccessEmails) && paymentsAccessEmails.ToLower().Contains(adminEmailAddress.ToLower()))
                {
                    isPaymentAccessible = true;
                }

            }
            if (adminRole == AdminRoleType.Administrator.ToString() || isPaymentAccessible)
            {
                if (_adminUserId > 0)
                {
                    string connectionString = Utility.GetAppSettings("PaymentLibConnStr");
                    var spanProducts = _adminService.GetSpanProducts(connectionString);
                    var spanPaymentProcessors = _adminService.GetSpanPaymentProcessors(connectionString);
                    ViewData["SpanProducts"] = spanProducts;
                    ViewData["SpanPaymentProcessors"] = spanPaymentProcessors;
                    ViewData["BeginDate"] = DateTime.Now.ToString("MM/dd/yyyy");
                    ViewData["EndDate"] = DateTime.Now.ToString("MM/dd/yyyy");
                    return View();
                }
                else
                {
                    return RedirectToAction(ActionNames.SignOut, ControllerNames.Admin);
                }
            }
            else
            {
                return RedirectToAction(ActionNames.MainMenu, ControllerNames.Admin);
            }

        }
        #endregion

        #region _ReturnCreatedReports
        [TaxAuthorize()]
        public ActionResult _PaymentRefunds(short id, DateTime id1, DateTime id2)
        {
            string connectionString = Utility.GetAppSettings("PaymentLibConnStr");
            PaymentRefundLog transactionReport = new PaymentRefundLog();
            transactionReport.SpanProductId = id;
            transactionReport.BeginDate = id1 != DateTime.MinValue ? id1 : DateTime.Now;
            transactionReport.EndDate = id2 != DateTime.MinValue ? id2 : DateTime.Now;
            transactionReport.EndDate = transactionReport.EndDate.AddDays(1);
            List<PaymentRefundLog> transactionReportDetails = new List<PaymentRefundLog>();
            transactionReportDetails = _adminService.GetPaymentRefundDetails(transactionReport, connectionString);
            return PartialView(transactionReportDetails);
        }
        #endregion

        #region Two Factor Authentication
        /// <summary>
        /// Two Factor Authentication
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult TwoFactorAuthentication()
        {
            AdminUser adminUser = new AdminUser();
            var adminUserId = Utility.GetMobileVerifyUserIdFromSession();
            adminUser = _adminService.GetAdminUserUthenticatorDetailsByAdminUserId(adminUserId);
            ViewData["IsEnabledAuthentication"] = adminUser.IsEnabledAuthenticator;
            var requestAccount = new AdminUser { AdminUserId = adminUser.AdminUserId, EmailAddress = adminUser.EmailAddress };
            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            var setupInfo = twoFactor.GenerateSetupCode(Utility.GetAppSettings(Constants.AuthenticatorAppName), requestAccount.EmailAddress, Utility.GetTwoFactorKeyWithEmail(requestAccount.EmailAddress), false, 3);
            ViewBag.BarcodeImageUrl = setupInfo.QrCodeSetupImageUrl;
            ViewBag.QRCodeSecretKey = setupInfo.ManualEntryKey;
            ViewData["AdminUserId"] = adminUserId;
            return View(adminUser);
        }
        #endregion

        #region Verify Authentication Code
        /// <summary>
        /// Verify Authentication Code
        /// </summary>
        /// <param name="mobileVerification"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult VerifyAuthenticationCode(MobileVerification mobileVerification)
        {
            if (mobileVerification != null)
            {
                AdminUser adminUser = new AdminUser();
                var adminUserId = Utility.GetMobileVerifyUserIdFromSession();
                adminUser = _adminService.GetAdminUserUthenticatorDetailsByAdminUserId(adminUserId);
                TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
                var isValid = twoFactor.ValidateTwoFactorPIN(Utility.GetTwoFactorKeyWithEmail(adminUser.EmailAddress), mobileVerification.VerificationCode, TimeSpan.FromSeconds(30));
                if (!isValid)
                {

                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var userId = Utility.GetMobileVerifyUserIdFromSession();
                    AdminUser _adminDetails = _adminService.GetAdminUserById(userId);
                    Utility.AddToSession(_adminDetails, false);

                    //set forms auth cookie
                    FormsAuthentication.SetAuthCookie(_adminDetails.AdminUserName, false);

                    if (adminUser.IsEnabledAuthenticator == false)
                    {
                        _adminService.UpdateAuthenticationForAdminUser(mobileVerification.AdminUserId);
                    }
                    string redirectURL = "";
                    string appName = mobileVerification.ProductName;

                    if (string.IsNullOrEmpty(appName) && Session["ReferenceApplication"] != null && !string.IsNullOrEmpty(Session["ReferenceApplication"].ToString()))
                    {
                        appName = Session["ReferenceApplication"].ToString();
                        Session["ReferenceApplication"] = null;
                    }
                    if (!string.IsNullOrWhiteSpace(appName) && _adminDetails.NotifyUser == true)
                    {
                        if (appName == "sales")
                        {

                            redirectURL = "/Admin/Sales";
                        }
                        else if (appName == "biz")
                        {
                            redirectURL = "/Admin/Biz";
                        }
                        else
                        {
                            redirectURL = "/Admin/Tax?appName=" + appName;
                        }
                    }
                    else
                    {
                        redirectURL = "/Admin/MainMenu";
                    }

                    return Json(redirectURL, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction(ActionNames.SignIn, ControllerNames.Admin);
            }
        }
        #endregion

        #region Reset Authentication
        /// <summary>
        /// Reset Authentication
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [TaxAuthorize()]
        public JsonResult ResetAuthentication(long Id)
        {
            bool _updateStatus = false;
            if (Id > 0)
            {
                _updateStatus = this._adminService.ResetAuthentication(Id);
            }
            return Json(_updateStatus, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Payment Reconcilation Report 
        [TaxAuthorize()]
        public ActionResult PaymentReconcilationReport()
        {
            long _adminUserId = Utility.GetAdminUserIdFromSession();
            var adminRole = Convert.ToString(Session[SessionItemKey.AdminRole]);
            var adminEmailAddress = Convert.ToString(Session[SessionItemKey.AdminEmailAddress]);
            bool isPaymentAccessible = false;
            if (!string.IsNullOrWhiteSpace(adminEmailAddress))
            {
                string paymentsAccessEmails = Utility.GetAppSettings(Constants.PaymentsAccessEmails);
                if (!string.IsNullOrWhiteSpace(paymentsAccessEmails) && paymentsAccessEmails.ToLower().Contains(adminEmailAddress.ToLower()))
                {
                    isPaymentAccessible = true;
                }

            }
            if (adminRole == AdminRoleType.Administrator.ToString() || isPaymentAccessible)
            {
                if (_adminUserId > 0)
                {
                    string connectionString = Utility.GetAppSettings("PaymentLibConnStr");
                    var spanProducts = _adminService.GetSpanProducts(connectionString);
                    var spanPaymentProcessors = _adminService.GetSpanPaymentProcessors(connectionString);
                    List<string> productname = new List<string>();
                    productname.Add("TBS");
                    productname.Add("ETT");
                    productname.Add("EE");

                    var products = spanProducts.Where(x => productname.Contains(x.ProductName)).ToList();
                    ViewData["SpanProducts"] = products;
                    ViewData["SpanPaymentProcessors"] = spanPaymentProcessors;
                    ViewData["BeginDate"] = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
                    ViewData["EndDate"] = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
                    return View();
                }
                else
                {
                    return RedirectToAction(ActionNames.SignOut, ControllerNames.Admin);
                }
            }
            else
            {
                return RedirectToAction(ActionNames.MainMenu, ControllerNames.Admin);
            }
        }
        #endregion

        #region _Payment Reconcilation Report
        [TaxAuthorize()]
        public ActionResult _PaymentReconcilationReport(short id, DateTime id1/*, DateTime id2*/)
        {
            List<PaymentReconReport> paymentReconReport = new List<PaymentReconReport>();
            DateTime beginDate = id1 != DateTime.MinValue ? id1 : DateTime.Now.AddDays(-1);
            DateTime endDate = new DateTime(id1.Year, id1.Month, id1.Day, 23, 59, 59);
            //DateTime endDates = endDate != DateTime.MinValue ? endDate : DateTime.Now.AddMilliseconds(-1);
            //WelsFargo and Stripe Report generated here
            AuthorizeResponse authResponse = _adminService.GetDailySPANReport(beginDate, endDate);
            List<string> StripeTransactionId = new List<string>(); //Adding Transaction Id from Get Daily SPAN Report Method
            if (authResponse.TransactionId != null && authResponse.TransactionId.Count > 0)
            {
                StripeTransactionId.AddRange(authResponse.TransactionId);
            }
            paymentReconReport = _adminService.GetPaymentBatchDetailsReport(beginDate, endDate, id, StripeTransactionId);


            return PartialView(paymentReconReport);
        }
        #endregion
    }
}


