using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    [DataContract]
    public class WfVoidRefundResponse
    {
        [DataMember]
        public string TransactionReferenceId { get; set; }
        [DataMember]
        public PaymentApiStatusType OperationStatus { get; set; }
        [DataMember]
        public List<ErrorMessage> Errors { get; set; }


    }
}
