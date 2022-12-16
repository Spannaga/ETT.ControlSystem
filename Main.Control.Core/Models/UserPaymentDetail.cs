using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class UserPaymentDetail
    {
        public string ServiceName { get; set; }
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }
}
