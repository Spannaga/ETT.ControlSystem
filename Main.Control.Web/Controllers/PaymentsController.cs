using Main.Control.Core.Models;
using Main.Control.Core.Services;
using Main.Control.Web.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Main.Control.Web.Controllers
{
    public class PaymentsController : Controller
    {
        #region Declaration
        private readonly IAdminService _adminService;
        #endregion

        #region Constructor

        public PaymentsController(IAdminService adminservice)
        {

            this._adminService = adminservice;

        }
        #endregion

        #region User Payments
        [HttpGet]
        public ActionResult UserPayments()
        {
            List<UserPayments> userPayments = new List<UserPayments>();
            userPayments = _adminService.GetAllUserPayments();
            return View(userPayments);
        }

        #endregion

        #region Add User Payment
        [HttpGet]
        public ActionResult _AddUserPayment(Guid UserPaymentId)
        {
            UserPayments userPayment = new UserPayments();
            if (UserPaymentId != Guid.Empty)
            {
                userPayment = _adminService.GetUserPaymentDetailByTokenId(UserPaymentId);
            }
            else
            {
                userPayment.UserPaymentDetails = new List<UserPaymentDetail>();
                userPayment.UserPaymentDetails.Add(new UserPaymentDetail());
            }
            userPayment.Projects = _adminService.GetAllAdminProjects(6).ToList();
            userPayment.States = _adminService.GetAllStates();
            userPayment.Countries = _adminService.GetCountries();
            return PartialView(userPayment);
        }

        #endregion

        #region Save User Payment
        [HttpPost]
        public ActionResult SavePaymentDetails(UserPayments userPayment)
        {
            userPayment.UserPaymentDetails.RemoveAll(x => x.ServiceName == "##REMOVED");
            userPayment.PaymentStatus = PaymentEmailStatus.CREATED.ToString();
            userPayment.OrderDescription = Newtonsoft.Json.JsonConvert.SerializeObject(userPayment.UserPaymentDetails);
            userPayment.PaymentAmount = userPayment.UserPaymentDetails.Sum(x => x.Amount);
            _adminService.SaveUserPayments(userPayment);
            UserPaymentLog userPaymentLog = new UserPaymentLog();
            if (userPayment.UserPaymentId != Guid.Empty)
            {
                userPaymentLog.AcitivityMsg = "Payment Updated";
            }
            else
            {
                userPaymentLog.AcitivityMsg = "Payment Created";
            }
            userPaymentLog.ActivityTimeStamp = DateTime.Now;
            userPaymentLog.ActivityType = PaymentActivityType.ADMIN.ToString();
            userPaymentLog.AdminUserId = Utility.GetAdminUserIdFromSession();
            userPaymentLog.UserPaymentId = userPayment.UserPaymentId;
            _adminService.SaveUserPaymentLog(userPaymentLog);
            return Json(userPayment.UserPaymentId, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Prepare Payment Email
        [HttpGet]
        public ActionResult _PreparePaymentEmail(Guid UserPaymentId)
        {
            UserPayments userPayment = new UserPayments();
            userPayment = _adminService.GetUserPaymentDetailByTokenId(UserPaymentId);
            var paymentTemplateDetail = _adminService.GetPaymentMailHtml(userPayment);
            userPayment.MailHtml = paymentTemplateDetail.MailTemplateHtml;
            userPayment.MailSubject = paymentTemplateDetail.MailSubject;
            return PartialView(userPayment);
        }

        #endregion

        #region Prepare Payment Email - POST
        [HttpPost]
        public ActionResult PreparePaymentEmail(UserPayments userPayments)
        {
            var userPayment = _adminService.GetUserPaymentDetailByTokenId(userPayments.UserPaymentId);
            var projectDetails = _adminService.GetProjectNameByProjectId(userPayment.Projectid);
            var paymentTemplateDetail = _adminService.GetPaymentMailHtml(userPayment);
            userPayment.MailHtml = paymentTemplateDetail.MailTemplateHtml;
            MemoryStream htmlStream = new MemoryStream(Encoding.UTF8.GetBytes(userPayment.MailHtml));
            string htmlPath = "UserPayments/" + projectDetails.ProjectName.Replace(" ", "") + "/" + Guid.NewGuid().ToString().Replace("-", "") + ".html";
            Utility.UploadImageInWebStorage(htmlPath, htmlStream);
            userPayment.MailBodyS3Path = Utility.GetS3FullUrl(htmlPath);
            userPayment.MailSubject = userPayments.MailSubject;
            userPayment.SpanLibrConnStr = Utility.GetAppSettings("PaymentLibConnStr");
            var emailDetails = _adminService.SendPaymentsEmail(userPayment);
            if (!string.IsNullOrWhiteSpace(emailDetails.MessageId))
            {
                userPayment.MailSentTime = DateTime.Now;
                userPayment.PaymentStatus = PaymentEmailStatus.MAILSENT.ToString();
                userPayment.MessageId = emailDetails.MessageId;
            }
            else if (!string.IsNullOrWhiteSpace(emailDetails.FailureMessage))
            {
                userPayment.PaymentStatus = PaymentEmailStatus.MAILERROR.ToString();
                userPayment.FailureMsg = emailDetails.FailureMessage;
            }
            else
            {
                userPayment.PaymentStatus = PaymentEmailStatus.MAILERROR.ToString();
            }
            userPayment.SpanLibrConnStr = Utility.GetAppSettings("PaymentLibConnStr");
            _adminService.SaveUserPayments(userPayment);
            UserPaymentLog userPaymentLog = new UserPaymentLog();
            if (!string.IsNullOrWhiteSpace(emailDetails.MessageId))
            {
                userPaymentLog.AcitivityMsg = "Mail sent - Message Id : " + emailDetails.MessageId;
            }
            else
            {
                userPaymentLog.AcitivityMsg = "Failed sending mail" + emailDetails.FailureMessage;
            }
            userPaymentLog.ActivityTimeStamp = DateTime.Now;
            userPaymentLog.ActivityType = PaymentActivityType.ADMIN.ToString();
            userPaymentLog.AdminUserId = Utility.GetAdminUserIdFromSession();
            userPaymentLog.UserPaymentId = userPayment.UserPaymentId;
            _adminService.SaveUserPaymentLog(userPaymentLog);
            return Json(userPayment, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Get project Details By project Id



        #endregion

        #region View Failure Msg
        [HttpGet]
        public JsonResult GetFailureMsg(Guid paymentId)
        {
            var userPaymentDetails = _adminService.GetUserPaymentDetailByTokenId(paymentId);
            return Json(userPaymentDetails.FailureMsg, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PaymentLogs
        [HttpGet]
        public ActionResult _PaymentLogs(Guid paymentId, PaymentActivityType activityType)
        {
            var activityLogs = _adminService.GetPaymentLogs(paymentId, activityType);
            return PartialView(activityLogs);
        }


        #endregion

        #region Get Email Address From

        public JsonResult SearchEmailByProduct(string emailAddress, int project)
        {
            var emails = _adminService.SearchEmailByProduct(emailAddress, project);
            return Json(emails, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}