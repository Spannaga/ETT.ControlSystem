using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    [DataContract]
    public class WfVoidRequest
    {
        [DataMember]
        public string ProductCode { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string TransactionReferenceId { get; set; }
        [DataMember]
        public string Comments { get; set; }
        [DataMember]
        public decimal VoidAmount { get; set; }
        [DataMember]
        public DateTime PaidDate { get; set; }
        [DataMember]
        public string AdminUserName { get; set; }
    }
}
