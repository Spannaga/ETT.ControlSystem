using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Main.Control.Core.Models
{

    [Serializable()]
    public class LeadLog
    {
        [XmlAttribute()]
        public string LeadStatus { get; set; }
        [XmlAttribute()]
        public long LeadStatusId { get; set; }
        [XmlAttribute()]
        public long LeadLogId { get; set; }
        [XmlAttribute]
        public string BusinessName { get; set; }
        [XmlAttribute]
        public string BusinessType { get; set; }
        [XmlAttribute]
        public string TypeOfUser { get; set; }
        [XmlAttribute]
        public string ContactName { get; set; }
        [XmlAttribute]
        public string EmailAddress { get; set; }
        [XmlAttribute()]
        public string PhoneNumber { get; set; }
        [XmlAttribute()]
        public string Address { get; set; }
        [XmlAttribute]
        public string WebSiteAddress { get; set; }
        [XmlAttribute]
        public string FaceBook { get; set; }
        [XmlAttribute()]
        public string Twitter { get; set; }
        [XmlAttribute]
        public string PreferredMethodContact { get; set; }
        [XmlAttribute]
        public string StatusChange { get; set; }
        [XmlAttribute()]
        public DateTime FollowUpDate { get; set; }
        [XmlAttribute()]
        public DateTime? LastNoteAdded { get; set; }
        [XmlAttribute()]
        public DateTime FFollowUpDate { get; set; }
        public string ProjectType { get; set; }
        [XmlAttribute]
        public string State { get; set; }
        [XmlAttribute()]
        public List<State> StateList { get; set; }
        [XmlAttribute]
        public long StateId { get; set; }
        [XmlAttribute]
        public string Country { get; set; }
        [XmlAttribute]
        public string FollowUpMessage { get; set; }
        [XmlAttribute()]
        public DateTime LogDate { get; set; }
        [XmlAttribute()]
        public DateTime LogTime { get; set; }
        [XmlAttribute]
        public string LeadSource { get; set; }
        [XmlAttribute]
        public string LeadDeletedBy { get; set; }
        [XmlAttribute()]
        public DateTime LeadLogDate { get; set; }
        [XmlAttribute()]
        public String SLeadLogDate { get; set; }
        [XmlAttribute]
        public bool IsDoNotFollowUp { get; set; }
        [XmlAttribute()]
        public bool IsDoNotFFollowUp { get; set; }
        [XmlAttribute]
        public string Comment { get; set; }
        [XmlAttribute]
        public string NSupportType { get; set; }
        [XmlAttribute()]
        public DateTime NFollowUpDate { get; set; }
        [XmlAttribute]
        public long AdminFollowupId { get; set; }
        [XmlAttribute]
        public DateTime Createdon { get; set; }
        [XmlAttribute]
        public string Screated { get; set; }
        [XmlAttribute]
        public List<LeadLog> LeadLogFollowIdList { get; set; }
        [XmlAttribute()]
        public long LeadLogFollowId { get; set; }
        [XmlAttribute]
        public DateTime LastFollowUp { get; set; }
        [XmlAttribute]
        public string LastFollowUpDate { get; set; }
        [XmlAttribute()]
        public String SFollowUpDate { get; set; }
        [XmlAttribute()]
        public string NLeadStatus { get; set; }
        [XmlAttribute()]
        public string GeneralNotes { get; set; }
        [XmlAttribute()]
        public string NoteDate { get; set; }
        [XmlAttribute()]
        public string NoteTime { get; set; }
        [XmlAttribute()]
        public int NoofTrucksETT { get; set; }
        [XmlAttribute()]
        public int NoofOwnerOperatorsETT { get; set; }
        [XmlAttribute()]
        public string FeatureLookingfor { get; set; }
        [XmlAttribute()]
        public int NoofReturnsFiledperyear { get; set; }
        [XmlAttribute()]
        public string InterestedForms { get; set; }
        [XmlAttribute()]
        public bool IsETF { get; set; }
        [XmlAttribute()]
        public bool IsETT { get; set; }
        [XmlAttribute()]
        public bool IsIFTA { get; set; }
        [XmlAttribute()]
        public string InterestedFormsEtf { get; set; }
        [XmlAttribute()]
        public int NoofContractors { get; set; }
        [XmlAttribute()]
        public int NoofEmployees { get; set; }
        [XmlAttribute()]
        public string FeatureLookingforEtf { get; set; }
        [XmlAttribute()]
        public int NoofTrucksIfta { get; set; }
        [XmlAttribute()]
        public int NoofOwnerOperatorsIfta { get; set; }
        [XmlAttribute()]
        public string FeatureLookingforIfta { get; set; }
        [XmlAttribute()]
        public string InterestedFormsIfta { get; set; }
        [XmlAttribute()]
        public string Status { get; set; }
        [XmlAttribute()]
        public long AdminUserId { get; set; }
        [XmlAttribute()]
        public string AdminUserName { get; set; }
        [XmlAttribute()]
        public string RoleType { get; set; }
        [XmlAttribute()]
        public string OthersEtt { get; set; }
        [XmlAttribute()]
        public string OthersEtf { get; set; }
        [XmlAttribute()]
        public string OthersIfta { get; set; }
        [XmlAttribute()]
        public bool NIsDoNotFollowUp { get; set; }
        [XmlAttribute()]
        public string OtherBusinessType { get; set; }
        [XmlAttribute()]
        public string OthersLeadSource { get; set; }
        [XmlAttribute()]
        public DateTime NUpdatedDate { get; set; }
        [XmlAttribute()]
        public string City { get; set; }
        [XmlAttribute()]
        public string ZipCode { get; set; }
        [XmlAttribute()]
        public string EIN { get; set; }
        [XmlAttribute()]
        public string DBAName { get; set; }
        [XmlAttribute()]
        public long LeadLogStatusSubcategoryId { get; set; }
        [XmlAttribute()]
        public long LeadLogStatusId { get; set; }
        [XmlAttribute()]
        public string LeadLogStatusbcategory { get; set; }
        [XmlAttribute()]
        public string LeadStatusDetail { get; set; }
        [XmlAttribute]
        public long SalesPersonId { get; set; }
        [XmlAttribute]
        public bool IsAdmin { get; set; }
        [XmlAttribute]
        public bool Unassign { get; set; }
        [XmlAttribute]
        public long PositionId { get; set; }
        [XmlAttribute]
        public string Position { get; set; }
        [XmlAttribute()]
        public string Name { get; set; }
        [XmlAttribute]
        public string BusinessNames { get; set; }
        [XmlAttribute()]
        public long AdminLeadLogId { get; set; }
        [XmlAttribute()]
        public string LastContactedBy { get; set; }
        [XmlAttribute]
        public string LastContactedDate { get; set; }
    }
}
