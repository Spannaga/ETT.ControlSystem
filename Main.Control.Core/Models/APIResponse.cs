using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class APIResponse
    {
        public int CreditCardId { get; set; }
        public string CardNumber { get; set; }
        public string CardType { get; set; }
        public string SecurityCode { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string CardExpiry { get; set; }
        public string NameOnCard { get; set; }
        public object EmailAddress { get; set; }
        public string Address1 { get; set; }
        public object Address2 { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public object StateName { get; set; }
        public int CountryId { get; set; }
        public object CountryName { get; set; }
        public string Zip { get; set; }
        public object PhoneNumber { get; set; }
        public object UserId { get; set; }
        public string TransactionId { get; set; }
        public int PaymentProfileId { get; set; }
        public int CustomerProfileId { get; set; }
        public float ChargeAmount { get; set; }
        public object IPAddress { get; set; }
        public object ResponseXML { get; set; }
        public bool IsPaymentDue { get; set; }
        public object AuthCode { get; set; }
        public bool IsProcessed { get; set; }
        public DateTime PaidTime { get; set; }
        public object PaymentMethod { get; set; }
        public int OperationStatus { get; set; }
        public object SiteName { get; set; }
        public string ProductCode { get; set; }
        public Errormessage[] ErrorMessages { get; set; }
        public string PaymentProcessor { get; set; }
        public int PaymentProcessorId { get; set; }
        public object SecretKey { get; set; }
        public object AccessKey { get; set; }
        public bool IsVoided { get; set; }
        public bool IsPaymentProcessorActive { get; set; }
        public object Description { get; set; }
        public object CCInvoiceNumber { get; set; }
        public string AuthKey { get; set; }
        public bool IsCCAuthorize { get; set; }
        public string UserId_Guid { get; set; }
        public object AciFundingToken { get; set; }
        public object AciConfirmationNo { get; set; }
        public object AciAuthCode { get; set; }
        public object AciTraceCode { get; set; }
        public string AciBillingAccountNo { get; set; }
        public object StateCode { get; set; }
        public string CardTypeStr { get; set; }
        public bool IsGo2290 { get; set; }
        public object WfAuthCode { get; set; }
        public string WfSubscriptionId { get; set; }
        public object OrderReferenceNo { get; set; }
        public object MaskedCardNo { get; set; }
        public Gatewayerror[] GatewayErrors { get; set; }
        public object IsoCountryCode { get; set; }
    }

    public class Errormessage
    {
        public string ErrorCode { get; set; }
        public string LongMessage { get; set; }
        public int SeverityCode { get; set; }
        public object ShortMessage { get; set; }
    }

    public class Gatewayerror
    {
        public string ErrorCode { get; set; }
        public string LongMessage { get; set; }
        public int SeverityCode { get; set; }
        public object ShortMessage { get; set; }
    }
}
