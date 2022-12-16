using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    [DataContract]
    public class EmailDetailAPI
    {
        [DataMember]
        public string FromAddress { get; set; }
        [DataMember]
        public List<string> ToAddress { get; set; }
        [DataMember]
        public List<string> CcAddress { get; set; }
        [DataMember]
        public List<string> BccAddress { get; set; }
        [DataMember]
        public string ReplyToAddress { get; set; }
        [DataMember]
        public string Subject { get; set; }
        [DataMember]
        public string MailBodyS3Path { get; set; }
        [DataMember]
        public string MailStatus { get; set; }
        [DataMember]
        public bool IsAttachment { get; set; }
        [DataMember]
        public string MessageId { get; set; }
        [DataMember]
        public string FailureMessage { get; set; }
        [DataMember]
        public string ProductCode { get; set; }
        [DataMember]
        public List<EmailDetailAttachment> Attachments { get; set; }
    }

    [DataContract]
    public class EmailDetailAttachment
    {
        [DataMember]
        public string AttachmentName { get; set; }
        [DataMember]
        public string AttachmentS3Path { get; set; }
    }
}
