using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class PaymentReconReport
    {
        public short ProductId { get; set; }
        public int SlNo { get; set; }
        public DateTime Date { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Email { get; set; }
        public bool IsProduct { get; set; }
        public bool IsApi { get; set; }
        public bool IsCybersource { get; set; }
        public string IsStripe { get; set; }
        public string Comments { get; set; }
        public string ApiCallStatus { get; set; }
    }
    public class ReturnCountReport
    {
        public string ProductName { get; set; }
        public string FormType { get; set; }
        public string ApiTxnId { get; set; }
        public string AppTxnId { get; set; }
        public string CyberSourceTxnId { get; set; }
        public decimal TransactionAmount { get; set; }
        public DateTime CreatedTimeStamp { get; set; }
        public string Email { get; set; }
        public int PaymentReportId { get; set; }
        public string ApiCallStatus { get; set; }
    }
    public class CyberSource
    {
        public List<CyberSource> strRemittanceAmtList { get; set; }
        public string CyberSourceTxnIdRefId { get; set; }
        public decimal TransactionAmount { get; set; }
        public DateTime CreatedTimeStamp { get; set; }
    }
    public class AuthorizeResponse
    {
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<AuthorizePaymentReport> ProductBasedAmount { get; set; }
        public bool isSentAndSave { get; set; }

        public List<string> TransactionId { get; set; }
    }

    public enum SpanProductCode
    {
        ETT,
        ETF,
        TL,
        UW,
        AW,
        TSNA,
        ETE,
        EE,
        ACAXML,
        ETTACI,
        ETTACIAMEX,
        TSNAACI,
        TSNAACIAMEX,
        EEFACI,
        EEFACIAMEX,
        TBSACI,
        TBSACIAMEX,
        TBS,
        ETTWFG,
        TSNAWFG,
        TBSWFG,
        EPS,
        EEFWFG,
        UWWFG,
        EEWFG,
        ETEWFG,
        IFTA,
        TLWFG,
        AWWFG
    }
    public class AuthorizePaymentReport
    {
        public long SCAuthorizePaymentReportId { get; set; }
        public long AuthorizeCredentialId { get; set; }
        public DateTime TransactionSettlementDate { get; set; }
        public string ProductCode { get; set; }
        public decimal SettledAmount { get; set; }
        public decimal ProductsTotalMonthAmount { get; set; }
        public decimal ProductsYearTotalAmount { get; set; }
        public bool IsFileAvailable { get; set; }
        public int SequenceNo { get; set; }
        public List<PaymentReconReport> MissingPaymentTransactionId { get; set; }
    }

}
