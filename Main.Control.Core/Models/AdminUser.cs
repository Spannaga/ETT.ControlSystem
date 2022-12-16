using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Main.Control.Core.Models
{
    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum AdminRoleType
    {
        Administrator = 1,
        Manager = 2,
        Team = 3,
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum AdminLeadType
    {
        Unassigned = 0,
        Lead = 1,
        Interested = 2,
        InNegotiation = 3,
        Client = 4,
        NotInterested = 5,
        Inactive = 6
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum SEBizProjects
    {
        UNITWISE,
        UNITHUB,
        SPANPLAN
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum VerificationCodeType
    {
        SMS = 1,
        EMAIL = 2,
        AUTHENTICATOR = 3
    }


    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum PaymentEmailStatus
    {
        NONE,
        CREATED,
        MAILSENT,
        MAILERROR,
        PAYMENTFAILED,
        PAYMENTSUCCESS
    }
    [Serializable()]
    public class AdminUser : AdminEntityBase
    {
        [XmlAttribute()]
        public string AdminSalt { get; set; }
        [XmlAttribute()]
        public string AdminUserName { get; set; }
        [XmlAttribute()]
        public string AdminPassword { get; set; }
        [XmlAttribute]
        public string RetypePassword { get; set; }
        [XmlAttribute()]
        public string AdminFirstName { get; set; }
        [XmlAttribute()]
        public string AdminLastName { get; set; }
        [XmlAttribute()]
        public string AdminEmailAddress { get; set; }
        [XmlAttribute()]
        public string AdminRoles { get; set; }
        [XmlAttribute()]
        public string ProjectType { get; set; }
        [XmlAttribute()]
        public bool IsAdmin { get; set; }
        [XmlAttribute()]
        public long AdminRolesId { get; set; }


        [XmlAttribute()]
        public string UWModules { get; set; }
        [XmlAttribute()]
        public string AdminName = "AdminName";
        [XmlAttribute()]
        public string AdminDisplayName = "AdminName";
        [XmlAttribute()]
        public long AdminRoleId { get; set; }
        [XmlAttribute()]
        public string AdminSKUType { get; set; }
        [XmlAttribute()]
        public bool Is_Existing { get; set; }
        [XmlAttribute()]
        public long CreatedUserId { get; set; }
        [XmlAttribute()]
        public string CreatedUserName { get; set; }
        [XmlAttribute()]
        public string UpdatedUserName { get; set; }

        [XmlAttribute()]
        public bool IsUnitwise { get; set; }
        [XmlAttribute()]
        public bool IsTax { get; set; }
        [XmlAttribute()]
        public bool IsUnitHub { get; set; }
        [XmlAttribute()]
        public bool IsSpanPlan { get; set; }
        [XmlAttribute()]
        public bool IsAdministrator { get; set; }
        [XmlAttribute()]
        public bool IsApproved { get; set; }
        [XmlAttribute()]
        public bool NotifyUser { get; set; }
        [XmlAttribute()]
        public string UserName { get; set; }
        [XmlAttribute()]
        public string PhoneNumber { get; set; }
        [XmlAttribute]
        public string EmailAddress { get; set; }
        [XmlAttribute]
        public string FollowupMessage { get; set; }
        [XmlAttribute()]
        public DateTime FollowUpDate { get; set; }
        [XmlAttribute()]
        public long UserId { get; set; }
        [XmlAttribute()]
        public string FirstName { get; set; }
        [XmlAttribute()]
        public string LastName { get; set; }

        [XmlAttribute()]
        public bool IsDefaultsCreated { get; set; }
        [XmlAttribute()]
        public DateTime UserCreatedOn { get; set; }
        [XmlAttribute()]
        public long RoleId { get; set; }
        [XmlAttribute()]
        public string RoleName { get; set; }
        [XmlAttribute()]
        public string BrandUserName { get; set; }
        [XmlAttribute()]
        public string SupportType { get; set; }
        [XmlAttribute()]
        public int Starrating { get; set; }
        [XmlAttribute()]
        public long LeadLogId { get; set; }
        [XmlAttribute()]
        public long LeadLogFollowupId { get; set; }

        public List<AdminUserRole> AdminUserRoleList { get; set; }
        public List<AdminProject> AdminProjectList { get; set; }

        [XmlAttribute()]
        public bool IsNewUser { get; set; }

        [XmlAttribute()]
        public UserStatus UserStatus { get; set; }
        [XmlAttribute()]
        public List<string> IpAddress { get; set; }

        [XmlAttribute()]
        public int TotalAssignedCount { get; set; }

        public string AdminLocation { get; set; }
        [XmlAttribute()]
        public string VerificationCodType { get; set; }
        [XmlAttribute()]
        public string AlternateAdminEmailAddress { get; set; }
        [XmlAttribute()]
        public string VerificationCode { get; set; }
        [XmlAttribute()]
        public bool? IsEnabledAuthenticator { get; set; }

    }

    public enum PaymentActivityType
    {
        ADMIN,
        USER
    }
}
