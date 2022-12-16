using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Main.Control.Utilities.Infrastructure
{
    public class SessionItemKey
    {
        public const string AdminUserId = "AdminUserId";
        public const string UserId = "UserId";
        public const string AdminUserName = "AdminUserName";
        public const string AdminEmailAddress = "AdminEmailAddress";
        public const string AdminDisplayName = "AdminDisplayName";
        public const string AdminRole = "AdminRole";
        public const string Administrator = "Administrator";
        public const string ProjectType = "ProjectType";
        public const string ProjectID = "ProjectID";
        public const string MobileVerifyUserId = "MobileVerifyUserId";
        public const string UserMobileNumber = "UserMobileNumber";
        public const string VerifyUniqueId = "VerifyUniqueId";
        public const string MobileVerifyEmail = "MobileVerifyEmail";
        public const string IpAddressList = "IpAddressList";
        //Cookie Session
        public const string CookieSession = "CookieSession";
        public const string UWModules = "UWModules";
        public const string AdminSkuType = "AdminSkuType";
        public const string IsAdmin = "IsAdmin";
        public const string LastException = "LastException";
        public static string AdminCategoryId = "AdminCategoryId";
        public const string ProductId = "ProductId";
        public const string PaymentProcessorid = "PaymentProcessorid";
        public const string FromDate = "FromDate";
        public const string ToDate = "ToDate";
        public const string TransactionType = "TransactionType";

    }

    [Serializable]
    [Flags]
    public enum PURoles
    {
        Administrator,
        Director,
        Supersupport,
        Support,
        Sales,
        None
    }
}
