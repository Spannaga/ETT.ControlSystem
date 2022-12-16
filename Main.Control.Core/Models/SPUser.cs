using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Main.Control.Core.Models
{
    [Serializable()]
    public enum SPInvoiceType
    {
        TEXTBLAST,
        ENDICIA,
        DOMAIN,
        SUBSCRIPTION
    }


    [Serializable()]
    public class SPUser
    {
        [XmlAttribute()]
        public long UserId { get; set; }
        [XmlAttribute()]
        public string FirstName { get; set; }
        [XmlAttribute()]
        public string LastName { get; set; }
        [XmlAttribute()]
        public string MiddleInitial { get; set; }
        [XmlAttribute()]
        public string EmailAddress { get; set; }
        [XmlAttribute()]
        public SPAddress Address { get; set; }
        [XmlAttribute()]
        public bool IsConfirmed { get; set; }
        [XmlAttribute()]
        public SPUserStatus UserStatus { get; set; }
        [XmlAttribute()]
        public string US { get; set; }
        [XmlAttribute()]
        public bool IsDefaultsCreated { get; set; }
        [XmlAttribute()]
        public DateTime UserCreatedOn { get; set; }
        [XmlAttribute()]
        public string UserName { get; set; }
        [XmlAttribute()]
        public string HowYouFindUs { get; set; }
        [XmlAttribute()]
        public string PhoneNumber { get; set; }
        [XmlAttribute()]
        public string BrandName { get; set; }
        [XmlAttribute()]
        public string RelationShipName { get; set; }
        [XmlAttribute()]
        public long RelationShipTypeID { get; set; }
        [XmlAttribute()]

        public int PositionId { get; set; }
        [XmlAttribute()]
        public long SKUId { get; set; }
        [XmlAttribute()]
        public string SKUDescription { get; set; }
        [XmlAttribute()]
        public string SKUModules { get; set; }
        [XmlAttribute()]
        public decimal BasePrice { get; set; }
        [XmlAttribute()]
        public decimal OrderAmount { get; set; }
        [XmlAttribute()]
        public decimal ReferralAmount { get; set; }
        [XmlAttribute()]
        public decimal SubTotal { get; set; }
        [XmlAttribute()]
        public decimal CreditAmount { get; set; }
        [XmlAttribute()]
        public decimal TotalAmount { get; set; }
        [XmlAttribute()]
        public long DiscountId { get; set; }
        [XmlAttribute()]
        public string DiscountName { get; set; }
        [XmlAttribute()]
        public DateTime LastPaidDate { get; set; }
        [XmlAttribute()]
        public DateTime NextDueDate { get; set; }
        [XmlAttribute()]
        public bool IsOpen { get; set; }
        [XmlAttribute()]
        public SPPaymentFrequency PaymentFrequency { get; set; }
        [XmlAttribute()]
        public int DiscountCount { get; set; }
        [XmlAttribute()]
        public bool IsLifetime { get; set; }
        [XmlAttribute()]
        public string TimeZone { get; set; }
        [XmlAttribute()]
        public bool IsInvoiced { get; set; }
        [XmlAttribute()]
        public long OrderId { get; set; }
        [XmlAttribute()]
        public long InvoiceId { get; set; }
        [XmlAttribute()]
        public DateTime InvoiceDate { get; set; }
        [XmlAttribute()]
        public DateTime PaidDate { get; set; }
        [XmlAttribute()]
        public StatusType OperationStatus { get; set; }
        [XmlAttribute()]
        public string InvoiceHtml { get; set; }
        [XmlAttribute()]
        public SPInvoiceStatus InvoiceStatus { get; set; }
        [XmlAttribute()]
        public int BD { get; set; }//Billing Day
        [XmlAttribute()]
        public bool IsTrial { get; set; }
        [XmlAttribute()]
        public PaymentMethod PM { get; set; }//Payment Method
        [XmlAttribute()]
        public bool IAC { get; set; } //Is For Additional Cost

        [XmlAttribute()]
        public string IPAddress { get; set; }
        [XmlAttribute()]
        public DateTime LastLoginDate { get; set; }
        [XmlAttribute()]
        public string StateName { get; set; }
        [XmlAttribute()]
        public string StateCode { get; set; }
        [XmlAttribute()]
        public int FS { get; set; } //Folder Size
        [XmlAttribute()]
        public DateTime SigninDate { get; set; }
        [XmlAttribute()]
        public string Comment { get; set; }
        [XmlAttribute()]
        public DateTime CreateDate { get; set; }
        [XmlAttribute()]
        public DateTime DOB { get; set; }

        [XmlAttribute()]
        public string InvoiceType { get; set; }
        [XmlAttribute()]
        public decimal InvoiceAmount { get; set; }
        [XmlAttribute()]
        public string Response { get; set; }

        [XmlAttribute()]
        public int SubscribedTrialUsersCount { get; set; }
        [XmlAttribute()]
        public int SubscribedUsersCount { get; set; }
        [XmlAttribute()]
        public int PastSubscribedUsersCount { get; set; }
        [XmlAttribute()]
        public int InactiveSubscribedUsersCount { get; set; }
        [XmlAttribute()]
        public int DeletedSubscribedUsersCount { get; set; }
        [XmlAttribute()]
        public int TrialUsersCount { get; set; }
        [XmlAttribute()]
        public int PastTrialUsersCount { get; set; }
        [XmlAttribute()]
        public int InactiveTrialUsersCount { get; set; }
        [XmlAttribute()]
        public int DeletedTrialUsersCount { get; set; }
        [XmlAttribute()]
        public int CancelledUsersCount { get; set; }
        [XmlAttribute()]
        public int UnSubscribedUsersCount { get; set; }
        [XmlAttribute()]
        public int LifeTimeUsersCount { get; set; }
        [XmlAttribute()]
        public int AllUsersCount { get; set; }
        [XmlAttribute()]
        public string Promocode { get; set; }
        [XmlAttribute()]
        public SPSKUType SKUType { get; set; }
        [XmlAttribute()]
        public String RegisterSource { get; set; }

        [XmlAttribute()]
        public string VerficationDate { get; set; }

        [XmlAttribute()]
        public string VerficationBy { get; set; }
        [XmlAttribute()]
        public List<AdminUser> SubUserLst { get; set; }
        [XmlAttribute()]
        public string BrandUserName { get; set; }
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum SKUType
    {
        Standard = 3,
        Consultant = 1,
        Director = 2,
        ConsEss = 4,
        SoclMedia = 5,
        RoseBud = 6,
        SoclMediaMktg = 7,
        Inventory = 8,
        InventoryWebMkt = 9,
        ConsBasic = 10,
        None = 0,
        ConsPlan = 11
    }
    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum UserStatus
    {
        SIGNUP = 1,
        CONFIRMED = 2,
        SUBSCRIBED = 3,
        BETA = 4,
        UNSUBSCRIBED = 5,
        CANCELLED = 6,
        INACTIVE = 7,
        DELETED = 8,
        DUPLICATE = 9,
        DEMO = 10,
        PASTSUBSCRIBEDUSER = 11,
        INACTIVESUBSCRIBED = 12,
        DELETEDSUBSCRIBED = 13,
        TRIALUSER = 14,
        PASTTRIALUSER = 15,
        INACTIVETRIAL = 16,
        DELETEDTRIAL = 17,
        LIFETIME = 18,
        SUBSCRIBEDTRIAL = 19,
        NONE = 0
    }
}
