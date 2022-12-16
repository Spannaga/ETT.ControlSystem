using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Control.Core.Services;
using Main.Control.Core.Models;
using Main.Control.Core.Repositories;
using Main.Control.Services.Utilities;
using Main.Control.Core.Infrastructure;


namespace Main.Control.Services
{
    public class SpanControlService : ISpanControlService
    {
        #region Declaration
        private ISpanControlRepository _repository;
        private const string IPADDRESS = "IPADDRESS";
        #endregion

        #region Constructor
        public SpanControlService(ISpanControlRepository repository)
        {
            this._repository = repository;
        }
        #endregion

        #region ISpanControlService Members

        #region Admin Sign In
        /// <summary>
        /// Admin Sign In
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public AdminUser AdminSignIn(AdminUser admin)
        {
            //assign password to temporary variable
            string oPassword = admin.AdminPassword;

            //Get the Admin Details
            admin = this._repository.GetAdminDetailsByUserName(admin);

            if (admin != null && admin.AdminUserId > 0)
            {
                //if (admin.IsConfirmed)
                //{
                //    encrypt the entered password with the existing salt and compare 
                //    with the encrypted password from DB                  
                string encPassword = Utilities.Utilities.EncryptPassword(oPassword, admin.AdminSalt);
                if (encPassword.Equals(admin.AdminPassword))
                {
                    //assign success status
                    admin.OperationStatus = StatusType.Success;
                }
                else
                {
                    //Incorrect Password
                    admin.OperationStatus = StatusType.Failure;
                    //throw new RulesException("Password", "Incorrect Password.", admin);
                }
            }
            else
            {
                //Invalid Admin details
                admin.OperationStatus = StatusType.Failure;
                //throw new RulesException("UserName", "UserName or  Password is invalid.", admin);
            }

            //Return the admin datas 
            return admin;
        }
        #endregion

        #region Admin Sign In - ETF
        /// <summary>
        /// Admin Sign In
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public AdminUser AdminSignInETF(AdminUser admin)
        {
            //assign password to temporary variable
            string oPassword = admin.AdminPassword;
            long _projectId = (long)Project.ExpressTaxFilings;
            //Get the Admin Details
            admin = this._repository.GetAdminDetailsByUserNameAndProduct(admin, _projectId);

            if (admin != null && admin.AdminUserId > 0)
            {
                //if (admin.IsConfirmed)
                //{
                //    encrypt the entered password with the existing salt and compare 
                //    with the encrypted password from DB                  
                string encPassword = Utilities.Utilities.EncryptPassword(oPassword, admin.AdminSalt);
                if (encPassword.Equals(admin.AdminPassword))
                {
                    //assign success status
                    admin.OperationStatus = StatusType.Success;
                }
                else
                {
                    //Incorrect Password
                    admin.OperationStatus = StatusType.Failure;
                    //throw new RulesException("Password", "Incorrect Password.", admin);
                }
            }
            else
            {
                //Invalid Admin details
                admin.OperationStatus = StatusType.Failure;
                //throw new RulesException("UserName", "UserName or  Password is invalid.", admin);
            }

            //Return the admin datas 
            return admin;
        }
        #endregion

        #region Admin Sign In - E990
        /// <summary>
        /// Admin Sign In
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public AdminUser AdminSignInE990(AdminUser admin)
        {
            //assign password to temporary variable
            string oPassword = admin.AdminPassword;
            long _projectId = (long)Project.Express990;
            //Get the Admin Details
            admin = this._repository.GetAdminDetailsByUserNameAndProduct(admin, _projectId);

            if (admin != null && admin.AdminUserId > 0)
            {
                //if (admin.IsConfirmed)
                //{
                //    encrypt the entered password with the existing salt and compare 
                //    with the encrypted password from DB                  
                string encPassword = Utilities.Utilities.EncryptPassword(oPassword, admin.AdminSalt);
                if (encPassword.Equals(admin.AdminPassword))
                {
                    //assign success status
                    admin.OperationStatus = StatusType.Success;
                }
                else
                {
                    //Incorrect Password
                    admin.OperationStatus = StatusType.Failure;
                    //throw new RulesException("Password", "Incorrect Password.", admin);
                }
            }
            else
            {
                //Invalid Admin details
                admin.OperationStatus = StatusType.Failure;
                //throw new RulesException("UserName", "UserName or  Password is invalid.", admin);
            }

            //Return the admin datas 
            return admin;
        }
        #endregion

