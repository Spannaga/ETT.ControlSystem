using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class UserPayments
    {
        public Guid UserPaymentId { get; set; }
        public int Projectid { get; set; }
        public string PaymentStatus { get; set; }
        public string CardNo { get; set; }
        public string NameOnCard { get; set; }
        public string PaymentProcessor { get; set; }
        public string PaymentApprovalCode { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public decimal PaymentAmount { get; set; }
        public string BusinessName { get; set; }
        public string PhoneNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public int? StateId { get; set; }
        public long? CountryId { get; set; }
        public string PostalZipCode { get; set; }
        public string MailBodyS3Path { get; set; }
        public string MessageId { get; set; }
        public DateTime? MailSentTime { get; set; }
        public DateTime? PaidTime { get; set; }
        public List<UserPaymentLog> UserPaymentLogs { get; set; }
        public short ExpiryMonth { get; set; }
        public short ExpiryYear { get; set; }
        public string CardExpiry { get; set; }
        public string SecurityCode { get; set; }
        public List<UserPaymentDetail> UserPaymentDetails { get; set; }
        public string OrderDescription { get; set; }
        public List<AdminProject> Projects { get; set; }
        public List<State> States { get; set; }
        public List<Country> Countries { get; set; }
        public string MailHtml { get; set; }
        public string MailSubject { get; set; }
        public string SpanLibrConnStr { get; set; }
        public string FailureMsg { get; set; }
        public string CardType { get; set; }
        public StatusType OperationStatus { get; set; }
        public bool IsProcessed { get; set; }
        public List<ErrorMessage> ErrorMessages { get; set; }
        public string ErrorMessage { get; set; }
        public string ProductCode { get; set; }
        public string SecretKey { get; set; }
        public string AccessKey { get; set; }
        public string InvoiceNo { get; set; }
        public string ReceiptS3Path { get; set; }
        public string ProductUserId { get; set; }
    }
}
