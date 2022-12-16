using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Main.Control.Service.Utilities
{
    [Serializable]
    [Flags]
    public enum PURoles
    {
        User,
        SubUser,
        Administrator,
        Director,
        Support,
        Supervisor,
        None
    }

    public class Constants
    {
        public const string True = "True";
        public const string False = "False";
        public const string ReturnUrl = "ReturnUrl";
        public const string controller = "controller";
        public const string action = "action";
        public const string UWSiteURL = "UWSiteURL";
        public const string SPSiteURL = "SPSiteURL";
        public const string UHSiteURL = "UHSiteURL";
        public const string Administrator = "Administrator";
        public const string Director = "Director";
        public const string Supersupport = "Supersupport";
        public const string Support = "Support";
        public const string Sales = "Sales";
        public const string Comma = ",";
        public const string IsLive = "IsLive";
        public const string Is_AWS_ELB = "Is_AWS_ELB";
        public const string UWDonotReply = "UWDoNotReply";
        public const string UHDonotReply = "UHDoNotReply";
        public const string UWSiteName = "UWSiteName";
        public const string UHSiteName = "UHSiteName";
        public const string UWSupportPhone = "UWSupportPhone";
        public const string UHSupportPhone = "UHSupportPhone";
        public const string IsSMTP = "IsSMTP";
        public const string UWMailFromAddress = "UWMailFromAddress";
        public const string UHMailFromAddress = "UHMailFromAddress";
        public const string UWSupportEmail = "UWSupportEmail";
        public const string UHSupportEmail = "UHSupportEmail";
        public const string AttachmentFilePath = "AttachmentFilePath";
        public const string EventLogSource = "EventLogSource";
        public const string ASPPdfKey = "ASPPdfKey";
        public const string TempDownloadsPath = "TempDownloadsPath";
        public const string UserDownloadsPath = "UserDownloadsPath";
        public const string CurrentYear = "@@CurYear";
        public const string EmailAd = "@@EmailAd";
        public const string MailSiteName = "@@sitename";
        public const string MailSupportEmail = "@@supportemail";
        public const string MailSupportPhone = "@@supportphone";
        public const string MailSiteURL = "@@siteurl";
        public const string IsAWS_SES_Email = "IsAWS_SES_Email";
        public const string AWSAccessKey = "AWSAccessKey";
        public const string AWSSecretKey = "AWSSecretKey";
        public const string MailHost = "MailHost";
        public const string MailFromAddress = "MailFromAddress";
        public const string SupportEmail = "SupportEmail";
        public const string SupportPhoneNumber = "SupportPhone";
        public const string SiteName = "SiteName";
        public const string MailBCCIds = "MailBCCIds";
        public const string IsMailBCC = "IsMailBCC";
        public const string SiteURL = "SiteURL";
        public const string EmailAdKey = "EmailAd";
        public const string TextAlertPrice = "TextAlertPrice";
        public const string FaxAlertPrice = "FaxAlertPrice";
        public const string AdminEmail = "AdminEmail";
        public const string PasswordVista = "PasswordVista";
        public const string MainControlURL = "MainControlURL";
        public const string Back = "Back";
        public const string BulkUploadCount = "BulkUploadCount";
        // Support Users Assigned Field

        public const string T_Name = "Name";
        public const string T_Email_Address = "Email Address";
        public const string T_Business_Name = "Business Name";
        public const string T_EIN = "EIN";
        public const string T_Phone_Number = "Phone Number";
        public const string T_Address = "Address";
        public const string T_Signed_up_On = "Signed-up On?";
        public const string T_User_Type = "User Type?";
        public const string T_No_Of_Trucks = "No Of Trucks";
        public const string T_No_Of_Trucks_with_Hash = "No# Of Trucks";
        public const string T_Last_Filed_On = "Last Filed On";
        public const string T_Return_Number = "Return Number";
        //


        // MAIL TEMPLATE ID
        public const int CampaignCreatetoSupportUser = 1;
        public const int RequesttoTechTeamforFileUpload = 2;
        public const int DiscaredCampaignRequest = 3;
        public const int FileUploadedTechTeam = 4;
        public const int FileUploadBatchProcess = 5;
        public const int FileUploadCompleted = 6;
        public const int ManagerCampaignCreatetoSupportUser = 7;
        //

        // MAIL SEND TO CONSTANTS

        public const string CampaignName = "@@CampaignName";
        public const string NoOfLeadsAssigned = "@@NoOfLeadsAssigned";
        public const string Goals = "@@Goals";
        public const string CampaignPeroid = "@@CampaignPeroid";
        public const string SupportUserName = "@@SupportUserName";
        public const string AdminUserName = "@@AdminUserName";
        public const string redirectLink = "@@redirectLink";
        public const string ProductName = "@@ProductName";
        public const string DemographicInformation = "@@DemographicInformation";
        public const string Reason = "@@Reason";
        public const string Notes = "@@Notes";
        public const string UploaderName = "@@UploaderName";
        public const string FileName = "@@FileName";
        public const string TotalRecords = "@@TotalRecords";
        public const string SkippedRecords = "@@SkippedRecords";

        // Span Control URL CONSTANTS
        public static string ETTBaseURL = "ETTBaseURL";

        //Time Zone
        public static string AlaskanStandardTime = "Alaskan Standard Time";
        public static string CentralStandardTime = "Central Standard Time";
        public static string EasternStandardTime = "Eastern Standard Time";
        public static string HawaiianStandardTime = "Hawaiian Standard Time";
        public static string MountainStandardTime = "Mountain Standard Time";
        public static string PacificStandardTime = "Pacific Standard Time";

        public static string BucketName = "BucketName";
        public static int PaymentTemplateIdTBS = 1;
        public static int PaymentTemplateIdETT = 2;
        public static int PaymentSuccessTemplateTBS = 5;
        public static int PaymentSuccessTemplateETT = 6;
        public static int PaymentReceiptTemplateTBS = 4;
        public static int PaymentReceiptTemplateETT = 3;
        public static string TmplateConstName = "@@Name";
        public static string TmplateConstServiceName = "@@ServiceName";
        public static string TmplateConstAmount = "@@Amount";
        public static string TmplateConstPayLink = "@@PayLink";
        public static string TmplatePaymentDate = "@@PaymentDate";
        public static string TmplatePayerContactName = "@@PayerContactName";
        public static string TmplateAddressDetails = "@@AddressDetails";
        public static string TmplatePhoneNo = "@@PhoneNo";
        public static string TmplateEmail = "@@Email";
        public static string TmplatePaymentApprovalCode = "@@PaymentApprovalCode";
        public static string TmplateCardNo = "@@CardNo";
        public static string TmplateNameOnCard = "@@NameOnCard";
        public static string TmplateReceiptNo = "@@ReceiptNo";
        public static string TmplateReceiptDescription = "@@ReceiptDescription";
        public static string PaymentsAccessEmails = "PaymentsAccessEmails";
        public static string S3BaseUrl = "S3BaseUrl";
        public static string S3AnotherFileBaseUrl = "S3AnotherFileBaseUrl";

        // Campaign create date
        public static string CampaignCreateDate = "CampaignCreateDate";

        //Two Factor Key
        public const string TwoFactorKey = "TwoFactorKey";
        public const string AuthenticatorAppName = "AuthenticatorAppName";
        public const string IsAuthenticatorEnabled = "IsAuthenticatorEnabled";

        public const string TBS = "TBS";
        public const string ETT = "ETT";


        public const int ProductIdLive = 12;
        public const int ProductIdSprint = 11;

        public const string PaymentLibConnStr = "PaymentLibConnStr";
        public const string TBSAppConnStr = "TBSAppConnStr";
        public const string ETTConnStr = "ETTConnStr";


        public const string WfgTbsMerchantCode = "WfgTbsMerchantCode";
        public const string WfgETTMerchantCode = "WfgETTMerchantCode";
        public const string WfgRemittanceDate = "WfgRemittanceDate";
        public const string WfgExpressExtensionMerchantCode = "WfgExpressExtensionMerchantCode";

        public const string PaymentBatchDetailReport = "PaymentBatchDetailReport";
        public const string REFUND = "REFUND";
        public const string VOID = "VOID";
        public const string Voided = "Voided";
        public const string Refunded = "Refunded";
        public const string WithStripe = "_STRIPE";
        public const string No = "No";

        //Express Extension
        public const string EE = "EE";
        public const int EEProductIdLive = 16;
        public const int EEProductIdSprint = 15;
        public const string EEConnStr = "EEConnStr";
        public const string WfgEefMerchantCode = "WfgEefMerchantCode";

        public const string MicrosoftCallbackUrl = "MicrosoftCallbackUrl";
        public const string MicrosoftClientId = "MicrosoftClientId";
        public const string MicrosoftClientSecret = "MicrosoftClientSecret";

    }
}