        #region Admin Sign In - EXTN
        /// <summary>
        /// Admin Sign In
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public AdminUser AdminSignInEXTN(AdminUser admin)
        {
            //assign password to temporary variable
            string oPassword = admin.AdminPassword;
            long _projectId = (long)Project.ExpressExtension;
            //Get the Admin Details
            admin = this._repository.GetAdminDetailsByUserNameAndProduct(admin, _projectId);

            if (admin != null && admin.AdminUserId > 0)
            {
                //if (admin.IsConfirmed)
                //{
                //    encrypt the entered password with the existing salt and compare 
                //    with the encrypted password from DB                  
                string encPassword = Utilities.Utilities.EncryptPassword(oPassword, admin.AdminSalt);
                if (encPassword.Equals(admin.AdminPassword))
                {
                    //assign success status
                    admin.OperationStatus = StatusType.Success;
                }
                else
                {
                    //Incorrect Password
                    admin.OperationStatus = StatusType.Failure;
                    //throw new RulesException("Password", "Incorrect Password.", admin);
                }
            }
            else
            {
                //Invalid Admin details
                admin.OperationStatus = StatusType.Failure;
                //throw new RulesException("UserName", "UserName or  Password is invalid.", admin);
            }

            //Return the admin datas 
            return admin;
        }
        #endregion

        #region Admin Sign In - TSNA
        /// <summary>
        /// Admin Sign In
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public AdminUser AdminSignInTSNA(AdminUser admin)
        {
            //assign password to temporary variable
            string oPassword = admin.AdminPassword;
            long _projectId = (long)Project.TSNAAdmin;
            //Get the Admin Details
            admin = this._repository.GetAdminDetailsByUserNameAndProduct(admin, _projectId);

            if (admin != null && admin.AdminUserId > 0)
            {
                //if (admin.IsConfirmed)
                //{
                //    encrypt the entered password with the existing salt and compare 
                //    with the encrypted password from DB                  
                string encPassword = Utilities.Utilities.EncryptPassword(oPassword, admin.AdminSalt);
                if (encPassword.Equals(admin.AdminPassword))
                {
                    //assign success status
                    admin.OperationStatus = StatusType.Success;
                }
                else
                {
                    //Incorrect Password
                    admin.OperationStatus = StatusType.Failure;
                    //throw new RulesException("Password", "Incorrect Password.", admin);
                }
            }
            else
            {
                //Invalid Admin details
                admin.OperationStatus = StatusType.Failure;
                //throw new RulesException("UserName", "UserName or  Password is invalid.", admin);
            }

            //Return the admin datas 
            return admin;
        }
        #endregion

        #region Admin Sign In - STE
        /// <summary>
        /// Admin Sign In
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public AdminUser AdminSignInSTE(AdminUser admin)
        {
            //assign password to temporary variable
            string oPassword = admin.AdminPassword;
            long _projectId = (long)Project.StayTaxExempt;
            //Get the Admin Details
            admin = this._repository.GetAdminDetailsByUserNameAndProduct(admin, _projectId);

            if (admin != null && admin.AdminUserId > 0)
            {
                //if (admin.IsConfirmed)
                //{
                //    encrypt the entered password with the existing salt and compare 
                //    with the encrypted password from DB                  
                string encPassword = Utilities.Utilities.EncryptPassword(oPassword, admin.AdminSalt);
                if (encPassword.Equals(admin.AdminPassword))
                {
                    //assign success status
                    admin.OperationStatus = StatusType.Success;
                }
                else
                {
                    //Incorrect Password
                    admin.OperationStatus = StatusType.Failure;
                    //throw new RulesException("Password", "Incorrect Password.", admin);
                }
            }
            else
            {
                //Invalid Admin details
                admin.OperationStatus = StatusType.Failure;
                //throw new RulesException("UserName", "UserName or  Password is invalid.", admin);
            }

            //Return the admin datas 
            return admin;
        }
        #endregion

        #region Get Admin User By Id
        /// <summary>
        /// Get Admin User By Id
        /// </summary>
        /// <returns></returns>
        public AdminUser GetAdminUserById(long Id)
        {
            return this._repository.GetAdminUserById(Id);
        }
        #endregion

        #region Get Admin User By Id
        /// <summary>
        /// Get Admin User By Id
        /// </summary>
        /// <returns></returns>
        public AdminUser GetAdminUserByIdAndProduct(long Id, long productId)
        {
            return this._repository.GetAdminUserByIdAndProduct(Id, productId);
        }
        #endregion

        #region Get Admin User By Id
        /// <summary>
        /// Get Admin User By Id
        /// </summary>
        /// <returns></returns>
        public AdminUser GetAdminUserByIdETF(long Id, long productId)
        {
            return this._repository.GetAdminUserByIdETF(Id, productId);
        }
        #endregion

