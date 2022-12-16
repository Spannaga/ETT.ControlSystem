using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    [DataContract]
    [Serializable]
    public class CreditCardAPI
    {
        //public string ExpiryMonth { get; set; }

        //public string ExpiryYear { get; set; }

        [DataMember()]
        public long CreditCardId { get; set; }

        [DataMember()]
        public string CardNumber { get; set; }

        [DataMember()]
        public string CardType { get; set; }

        [DataMember()]
        public string SecurityCode { get; set; }

        [DataMember()]
        public int ExpiryMonth { get; set; }

        [DataMember()]
        public int ExpiryYear { get; set; }

        [DataMember()]
        public string CardExpiry { get; set; }

        [DataMember()]
        public string NameOnCard { get; set; }

        [DataMember()]
        public string EmailAddress { get; set; }

        [DataMember()]
        public string Address1 { get; set; }

        [DataMember()]
        public string Address2 { get; set; }

        [DataMember()]
        public string City { get; set; }

        [DataMember()]
        public short StateId { get; set; }

        [DataMember()]
        public string StateName { get; set; }

        [DataMember()]
        public short CountryId { get; set; }

        [DataMember()]
        public string CountryName { get; set; }

        [DataMember()]
        public string Zip { get; set; }

        [DataMember()]
        public string PhoneNumber { get; set; }

        [DataMember()]
        public Guid UserId { get; set; }

        [DataMember()]
        public string TransactionId { get; set; }

        [DataMember()]
        public long PaymentProfileId { get; set; }

        [DataMember()]
        public long CustomerProfileId { get; set; }

        [DataMember()]
        public decimal ChargeAmount { get; set; }

        [DataMember()]
        public string IPAddress { get; set; }

        [DataMember()]
        public string ResponseXML { get; set; }

        [DataMember()]
        public bool IsPaymentDue { get; set; }

        [DataMember()]
        public string AuthCode { get; set; }

        [DataMember()]
        public bool IsProcessed { get; set; }

        [DataMember()]
        public DateTime PaidTime { get; set; }

        [DataMember()]
        public string PaymentMethod { get; set; }

        [DataMember()]
        public PaymentApiStatusType OperationStatus { get; set; }

        [DataMember()]
        public string SiteName { get; set; }

        [DataMember()]
        public string ProductCode { get; set; }

        [DataMember()]
        public List<ErrorMessage> ErrorMessages { get; set; }

        [DataMember]
        public string PaymentProcessor { get; set; }

        [DataMember()]
        public short PaymentProcessorId { get; set; }

        [DataMember()]
        public string SecretKey { get; set; }

        [DataMember()]
        public string AccessKey { get; set; }

        [DataMember()]
        public bool IsVoided { get; set; }

        [DataMember()]
        public bool IsPaymentProcessorActive { get; set; }

        [DataMember()]
        public string Description { get; set; }

        [DataMember()]
        public string CCInvoiceNumber { get; set; }

        [DataMember()]
        public string AuthKey { get; set; }

        [DataMember()]
        public bool IsCCAuthorize { get; set; }

        public string ErrorMessage { get; set; }
        [DataMember()]
        public string AciBillingAccountNo { get; set; }
        [DataMember()]
        public string AciFundingToken { get; set; }
        [DataMember()]
        public string AciConfirmationNo { get; set; }
        [DataMember()]
        public string CardTypeStr { get; set; }
        [DataMember()]
        public string StateCode { get; set; }
        [DataMember()]
        public string WfAuthCode { get; set; }
        [DataMember()]
        public string WfSubscriptionId { get; set; }
    }

    [Serializable]
    public class ErrorMessage
    {
        public string ErrorCode;
        public string LongMessage;     
    }

}
