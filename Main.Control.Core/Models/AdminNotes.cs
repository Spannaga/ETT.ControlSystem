using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Main.Control.Core.Models
{
    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum NotesStatus
    {
        NOTSTARTED = 0,
        INPROGRESS = 1,
        COMPLETED = 2,
        WAITINGFORINPUT = 3,
        DEFERRED = 4
    }

    [Serializable()]
    public class AdminNotes
    {
        [XmlAttribute()]
        public long AdminNotesId { get; set; }
        [XmlAttribute()]
        public long AdminUserId { get; set; }
        [XmlAttribute()]
        public string AdminUserName { get; set; }
        [XmlAttribute()]
        public long UserId { get; set; }
        [XmlAttribute()]
        public string UserName { get; set; }
        [XmlAttribute()]
        public string Comment { get; set; }
        [XmlAttribute()]
        public string Subject { get; set; }
        [XmlAttribute()]
        public DateTime FollowupDate { get; set; }
        [XmlAttribute()]
        public string DisplayDate { get; set; }
        [XmlAttribute()]
        public string CreateDate { get; set; }
        [XmlAttribute()]
        public DateTime CreateTime { get; set; }
        [XmlAttribute()]
        public NotesStatus NotesStatus { get; set; }
        [XmlAttribute()]
        public StatusType OperationStatus { get; set; }
        [XmlAttribute]
        public String Followdate { get; set; }

        [XmlAttribute()]
        public long LeadStatusId { get; set; }
        [XmlAttribute()]
        public string LeadStatus { get; set; }
        [XmlAttribute()]
        public long LeadStatusdetailsId { get; set; }
        [XmlAttribute()]
        public string LeadStatusdetails { get; set; }
            
    }
}