        public List<AdminUserRole> GetAllAdminProjectRole(long userId)
        {
            return this._repository.GetAllAdminProjectRole(userId);
        }

        public string GetAdminRoleByProjectIdAndAdminUserId(long projectId, long adminUserId)
        {
            return this._repository.GetAdminRoleByProjectIdAndAdminUserId(projectId, adminUserId);
        }

        public string GetAdminUserNameByUserId(long userId)
        {
            return this._repository.GetAdminUserNameByUserId(userId);
        }


        #region Admin Sign In ETL
        /// <summary>
        /// Admin Sign In
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public AdminUser AdminSignInETL(AdminUser admin)
        {
            //assign password to temporary variable
            string oPassword = admin.AdminPassword;
            long _projectId = (long)Project.TruckLogics;
            //Get the Admin Details
            admin = this._repository.GetAdminDetailsByUserNameAndProduct(admin, _projectId);

            if (admin != null && admin.AdminUserId > 0)
            {
                //if (admin.IsConfirmed)
                //{
                //    encrypt the entered password with the existing salt and compare 
                //    with the encrypted password from DB                  
                string encPassword = Utilities.Utilities.EncryptPassword(oPassword, admin.AdminSalt);
                if (encPassword.Equals(admin.AdminPassword))
                {
                    //assign success status
                    admin.OperationStatus = StatusType.Success;
                }
                else
                {
                    //Incorrect Password
                    admin.OperationStatus = StatusType.Failure;
                    //throw new RulesException("Password", "Incorrect Password.", admin);
                }
            }
            else
            {
                //Invalid Admin details
                admin.OperationStatus = StatusType.Failure;
                //throw new RulesException("UserName", "UserName or  Password is invalid.", admin);
            }

            //Return the admin datas 
            return admin;
        }
        #endregion

        #endregion

        #region Get Admin UserNameList
        public List<AdminUser> GetAdminUserNameList()
        {
            return this._repository.GetAdminUserNameList();
        }

        #endregion

        #region Validate User

        public AdminUser ValidateUser(AdminUser pTaxUser)
        {
            var _taxUser = new AdminUser();
            if (pTaxUser != null && !string.IsNullOrWhiteSpace(pTaxUser.AdminUserName))
            {
                _taxUser = _repository.GetUserDetailsByUserName(pTaxUser.AdminUserName.Trim());

                //check the user's login
                if (_taxUser != null && _taxUser.AdminUserId > 0)
                {
                    //encrypt the entered password with the existing salt and compare 
                    //with the encrypted password from DB

                    var oPassword = pTaxUser.AdminPassword;
                    var encPassword = Utilities.Utilities.EncryptPassword(oPassword, _taxUser.AdminSalt);
                    if (encPassword.Equals(_taxUser.AdminPassword))
                    {
                        _taxUser.OperationStatus = StatusType.Success;
                        _taxUser.IsValidUser = true;
                    }
                    else
                    {
                        //Incorrect Password
                        _taxUser.OperationStatus = StatusType.Failure;
                        _taxUser.IsValidUser = false;
                    }
                }

            }
            else
            {
                //Incorrect Password
                _taxUser.OperationStatus = StatusType.Failure;
                _taxUser.IsValidUser = false;
            }
            return _taxUser;
        }

        #endregion

        #region Get User Details By Email Address

        public AdminUser GetUserDetailsByUserName(string userName)
        {
            return _repository.GetUserDetailsByUserName(userName);
        }

        #endregion

        #region GetAllValidIpAddress
        /// <summary>
        /// GetAllValidIpAddress
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllValidIpAddress(long projectId)
        {
            CacheBlock _cacheBlock = new CacheBlock();
            List<AdminIpAddress> ipAddress = new List<AdminIpAddress>();
            if (_cacheBlock.Contains(IPADDRESS))
            {
                ipAddress = (List<AdminIpAddress>)_cacheBlock.Get(IPADDRESS);
            }

            //if Ip Address not available in cache then call db.
            if (ipAddress.Count == 0)
            {
                ipAddress = this._repository.GetAllValidIpAddress();
                _cacheBlock.Add(IPADDRESS, ipAddress);
            }

            ipAddress = ipAddress.Where(r => projectId > 0 ? r.ProjectId == projectId : true).ToList();

            return ipAddress.Select(e => e.IpAddress).ToList();
        }
        #endregion

        #region Remove Project Cache
        /// <summary>
        /// RemoveProjectCache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void RemoveProjectCache(string key)
        {
            CacheBlock cacheBlock = new CacheBlock();
            if (cacheBlock.Contains(key))
            {
                cacheBlock.Remove(key);
            }
        }
        #endregion
    }
}
