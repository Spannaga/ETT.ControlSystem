using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plivo;
using Plivo.Exception;
using Main.Control.Core.Models;
using System.Configuration;

namespace Main.Control.Services.Utilities
{
    public class PlivoMessageService
    {
        public static MobileVerification SendPlivoMessage(MobileVerification mobileverification)
        {
            string plivoAuthId = ConfigurationManager.AppSettings["PlivoAuthId"];
            string plivoAuthToken = ConfigurationManager.AppSettings["PlivoAuthToken"];
            string plivoNumber = ConfigurationManager.AppSettings["PlivoNumber"];
            // string url = ConfigurationManager.AppSettings["ReplyToSMSURL"];
            string reciever = mobileverification.MobileNumber; // The number to which the message will be sent
            string plivoNumberExt = ConfigurationManager.AppSettings["PlivoTestExt"];
            string plivoExtensionNo = !string.IsNullOrEmpty(plivoNumberExt) ? (plivoNumberExt == "US" ? "1" : plivoNumberExt) : "1";

            string message = "";
            message = (mobileverification.ProductName == "IPF" ? "IPF " : string.Empty) + "SpanControl SignIn Verification Code: " + mobileverification.VerificationCode;
            var api = new PlivoApi(plivoAuthId, plivoAuthToken);

            try
            {
                if (!string.IsNullOrEmpty(reciever))
                {
                    var destinationNumber = reciever.Contains("+1") ? reciever.Replace("+1", "1") : reciever.Contains("+91") ? reciever.Replace("+91", "91") : (plivoExtensionNo + reciever);
                    var response = api.Message.Create
                    (
                    src: plivoNumber,
                    //dst: new List<string> { "919677012248" },
                    dst: new List<string> { destinationNumber },
                    text: message,
                    method: "POST"
                    );

                    if (response != null && response.MessageUuid != null)
                    {
                        var messageUUId = response.MessageUuid[0].ToString();

                        var res = api.Message.Get
                            (
                            messageUuid: messageUUId
                            );

                        if (!string.IsNullOrEmpty(messageUUId))
                        {
                            mobileverification.UniqueId = messageUUId;
                            if (res != null)
                            {
                                mobileverification.SenderNumber = res.ToNumber != null ? res.ToNumber : string.Empty;
                                if (res.ErrorCode == "000" || string.IsNullOrEmpty(res.ErrorCode))
                                {
                                    mobileverification.OperationStatus = "Success";
                                }
                                else
                                {
                                    mobileverification.OperationStatus = "Failure";
                                }
                            }
                        }
                    }
                }
            }
            catch (PlivoRestException e)
            {
                Console.WriteLine("Exception: " + e.Message);
                mobileverification.FailureReason = e.Message;
            }
            return mobileverification;
        }

        public static bool SendPlivoMessageToAdmin(string mobileNumber, string emailAddress, string ipAddress, string status, string projectName, string ipName)
        {
            string plivoAuthId = ConfigurationManager.AppSettings["PlivoAuthId"];
            string plivoAuthToken = ConfigurationManager.AppSettings["PlivoAuthToken"];
            string plivoNumber = ConfigurationManager.AppSettings["PlivoNumber"];
            // string url = ConfigurationManager.AppSettings["ReplyToSMSURL"];
            string reciever = mobileNumber; // The number to which the message will be sent
            string plivoNumberExt = ConfigurationManager.AppSettings["PlivoTestExt"];
            string plivoExtensionNo = !string.IsNullOrEmpty(plivoNumberExt) ? (plivoNumberExt == "US" ? "1" : plivoNumberExt) : "1";

            string message = "";
            message = emailAddress + " - " + (!string.IsNullOrEmpty(status) ? char.ToLower(status[0]) + status.Substring(1) : "") + " - " + projectName + " - " + ipAddress + " " + "(" + ipName + ")";
            var api = new PlivoApi(plivoAuthId, plivoAuthToken);

            try
            {
                if (!string.IsNullOrEmpty(reciever))
                {
                    var destinationNumber = reciever.Contains("+1") ? reciever.Replace("+1", "1") : reciever.Contains("+91") ? reciever.Replace("+91", "91") : (plivoExtensionNo + reciever);
                    var response = api.Message.Create
                    (
                    src: plivoNumber,
                    //dst: new List<string> { "919677012248" },
                    dst: new List<string> { destinationNumber },
                    text: message,
                    method: "POST"
                    );

                    if (response != null && response.MessageUuid != null)
                    {
                        var messageUUId = response.MessageUuid[0].ToString();

                        var res = api.Message.Get
                            (
                            messageUuid: messageUUId
                            );

                        if (!string.IsNullOrEmpty(messageUUId))
                        {
                            if (res != null)
                            {
                                if (res.ErrorCode == "000" || string.IsNullOrEmpty(res.ErrorCode))
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
                }
            }
            catch (PlivoRestException e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            return false;
        }
    }
}
