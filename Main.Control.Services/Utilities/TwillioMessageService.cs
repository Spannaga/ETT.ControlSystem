using Main.Control.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using System.Configuration;

namespace Main.Control.Services.Utilities
{
    public class TwillioMessageService
    {
        public static MobileVerification SendMessage(MobileVerification mobileverification)
        {
            string AccountSid = ConfigurationManager.AppSettings["AccountSID"];
            string AuthToken = ConfigurationManager.AppSettings["AuthToken"];
            string senderNumber = ConfigurationManager.AppSettings["SenderNumber"];
            string Receiver = mobileverification.MobileNumber;
            string Message = "";
            Message = "MainControl SignIn Verification Code: " + mobileverification.VerificationCode;
            var twilio = new TwilioRestClient(AccountSid, AuthToken);
            string siteURL = ConfigurationManager.AppSettings["TwillioCallBackURL"].ToString();
            var message = twilio.SendMessage(senderNumber, Receiver, Message, siteURL);
            mobileverification.AccountSId = message.AccountSid;
            mobileverification.UniqueId = message.Sid;
            mobileverification.SenderNumber = message.To;
            mobileverification.Status = message.Status;
            if (message.Sid != null)
            {
                mobileverification.OperationStatus = "Success";
            }
            else
            {
                mobileverification.OperationStatus = "Failure";
            }
            return mobileverification;
        }

        public static bool SendMessageToAdmin(string mobileNumber, string emailAddress, string ipAddress, string status, string projectName, string ipName)
        {
            string AccountSid = ConfigurationManager.AppSettings["AccountSID"];
            string AuthToken = ConfigurationManager.AppSettings["AuthToken"];
            string senderNumber = ConfigurationManager.AppSettings["SenderNumber"];
            string Receiver = mobileNumber;
            string Message = "";

            Message = emailAddress + " - " + (!string.IsNullOrEmpty(status) ? char.ToLower(status[0]) + status.Substring(1) : "") + " - " + projectName + " - " + ipAddress + " " + "(" + ipName + ")";

            var twilio = new TwilioRestClient(AccountSid, AuthToken);
            string siteURL = ConfigurationManager.AppSettings["TwillioCallBackURL"].ToString();
            var message = twilio.SendMessage(senderNumber, Receiver, Message, siteURL);
            if (message.Sid != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
