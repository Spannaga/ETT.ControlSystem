using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Xml.Serialization;

namespace Main.Control.Core.Models
{
    [Serializable()]
    public class MailDetails
    {
        [XmlAttribute()]
        public long MailTemplateId { get; set; }

        [XmlAttribute()]
        public string MailTemplateName { get; set; }

        [XmlAttribute()]
        public long UserId { get; set; }

        [XmlAttribute()]
        public string UserName { get; set; }

        [XmlAttribute()]
        public string MailSubject { get; set; }

        [XmlAttribute()]
        public string MailBody { get; set; }

        [XmlAttribute()]
        public string MailFromAddress { get; set; }
        [XmlAttribute()]
        public string MailReplyTo { get; set; }

        [XmlAttribute()]
        public bool IsBCCRequired { get; set; }

        [XmlAttribute()]
        public string MailToAddress { get; set; }
        [XmlAttribute()]
        public string MailBCCAddress { get; set; }

        [XmlAttribute()]
        public string MailHost { get; set; }

        [XmlAttribute()]
        public List<Attachment> Attachments { get; set; }

        [XmlAttribute()]
        public byte[] Attachment { get; set; }

        [XmlAttribute()]
        public string AttachmentName { get; set; }
        public bool IsAddBCCMail { get; set; }

        //Remainder  Comment & Author Name
        public string Body { get; set; }
        public string Name { get; set; }
    }
}
