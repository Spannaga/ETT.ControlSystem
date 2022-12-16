using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class TransactionReport
    {
        public short ProductId { get; set; }
        public int PaymentProcessorid { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string  ConnectionString { get; set; }
        public string TransactionType { get; set; }
    }
}
