using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class TransactionReportDetails
    {
        public short ProductId { get; set; }
        public string TransactionReferenceId { get; set; }
        public string EmailAddress { get; set; }
        public decimal OrderAmount { get; set; }
        public short PaymentProcessorType { get; set; }
        public string PaymentApprovalCode { get; set; }
        public string ApiCallStatus { get; set; }
        public string ResponseS3FilePath { get; set; }
        public string TransactionLogText { get; set; }
        public string UrlPath { get; set; }
        public DateTime PaymentDate { get; set; }
        public RequestResponseLogDetails RequestResponseLogDetails { get; set; }
        public Gatewayerror[] GateWayErrorsList { get; set; }
        public bool IsRefundOrVoid { get; set; }
        public string ProductCode { get; set; }


    }
}
