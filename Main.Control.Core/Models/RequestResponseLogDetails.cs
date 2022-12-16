using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class RequestResponseLogDetails
    {
        public string ApiRequest { get; set; }
        public string AciFundingTokenRequest { get; set; }
        public string AciFundingTokenResponse { get; set; }
        public string AciMakePaymentRequest { get; set; }
        public string AciMakePaymentResponse { get; set; }
        public string AuthorizeMakePaymentRequest { get; set; }
        public string AuthorizeMakePaymentResponse { get; set; }
        public string StripeTokenRequest { get; set; }
        public string StripeTokenResponse { get; set; }
        public string StripeMakePaymentRequest { get; set; }
        public string StripeMakePaymentResponse { get; set; }
        public string ApiResponse { get; set; }
        public string WfPaymentRequest { get; set; }
        public string WfPaymentResponse { get; set; }
        public string TransactionReferenceId { get; set; }
        public string PaymentApprovalCode { get; set; }

        public string ACIMakePaymentRequest { get; set; }
        public string ACIMakePaymentResponse { get; set; }


    }
}
