using Main.Control.Core.Services;
using Main.Control.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Main.Control.Web.Controllers
{
    [SessionAlive(Order = 2)]
    public class TextController : Controller
    {

        #region Declaration

        private readonly IAdminService _adminService;

        #endregion

        #region Constructor
        public TextController(IAdminService adminService)
        {
            this._adminService = adminService;
        }

        #endregion

        // GET: Text
        public ActionResult Index()
        {
            return View();
        }

        #region Update Mobile Verification Status
        /// <summary>
        /// Update Mobile Verification Status
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateMobileVerificationStatus()
        {
            string uniqueId = Request["SmsSid"] ?? string.Empty;
            string phoneNumber = Request["To"] ?? string.Empty;
            string status = Request["SmsStatus"] ?? string.Empty;
            this._adminService.UpdateMobileVerificationStatus(uniqueId);

            return View();
        }
        #endregion
    }
}