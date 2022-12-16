using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Main.Control.Core.Models
{
    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum StatusType
    {
        Success,
        Failure
    }
    [DataContract]
    public enum PaymentApiStatusType
    {
        Failure,
        Success,
        Duplicate
    }
    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum Products
    {
        Biz = 1,
        Sales = 2,
        Tax = 3,
        NONE = 0
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum Project
    {
        UnitWise = 1,
        UnitHub = 2,
        SpanPlan = 3,
        ExpressExtension = 4,
        ExpressTruckTax = 5,
        ExpressIFTA = 6,
        ExpressTaxFilings = 7,
        TaxProductsSales = 8,
        UnitWiseSales = 9,
        UnitHubSales = 10,
        SpanPlanSales = 11,
        Express990 = 12,
        TruckLogics = 13,
        ExtensionEfileAdmin = 14,
        ExpressTruckTaxEfileAdmin = 15,
        E990EfileAdmin = 16,
        ETFEfileAdmin = 17,
        TSNAAdmin = 18,
        StayTaxExempt = 19,
        ACAEfileAdmin = 20,
        MefServices = 21,
        ToolsServices = 22,
        UWServices = 23,
        INServices = 24,
        ACAwise = 25,
        ACAwiseEfileAdmin = 26,
        ExpressTruckTaxEfileAdminMobile = 27,
        PayWow = 28,
        PayWowCompliance = 29,
        ETFEfileAdmin2016 = 30,
        ExpressEfile = 31,
        TaxBanditsPEO = 32,
        AWTools = 35,
        ETTSUPPORTADMIN = 36,
        SERVICECONTROLCENTER = 37,
        PayWowT_AND_A = 38,
        PayWowSymmetryCompliance = 39,
        ExpressPaystubGenerator = 40,
        EESUPPORTADMIN = 41,
        EPSSupportAdmin = 42,
        EXPRESSNOTIFY = 44,
        EXPRESSSUPPORTADMIN = 46,
        EXPRESSEFILEADMIN = 47,
        ExpressEFileAngular = 48,
        ExpressEFileAdmin1 = 49,
        TaxBandits2 = 50,
        ExpressPayments = 52,
        ExpressTruckTaxTestEfileAdmin = 53,
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum CampaignStatus
    {
        Draft,
        Upcoming,
        Active,
        Expired,
        Suspended
    }


    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum TechTeamStatus
    {
        TECH_TEAM_NOTIFY,
        FILE_UPLOADED,
        DISCARDED_REQUEST,
        CAMPAIGN_CREATED,

    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum MethodOfContact
    {
        Phone = 1,
        Mail = 2,
        CallBack = 3,
        LeftVM = 4,
        InvoiceSent = 5,
        NotInterested = 6,
        DoNotContact = 7
    }
    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum TypeofCall
    {
        Spoke = 1,
        VoiceMail = 2
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum CampaignType
    {
        LEADS,
        CLIENTS
    }

    public enum LeadStatusflag
    {
        Pending,
        Resolved
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum LeadTimeZone
    {
        AST, //AlaskanStandardTime
        CST, //CentralStandardTime
        EST, //EasternStandardTime
        HST, //HawaiianStandardTime
        MST, //MountainStandardTime
        PST, //PacificStandardTime
    }

    public enum LeadListStatus
    {
        All,
        Upcoming,
        Active,
        Expired
    }
    public enum FollowupFilter
    {
        All,
        Today,
        PastDue,
        Upcoming
    }
    public enum Location
    {
        CBE,
        CHE,
        USA,
        ERD
    }

    public enum BatchUploadStatus
    {
        INIT,
        IN_PROGRESS,
        SUCCESS,
    }

    [Serializable()]
    [DataContract]
    public class AdminEntityBase
    {
        [XmlAttribute()]
        public long AdminUserId { get; set; }
        [XmlIgnore()]
        public StatusType OperationStatus { get; set; }
        [XmlIgnore()]
        public string SuccessMessage { get; set; }
        [XmlIgnore()]
        public bool IsDeleted { get; set; }
        [XmlIgnore()]
        public bool IsValidUser { get; set; }
        [XmlIgnore()]
        public DateTime CreateTimeStamp { get; set; }
        [XmlIgnore()]
        public DateTime UpdateTimeStamp { get; set; }
        [XmlIgnore()]
        [DataMember]
        public MethodOfContact MethodOfContact { get; set; }
        [XmlIgnore()]
        public TypeofCall TypeofCall { get; set; }
        [XmlIgnore()]
        public LeadTimeZone LeadTimeZone { get; set; }
        [XmlIgnore()]
        public LeadListStatus LeadListStatus { get; set; }
        [XmlIgnore()]
        public Project Project { get; set; }
    }
}
