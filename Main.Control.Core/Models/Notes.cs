using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    [Serializable()]
    [DataContract]
    public class Notes :SPEntityBase
    {
        [DataMember]
        public long NotesId { get; set; }
        [DataMember]
        public string AdminUserName { get; set; }
        [DataMember]
        public string Note { get; set; }
        [DataMember]
        public int? MethodOfContact { get; set; }
        [DataMember]

        public int? NoteStatus { get; set; }
        [DataMember]
        public long? AssignedTo { get; set; }
        [DataMember]
        public string AssignedToName { get; set; }
        [DataMember]
        public DateTime? FollowUpDate { get; set; }
        [DataMember]
        public DateTime? FollowUpTime { get; set; }
        [DataMember]
        public bool IsFollowUp { get; set; }
        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string FollowUp { get; set; }
        [DataMember]
        public bool IsIncoming { get; set; }
        [DataMember]
        public string NoteSubject { get; set; }
        [DataMember]
        public bool IsCreated { get; set; }
        [DataMember]
        public DateTime? Contactedon { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public long UserProfileId { get; set; }
        [DataMember]
        public string Comments { get; set; }
        [DataMember]
        public string CreateTimeStampNew { get; set; }
        [DataMember]
        public DateTime CommentDate { get; set; }
        [DataMember]
        public string StrDate { get; set; }
        [DataMember]
        public bool IsEditedComment { get; set; }
        [DataMember]
        public string ContactName { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string UserEmailAddress { get; set; }
        [DataMember]
        public string StrTime { get; set; }
        [DataMember]
        public string ContactType { get; set; }
        [DataMember]
        public bool IsAttachment { get; set; }
        [DataMember]
        public byte[] AttachmentFile { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public long AdminUserId { get; set; }
        [DataMember]
        public string EIN { get; set; }
        [DataMember]
        public string BusinessName { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public string PhoneNumber { get; set; }
        [DataMember]
        public string ContactedDate { get; set; }
        [DataMember]
        public string MethodOfContacts { get; set; }
        [DataMember]
        public string Subject { get; set; }
        [DataMember]
        public int AffiliateId { get; set; }
        [DataMember]
        public List<Notes> commentsList { get; set; } // commentsList
        [DataMember]
        public List<UserProfileComments> UserProfileCommentsList { get; set; }
    }
}
