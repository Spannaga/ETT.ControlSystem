using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class PaymentRefundLog
    {
        public long RefundId { get; set; }
        public short SpanProductId { get; set; }
        public short PaymentProcessorId { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public string Comments { get; set; }
        public string EmailAddress { get; set; }
        public DateTime PaidDate { get; set; }
        public DateTime VoidRefundDate { get; set; }
        public string AdminUserName { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TransactionReferenceId { get; set; }
        public string ChargeBackType { get; set; }
        public bool IsApiRefund { get; set; }
    }

}
