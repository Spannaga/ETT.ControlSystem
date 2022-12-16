using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Main.Control.Core.Models
{
    [Serializable()]
    public class SPLeadLog
    {
        [XmlAttribute()]
        public long AdminLeadLogId { get; set; }
        [XmlAttribute]
        public string BusinessName { get; set; }
        [XmlAttribute]
        public string StatusChange { get; set; }
        [XmlAttribute]
        public string BusinessType { get; set; }
        [XmlAttribute]
        public string Address { get; set; }
        [XmlAttribute]
        public string WebSiteAddress { get; set; }
        [XmlAttribute]
        public string City { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string State { get; set; }
        [XmlAttribute()]
        public string PhoneNumber { get; set; }
        [XmlAttribute]
        public string MainContact { get; set; }
        [XmlAttribute()]
        public string Mobile { get; set; }
        [XmlAttribute]
        public string Position { get; set; }
        [XmlAttribute]
        public string EmailAddress { get; set; }
        [XmlAttribute]
        public string LastContactedBy { get; set; }
        [XmlAttribute]
        public string PreferredMethodContact { get; set; }
        [XmlAttribute()]
        public DateTime LastContactDate { get; set; }
        [XmlAttribute]
        public DateTime LastFollowUpDate { get; set; }
        [XmlAttribute]
        public string Comment { get; set; }
        [XmlAttribute]
        public string NSupportType { get; set; }
        [XmlAttribute]
        public long AdminFollowupId { get; set; }
        [XmlAttribute()]
        public DateTime NFollowUpDate { get; set; }
        [XmlAttribute]
        public string AdminUserName { get; set; }
        [XmlAnyAttribute]
        public string Zip { get; set; }
        [XmlAttribute]
        public string FollowUpDate { get; set; }
        [XmlAttribute]
        public string FollowUpTime { get; set; }
        [XmlAttribute()]
        public string NoteDate { get; set; }
        [XmlAttribute()]
        public string NoteTime { get; set; }
        [XmlAttribute()]
        public DateTime LeadLogDate { get; set; }
        [XmlAttribute]
        public string BType { get; set; }
    }
}
