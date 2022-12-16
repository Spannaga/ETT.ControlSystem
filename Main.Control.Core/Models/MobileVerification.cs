using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{

    public class MobileVerification
    {
        public long UserId { get; set; }
        public string MobileNumber { get; set; }
        public long MobileverificationId { get; set; }
        public string VerificationCode { get; set; }
        public string FailureReason { get; set; }
        public string AccountSId { get; set; }
        public string UniqueId { get; set; }
        public string SenderNumber { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string UserEmail { get; set; }
        public string MessageType { get; set; }
        public string ProductName { get; set; }
        public string OperationStatus { get; set; }
        public long AdminUserId { get; set; }
    }
}
