using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Main.Control.Core.Models
{


    public enum StaticRelationShipTypes
    {
        BRIDE = 1,
        GROOM = 2,
        FATHER_OF_BRIDE = 3,
        FATHER_OF_GROOM = 4,
        FACILITY = 5,
        WEDDING_PLANNERS = 6,
        FLORIST = 7,
        DJ = 9,
        PHOTOGRAPHERS = 10,
        VIDEOGRAPHERS = 11,
        DRIVER = 12,
        WEDDING_BAND = 13,
        BRIDAL_STORE = 14,
        BAKER = 15,
        CATERER = 16,
        OFFICIANTS = 17,
        DESIGNER = 18,
        PHOTO_BOOTH_RENTALS = 19,
        OTHER = 20,
        INITIAL_CONTACT = 22,
        EVENT_PLANNER = 23,
        CONTRACT_CONTACT = 24,
        OFFICIANT = 25,
        EQUIPMENT_SUPPLIES_RENTAL = 26,
        BARTENDERS = 27,
        MAKEUP_ARTISTS = 28,
        HAIRSTYLIST = 29,
        MANI_PEDICURE_ARTIST = 30

    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum SPInvoiceStatus
    {
        OPEN,
        PAID,
        PARTIAL_PAID,
        CANCELLED,
        VOID,
        NONE
    }


    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum SPUserStatus
    {
        REGISTERED = 1,
        TRIALSUBSCRIBED = 2,
        SUBSCRIBED = 3,
        CANCELLED = 4,
        DELETED = 5,
        ABANDONED = 6,
        LIFETIME = 7,
        DUPLICATE = 8,
        BETA = 9,
        DEMO = 10,
        UNSUBSCRIBED = 11,
        NONE = 0
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum ReceiptType
    {
        INVOICE,
        RECEIPT,
        CONTRACT
    }

    [XmlTypeAttribute()]
    public enum EmailTemplateType
    {
        USER_DEFINED,
        REFERRAL,
        FREE_FORMAT,
        DEFAULT
    }

    [XmlTypeAttribute()]
    public enum RateService
    {
        EXCELLENT = 1,
        ABOVE_AVERAGE = 2,
        AVERAGE = 3,
        BELOW_AVERAGE = 4,
        POOR = 5
    }

    [XmlTypeAttribute()]
    public enum FeedbackStatus
    {
        SENT,
        NOT_SENT,
        COMPLETED
    }

    [XmlTypeAttribute()]
    public enum PURoles
    {
        User,
        SubUser,
        Admin,
        Support,
        None
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum SPPaymentProvider
    {
        RBC,
        CITISOUTH
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum TicklerType
    {
        APPOINTMENT,
        TASK,
        EVENT,
        NONBUSINESS,
        CALL,
        NONE
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum TicklerStatus
    {
        OPEN,
        CANCEL,
        NONE
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum TicklerInstance
    {
        ONLY,
        ALL,
        FOLLOW
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum SPStatusType
    {
        Success,
        Failure
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum SPPaymentGateway
    {
        AuthorizeNet,
        Paypal,
        ProPay
    }

    [Serializable]
    public class SPEntityBase
    {
        public Guid UniqueId { get; set; }
        public long UserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateTimeStamp { get; set; }
        public DateTime UpdateTimeStamp { get; set; }
        public SPOperationStatus OperationStatus { get; set; }
        public long CountryId { get; set; }
        public long StateId { get; set; }
        public string StateCode { get; set; }
        [XmlAttribute()]
        public bool IsChecked { get; set; }
        public string TimeZone { get; set; }
        public bool IsHomeAddress { get; set; }
        public bool IsPrimary { get; set; }
        public string OtherAddressType { get; set; }
        public string AddressTypeString { get; set; }
        public bool IsOwner { get; set; }
        public DefaultLegendCategory DefaultLegendCategory { get; set; }
        public long ParentUserId { get; set; }
        public long SubUserId { get; set; }
        public string ImagePathIcon { get; set; }
    }

    //Line of Business(LOB Id)
    public enum LineOfBusiness
    {
        Photography = 1,
        Professional_DJ = 2
    }

    //Line of Business(LOB Id)
    public enum RolePrivilege
    {
        Reports_LOB_1 = 1,
        Reports_LOB_2 = 2
    }

    public enum MailTemplate
    {
        FORGOT_PASSWORD = 1,
        REGISTRATION_EMAIL_APPROVAL = 2,
        CREATE_USER_EMAIL = 3,
        NEW_PASSWORD = 4,
        THANKS_REQUEST = 5,
        REFERRAL_EMAIL = 6,
        INVOICE_EMAIL_TEMPLATE = 7,
        INVOICE_PRINT = 12,
        MAIL_TO_CONTACT_US_ADMIN = 13,
        MAIL_TO_CONTACT_US_RESPONSE = 14,
        CLIENT_FORGOT_PASSWORD = 15,
        CLIENT_NEW_PASSWORD = 16,
        USER_CANCELLATION = 21,
        CONTRACT_PRINT = 23,
        AGENDA_PRINT = 24,
        EVENT_REMINDER = 26,
        USER_INVOICE = 27,
        SUGGESTION_THANKS = 29,
        THANKS_REFERRAL_EMAIL = 31,
        VA_REGISTER_TEMPLATE_ID = 32,
        VA_MODULES_TEMPLATE_ID = 33,
        VERIFIED_CREDIT_CARD = 36,
        MOBILE_VERIFICATION = 37,
        CREDIT_CARD_CHARGED = 40,
        CREDIT_CARD_FAILED = 41,
        SUGGESTION_RESPONSE = 43,
        CUSTOMER_COMPLAINT = 45,
        DEFAULT_SERVICE = 46,
        USER_SUGGESTION_REPLY = 47,
        USER_REPLY_TO_CLIENT_SUGGESTION = 48,
        CONTRACT_ATTACHMENT = 50,
        PAYMENT_SUCCESS_EMAIL = 52,
        PAYMENT_NOT_DONE_EMAIL = 53,
        CREATE_SUB_USER_TEMPLATE_ID = 54,
        ASSIGN_EVENT_NOTIFICATION_TO_CUSTOMER = 55,
        ASSIGN_EVENT_TO_USER = 56,
        NEW_LEAD_FROM_WEBSITE = 57,
        CLIENT_PAYMENT_RECEIVED = 62,
        CLIENT_SIGNED_CONTRACT = 63
    }

    public enum SPOperationStatus
    {
        Success,
        Failure
    }

    public enum DefaultCategories
    {
        Wedding,
        Portrait
    }

    public enum LeadSource
    {
        None = 0,
        Phone = 1,
        Email = 2,
        Referrals = 3,
        Advertisement = 4,
        Website = 5,
        Other = 6
    }

    public enum DisplayType
    {
        FNLN,
        LNFN,
        LN

    }

    public enum DefaultLegendCategory
    {
        Custom,
        URL,
        Invoice,
        Quote,
        Contract,
        Know_Your_Customer,
        Feedback
    }

    public enum TemplateType
    {
        REQUEST,
        INVOICE,
        QUOTE,
        CONTRACT,
        KYCF,
        PACKAGE,
        FEEDBACK,
        CLIENT_COMPLAINT,
        GENERAL,
        REFERRAL,
        USER_DEFINED,
        PAYMENT_SUCCESS,
        NONE
    }

    public enum TemplateTypeId
    {
        PackageId = 8
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum SPSKUType
    {
        Trial = 1,
        Team = 3,
        Elite = 4,
        Enterprise = 5,
        None = 0
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum SPCreditCardType
    {
        Visa = 0,
        MasterCard = 1,
        Discover = 2,
        Amex = 3,
        Switch = 4,
        Solo = 5,
        DinersClub = 6,
        JCB = 7,
        AmericanExpress = 8,
        Unknown = 9
    }
    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum Action_On
    {
        KYE = 1,
        CONTRACT = 2,
        PRICING = 3,
        FEEDBACK = 4,
        NOTE = 5,
        EVENT = 6,
        CONTACT = 7,
        APPOINTMENT = 8,
        LOCATION = 9,
        QUOTE = 10,
        TASK = 11,
        EXPENSES = 12,
        INVOICE = 13,
        EMAIL = 14,
        USER = 15,
        PRODUCT = 16,
        PACKAGES = 17,
        PREFERENCES_BUSINESS_DETAILS = 18,
        PREFERENCES_PUBLIC_PROFILE = 19,
        PREFERENCES_LOCALIZATION = 20,
        PREFERENCES_EVENT_CATEGORY = 21,
        PREFERENCES_LEAD = 22,
        PREFERENCES_GENERAL = 23,
        PREFERENCES_PAYMENT_GATEWAYS = 24,
        INCOME_CATEGORIES = 25,
        EXPENSE_CATEGORIES = 26,
        CUSTOM_FIELDS = 27,
        EVENT_QUESTIONS = 28,
        EXTRA_SERVICES = 29,
        PRINT_TEMPLATES = 30,
        TERMS_CONDITIONS = 31,
        MY_ACCOUNT = 32,
        MANAGE_PRIVILEGES = 33,
        MANAGE_PROFESSION = 34,
        MANAGE_PROFESSIONALS = 35,
        REFERRAL = 36,
        NOTIFICATIONS = 37,
        APPOINMENTS = 38,
        LEAD = 39,
        EVENT_PREFERENCES = 40,
        EVENT_INVENTORY = 41,
        REIMBURSEMENT = 42,
        PROFEESIONAL_FEES = 43,
        RECORD_PAYMENTS = 44,
        BUSINESS_DETAILS = 45,
        BUSINESS_PUBLIC_PROFILE = 46,
        REPORTS = 47,
        CLINT_INFO_FILES = 48,
        VENDOR = 49,
        ASSIGN_PROFESSIONALS = 50,
        RELATIONSHIP = 51,
        INCOME = 52,
        MARKETTING = 53,
        RESOURCE_LIBRARY = 54,
        WEBSITE_INTEGRATION = 55,
        PUBLIC_WEBSITE = 56,
        WEBSITE = 57,
        CLIENT_PORTAL = 58,
        COMMUNICATION = 59,
        SETTINGS = 60,
        MERGE_TAGS = 61,
        KYE_QUESTIONS = 62,
        PERSONAL_TIME_OFF = 63
    }
    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum Action_Type
    {
        ADD = 1,
        EDIT = 2,
        DELETE = 3,
        SENT = 4,
        RESPOND = 5,
        SIGNED = 6,
        REJECTED = 7,
        VIEWED = 8,
        PARTIAL_PAID = 9,
        PAID = 10,
        FINAL_PAYMENT = 11,
        SCHEDULED = 12,
        AMEND = 13,
        AWAITING_CLIENT_SIGNATURE = 14,
        AWAITING_USER_SIGNATURE = 15,
        INVOICE_PENDING = 16,
        UPDATED_BY_CLIENT = 17,
        CONTRACT_SIGNED_CUSTOMER = 18,
        CONTRACT_SIGNED_USER = 19,
        DEPOSIT = 20,
        KNOW_YOUR_CUSTOMER = 21,
        AWAITING_CUSTOMER_SIGNATURE = 22,
        INVOICE_CHECK = 23,
        INVOICE_PHONE = 24,
        LOG_IN = 25,
        LOG_OUT = 26,
        ASSIGN = 27,
        REQUEST = 28,
        IMPORT = 29,
        VOID = 30
    }
    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum Action_By
    {
        USER = 1,
        CLIENT = 2
    }

    [SerializableAttribute()]
    public enum BannerType
    {
        NONE,
        BANNER,
        LOGO

    }

    [SerializableAttribute()]
    public enum CommType
    {
        CLIENT,
        USER
    }

    [SerializableAttribute()]
    public enum EventCommunicationCategory
    {
        PACKAGE_PRICING,
        CUSTOM_QUOTE,
        KNOW_YOUR_EVENT,
        CONTRACT,
        INVOICE
    }
    [SerializableAttribute()]
    public enum UserRoles
    {
        Administrator = 1,
        Office_Manager = 2,
        Office_Assistant = 3,
        Custom_Role_A = 4,
        Custom_Role_B = 5,
        No_Login = 6,
        Website_Assistant = 7,
        Professional = 9
    }

    [SerializableAttribute()]
    public enum PagePrivilege
    {

        MyAccount_Billing = 1,
        Manage_Users = 2,
        Setting_View = 3,
        Setting_Edit = 4,
        Dashboard_Activity_All = 5,
        Dashboard_Activity_User = 6,
        Dashboard_Event_All = 7,
        Dashboard_Event_User = 8,
        Financial_Reports = 9,
        Non_Financial_Reports = 10,
        Import_Leads = 11,
        Create_Lead = 12,
        Incoming_Lead = 13,
        Inactive_Lead = 14,
        Client_Portal = 15,
        Public_Portal = 16,
        Integration = 17,
        Import_Contacts = 20,
        Create_Contacts = 21,
        Non_Event_Expenses = 22,
        Event_Expenses = 23,
        Income = 24,
        Pending_Payments = 25,
        Calendar = 26,
        User_Calendar = 27,
        Manage_Activities = 28,
        Notifications = 29,
        Booked_Events = 30,
        User_Booked_Events = 31,
        Sign_Contract = 32,
        Cancel_Event = 33,
        Amend_Contract = 34,
        Cancelled_Events = 35,
        Past_Events = 36,
        Active_Lead = 37,
        Calendar_User = 40,
        Manage_Activities_User = 41,
        Notifications_User = 42,
        Active_Lead_User = 43,
        Event_Expenses_User = 44,
        Pending_Payments_User = 45,
        Cancelled_Events_User = 46,
        Past_Events_User = 47,
        Library = 48,
        Email_Notifications = 49

    }

    [SerializableAttribute()]
    public enum PagePrivilegeCategory
    {
        My_Account = 1,
        Settings = 2,
        Home = 3,
        Leads = 4,
        Website = 5,
        Address_Book = 6,
        Accounts = 7,
        Activities = 8,
        Events = 9,

    }
    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum SPPaymentMethod
    {
        CreditCard,
        BankDraft
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum SPInvoiceSource
    {
        CONTRACT,
        DIRECT,
        NONE
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum SPPaymentFrequency
    {
        Monthly = 1,
        Quarterly = 3,
        HalfYearly = 6,
        Yearly = 12,
        None = 0
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum SPDiscountType
    {
        MONTH,
        AMOUNT,
        PERCENTAGE,
        DAYS
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum PaymentPlanType
    {
        ADVANCE_PAYMENT,
        FINAL_PAYMENT,
        EXPENSE_REIMBURSEMENT,
        OTHER_PAYMENT,
        CREDIT_AMOUNT,
        NONE
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum PaymentStatus
    {
        PAID,
        FAILURE,
        VOID,
        NONE
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum BrandAddressType
    {
        Legal,
        Mailing
    }

    public enum SPUserNotificationType
    {
        USER,
        ADMIN
    }

    public enum InitialPaymentName
    {
        DEPOSIT,
        RETAINER,
        NONE
    }

    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum PaymentMethod
    {
        CreditCard,
        BankDraft
    }
    [SerializableAttribute()]
    [XmlTypeAttribute()]
    public enum PaymentProcessor
    {
        AUTHORIZENET,
        STRIPE,
        PAYPAL,
        ACI
    }
}
