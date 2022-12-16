using Main.Control.Core.Models;
using Main.Control.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Main.Control.Services.Utilities;
using System.Net.Http;
using System.Configuration;
using Main.Control.Service.Utilities;
using Main.Control.Resources;
using Dapper;

namespace Main.Control.Resources
{
    public class AdminRepository : IAdminRepository
    {
        #region Declaration
        private MainControlDB_Entities _context;
        #endregion

        #region Constructor
        public AdminRepository()
        {
            _context = new MainControlDB_Entities();
        }
        #endregion

        #region Get Admin Details By Id
        /// <summary>
        /// Get Admin Details By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public AdminUser GetAdminUserById(long Id)
        {
            AdminUser _adminUser = new AdminUser();
            var _dbAdminUser = (from a in _context.Biz_Admin_Users
                                where a.Admin_User_Id == Id && !a.Is_Deleted
                                select a).SingleOrDefault();

            if (_dbAdminUser != null && _dbAdminUser.Admin_User_Id > 0)
            {
                _adminUser.AdminUserId = _dbAdminUser.Admin_User_Id;
                _adminUser.AdminUserName = _dbAdminUser.Admin_User_Name;
                _adminUser.AdminFirstName = _dbAdminUser.Admin_First_Name;
                _adminUser.AdminLastName = _dbAdminUser.Admin_Last_Name;
                _adminUser.AdminEmailAddress = _dbAdminUser.Admin_Email_Address;
                _adminUser.AdminRoleId = _dbAdminUser.Admin_Role;
                _adminUser.AdminRoles = GetAdminRoleById(_dbAdminUser.Admin_Role);
                _adminUser.ProjectType = _dbAdminUser.Projects;
                _adminUser.IsApproved = _dbAdminUser.Is_Approved ?? true;
                _adminUser.IsAdmin = CheckForAdminRole(_dbAdminUser.Admin_User_Id);
                _adminUser.PhoneNumber = _dbAdminUser.Phone_Number;
                _adminUser.AdminLocation = _dbAdminUser.Location;
                _adminUser.VerificationCodType = _dbAdminUser.Verification_Code_Type ?? VerificationCodeType.SMS.ToString();
                _adminUser.AlternateAdminEmailAddress = _dbAdminUser.Alternate_Admin_Email_Address ?? string.Empty;
                if (_dbAdminUser.Admin_First_Name != null && _dbAdminUser.Admin_First_Name != "")
                {
                    _adminUser.AdminDisplayName = _dbAdminUser.Admin_First_Name;
                }
                if (_dbAdminUser.Admin_Last_Name != null && _dbAdminUser.Admin_Last_Name != "")
                {
                    _adminUser.AdminDisplayName = _dbAdminUser.Admin_First_Name + " " + _dbAdminUser.Admin_Last_Name;
                }
            }

            return _adminUser;
        }
        #endregion

        #region Get Admin Details By Email Id
        /// <summary>
        /// Get Admin Details By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public AdminUser GetAdminUserByEmailId(string email)
        {
            email = email.Trim();
            AdminUser _adminUser = new AdminUser();
            var _dbAdminUser = (from a in _context.Biz_Admin_Users
                                where a.Admin_Email_Address == email && !a.Is_Deleted
                                select a).SingleOrDefault();

            if (_dbAdminUser != null && _dbAdminUser.Admin_User_Id > 0)
            {
                _adminUser.AdminUserId = _dbAdminUser.Admin_User_Id;
                _adminUser.AdminUserName = _dbAdminUser.Admin_User_Name;
                _adminUser.AdminFirstName = _dbAdminUser.Admin_First_Name;
                _adminUser.AdminLastName = _dbAdminUser.Admin_Last_Name;
                _adminUser.AdminEmailAddress = _dbAdminUser.Admin_Email_Address;
                _adminUser.AdminRoleId = _dbAdminUser.Admin_Role;
                _adminUser.AdminRoles = GetAdminRoleById(_dbAdminUser.Admin_Role);
                _adminUser.ProjectType = _dbAdminUser.Projects;
                _adminUser.IsApproved = _dbAdminUser.Is_Approved ?? true;
                _adminUser.IsAdmin = CheckForAdminRole(_dbAdminUser.Admin_User_Id);
                _adminUser.PhoneNumber = _dbAdminUser.Phone_Number;
                _adminUser.VerificationCodType = _dbAdminUser.Verification_Code_Type ?? VerificationCodeType.SMS.ToString();
                _adminUser.AlternateAdminEmailAddress = _dbAdminUser.Alternate_Admin_Email_Address ?? string.Empty;
            }

            return _adminUser;
        }
        #endregion

        #region Get Admin Details By Admin User Name
        /// <summary>
        /// Get Admin Details By AdminUserName
        /// </summary>
        /// <param name="admin"></param>
        /// <returns>Admin Details</returns>
        public AdminUser GetAdminDetailsByUserName(AdminUser adminUser)
        {
            if (adminUser.AdminUserName != null)
            {
                //Get the Admin Details based on the AdminUserName         
                var _dbadmin = (from a in _context.Biz_Admin_Users
                                where a.Admin_User_Name == adminUser.AdminUserName && (a.Is_Approved == true || a.Is_Approved == null) && !a.Is_Deleted
                                select new
                                {
                                    a.Admin_User_Id,
                                    a.Admin_User_Name,
                                    a.Admin_Password,
                                    a.Admin_Salt,
                                    a.Admin_Role,
                                    a.Projects,
                                    a.Admin_First_Name,
                                    a.Admin_Last_Name,
                                    a.Admin_Email_Address,
                                    a.Verification_Code_Type,
                                    a.Alternate_Admin_Email_Address
                                }).SingleOrDefault();

                if (_dbadmin != null)
                {
                    //Assign the admin details to admin model entity from the above _dbadmin object
                    adminUser.AdminUserName = _dbadmin.Admin_User_Name;
                    adminUser.AdminEmailAddress = _dbadmin.Admin_Email_Address;
                    adminUser.AdminPassword = _dbadmin.Admin_Password;
                    string _adminRole = GetAdminRoleById(_dbadmin.Admin_Role);
                    adminUser.AdminRoles = _adminRole;
                    adminUser.AdminSalt = _dbadmin.Admin_Salt;
                    adminUser.AdminUserId = _dbadmin.Admin_User_Id;
                    adminUser.ProjectType = _dbadmin.Projects;
                    adminUser.IsAdmin = CheckForAdminRole(_dbadmin.Admin_User_Id);
                    adminUser.AdminSKUType = _adminRole;
                    if (_dbadmin.Admin_First_Name != null && _dbadmin.Admin_First_Name != "")
                    {
                        adminUser.AdminDisplayName = _dbadmin.Admin_First_Name;
                    }
                    if (_dbadmin.Admin_Last_Name != null && _dbadmin.Admin_Last_Name != "")
                    {
                        adminUser.AdminDisplayName = _dbadmin.Admin_First_Name + " " + _dbadmin.Admin_Last_Name;
                    }
                    adminUser.VerificationCodType = _dbadmin.Verification_Code_Type ?? VerificationCodeType.SMS.ToString();
                    adminUser.AlternateAdminEmailAddress = _dbadmin.Alternate_Admin_Email_Address ?? string.Empty;
                }
            }
            //return the admin details.
            return adminUser;

        }

        private bool CheckForAdminRole(long adminUserId)
        {
            return _context.Biz_Admin_User_Roles.Any(a => a.Admin_User_Id == adminUserId && a.Admin_Role_Id == 1 && !a.Is_Deleted);
        }
        #endregion

        #region Get Admin Role By Id
        /// <summary>
        /// Get Admin Role By Id
        /// </summary>
        /// <param name="adminRoleId"></param>
        /// <returns></returns>
        public string GetAdminRoleById(long adminRoleId)
        {
            string _adminRole = string.Empty;

            var _dbAdminRole = (from a in _context.Biz_Admin_Roles
                                where a.Admin_Role_Id == adminRoleId && !a.Is_Deleted
                                select a.Admin_Role).SingleOrDefault();

            if (_dbAdminRole != null)
            {
                _adminRole = _dbAdminRole;
            }
            return _adminRole;
        }

        public string GetAdminRoleByProjectIdAndAdminUserId(long projectId, long adminUserId)
        {
            string _adminRole = string.Empty;

            var _dbAdminRole = (from a in _context.Biz_Admin_User_Roles
                                where a.Admin_Project_Id == projectId && a.Admin_User_Id == adminUserId && !a.Is_Deleted
                                select a.Admin_Role_Id).SingleOrDefault();

            if (_dbAdminRole > 0)
            {
                AdminRoleType _admin = ((AdminRoleType)_dbAdminRole);
                _adminRole = _admin.ToString();
            }
            return _adminRole;
        }
        #endregion

        #region Get All Admin Roles
        public IQueryable<AdminRole> GetAllAdminRoles(string categoryid)
        {
            IQueryable<Biz_Admin_Roles> _dbAdminRoleList = (from a in _context.Biz_Admin_Roles
                                                            where !a.Is_Deleted && a.Is_Active && a.Category_Ids.Contains(categoryid)
                                                            select a);

            return (from p in _dbAdminRoleList
                    select new AdminRole
                    {
                        Role = p.Admin_Role,
                        AdminRoleId = p.Admin_Role_Id,
                        Description = p.Description
                    }).AsQueryable();
        }
        #endregion

        #region Get All Admin User Roles
        public long GetAllAdminUserRoles(long adminUserRoleId, long adminProjectId)
        {
            long _adminRole = 0;

            var _dbAdminRole = (from a in _context.Biz_Admin_User_Roles
                                where a.Admin_User_Id == adminUserRoleId && a.Admin_Project_Id == adminProjectId && !a.Is_Deleted
                                select a.Admin_Role_Id).SingleOrDefault();

            if (_dbAdminRole > 0)
            {
                _adminRole = _dbAdminRole;
            }
            return _adminRole;
        }
        #endregion

        #region Get Admin Role By RoleIds
        public string GetAdminRoleByRoleId(long adminRoleId)
        {
            string _adminRole = string.Empty;

            var _dbAdminRole = (from a in _context.Biz_Admin_Roles
                                where a.Admin_Role_Id == adminRoleId
                                select a.Admin_Role).SingleOrDefault();

            if (_dbAdminRole != null && _dbAdminRole != "")
            {
                _adminRole = _dbAdminRole;
            }
            return _adminRole;
        }
        #endregion

        #region Is UserName Available
        /// <summary>
        /// Is UserName Available
        /// </summary>
        /// <param name="subDomain"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public bool VerifyUserName(string subDomain, long UserId)
        {
            if (UserId > 0)
            {
                var _subDomain = (from u in _context.Biz_Admin_Users
                                  where u.Admin_Email_Address == subDomain && u.Admin_User_Id != UserId && u.Is_Deleted == false
                                  select u);

                //check for user SubDomain avaialable
                if (_subDomain != null && _subDomain.Count() > 0)
                    return false;
                else
                    return true;
            }
            else
            {
                var _subDomain = (from u in _context.Biz_Admin_Users
                                  where u.Admin_Email_Address == subDomain && u.Is_Deleted == false
                                  select u);

                //check for user SubDomain avaialable
                if (_subDomain != null && _subDomain.Count() > 0)
                    return false;
                else
                    return true;
            }
        }
        #endregion

        #region Get All Admin Project Role
        public List<AdminUserRole> GetAllAdminProjectRole(long userId)
        {

            var dbAdminProjectRoleList = (from a in _context.Biz_Admin_User_Roles
                                          join b in _context.Static_Biz_Admin_Project on a.Admin_Project_Id equals b.Project_Id
                                          join c in _context.Biz_Admin_Roles on a.Admin_Role_Id equals c.Admin_Role_Id
                                          join d in _context.Biz_Admin_Users on a.Admin_User_Id equals d.Admin_User_Id
                                          join e in _context.Static_Biz_Admin_Category_Project on a.Admin_Project_Id equals e.Project_Id
                                          where !a.Is_Deleted && b.Is_Deleted == false && !c.Is_Deleted && !d.Is_Deleted && e.Is_Deleted == false
                                          && a.Admin_User_Id == userId
                                          orderby b.Project_Id
                                          select new
                                          {
                                              a.Admin_User_Id,
                                              a.Admin_Role_Id,
                                              a.Admin_User_Role_Id,
                                              a.Admin_Project_Id,
                                              b.Project_Name,
                                              c.Admin_Role,
                                              d.Admin_User_Name,
                                              e.Category_Id
                                          }).ToList();


            List<AdminUserRole> lstAdminRole = new List<AdminUserRole>();

            if (dbAdminProjectRoleList.Any())
            {
                foreach (var temp in dbAdminProjectRoleList)
                {
                    AdminUserRole adminRole = new AdminUserRole();
                    adminRole.AdminUserRoleId = temp.Admin_User_Role_Id;
                    adminRole.AdminUserId = temp.Admin_User_Id;
                    adminRole.AdminRoleId = temp.Admin_Role_Id;
                    adminRole.AdminProjectId = temp.Admin_Project_Id;
                    adminRole.ProjectName = temp.Project_Name;
                    adminRole.Role = temp.Admin_Role;
                    adminRole.AdminUserName = temp.Admin_User_Name;
                    adminRole.AdminCategoryId = temp.Category_Id ?? 0;
                    lstAdminRole.Add(adminRole);
                }
            }
            return lstAdminRole;
        }
        #endregion

        #region Get All Admin Users
        /// <summary>
        ///  Get All Admin Users 
        /// </summary>
        /// <param name="admin"></param>
        /// <returns>Admin Users</returns>
        public List<AdminUser> GetAllAdminUsers(long adminprojectId)
        {
            List<AdminUser> adminList = null;
            if (adminprojectId == 0)
            {
                var dbadmin = (from a in _context.Biz_Admin_Users
                               where !a.Is_Deleted
                               select new
                               {
                                   a.Admin_User_Id,
                                   a.Admin_User_Name,
                                   a.Admin_First_Name,
                                   a.Admin_Last_Name,
                                   a.Admin_Password,
                                   a.Admin_Email_Address,
                                   a.Created_By,
                                   a.Last_Updated_By,
                                   a.Is_Approved,
                                   a.Admin_Role,
                                   a.Verification_Code_Type,
                                   a.Alternate_Admin_Email_Address, 
                                   a.Is_Enabled_Authenticator,
                                   a.Location
                               }).ToList();


                if (dbadmin.Any())
                {
                    adminList = new List<AdminUser>();
                    foreach (var _thisadminUser in dbadmin)
                    {
                        AdminUser adminuser = new AdminUser();
                        adminuser.AdminUserId = _thisadminUser.Admin_User_Id;
                        adminuser.AdminUserName = _thisadminUser.Admin_User_Name;
                        adminuser.AdminFirstName = _thisadminUser.Admin_First_Name;
                        adminuser.AdminLastName = _thisadminUser.Admin_Last_Name;
                        adminuser.AdminRoles = GetAdminRoleById(_thisadminUser.Admin_Role);
                        adminuser.AdminPassword = _thisadminUser.Admin_Password;
                        adminuser.AdminEmailAddress = _thisadminUser.Admin_Email_Address;
                        adminuser.CreatedUserName = GetAdminNameByUserId(_thisadminUser.Created_By ?? 0);
                        adminuser.UpdatedUserName = GetAdminNameByUserId(_thisadminUser.Last_Updated_By ?? 0);
                        adminuser.IsApproved = _thisadminUser.Is_Approved ?? false;
                        adminuser.IsAdmin = CheckForAdminRole(_thisadminUser.Admin_User_Id);
                        adminuser.VerificationCodType = _thisadminUser.Verification_Code_Type ?? VerificationCodeType.SMS.ToString();
                        adminuser.AlternateAdminEmailAddress = _thisadminUser.Alternate_Admin_Email_Address ?? string.Empty;
                        adminuser.IsEnabledAuthenticator = _thisadminUser.Is_Enabled_Authenticator;
                        adminuser.VerificationCode = GetVerificationCodeByUserId(adminuser.AdminUserId);
                        adminuser.AdminLocation = _thisadminUser.Location;
                        adminList.Add(adminuser);
                    }
                }
            }
            else
            {
                var dbadmin = (from a in _context.Biz_Admin_Users
                               join b in _context.Biz_Admin_User_Roles on a.Admin_User_Id equals b.Admin_User_Id
                               where !a.Is_Deleted && b.Admin_Project_Id == adminprojectId && !b.Is_Deleted
                               select new
                               {
                                   a.Admin_User_Id,
                                   a.Admin_User_Name,
                                   a.Admin_First_Name,
                                   a.Admin_Last_Name,
                                   a.Admin_Password,
                                   a.Admin_Email_Address,
                                   a.Created_By,
                                   a.Last_Updated_By,
                                   a.Is_Approved,
                                   b.Admin_Project_Id,
                                   b.Admin_Role_Id,
                                   a.Verification_Code_Type,
                                   a.Alternate_Admin_Email_Address
                               }).ToList();

                if (dbadmin.Any())
                {
                    adminList = new List<AdminUser>();
                    foreach (var thisadminUser in dbadmin)
                    {
                        AdminUser adminuser = new AdminUser();
                        adminuser.AdminUserId = thisadminUser.Admin_User_Id;
                        adminuser.AdminUserName = thisadminUser.Admin_User_Name;
                        adminuser.AdminFirstName = thisadminUser.Admin_First_Name;
                        adminuser.AdminLastName = thisadminUser.Admin_Last_Name;
                        adminuser.AdminPassword = thisadminUser.Admin_Password;
                        adminuser.AdminEmailAddress = thisadminUser.Admin_Email_Address;
                        adminuser.ProjectType = GetProjectType(thisadminUser.Admin_User_Id);
                        adminuser.CreatedUserName = GetAdminNameByUserId(thisadminUser.Created_By ?? 0);
                        adminuser.UpdatedUserName = GetAdminNameByUserId(thisadminUser.Last_Updated_By ?? 0);
                        adminuser.IsApproved = thisadminUser.Is_Approved ?? false;
                        adminuser.VerificationCodType = thisadminUser.Verification_Code_Type ?? VerificationCodeType.SMS.ToString();
                        adminuser.AlternateAdminEmailAddress = thisadminUser.Alternate_Admin_Email_Address ?? string.Empty;
                        adminuser.VerificationCode = GetVerificationCodeByUserId(adminuser.AdminUserId);
                        adminList.Add(adminuser);
                    }
                }
            }

            return adminList;
        }




        private string GetProjectType(long adminUserId)
        {
            var _dbAdminProjectRoleList = (from a in _context.Biz_Admin_User_Roles
                                           join b in _context.Static_Biz_Admin_Project on a.Admin_Project_Id equals b.Project_Id

                                           where !a.Is_Deleted && a.Admin_User_Id == adminUserId
                                           select new
                                           {


                                               b.Project_Name,

                                           });
            string project = string.Empty;
            foreach (var p in _dbAdminProjectRoleList)
            {
                project += p.Project_Name + ",";
            }
            return project;
        }

        #endregion

        #region Save Password

        public AdminUser SavePassword(AdminUser admin)
        {
            if (admin != null && admin.AdminUserId > 0)
            {
                Biz_Admin_Users _dbAdminUser = _context.Biz_Admin_Users.SingleOrDefault(au => au.Admin_User_Id == admin.AdminUserId && !au.Is_Deleted);
                if (_dbAdminUser != null && _dbAdminUser.Admin_User_Id > 0)
                {
                    _dbAdminUser.Admin_Password = admin.AdminPassword;
                    _dbAdminUser.Admin_Salt = admin.AdminSalt;
                    _dbAdminUser.Update_Time_Stamp = DateTime.Now;
                    _context.SaveChanges();
                    admin.OperationStatus = StatusType.Success;
                }
            }
            return admin;
        }
        #endregion

        #region Get Admin Name By UserId
        /// <summary>
        /// Get Admin Details By AdminUserId
        /// </summary>
        /// <param name="admin"></param>
        /// <returns>Admin Details</returns>
        public string GetAdminNameByUserId(long UserId)
        {
            string _adminName = string.Empty;
            var _dbadmin = (from a in _context.Biz_Admin_Users
                            where a.Admin_User_Id == UserId && !a.Is_Deleted
                            select new
                            {
                                a.Admin_First_Name,
                                a.Admin_Last_Name
                            }).SingleOrDefault();

            if (_dbadmin != null)
            {
                //Assign the admin details to admin model entity from the above _dbadmin object
                _adminName = _dbadmin.Admin_First_Name + " " + _dbadmin.Admin_Last_Name;
            }
            return _adminName;
        }
        #endregion

        #region Save Admin User
        /// <summary>
        ///  Save Admin User 
        /// </summary>
        /// <param name="admin"></param>
        /// <returns>Admin User Details</returns>
        public AdminUser SaveAdminUser(AdminUser admin)
        {
            Biz_Admin_Users _dbAdminUser = null;

            bool IsRecordExists = false;

            if (admin.AdminUserId > 0)
            {
                _dbAdminUser = _context.Biz_Admin_Users.SingleOrDefault(au => au.Admin_User_Id == admin.AdminUserId && au.Admin_User_Id == admin.AdminUserId && !au.Is_Deleted);
            }
            if (_dbAdminUser != null && _dbAdminUser.Admin_User_Id > 0)
            {
                IsRecordExists = true;
            }
            else
            {
                _dbAdminUser = new Biz_Admin_Users();
            }
            _dbAdminUser.Admin_User_Id = admin.AdminUserId;
            _dbAdminUser.Admin_User_Name = admin.AdminFirstName;
            _dbAdminUser.Admin_First_Name = admin.AdminFirstName;
            _dbAdminUser.Admin_Last_Name = admin.AdminLastName;
            _dbAdminUser.Phone_Number = admin.PhoneNumber;
            _dbAdminUser.Projects = admin.ProjectType;
            _dbAdminUser.Is_Enabled_Authenticator = admin.IsEnabledAuthenticator;
            if (admin != null)
            {
                _dbAdminUser.Admin_Password = admin.AdminPassword;
                _dbAdminUser.Admin_Salt = admin.AdminSalt;
                if (!string.IsNullOrWhiteSpace((admin.AdminFirstName)) && !string.IsNullOrWhiteSpace((admin.AdminLastName)))
                {
                    _dbAdminUser.Admin_Initial = (admin.AdminFirstName.Substring(0, 1) + admin.AdminLastName.Substring(0, 1));
                }
            }
            _dbAdminUser.Admin_Email_Address = admin.AdminEmailAddress;
            _dbAdminUser.Location = admin.AdminLocation;
            _dbAdminUser.Is_Approved = !admin.IsApproved;
            _dbAdminUser.Admin_Role = admin.AdminRoleId;
            _dbAdminUser.Last_Updated_By = admin.CreatedUserId;
            _dbAdminUser.Is_Deleted = false;
            _dbAdminUser.Update_Time_Stamp = DateTime.Now;
            _dbAdminUser.Verification_Code_Type = admin.VerificationCodType;
            _dbAdminUser.Alternate_Admin_Email_Address = admin.AlternateAdminEmailAddress;
            _dbAdminUser.Is_Enabled_Authenticator = admin.IsEnabledAuthenticator;
            if (!IsRecordExists)
            {
                _dbAdminUser.Create_Time_Stamp = DateTime.Now;
                _dbAdminUser.Created_By = admin.CreatedUserId;
                _context.Biz_Admin_Users.Add(_dbAdminUser);
                admin.Is_Existing = false;
            }
            else
            {
                admin.Is_Existing = true;
            }

            _context.SaveChanges();


            admin.OperationStatus = StatusType.Success;

            return admin;
        }

        #endregion

        #region Save Project Role

        public AdminUserRole SaveProjectRole(AdminUserRole adminRole)
        {
            Biz_Admin_User_Roles dbAdminUserRole = null;

            bool IsRecordExists = false;

            if (adminRole.AdminUserId > 0 && adminRole.AdminProjectId > 0)
            {
                dbAdminUserRole = _context.Biz_Admin_User_Roles.SingleOrDefault(au => au.Admin_User_Id == adminRole.AdminUserId
                    && au.Admin_Project_Id == adminRole.AdminProjectId && !au.Is_Deleted);
            }
            if (dbAdminUserRole != null && dbAdminUserRole.Admin_User_Role_Id > 0)
            {
                IsRecordExists = true;
            }
            else
            {
                dbAdminUserRole = new Biz_Admin_User_Roles();
            }

            dbAdminUserRole.Admin_User_Id = adminRole.AdminUserId;
            dbAdminUserRole.Admin_Role_Id = adminRole.AdminRoleId;
            dbAdminUserRole.Admin_Project_Id = adminRole.AdminProjectId;

            dbAdminUserRole.Is_Deleted = false;
            dbAdminUserRole.Update_Time_Stamp = DateTime.Now;

            if (!IsRecordExists)
            {
                dbAdminUserRole.Create_Time_Stamp = DateTime.Now;

                _context.Biz_Admin_User_Roles.Add(dbAdminUserRole);

            }
            _context.SaveChanges();

            adminRole.Status = StatusType.Success.ToString();
            return adminRole;
        }

        #endregion

        #region Get All Admin projects
        public IQueryable<AdminProject> GetAllAdminProjects(long? categoryid)
        {


            IQueryable<Static_Biz_Admin_Project> _dbAdminProjectList;
            _dbAdminProjectList = (from a in _context.Static_Biz_Admin_Project
                                   join b in _context.Static_Biz_Admin_Category_Project on a.Project_Id equals b.Project_Id
                                   where a.Is_Deleted == false && b.Category_Id == categoryid
                                   select a);


            return (from p in _dbAdminProjectList
                    select new AdminProject
                    {
                        AdminProjectId = p.Project_Id,
                        ProjectName = p.Project_Name,
                    }).AsQueryable();


        }
        #endregion

        #region Get All Admin Categories
        public IQueryable<AdminCategory> GetAllAdminCategories(long? userid)
        {


            IQueryable<Static_Biz_Admin_Category> _dbAdminCategoryList;
            _dbAdminCategoryList = (from a in _context.Static_Biz_Admin_Category
                                    where a.Is_Deleted == false
                                    select a);


            return (from p in _dbAdminCategoryList
                    select new AdminCategory
                    {
                        AdminCategoryId = p.Category_Id,
                        AdminCategoryName = p.Category_Name,
                    }).AsQueryable();


        }
        #endregion

        #region Delete User

        public bool DeleteAdminUser(long Id)
        {
            bool _isDeleted = false;

            if (Id > 0)
            {
                var _dbAdminUser = (from a in _context.Biz_Admin_Users
                                    where a.Admin_User_Id == Id && !a.Is_Deleted
                                    select a).SingleOrDefault();

                if (_dbAdminUser != null && _dbAdminUser.Admin_User_Id > 0)
                {
                    _dbAdminUser.Is_Deleted = true;
                    _dbAdminUser.Update_Time_Stamp = DateTime.Now;
                    _context.SaveChanges();
                    _isDeleted = true;
                }
            }
            return _isDeleted;
        }
        #endregion

        #region Delete user project Role

        public bool DeleteProjectRole(long Id)
        {
            bool _isDeleted = false;

            if (Id > 0)
            {
                var _dbAdminUserRole = (from a in _context.Biz_Admin_User_Roles
                                        where a.Admin_User_Role_Id == Id && !a.Is_Deleted
                                        select a).SingleOrDefault();

                if (_dbAdminUserRole != null && _dbAdminUserRole.Admin_User_Role_Id > 0)
                {
                    _dbAdminUserRole.Is_Deleted = true;
                    _dbAdminUserRole.Update_Time_Stamp = DateTime.Now;
                    _context.SaveChanges();
                    _isDeleted = true;
                }
            }
            return _isDeleted;
        }
        #endregion

        #region Get approved Admin Details By Admin User Name
        /// <summary>
        /// Get Admin Details By AdminUserName
        /// </summary>
        /// <param name="admin"></param>
        /// <returns>Admin Details</returns>
        public AdminUser GetAdminDetailsByUserNameApproved(string adminUser)
        {
            AdminUser adminUser1 = new AdminUser();

            if (adminUser != null)
            {
                //Get the Admin Details based on the AdminUserName         
                var _dbadmin = (from a in _context.Biz_Admin_Users
                                where a.Admin_User_Name == adminUser && !a.Is_Deleted
                                select new
                                {
                                    a.Admin_User_Id,
                                    a.Admin_User_Name,
                                    a.Admin_Password,
                                    a.Admin_Salt,
                                    a.Admin_Role,
                                    a.Projects,
                                    a.Is_Approved
                                }).SingleOrDefault();
                if (_dbadmin != null)
                {
                    //Assign the admin details to admin model entity from the above _dbadmin object
                    adminUser1.AdminUserName = _dbadmin.Admin_User_Name;
                    adminUser1.AdminPassword = _dbadmin.Admin_Password;
                    string _adminRole = GetAdminRoleById(_dbadmin.Admin_Role);
                    adminUser1.AdminRoles = _adminRole;
                    adminUser1.AdminSalt = _dbadmin.Admin_Salt;
                    adminUser1.AdminUserId = _dbadmin.Admin_User_Id;
                    adminUser1.ProjectType = _dbadmin.Projects;
                    adminUser1.IsAdmin = CheckForAdminRole(_dbadmin.Admin_User_Id);
                    adminUser1.AdminSKUType = _adminRole;
                    adminUser1.IsApproved = _dbadmin.Is_Approved ?? false;
                }
            }
            //return the admin details.
            return adminUser1;

        }

        #endregion

        #region Get UWAdmin State By Id
        /// <summary>
        /// Get Admin Stste By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public List<LeadLogState> GetAllAdminState()
        {
            List<LeadLogState> _stateList = new List<LeadLogState>();
            var _dbStateList = (from s in _context.Static_Biz_Admin_States select s).ToList();

            if (_dbStateList != null && _dbStateList.Count > 0)
            {
                foreach (var _dbState in _dbStateList)
                {
                    LeadLogState _state = new LeadLogState();
                    _state.StateId = _dbState.State_Id;
                    _state.CountryId = _dbState.Country_Id;
                    _state.StateName = _dbState.State_Name;
                    _state.StateCode = _dbState.State_Code;
                    _stateList.Add(_state);
                }
            }

            return _stateList;

        }
        #endregion

        #region Save Activity Log

        public AdminActivityLog SaveActivityLog(AdminActivityLog adminActivityLog)
        {
            Biz_SCActivityLog _dbBiz_SCActivityLog = null;

            bool IsRecordExists = false;

            if (adminActivityLog.SCActivityLogId > 0)
            {
                _dbBiz_SCActivityLog = _context.Biz_SCActivityLog.SingleOrDefault(au => au.SCActivityLog_Id == adminActivityLog.SCActivityLogId && au.Is_Deleted == false);
            }
            if (_dbBiz_SCActivityLog != null && _dbBiz_SCActivityLog.SCActivityLog_Id > 0)
            {
                IsRecordExists = true;
            }
            else
            {
                _dbBiz_SCActivityLog = new Biz_SCActivityLog();
            }
            _dbBiz_SCActivityLog.Action = adminActivityLog.Action;
            _dbBiz_SCActivityLog.Additional_Info_1 = adminActivityLog.AdditionalInfo1;
            _dbBiz_SCActivityLog.Additional_Info_2 = adminActivityLog.AdditionalInfo2;
            _dbBiz_SCActivityLog.Additional_Info_3 = adminActivityLog.AdditionalInfo3;
            _dbBiz_SCActivityLog.Admin_User_Id = adminActivityLog.AdminUserId;
            _dbBiz_SCActivityLog.Ip_Address = adminActivityLog.IpAddress;
            _dbBiz_SCActivityLog.Project_Id = adminActivityLog.ProjectId;
            _dbBiz_SCActivityLog.Visted_URL = adminActivityLog.VistedURL;

            _dbBiz_SCActivityLog.Is_Deleted = false;
            _dbBiz_SCActivityLog.Update_Time_Stamp = DateTime.Now;
            if (!IsRecordExists)
            {
                _dbBiz_SCActivityLog.Create_Time_Stamp = DateTime.Now;
                _context.Biz_SCActivityLog.Add(_dbBiz_SCActivityLog);
            }
            _context.SaveChanges();
            adminActivityLog.SCActivityLogId = _dbBiz_SCActivityLog.SCActivityLog_Id;
            return adminActivityLog;

        }
        #endregion

        #region  Get All Activity Log
        public List<AdminActivityLog> GetAllActivityLog()
        {
            List<AdminActivityLog> _adminList = null;

            var _dbadmin = (from al in _context.Biz_SCActivityLog
                            join u in _context.Biz_Admin_Users
                            on al.Admin_User_Id equals u.Admin_User_Id
                            join p in _context.Static_Biz_Admin_Project
                            on al.Project_Id equals p.Project_Id
                            select new
                            {
                                al.Admin_User_Id,
                                u.Admin_User_Name,
                                al.Ip_Address,
                                al.Visted_URL,
                                p.Project_Name,
                                al.Action,
                                al.Create_Time_Stamp

                            });


            if (_dbadmin != null && _dbadmin.Count() > 0)
            {
                _adminList = new List<AdminActivityLog>();
                foreach (var _thisadminUser in _dbadmin)
                {
                    AdminActivityLog _adminuser = new AdminActivityLog();

                    _adminuser.AdminUserName = _thisadminUser.Admin_User_Name;
                    _adminuser.IpAddress = _thisadminUser.Ip_Address;
                    _adminuser.VistedURL = _thisadminUser.Visted_URL;
                    _adminuser.ProjectName = _thisadminUser.Project_Name;
                    _adminuser.Action = _thisadminUser.Action;
                    _adminuser.UserCreatedOn = Convert.ToDateTime(_thisadminUser.Create_Time_Stamp);
                    _adminList.Add(_adminuser);
                }
            }
            return _adminList;

        }
        #endregion

        #region GetAll ActivityLog By User Id
        public List<AdminActivityLog> GetActivityLogByUserID(long Id)
        {
            List<AdminActivityLog> _adminList = new List<AdminActivityLog>();

            var _dbadmin = (from al in _context.Biz_SCActivityLog
                            join u in _context.Biz_Admin_Users
                            on al.Admin_User_Id equals u.Admin_User_Id
                            join p in _context.Static_Biz_Admin_Project
                            on al.Project_Id equals p.Project_Id
                            where al.Admin_User_Id == Id && !u.Is_Deleted

                            select new
                            {
                                al.Admin_User_Id,
                                u.Admin_User_Name,
                                al.Ip_Address,
                                al.Visted_URL,
                                al.Project_Id,
                                p.Project_Name,
                                al.Action,
                                al.Create_Time_Stamp
                            });

            if (_dbadmin != null && _dbadmin.Count() > 0)
            {
                foreach (var _thisadminUser in _dbadmin)
                {
                    AdminActivityLog _adminuser = new AdminActivityLog();

                    _adminuser.AdminUserName = _thisadminUser.Admin_User_Name;
                    _adminuser.IpAddress = _thisadminUser.Ip_Address;
                    _adminuser.VistedURL = _thisadminUser.Visted_URL;
                    _adminuser.ProjectId = Utilities.DataUtility.GetLong(_thisadminUser.Project_Id);
                    _adminuser.ProjectName = _thisadminUser.Project_Name;
                    _adminuser.Action = _thisadminUser.Action;
                    _adminuser.UserCreatedOn = Convert.ToDateTime(_thisadminUser.Create_Time_Stamp);
                    _adminList.Add(_adminuser);
                }
            }
            return _adminList;

        }
        #endregion

        #region Mobile Verification Code

        #region Save Mobile Verification

        /// <summary>
        ///     Save Mobile Verification
        /// </summary>
        /// <param name="mobileverification"></param>
        /// <returns></returns>
        public MobileVerification SendVerificationCode(MobileVerification mobileverification)
        {
            var isRecordExist = false;

            using (var entities = new MainControlDB_Entities())
            {
                var dbMobileVerification = (from mv in entities.Mobile_Verification
                                            where mv.User_Id == mobileverification.UserId && !mv.IsDeleted
                                            select mv).SingleOrDefault();

                if (dbMobileVerification != null && dbMobileVerification.Mobile_Verification_Id > 0)
                {
                    isRecordExist = true;
                }
                else
                {
                    dbMobileVerification = new Mobile_Verification();
                }
                dbMobileVerification.User_Id = mobileverification.UserId;
                dbMobileVerification.Verification_Code = mobileverification.VerificationCode;
                dbMobileVerification.Phone_Number = mobileverification.MobileNumber;
                dbMobileVerification.IsDeleted = false;
                dbMobileVerification.UpdateT_ime_Stamp = DateTime.Now;

                if (!isRecordExist)
                {
                    dbMobileVerification.Create_Time_Stamp = DateTime.Now;
                    entities.Mobile_Verification.Add(dbMobileVerification);
                }
                entities.SaveChanges();
                mobileverification.MobileverificationId = dbMobileVerification.Mobile_Verification_Id;
            }

            return mobileverification;
        }

        #endregion

        #region Update Mobile Verification Details
        /// <summary>
        /// Update Mobile Verification Details
        /// </summary>
        /// <param name="mobileVerification"></param>
        /// <returns></returns>
        public MobileVerification UpdateMobileVerificationDetails(MobileVerification mobileVerification)
        {
            if (mobileVerification.MobileverificationId > 0)
            {
                using (var entities = new MainControlDB_Entities())
                {
                    var dbMobileVerification = (from mv in entities.Mobile_Verification
                                                where mv.User_Id == mobileVerification.UserId && !mv.IsDeleted
                                                select mv).SingleOrDefault();

                    if (dbMobileVerification != null && dbMobileVerification.Mobile_Verification_Id > 0)
                    {
                        dbMobileVerification.Unique_Id = mobileVerification.UniqueId;
                        dbMobileVerification.Phone_Number = mobileVerification.MobileNumber;
                        dbMobileVerification.Status = "OPEN";
                        dbMobileVerification.UpdateT_ime_Stamp = DateTime.Now;
                        entities.SaveChanges();
                    }
                }

            }
            return mobileVerification;
        }
        #endregion

        #region Verify Mobile Code
        /// <summary>
        ///  Verify Mobile Code
        /// </summary>
        /// <param name="mobileverification"></param>
        /// <returns></returns>
        public bool VerifyMobileCode(MobileVerification mobileverification)
        {
            var isActualCode = false;

            using (var entities = new MainControlDB_Entities())
            {
                var dbVerificationId = (from mv in entities.Mobile_Verification
                                        where mv.User_Id == mobileverification.UserId && mv.Verification_Code == mobileverification.VerificationCode && !mv.IsDeleted
                                        select mv.Mobile_Verification_Id).SingleOrDefault();
                if (dbVerificationId > 0)
                {
                    isActualCode = true;
                }
            }
            return isActualCode;
        }
        #endregion

        #region Get Verification Code By UniqueId
        /// <summary>
        /// Get Verification Code By UniqueId
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public string GetVerificationCodeByUniqueId(string uniqueId)
        {
            string verificationCode = string.Empty;

            using (var entities = new MainControlDB_Entities())
            {
                var dbmobileVerificationCode = entities.Mobile_Verification.SingleOrDefault(mv => mv.Unique_Id == uniqueId && mv.Status == "SEND" && !mv.IsDeleted);

                if (dbmobileVerificationCode != null && !string.IsNullOrEmpty(dbmobileVerificationCode.Verification_Code) && dbmobileVerificationCode.Status == "SEND")
                {
                    verificationCode = dbmobileVerificationCode.Verification_Code;
                }

            }

            return verificationCode;
        }
        #endregion

        #region Update Mobile Verification Status
        /// <summary>
        /// Update Mobile Verification Status
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public bool UpdateMobileVerificationStatus(string uniqueId)
        {
            bool isUpdateStatus = false;
            using (var entities = new MainControlDB_Entities())
            {
                var dbMobileVerification = entities.Mobile_Verification.SingleOrDefault(mv => mv.Unique_Id == uniqueId && !mv.IsDeleted);

                if (dbMobileVerification != null && dbMobileVerification.Mobile_Verification_Id > 0)
                {
                    dbMobileVerification.Status = "SEND";
                    dbMobileVerification.UpdateT_ime_Stamp = DateTime.Now;
                    entities.SaveChanges();
                    isUpdateStatus = true;
                }
            }
            return isUpdateStatus;
        }
        #endregion

        #region Get Verification Code By UserId
        /// <summary>
        /// Get Verification Code By UserId
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public string GetVerificationCodeByUserId(long userId)
        {
            string verificationCode = string.Empty;

            using (var entities = new MainControlDB_Entities())
            {
                verificationCode = (from mv in entities.Mobile_Verification
                                    where mv.User_Id == userId && (mv.Status == "SEND" || mv.Status == "OPEN") && !mv.IsDeleted
                                    orderby mv.UpdateT_ime_Stamp descending
                                    select mv.Verification_Code).FirstOrDefault();
            }

            return !string.IsNullOrEmpty(verificationCode) ? verificationCode : string.Empty;
        }
        #endregion

        #endregion

        #region GetAllIpAddressList
        /// <summary>
        /// GetAllIpAddressList
        /// </summary>
        /// <returns></returns>
        public List<AdminIpAddress> GetAllIpAddressList()
        {
            using (var entities = new MainControlDB_Entities())
            {
                return (from ip in entities.Biz_Ip_Address
                        join sbp in entities.Static_Biz_Admin_Project
                        on ip.Project_id equals sbp.Project_Id
                        where !ip.Is_Deleted && !(sbp.Is_Deleted ?? false)
                        select new Core.Models.AdminIpAddress
                        {
                            IpAddress = ip.Ip_Address,
                            ProjectId = ip.Project_id ?? 0,
                            ProjectName = sbp.Project_Name,
                            IpAddressId = ip.Ip_Address_Id,
                            IpName = ip.Ip_Name,
                            IsStatic = ip.Is_Static ?? false
                        }).ToList();
            }
        }
        #endregion

        #region GetIpAddressDetailsById
        /// <summary>
        /// Get Ip Address Details ById
        /// </summary>
        /// <param name="ipAddressId"></param>
        /// <returns></returns>
        public AdminIpAddress GetIpAddressDetailsById(long ipAddressId)
        {
            using (var entities = new MainControlDB_Entities())
            {
                return (from ip in entities.Biz_Ip_Address
                        join sbp in entities.Static_Biz_Admin_Project
                        on ip.Project_id equals sbp.Project_Id
                        where !ip.Is_Deleted && !(sbp.Is_Deleted ?? false) && ip.Ip_Address_Id == ipAddressId
                        select new AdminIpAddress
                        {
                            IpAddress = ip.Ip_Address,
                            ProjectId = ip.Project_id ?? 0,
                            ProjectName = sbp.Project_Name,
                            IpAddressId = ip.Ip_Address_Id,
                            IpName = ip.Ip_Name,
                            IsStatic = ip.Is_Static ?? false
                        }).SingleOrDefault();
            }
        }
        #endregion

        #region DeleteallassignedProjects
        /// <summary>
        /// DeleteallassignedProjects
        /// </summary>
        /// <param name="ipAddressId"></param>
        /// <returns></returns>
        public bool DeleteallassignedProjects(string ipAddress)
        {
            using (var entities = new MainControlDB_Entities())
            {
                var dbBizIpAddress = entities.Biz_Ip_Address.Where(du => du.Ip_Address == ipAddress && !du.Is_Deleted).ToList();
                if (dbBizIpAddress != null && dbBizIpAddress.Count > 0)
                {
                    foreach (var item in dbBizIpAddress)
                    {
                        item.Is_Deleted = true;
                        item.Update_Time_Stamp = DateTime.Now;
                        entities.SaveChanges();
                    }
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region DeleteIpAddressDetailsById
        /// <summary>
        /// Delete Ip Address Details By Id
        /// </summary>
        /// <param name="ipAddressId"></param>
        /// <returns></returns>
        public bool DeleteIpAddressDetailsById(long ipAddressId, bool ismultiple)
        {
            using (var entities = new MainControlDB_Entities())
            {
                var dbBizIpAddress = entities.Biz_Ip_Address.SingleOrDefault(du => du.Ip_Address_Id == ipAddressId && !du.Is_Deleted);
                if (dbBizIpAddress != null && dbBizIpAddress.Ip_Address_Id > 0)
                {
                    dbBizIpAddress.Is_Deleted = true;
                    dbBizIpAddress.Update_Time_Stamp = DateTime.Now;
                    entities.SaveChanges();
                    if (ismultiple) // Remove all assigned Projects
                    {
                        return DeleteallassignedProjects(dbBizIpAddress.Ip_Address);
                    }
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region Save Ip Address Details By Id
        /// <summary>
        /// SaveIpAddressDetailsById
        /// </summary>
        /// <param name="adminIpAddress"></param>
        public bool SaveIpAddressDetailsById(AdminIpAddress adminIpAddress)
        {
            using (var entities = new MainControlDB_Entities())
            {
                if (adminIpAddress != null && !string.IsNullOrEmpty(adminIpAddress.ProjectName) && adminIpAddress.ProjectName != "All Project")
                {
                    var dbAdminIpAddress = entities.Biz_Ip_Address.SingleOrDefault(r => r.Ip_Address_Id == adminIpAddress.IpAddressId && !r.Is_Deleted);
                    var isRecordExist = false;
                    if (dbAdminIpAddress != null)
                    {
                        isRecordExist = true;
                    }
                    else
                    {
                        dbAdminIpAddress = new Biz_Ip_Address();
                    }
                    dbAdminIpAddress.Ip_Address = adminIpAddress.IpAddress;
                    dbAdminIpAddress.Project_id = adminIpAddress.ProjectId > 0 ? adminIpAddress.ProjectId : entities.Static_Biz_Admin_Project.Where(hc => hc.Project_Name == adminIpAddress.ProjectName).Select(hc => hc.Project_Id).SingleOrDefault();
                    dbAdminIpAddress.Ip_Name = adminIpAddress.IpName;
                    dbAdminIpAddress.Is_Deleted = false;
                    dbAdminIpAddress.Update_Time_Stamp = DateTime.Now;
                    dbAdminIpAddress.Is_Static = adminIpAddress.IsStatic;
                    if (!isRecordExist)
                    {
                        dbAdminIpAddress.Create_Time_Stamp = DateTime.Now;
                        entities.Biz_Ip_Address.Add(dbAdminIpAddress);
                    }
                    entities.SaveChanges();

                    if (dbAdminIpAddress.Ip_Address_Id > 0)
                    {
                        return true;
                    }
                }
                else
                {
                    SaveAllProjectIpAddress(adminIpAddress.IpAddress, adminIpAddress.IpName, adminIpAddress.IsStatic);
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region Get Static Biz Admin Projects
        /// <summary>
        /// GetStaticBizAdminProjects
        /// </summary>
        /// <returns></returns>
        public List<BizAdminProjects> GetStaticBizAdminProjects()
        {

            using (var entities = new MainControlDB_Entities())
            {
                return (from sbp in entities.Static_Biz_Admin_Project
                        where !sbp.Is_Deleted ?? true
                        orderby sbp.Project_Name ascending
                        select new BizAdminProjects
                        {
                            ProjectId = sbp.Project_Id,
                            ProjectName = sbp.Project_Name,
                            ProjectDescription = sbp.Project_Description
                        }).ToList();
            }
        }
        #endregion

        #region Add Admin

        public bool AddAdmin(long Id)
        {
            bool _isUpdateStatus = false;

            string admin = AdminRoleType.Administrator.ToString();

            if (Id > 0)
            {
                var _dbAdminUser = (from a in _context.Biz_Admin_Users
                                    where a.Admin_User_Id == Id && !a.Is_Deleted
                                    select a).SingleOrDefault();

                if (_dbAdminUser != null && _dbAdminUser.Admin_User_Id > 0)
                {

                    var Admin_Role_Id = (from a in _context.Biz_Admin_Roles
                                         where a.Admin_Role == admin && !a.Is_Deleted
                                         select a.Admin_Role_Id).SingleOrDefault();

                    _dbAdminUser.Admin_Role = Admin_Role_Id;
                    _dbAdminUser.Update_Time_Stamp = DateTime.Now;
                    _context.SaveChanges();
                    _isUpdateStatus = true;
                }
            }
            return _isUpdateStatus;
        }
        #endregion

        #region Remove Admin

        public bool RemoveAdmin(long Id)
        {
            bool _isUpdateStatus = false;
            string team = AdminRoleType.Team.ToString();
            if (Id > 0)
            {
                var _dbAdminUser = (from a in _context.Biz_Admin_Users
                                    where a.Admin_User_Id == Id && !a.Is_Deleted
                                    select a).SingleOrDefault();
                if (_dbAdminUser != null && _dbAdminUser.Admin_User_Id > 0)
                {
                    var Admin_Role_Id = (from a in _context.Biz_Admin_Roles
                                         where a.Admin_Role == team && !a.Is_Deleted
                                         select a.Admin_Role_Id).SingleOrDefault();

                    _dbAdminUser.Admin_Role = Admin_Role_Id;
                    _dbAdminUser.Update_Time_Stamp = DateTime.Now;
                    _context.SaveChanges();
                    _isUpdateStatus = true;
                }
            }
            return _isUpdateStatus;
        }
        #endregion

        #region  Get All Admin List
        /// <summary>
        /// Get All Admin List
        /// </summary>
        /// <returns></returns>
        public List<AdminUser> GetAllAdminList()
        {
            using (var entities = new MainControlDB_Entities())
            {
                string admin = AdminRoleType.Administrator.ToString();
                var Admin_Role_Id = (from a in _context.Biz_Admin_Roles
                                     where a.Admin_Role == admin && !a.Is_Deleted
                                     select a.Admin_Role_Id).SingleOrDefault();

                return (from bau in entities.Biz_Admin_Users
                        where bau.Admin_Role == Admin_Role_Id && !string.IsNullOrEmpty(bau.Phone_Number) && !string.IsNullOrEmpty(bau.Admin_Email_Address) && !bau.Is_Deleted
                        select new AdminUser
                        {
                            AdminEmailAddress = bau.Admin_Email_Address,
                            PhoneNumber = bau.Phone_Number
                        }).ToList();
            }
        }
        #endregion

        #region Save Activity Log Details
        /// <summary>
        /// Save Activity Log Details
        /// </summary>
        /// <param name="scActivityLogDetails"></param>
        /// <returns></returns>
        public bool SaveActivityLogDetails(ScActivityLog scActivityLogDetails)
        {
            bool isSaved = false;
            if (scActivityLogDetails != null)
            {
                using (var entities = new MainControlDB_Entities())
                {
                    var dbActivityLogDetail = new Sc_Activity_Log();

                    if (scActivityLogDetails.UserId > 0)
                    {
                        dbActivityLogDetail.User_Id = scActivityLogDetails.UserId;

                        //Get  display name by user id
                        var adminUserDetails = entities.Biz_Admin_Users.SingleOrDefault(x => x.Admin_User_Id == scActivityLogDetails.UserId && !x.Is_Deleted);

                        if (adminUserDetails != null)
                        {
                            dbActivityLogDetail.Display_Name = adminUserDetails.Admin_First_Name + " " + adminUserDetails.Admin_Last_Name;
                        }
                    }
                    else
                    {
                        dbActivityLogDetail.Display_Name = string.Empty;
                    }

                    dbActivityLogDetail.Project_Id = scActivityLogDetails.ProjectId;
                    dbActivityLogDetail.Action_Type = scActivityLogDetails.ActionType;
                    dbActivityLogDetail.Action_Name = scActivityLogDetails.ActionName;

                    dbActivityLogDetail.Controller_Name = scActivityLogDetails.ControllerName;
                    dbActivityLogDetail.Activity = scActivityLogDetails.Activity;
                    dbActivityLogDetail.Memo = scActivityLogDetails.Memo;
                    dbActivityLogDetail.Activity_Date = scActivityLogDetails.ActivityDate;
                    dbActivityLogDetail.IP_Address = scActivityLogDetails.IPAddress;
                    dbActivityLogDetail.Update_Time_Stamp = DateTime.Now;
                    dbActivityLogDetail.Create_Time_Stamp = DateTime.Now;

                    entities.Sc_Activity_Log.Add(dbActivityLogDetail);
                    entities.SaveChanges();

                    if (dbActivityLogDetail.Sc_Activity_Log_Id > 0)
                    {
                        isSaved = true;
                    }
                }
            }
            return isSaved;
        }
        #endregion

        #region Get All Activity Log By Admin User Id
        /// <summary>
        /// Gets the activity log by user identifier.
        /// </summary>
        /// <param name="adminUserId">The admin user identifier.</param>
        /// <returns></returns>
        public List<ScActivityLog> GetActivityLogByAdminUserId(long adminUserId)
        {
            List<ScActivityLog> _adminList = new List<ScActivityLog>();
            using (var entities = new MainControlDB_Entities())
            {
                var _dbadmin = entities.Sc_Activity_Log.Where(x => x.User_Id == adminUserId && !x.Is_Deleted).OrderByDescending(x => x.Update_Time_Stamp).ToList();

                if (_dbadmin != null && _dbadmin.Count() > 0)
                {
                    foreach (var _thisadminUser in _dbadmin)
                    {
                        ScActivityLog _adminuser = new ScActivityLog();

                        _adminuser.ActivityLogId = _thisadminUser.Sc_Activity_Log_Id;
                        _adminuser.DisplayName = _thisadminUser.Display_Name;
                        _adminuser.ProjectId = _thisadminUser.Project_Id ?? 0;
                        if (_thisadminUser.Project_Id != null && _thisadminUser.Project_Id > 0)
                        {
                            _adminuser.ProjectName = entities.Static_Biz_Admin_Project.Where(x => x.Project_Id == _thisadminUser.Project_Id && x.Is_Deleted == false).Select(x => x.Project_Name).SingleOrDefault();
                        }
                        _adminuser.ActionType = _thisadminUser.Action_Type;
                        _adminuser.ActionName = _thisadminUser.Action_Name;
                        _adminuser.ControllerName = _thisadminUser.Controller_Name;
                        _adminuser.Activity = _thisadminUser.Activity;
                        _adminuser.Memo = _thisadminUser.Memo;
                        _adminuser.ActivityDate = _thisadminUser.Activity_Date;
                        _adminuser.IPAddress = _thisadminUser.IP_Address;
                        _adminuser.UpdateTimeStamp = _thisadminUser.Update_Time_Stamp;
                        _adminuser.CreateTimeStamp = _thisadminUser.Create_Time_Stamp;

                        _adminList.Add(_adminuser);
                    }
                }
            }
            return _adminList;

        }
        #endregion

        #region Save All Project Ip Address
        /// <summary>
        /// SaveAllProjectIpAddress
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="ipName"></param>
        /// <param name="isStatic"></param>
        public void SaveAllProjectIpAddress(string ipAddress, string ipName, bool isStatic)
        {
            using (var entities = new MainControlDB_Entities())
            {
                var dbProjectIdList = (from sbp in entities.Static_Biz_Admin_Project
                                       where !sbp.Is_Deleted ?? false
                                       select sbp.Project_Id).ToList();
                if (dbProjectIdList != null && dbProjectIdList.Any())
                {
                    foreach (var dbProjectId in dbProjectIdList)
                    {
                        var dbAdminIpAddress = entities.Biz_Ip_Address.SingleOrDefault(r => r.Project_id == dbProjectId && r.Ip_Address == ipAddress && !r.Is_Deleted);
                        var isRecordExist = false;
                        if (dbAdminIpAddress != null)
                        {
                            isRecordExist = true;
                        }
                        else
                        {
                            dbAdminIpAddress = new Biz_Ip_Address();
                        }
                        dbAdminIpAddress.Ip_Address = ipAddress;
                        dbAdminIpAddress.Project_id = dbProjectId;
                        dbAdminIpAddress.Ip_Name = ipName;
                        dbAdminIpAddress.Is_Deleted = false;
                        dbAdminIpAddress.Update_Time_Stamp = DateTime.Now;
                        dbAdminIpAddress.Is_Static = isStatic;
                        if (!isRecordExist)
                        {
                            dbAdminIpAddress.Create_Time_Stamp = DateTime.Now;
                            entities.Biz_Ip_Address.Add(dbAdminIpAddress);
                        }
                        entities.SaveChanges();
                    }
                }
            }
        }
        #endregion

        #region IsProjectIpAlreadyExists
        /// <summary>
        /// IsProjectIpAlreadyExists
        /// </summary>
        /// <param name="projectIp"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public bool IsProjectIpAlreadyExists(long ipAddressId, string projectIp, string project)
        {
            using (var entities = new MainControlDB_Entities())
            {

                var dbAdminIpAddress = entities.Biz_Ip_Address.FirstOrDefault(r => (ipAddressId == 0 || r.Ip_Address_Id != ipAddressId) && r.Ip_Address == projectIp && r.Project_id == entities.Static_Biz_Admin_Project.Where(hc => hc.Project_Name == project).Select(hc => hc.Project_Id).FirstOrDefault() && !r.Is_Deleted);
                if (dbAdminIpAddress != null)
                {
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region Get Admin Activity Log Count
        /// <summary>
        /// Gets the admin activity log count.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns></returns>
        public int GetAdminActivityLogCount(JQueryDataTableParamModel param)
        {
            if (param.ToDate != DateTime.MinValue)
            {
                param.ToDate = param.ToDate.AddDays(1);
            }
            int totalRecordsCount = 0;
            if (param.FromDate == DateTime.MinValue && param.ToDate == DateTime.MinValue)
            {
                param.FromDate = DateTime.MinValue;
                param.ToDate = DateTime.MaxValue;
            }
            else if (param.ToDate == DateTime.MinValue)
            {
                param.ToDate = DateTime.Now.AddDays(1);
            }

            using (var _entities = new MainControlDB_Entities())
            {
                totalRecordsCount = (string.IsNullOrEmpty(param.sSearch) ? (param.AdminUserId > 0 ? param.ProjectId > 0 ? (from users in _entities.Sc_Activity_Log
                                                                                                                           where (!users.Is_Deleted && users.Project_Id == param.ProjectId && users.User_Id == param.AdminUserId && users.Create_Time_Stamp >= param.FromDate && users.Create_Time_Stamp <= param.ToDate)
                                                                                                                           select new
                                                                                                                           {
                                                                                                                               users.Sc_Activity_Log_Id
                                                                                                                           }).Count() :
                                                                                                     (from users in _entities.Sc_Activity_Log
                                                                                                      where (!users.Is_Deleted && users.User_Id == param.AdminUserId && users.Create_Time_Stamp >= param.FromDate && users.Create_Time_Stamp <= param.ToDate)
                                                                                                      select new
                                                                                                      {
                                                                                                          users.Sc_Activity_Log_Id
                                                                                                      }).Count()
                                                                                                : param.ProjectId > 0 ? (from users in _entities.Sc_Activity_Log
                                                                                                                         where (!users.Is_Deleted && users.Project_Id == param.ProjectId && users.Create_Time_Stamp >= param.FromDate && users.Create_Time_Stamp <= param.ToDate)
                                                                                                                         select new
                                                                                                                         {
                                                                                                                             users.Sc_Activity_Log_Id,
                                                                                                                         }).Count() :
                                                                                                   (from users in _entities.Sc_Activity_Log
                                                                                                    where (!users.Is_Deleted && users.User_Id == null && users.Create_Time_Stamp >= param.FromDate && users.Create_Time_Stamp <= param.ToDate)
                                                                                                    select new
                                                                                                    {
                                                                                                        users.Sc_Activity_Log_Id,
                                                                                                    }).Count())
                                             : (param.AdminUserId > 0 ? param.ProjectId > 0 ? (from users in _entities.Sc_Activity_Log
                                                                                               where (!users.Is_Deleted && users.Project_Id == param.ProjectId && users.User_Id == param.AdminUserId &&
                                                                                                  (users.Activity.Contains(param.sSearch) || users.Memo.Contains(param.sSearch) || users.IP_Address.Contains(param.sSearch)
                                                                                                  && users.Create_Time_Stamp >= param.FromDate && users.Create_Time_Stamp <= param.ToDate))
                                                                                               select new
                                                                                               {
                                                                                                   users.Sc_Activity_Log_Id,
                                                                                               }).Count() :
                                                                         (from users in _entities.Sc_Activity_Log
                                                                          where (!users.Is_Deleted && users.User_Id == param.AdminUserId &&
                                                                             (users.Activity.Contains(param.sSearch) || users.Memo.Contains(param.sSearch) || users.IP_Address.Contains(param.sSearch)
                                                                             && users.Create_Time_Stamp >= param.FromDate && users.Create_Time_Stamp <= param.ToDate))
                                                                          select new
                                                                          {
                                                                              users.Sc_Activity_Log_Id,
                                                                          }).Count()
                                                                          : param.ProjectId > 0 ? (from users in _entities.Sc_Activity_Log
                                                                                                   where (!users.Is_Deleted && users.Project_Id == param.ProjectId &&
                                                                                                   (users.Activity.Contains(param.sSearch) || users.Memo.Contains(param.sSearch) || users.IP_Address.Contains(param.sSearch)
                                                                                                   && users.Create_Time_Stamp >= param.FromDate && users.Create_Time_Stamp <= param.ToDate))
                                                                                                   select new
                                                                                                   {
                                                                                                       users.Sc_Activity_Log_Id,
                                                                                                   }).Count() :
                                                                             (from users in _entities.Sc_Activity_Log
                                                                              where (!users.Is_Deleted && users.User_Id == null &&
                                                                              (users.Activity.Contains(param.sSearch) || users.Memo.Contains(param.sSearch) || users.IP_Address.Contains(param.sSearch)
                                                                              && users.Create_Time_Stamp >= param.FromDate && users.Create_Time_Stamp <= param.ToDate))
                                                                              select new
                                                                              {
                                                                                  users.Sc_Activity_Log_Id,
                                                                              }).Count()
                                                                             )
                                                );
            }

            return totalRecordsCount;
        }
        #endregion

        #region Get Admin Activity Log List
        /// <summary>
        /// Gets the admin activity log list.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns></returns>
        public List<ScActivityLog> GetAdminActivityLogList(JQueryDataTableParamModel param)
        {
            List<ScActivityLog> adminLogList = new List<ScActivityLog>();
            //if (param.ToDate != DateTime.MinValue)
            //{
            //    param.ToDate = param.ToDate.AddDays(1);
            //}
            using (var _entities = new MainControlDB_Entities())
            {

                var _dbAdminLogList = (string.IsNullOrEmpty(param.sSearch) ? (param.AdminUserId > 0 ? param.ProjectId > 0 ? (from users in _entities.Sc_Activity_Log
                                                                                                                             where (!users.Is_Deleted && users.Project_Id == param.ProjectId && users.User_Id == param.AdminUserId && users.Create_Time_Stamp >= param.FromDate && users.Create_Time_Stamp <= param.ToDate)
                                                                                                                             orderby users.Update_Time_Stamp descending
                                                                                                                             select new
                                                                                                                             {
                                                                                                                                 users
                                                                                                                             }).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList() :
                                                                                                       (from users in _entities.Sc_Activity_Log
                                                                                                        where (!users.Is_Deleted && users.User_Id == param.AdminUserId && users.Create_Time_Stamp >= param.FromDate && users.Create_Time_Stamp <= param.ToDate)
                                                                                                        orderby users.Update_Time_Stamp descending
                                                                                                        select new
                                                                                                        {
                                                                                                            users
                                                                                                        }).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList()
                                                                                                : param.ProjectId > 0 ? (from users in _entities.Sc_Activity_Log
                                                                                                                         where (!users.Is_Deleted && users.Project_Id == param.ProjectId && users.Create_Time_Stamp >= param.FromDate && users.Create_Time_Stamp <= param.ToDate)
                                                                                                                         orderby users.Update_Time_Stamp descending
                                                                                                                         select new
                                                                                                                         {
                                                                                                                             users
                                                                                                                         }).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList() :
                                                                                                   (from users in _entities.Sc_Activity_Log
                                                                                                    where (!users.Is_Deleted && users.User_Id == null && users.Create_Time_Stamp >= param.FromDate && users.Create_Time_Stamp <= param.ToDate)
                                                                                                    orderby users.Update_Time_Stamp descending
                                                                                                    select new
                                                                                                    {
                                                                                                        users
                                                                                                    }).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList()
                                                                                                   )
                                             : (param.AdminUserId > 0 ? param.ProjectId > 0 ? (from users in _entities.Sc_Activity_Log
                                                                                               where (!users.Is_Deleted && users.Project_Id == param.ProjectId && users.User_Id == param.AdminUserId &&
                                                                                                  (users.Activity.Contains(param.sSearch) || users.Memo.Contains(param.sSearch) || users.IP_Address.Contains(param.sSearch)
                                                                                                  && users.Create_Time_Stamp >= param.FromDate && users.Create_Time_Stamp <= param.ToDate))
                                                                                               orderby users.Update_Time_Stamp descending
                                                                                               select new
                                                                                               {
                                                                                                   users
                                                                                               }).ToList() :
                                                                          (from users in _entities.Sc_Activity_Log
                                                                           where (!users.Is_Deleted && users.User_Id == param.AdminUserId &&
                                                                              (users.Activity.Contains(param.sSearch) || users.Memo.Contains(param.sSearch) || users.IP_Address.Contains(param.sSearch)
                                                                              && users.Create_Time_Stamp >= param.FromDate && users.Create_Time_Stamp <= param.ToDate))
                                                                           orderby users.Update_Time_Stamp descending
                                                                           select new
                                                                           {
                                                                               users
                                                                           }).ToList()

                                                                          : param.ProjectId > 0 ? (from users in _entities.Sc_Activity_Log
                                                                                                   where (!users.Is_Deleted && users.Project_Id == param.ProjectId &&
                                                                                                   (users.Activity.Contains(param.sSearch) || users.Memo.Contains(param.sSearch) || users.IP_Address.Contains(param.sSearch)
                                                                                                   && users.Create_Time_Stamp >= param.FromDate && users.Create_Time_Stamp <= param.ToDate))
                                                                                                   orderby users.Update_Time_Stamp descending
                                                                                                   select new
                                                                                                   {
                                                                                                       users
                                                                                                   }).ToList() :
                                                                             (from users in _entities.Sc_Activity_Log
                                                                              where (!users.Is_Deleted && users.User_Id == null &&
                                                                              (users.Activity.Contains(param.sSearch) || users.Memo.Contains(param.sSearch) || users.IP_Address.Contains(param.sSearch)
                                                                              && users.Create_Time_Stamp >= param.FromDate && users.Create_Time_Stamp <= param.ToDate))
                                                                              orderby users.Update_Time_Stamp descending
                                                                              select new
                                                                              {
                                                                                  users
                                                                              }).ToList()
                                                                             )
                                                );
                // var _dbAdminLogList = !string.IsNullOrWhiteSpace(param.sSearch) ?

                // (from users in _entities.Sc_Activity_Log
                //  where (!users.Is_Deleted &&
                //  users.Create_Time_Stamp >= param.FromDate &&
                //  users.Create_Time_Stamp <= param.ToDate)
                //  orderby users.Update_Time_Stamp descending
                //  select new
                //  {
                //      users
                //  }).Where(a => (a.users.Activity.ToLower().Contains(param.sSearch)
                //      || a.users.Memo.ToLower().Contains(param.sSearch)
                //      || a.users.IP_Address.ToLower().Contains(param.sSearch))).ToList() :
                //         (param.AdminUserId > 0 && param.FromDate != DateTime.MinValue && param.ToDate != DateTime.MinValue ?
                //(from users in _entities.Sc_Activity_Log
                // where ((users.Update_Time_Stamp > param.FromDate && users.Update_Time_Stamp < param.ToDate) || users.Update_Time_Stamp == param.FromDate || users.Update_Time_Stamp == param.ToDate)
                //&& !users.Is_Deleted && users.User_Id == param.AdminUserId
                // orderby users.Update_Time_Stamp descending
                // select new
                // {
                //     users
                // }).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList() :

                // (from users in _entities.Sc_Activity_Log
                //  where (!users.Is_Deleted && users.User_Id == null &&
                //  users.Create_Time_Stamp >= param.FromDate &&
                //  users.Create_Time_Stamp <= param.ToDate)
                //  orderby users.Update_Time_Stamp descending
                //  select new
                //  {
                //      users
                //  }).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList());

                if (_dbAdminLogList != null && _dbAdminLogList.Count > 0)
                {
                    foreach (var _dbUser in _dbAdminLogList)
                    {
                        if (_dbUser != null)
                        {
                            ScActivityLog _adminuser = new ScActivityLog();

                            _adminuser.ActivityLogId = _dbUser.users.Sc_Activity_Log_Id;
                            _adminuser.DisplayName = _dbUser.users.Display_Name;
                            _adminuser.ProjectId = _dbUser.users.Project_Id ?? 0;
                            if (_dbUser.users.Project_Id != null && _dbUser.users.Project_Id > 0)
                            {
                                _adminuser.ProjectName = _entities.Static_Biz_Admin_Project.Where(x => x.Project_Id == _dbUser.users.Project_Id && x.Is_Deleted == false).Select(x => x.Project_Name).SingleOrDefault();
                            }
                            _adminuser.ActionType = _dbUser.users.Action_Type;
                            _adminuser.ActionName = _dbUser.users.Action_Name;
                            _adminuser.ControllerName = _dbUser.users.Controller_Name;
                            _adminuser.Activity = _dbUser.users.Activity;
                            _adminuser.Memo = _dbUser.users.Memo;
                            _adminuser.ActivityDate = _dbUser.users.Activity_Date;
                            _adminuser.IPAddress = _dbUser.users.IP_Address;
                            _adminuser.UpdateTimeStamp = _dbUser.users.Update_Time_Stamp;
                            _adminuser.CreateTimeStamp = _dbUser.users.Create_Time_Stamp;

                            adminLogList.Add(_adminuser);
                        }
                    }
                }
            }
            return adminLogList;
        }
        #endregion

        #region Get All Admin projects
        /// <summary>
        /// Gets all projects.
        /// </summary>
        /// <returns></returns>
        public List<AdminProject> GetAllProjects()
        {
            List<AdminProject> _dbAdminProjectList = new List<AdminProject>();
            _dbAdminProjectList = (from a in _context.Static_Biz_Admin_Project
                                   join b in _context.Static_Biz_Admin_Category_Project on a.Project_Id equals b.Project_Id
                                   where a.Is_Deleted == false && b.Is_Deleted == false //&& b.Category_Id == categoryid 
                                   select new AdminProject
                                   {
                                       AdminProjectId = a.Project_Id,
                                       ProjectName = a.Project_Name,
                                   }).ToList();
            return _dbAdminProjectList;
        }
        #endregion

        #region Get Project Name By Project Id
        public AdminProject GetProjectNameByProjectId(int projectId)
        {
            string projectName = string.Empty;
            var dbProject = (from a in _context.Static_Biz_Admin_Project
                             where a.Is_Deleted == false && a.Project_Id == projectId
                             select new AdminProject
                             {
                                 AdminProjectId = a.Project_Id,
                                 ProjectName = a.Project_Name,
                                 TechnicalTeamEmail = a.Technical_Team_Email,
                                 PaymentLink = a.Payment_Url,
                                 PaymentBccAddress = a.Payment_Bcc_Address,
                                 PaymentCcAddress = a.Payment_Cc_Address,
                                 PaymentFromEmail = a.Payment_From_Email
                             }).SingleOrDefault();

            return dbProject;
        }
        #endregion

        #region Get Admin Support Users By Project Id
        public List<AdminUser> GetAdminSupportUsersByProjectId(int projectId)
        {
            var dbAdminSupportUserRoles = (from roles in _context.Biz_Admin_User_Roles
                                           join users in _context.Biz_Admin_Users on roles.Admin_User_Id equals users.Admin_User_Id
                                           where !roles.Is_Deleted && !users.Is_Deleted
                                           //&& roles.Admin_Project_Id == projectId 
                                           && roles.Admin_Role_Id == (int)AdminRoleType.Team // Support Users
                                           select new AdminUser
                                           {
                                               UserId = roles.Admin_User_Id,
                                               UserName = users.Admin_User_Name,
                                               AdminLocation = users.Location
                                           }).Distinct().ToList();

            return dbAdminSupportUserRoles;
        }
        #endregion

        #region Create CampainDetail
        public CampaignDetails CreateCampainDetails(CampaignDetails campaignDetails)
        {
            if (campaignDetails != null && campaignDetails.AdminUserId > 0 && !string.IsNullOrWhiteSpace(campaignDetails.CampaignName))
            {

                Biz_Campaign_Details dbBizCampaignDetails = null;

                dbBizCampaignDetails = _context.Biz_Campaign_Details.Where(m => m.Campaign_Details_Id == campaignDetails.CampaignDetailsId).SingleOrDefault();
                bool IsRecordExists = false;

                if (dbBizCampaignDetails != null && dbBizCampaignDetails.Campaign_Details_Id > 0)
                {
                    IsRecordExists = true;
                }
                else
                {
                    dbBizCampaignDetails = new Biz_Campaign_Details();
                }

                dbBizCampaignDetails.Admin_User_Id = campaignDetails.AdminUserId;
                dbBizCampaignDetails.Campaign_Name = campaignDetails.CampaignName;
                dbBizCampaignDetails.Campaign_File_Name = campaignDetails.CampaignFileName;
                dbBizCampaignDetails.File_Path = campaignDetails.FilePath;
                dbBizCampaignDetails.Goals = campaignDetails.Goals;
                dbBizCampaignDetails.No_Of_Leads = campaignDetails.NoOfLeads;
                dbBizCampaignDetails.Unique_Id = campaignDetails.UniqueId;
                dbBizCampaignDetails.Is_Paused = campaignDetails.IsPaused;
                dbBizCampaignDetails.Is_Suspended = campaignDetails.IsSuspended;
                dbBizCampaignDetails.Admin_Project_Id = campaignDetails.ProductId;
                dbBizCampaignDetails.Campaign_Start_Date = campaignDetails.CampaignStartDate;
                dbBizCampaignDetails.Campaign_End_Date = campaignDetails.CampaignEndDate;
                dbBizCampaignDetails.Is_Upload_File_Assign = campaignDetails.IsUploadFileAssign;
                dbBizCampaignDetails.Tech_Team_Status = campaignDetails.TechTeamStatus;
                dbBizCampaignDetails.Campaign_Type = campaignDetails.CampaignType;
                dbBizCampaignDetails.DemoGrapic_Information = !string.IsNullOrWhiteSpace(campaignDetails.DemoGrapicInformation)
                                                              && campaignDetails.DemoGrapicInformation.Length > 500 ? campaignDetails.DemoGrapicInformation.Substring(0, 499) : campaignDetails.DemoGrapicInformation;
                dbBizCampaignDetails.Tech_Team_File_Uploaded_Time = campaignDetails.TechTeamFileUploadedTime;
                dbBizCampaignDetails.Batch_Status = campaignDetails.BatchStatus;
                dbBizCampaignDetails.Is_Manager_Follow_Up = campaignDetails.IsManagerFollowUp;

                dbBizCampaignDetails.Is_Deleted = false;
                dbBizCampaignDetails.Updated_Time_Stamp = DateTime.Now;

                if (!IsRecordExists)
                {
                    dbBizCampaignDetails.Created_Time_Stamp = DateTime.Now;

                    _context.Biz_Campaign_Details.Add(dbBizCampaignDetails);

                }

                _context.SaveChanges();

                campaignDetails.CampaignDetailsId = dbBizCampaignDetails.Campaign_Details_Id;


                if (campaignDetails.ChampaignExcelHeaderDetailsList != null && campaignDetails.ChampaignExcelHeaderDetailsList.Count > 0)
                {
                    SaveCampaignExcelHeaderDetails(campaignDetails);
                }

                if (campaignDetails.CampaignAssignedDetailsList != null && campaignDetails.CampaignAssignedDetailsList.Count > 0)
                {
                    SaveCampaignAssignedDetails(campaignDetails);
                }

                if (campaignDetails.CampaignSupportUserDetailsList != null && campaignDetails.CampaignSupportUserDetailsList.Count > 0)
                {
                    campaignDetails.CampaignDetailsId = dbBizCampaignDetails.Campaign_Details_Id;
                    SaveCampaignSupportUserDetails(campaignDetails);
                }
            }
            return campaignDetails;
        }

        public void SaveCampaignSupportUserDetails(CampaignDetails campaignDetails)
        {
            if (campaignDetails.CampaignSupportUserDetailsList != null && campaignDetails.CampaignSupportUserDetailsList.Count > 0)
            {
                foreach (var campaignSupportUserDetails in campaignDetails.CampaignSupportUserDetailsList)
                {
                    Biz_Campaign_Support_User_Details dbBizCampaignSupportUserDetails = null;
                    if (campaignDetails.IsManagerFollowUp && campaignDetails.CampaignDetailsId > 0)
                    {
                        campaignSupportUserDetails.CampaignSupportUserDetailId = _context.Biz_Campaign_Support_User_Details.Where(m => m.Campaign_Details_Id == campaignDetails.CampaignDetailsId && !m.Is_Deleted && m.Support_User_Id == campaignSupportUserDetails.SupportUserId).Select(n => n.Campaign_Support_User_Detail_Id).SingleOrDefault();
                    }
                    dbBizCampaignSupportUserDetails = _context.Biz_Campaign_Support_User_Details.Where(m => m.Campaign_Support_User_Detail_Id == campaignSupportUserDetails.CampaignSupportUserDetailId).SingleOrDefault();
                    bool IsRecordExists = false;

                    if (dbBizCampaignSupportUserDetails != null && dbBizCampaignSupportUserDetails.Campaign_Support_User_Detail_Id > 0)
                    {
                        IsRecordExists = true;
                    }
                    else
                    {
                        dbBizCampaignSupportUserDetails = new Biz_Campaign_Support_User_Details();
                    }

                    dbBizCampaignSupportUserDetails.Campaign_Details_Id = campaignDetails.CampaignDetailsId;
                    dbBizCampaignSupportUserDetails.Admin_User_Id = campaignDetails.AdminUserId;
                    dbBizCampaignSupportUserDetails.Support_User_Id = campaignSupportUserDetails.SupportUserId;
                    dbBizCampaignSupportUserDetails.No_Of_User_Assigned = campaignSupportUserDetails.NoOfUserAssigned;
                    dbBizCampaignSupportUserDetails.No_Of_Completed = campaignSupportUserDetails.NoOfCompleted;
                    dbBizCampaignSupportUserDetails.No_Of_Pending = campaignSupportUserDetails.NoOfPending;
                    dbBizCampaignSupportUserDetails.User_Skipped_Count = campaignSupportUserDetails.UserSkippedCount;
                    dbBizCampaignSupportUserDetails.Is_Viewed = campaignSupportUserDetails.IsViewed;
                    dbBizCampaignSupportUserDetails.Is_Deleted = false;
                    dbBizCampaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;

                    if (!IsRecordExists)
                    {
                        dbBizCampaignSupportUserDetails.Created_Time_Stamp = DateTime.Now;
                        _context.Biz_Campaign_Support_User_Details.Add(dbBizCampaignSupportUserDetails);
                    }

                    _context.SaveChanges();
                }
            }
        }

        public void SaveCampaignAssignedDetails(CampaignDetails campaignDetails)
        {
            List<ChampaignExcelValueDetails> _champaignValuesList = new List<ChampaignExcelValueDetails>();
            if (campaignDetails.CampaignAssignedDetailsList != null && campaignDetails.CampaignAssignedDetailsList.Count > 0)
            {
                foreach (var campaignAssignedDetail in campaignDetails.CampaignAssignedDetailsList)
                {
                    Biz_Campaign_Assigned_Details dbBizCampaignAssignedDetails = null;
                    if (campaignAssignedDetail.CampaignFollowUpId > 0)
                    {
                        UpdateFollowUpDetails(campaignAssignedDetail.CampaignFollowUpId, BatchUploadStatus.IN_PROGRESS.ToString());
                    }
                    dbBizCampaignAssignedDetails = _context.Biz_Campaign_Assigned_Details.Where(m => m.Campaign_Assigned_Details_Id == campaignAssignedDetail.CampaignAssignedDetailsId && !m.Is_Deleted).SingleOrDefault();
                    bool IsRecordExists = false;

                    if (dbBizCampaignAssignedDetails != null && dbBizCampaignAssignedDetails.Campaign_Assigned_Details_Id > 0)
                    {
                        IsRecordExists = true;
                    }
                    else
                    {
                        dbBizCampaignAssignedDetails = new Biz_Campaign_Assigned_Details();
                    }
                    dbBizCampaignAssignedDetails.Campaign_Details_Id = campaignDetails.CampaignDetailsId;
                    dbBizCampaignAssignedDetails.Name = campaignAssignedDetail.Name;
                    dbBizCampaignAssignedDetails.Email_Address = campaignAssignedDetail.EmailAddress;
                    dbBizCampaignAssignedDetails.Phone_Number = campaignAssignedDetail.PhoneNumber;
                    if (campaignAssignedDetail.CampaignFollowUpId > 0)
                    {
                        int userId = 0;
                        List<BusinessDetails> businessDetails = new List<BusinessDetails>();
                        var followUpDetails = _context.Biz_Campaign_Manager_Follow_Up_Details.Where(f => f.Campaign_Follow_Up_Id == campaignAssignedDetail.CampaignFollowUpId && !f.Is_Deleted).SingleOrDefault();
                        if (followUpDetails.User_Id > 0)
                        {
                            userId = followUpDetails.User_Id;
                        }
                        if (userId > 0)
                        {
                            businessDetails = GetBusinessDetailsByUserId(userId);
                            if (businessDetails.Count == 1)
                            {
                                foreach (var item in businessDetails)
                                {
                                    dbBizCampaignAssignedDetails.Business_Name = item.BusinessName;
                                    dbBizCampaignAssignedDetails.EIN = item.EIN;
                                    dbBizCampaignAssignedDetails.Address = item.Address1 + ' ' + item.Address2 + ',' + item.StateName + ' ' + item.ZipCode;
                                }
                            }
                            else
                            {
                                dbBizCampaignAssignedDetails.Business_Name = campaignAssignedDetail.BusinessName;
                                dbBizCampaignAssignedDetails.EIN = campaignAssignedDetail.EIN;
                                dbBizCampaignAssignedDetails.Address = campaignAssignedDetail.Address;
                            }
                        }
                        else
                        {
                            dbBizCampaignAssignedDetails.Business_Name = campaignAssignedDetail.BusinessName;
                            dbBizCampaignAssignedDetails.EIN = campaignAssignedDetail.EIN;
                            dbBizCampaignAssignedDetails.Address = campaignAssignedDetail.Address;
                        }
                    }
                    else
                    {
                        dbBizCampaignAssignedDetails.Business_Name = campaignAssignedDetail.BusinessName;
                        dbBizCampaignAssignedDetails.EIN = campaignAssignedDetail.EIN;
                        dbBizCampaignAssignedDetails.Address = campaignAssignedDetail.Address;
                    }
                    if (campaignAssignedDetail.SignedUpOn > DateTime.MinValue)
                    {
                        dbBizCampaignAssignedDetails.Signed_Up_On = campaignAssignedDetail.SignedUpOn;
                    }

                    dbBizCampaignAssignedDetails.Product_Name = campaignDetails.ProductName;
                    dbBizCampaignAssignedDetails.User_Type = campaignAssignedDetail.UserType;
                    dbBizCampaignAssignedDetails.No_Of_Trucks = campaignAssignedDetail.NoOfTrucks;
                    dbBizCampaignAssignedDetails.Last_Filed_On = campaignAssignedDetail.LastFiledOn;
                    dbBizCampaignAssignedDetails.Subscribed = campaignAssignedDetail.Subscribed;
                    dbBizCampaignAssignedDetails.Support_User_Id = campaignAssignedDetail.SupportUserId;
                    dbBizCampaignAssignedDetails.Is_Deleted = false;
                    if (campaignAssignedDetail.CampaignFollowUpId > 0)
                    {
                        dbBizCampaignAssignedDetails.Campaign_Follow_Up_Id = campaignAssignedDetail.CampaignFollowUpId;
                    }
                    else
                    {
                        dbBizCampaignAssignedDetails.Campaign_Follow_Up_Id = null;
                    }
                    dbBizCampaignAssignedDetails.Is_Skip = false;
                    dbBizCampaignAssignedDetails.ReturnNumber = campaignAssignedDetail.ReturnNumber;
                    dbBizCampaignAssignedDetails.Updated_Time_Stamp = DateTime.Now;


                    if (!IsRecordExists)
                    {
                        dbBizCampaignAssignedDetails.Created_Time_Stamp = DateTime.Now;
                        _context.Biz_Campaign_Assigned_Details.Add(dbBizCampaignAssignedDetails);
                    }

                    _context.SaveChanges();

                    if (campaignDetails.CampaignDetailsId > 0)
                    {
                        UpdateFollowUpDetails(campaignAssignedDetail.CampaignFollowUpId, BatchUploadStatus.SUCCESS.ToString());
                        campaignDetails.ChampaignExcelHeaderDetailsList = GetCampaignHeadersDetails(campaignDetails.CampaignDetailsId).OrderBy(a => a.CampaignExcelHeaderId).ToList();
                    }

                    if (campaignAssignedDetail.ChampaignExcelValueDetailsList != null && campaignAssignedDetail.ChampaignExcelValueDetailsList.Any())
                    {
                        long _excelHeaderId = 0;
                        foreach (var item in campaignAssignedDetail.ChampaignExcelValueDetailsList)
                        {

                            ChampaignExcelValueDetails _champaignValues = new ChampaignExcelValueDetails();
                            if (campaignDetails.ChampaignExcelHeaderDetailsList != null && campaignDetails.ChampaignExcelHeaderDetailsList.Any())
                            {
                                _excelHeaderId = campaignDetails.ChampaignExcelHeaderDetailsList.Where(a => a.HeaderName == item.ExcelHeaderName && a.CampaignDetailsId == campaignDetails.CampaignDetailsId).Select(a => a.CampaignExcelHeaderId).SingleOrDefault();
                                if (_excelHeaderId > 0)
                                {
                                    _champaignValues.CampaignExcelHeaderId = _excelHeaderId;
                                }
                            }
                            _champaignValues.ExcelHeaderValue = item.ExcelHeaderValue;
                            _champaignValues.CampaignAssignedDetailsId = dbBizCampaignAssignedDetails.Campaign_Assigned_Details_Id;
                            _champaignValuesList.Add(_champaignValues);
                        }
                    }
                }

                if (_champaignValuesList != null && _champaignValuesList.Any())
                {
                    campaignDetails.ChampaignExcelValueDetailsList = _champaignValuesList;
                    SaveCampaignExcelValuesDetails(campaignDetails);
                }
            }
        }

        public void SaveCampaignExcelHeaderDetails(CampaignDetails campaignDetails)
        {
            if (campaignDetails.ChampaignExcelHeaderDetailsList != null && campaignDetails.ChampaignExcelHeaderDetailsList.Count > 0)
            {
                foreach (var campaignExcelHeader in campaignDetails.ChampaignExcelHeaderDetailsList)
                {
                    Biz_Campaign_Excel_Headers dbBizCampaignHeaderDetails = null;

                    dbBizCampaignHeaderDetails = _context.Biz_Campaign_Excel_Headers.Where(m => m.Campaign_Excel_Header_Id == campaignExcelHeader.CampaignExcelHeaderId && !m.Is_Deleted).SingleOrDefault();
                    bool IsRecordExists = false;

                    if (dbBizCampaignHeaderDetails != null && dbBizCampaignHeaderDetails.Campaign_Excel_Header_Id > 0)
                    {
                        IsRecordExists = true;
                    }
                    else
                    {
                        dbBizCampaignHeaderDetails = new Biz_Campaign_Excel_Headers();
                    }

                    dbBizCampaignHeaderDetails.Campaign_Details_Id = campaignDetails.CampaignDetailsId;
                    dbBizCampaignHeaderDetails.Header_Name = campaignExcelHeader.HeaderName;

                    dbBizCampaignHeaderDetails.Is_Deleted = false;
                    dbBizCampaignHeaderDetails.Updated_Time_Stamp = DateTime.Now;

                    if (!IsRecordExists)
                    {
                        dbBizCampaignHeaderDetails.Created_Time_Stamp = DateTime.Now;
                        _context.Biz_Campaign_Excel_Headers.Add(dbBizCampaignHeaderDetails);
                    }
                    _context.SaveChanges();
                }
            }
        }

        public void SaveCampaignExcelValuesDetails(CampaignDetails campaignDetails)
        {
            if (campaignDetails.ChampaignExcelValueDetailsList != null && campaignDetails.ChampaignExcelValueDetailsList.Count > 0)
            {
                foreach (var campaignExcelValue in campaignDetails.ChampaignExcelValueDetailsList)
                {
                    Biz_Campaign_Excel_Values dbBizCampaignValueDetails = null;

                    dbBizCampaignValueDetails = _context.Biz_Campaign_Excel_Values.Where(m => m.Campaign_Excel_Value_Id == campaignExcelValue.CampaignExcelValuesId && !m.Is_Deleted).SingleOrDefault();
                    bool IsRecordExists = false;

                    if (dbBizCampaignValueDetails != null && dbBizCampaignValueDetails.Campaign_Excel_Header_Id > 0)
                    {
                        IsRecordExists = true;
                    }
                    else
                    {
                        dbBizCampaignValueDetails = new Biz_Campaign_Excel_Values();
                    }

                    dbBizCampaignValueDetails.Campaign_Excel_Header_Id = campaignExcelValue.CampaignExcelHeaderId;
                    dbBizCampaignValueDetails.Campaign_Assigned_Details_Id = campaignExcelValue.CampaignAssignedDetailsId;
                    dbBizCampaignValueDetails.Header_Values = campaignExcelValue.ExcelHeaderValue;

                    dbBizCampaignValueDetails.Is_Deleted = false;
                    dbBizCampaignValueDetails.Updated_Time_Stamp = DateTime.Now;

                    if (!IsRecordExists)
                    {
                        dbBizCampaignValueDetails.Created_Time_Stamp = DateTime.Now;
                        _context.Biz_Campaign_Excel_Values.Add(dbBizCampaignValueDetails);
                    }
                    _context.SaveChanges();
                }
            }
        }

        #endregion

        #region Get Campaign Details by  Admin User Id
        public List<CampaignDetails> GetCampaignDetails(JQueryDataTableParamModel param)
        {
            List<CampaignDetails> campaignDetailsList = new List<CampaignDetails>();
            var _dbCampaignDetailsList = (from a in _context.Biz_Campaign_Details
                                          where a.Admin_User_Id == param.AdminUserId && !a.Is_Deleted && a.Admin_Project_Id > 0
                                          orderby a.Created_Time_Stamp descending
                                          select new
                                          {
                                              campaignDetails = a,
                                              campaignAssignedDetails = a.Biz_Campaign_Assigned_Details,
                                              campaignSupportUserDetails = a.Biz_Campaign_Support_User_Details,
                                          }).ToList();

            if (_dbCampaignDetailsList != null && _dbCampaignDetailsList.Count > 0)
            {
                foreach (var dynamicCampaign in _dbCampaignDetailsList)
                {
                    var _dbCampaignDetails = dynamicCampaign.campaignDetails;
                    var _dbCampaignSupportUserDetails = dynamicCampaign.campaignSupportUserDetails.Where(x => !x.Is_Deleted).ToList();

                    CampaignDetails campaignDetails = new CampaignDetails();
                    campaignDetails.CampaignDetailsId = _dbCampaignDetails.Campaign_Details_Id;
                    campaignDetails.AdminUserId = _dbCampaignDetails.Admin_User_Id;
                    campaignDetails.CampaignName = _dbCampaignDetails.Campaign_Name;
                    campaignDetails.CampaignFileName = _dbCampaignDetails.Campaign_File_Name;
                    campaignDetails.FilePath = _dbCampaignDetails.File_Path;
                    campaignDetails.Goals = _dbCampaignDetails.Goals;
                    campaignDetails.NoOfLeads = _dbCampaignDetails.No_Of_Leads;
                    campaignDetails.UniqueId = _dbCampaignDetails.Unique_Id;
                    campaignDetails.IsPaused = _dbCampaignDetails.Is_Paused;
                    campaignDetails.IsSuspended = _dbCampaignDetails.Is_Suspended;
                    campaignDetails.AdminProjectId = _dbCampaignDetails.Admin_Project_Id;
                    campaignDetails.CampaignStartDate = _dbCampaignDetails.Campaign_Start_Date;
                    campaignDetails.CampaignEndDate = _dbCampaignDetails.Campaign_End_Date;
                    campaignDetails.IsUploadFileAssign = _dbCampaignDetails.Is_Upload_File_Assign;
                    campaignDetails.CreateTimeStamp = _dbCampaignDetails.Created_Time_Stamp;
                    campaignDetails.TechTeamStatus = _dbCampaignDetails.Tech_Team_Status;
                    campaignDetails.IsDiscardedRequest = _dbCampaignDetails.Is_Discarded_Request;
                    campaignDetails.DiscardedReason = _dbCampaignDetails.Discarded_Reason;
                    campaignDetails.DemoGrapicInformation = _dbCampaignDetails.DemoGrapic_Information;
                    campaignDetails.TechTeamFileUploadedTime = _dbCampaignDetails.Tech_Team_File_Uploaded_Time;
                    campaignDetails.Notes = _dbCampaignDetails.Notes;
                    campaignDetails.SuspendDate = _dbCampaignDetails.SuspendDate ?? DateTime.Now;
                    campaignDetails.SuspendReason = _dbCampaignDetails.SuspendReason;


                    campaignDetails.CampaignSupportUserDetailsList = new List<CampaignSupportUserDetails>();

                    if (_dbCampaignSupportUserDetails != null && _dbCampaignSupportUserDetails.Count > 0)
                    {
                        foreach (var dbuserSupportDetails in _dbCampaignSupportUserDetails)
                        {
                            CampaignSupportUserDetails campaignSupportUserDetails = new CampaignSupportUserDetails();
                            campaignSupportUserDetails.CampaignSupportUserDetailId = dbuserSupportDetails.Campaign_Support_User_Detail_Id;
                            campaignSupportUserDetails.CampaignDetailsId = dbuserSupportDetails.Campaign_Details_Id ?? 0;
                            campaignSupportUserDetails.AdminUserId = dbuserSupportDetails.Admin_User_Id ?? 0;
                            campaignSupportUserDetails.SupportUserId = dbuserSupportDetails.Support_User_Id ?? 0;
                            campaignSupportUserDetails.NoOfUserAssigned = dbuserSupportDetails.No_Of_User_Assigned;
                            campaignSupportUserDetails.NoOfCompleted = dbuserSupportDetails.No_Of_Completed;
                            campaignSupportUserDetails.NoOfPending = dbuserSupportDetails.No_Of_Pending;
                            campaignSupportUserDetails.UserSkippedCount = dbuserSupportDetails.User_Skipped_Count;
                            campaignSupportUserDetails.IsViewed = dbuserSupportDetails.Is_Viewed;
                            campaignSupportUserDetails.AdminUserName = dbuserSupportDetails.Biz_Admin_Users1.Admin_User_Name;
                            campaignDetails.CampaignSupportUserDetailsList.Add(campaignSupportUserDetails);
                        }
                    }


                    campaignDetailsList.Add(campaignDetails);

                }
            }

            return campaignDetailsList;

        }

        #endregion

        #region Get Campaign Short Details by  Campain Detail User Id
        public CampaignDetails GetCampaignShortDetailsByCampaignId(long campaignId)
        {
            CampaignDetails campaignDetails = new CampaignDetails();
            var _dbCampaignDetails = (from a in _context.Biz_Campaign_Details
                                      where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Admin_Project_Id > 0
                                      select a).SingleOrDefault();


            if (_dbCampaignDetails != null)
            {
                campaignDetails.CampaignDetailsId = _dbCampaignDetails.Campaign_Details_Id;
                campaignDetails.AdminUserId = _dbCampaignDetails.Admin_User_Id;
                campaignDetails.CampaignName = _dbCampaignDetails.Campaign_Name;
                campaignDetails.CampaignFileName = _dbCampaignDetails.Campaign_File_Name;
                campaignDetails.FilePath = _dbCampaignDetails.File_Path;
                campaignDetails.Goals = _dbCampaignDetails.Goals;
                campaignDetails.NoOfLeads = _dbCampaignDetails.No_Of_Leads;
                campaignDetails.UniqueId = _dbCampaignDetails.Unique_Id;
                campaignDetails.IsPaused = _dbCampaignDetails.Is_Paused;
                campaignDetails.IsSuspended = _dbCampaignDetails.Is_Suspended;
                campaignDetails.AdminProjectId = _dbCampaignDetails.Admin_Project_Id;
                campaignDetails.CampaignStartDate = _dbCampaignDetails.Campaign_Start_Date;
                campaignDetails.CampaignEndDate = _dbCampaignDetails.Campaign_End_Date;
                campaignDetails.IsUploadFileAssign = _dbCampaignDetails.Is_Upload_File_Assign;
                campaignDetails.CreateTimeStamp = _dbCampaignDetails.Created_Time_Stamp;
                campaignDetails.ProductName = (_dbCampaignDetails.Static_Biz_Admin_Project != null && _dbCampaignDetails.Static_Biz_Admin_Project.Project_Name != null ? _dbCampaignDetails.Static_Biz_Admin_Project.Project_Name : string.Empty);
                campaignDetails.ProductId = _dbCampaignDetails.Admin_Project_Id ?? 0;
                campaignDetails.TechTeamStatus = _dbCampaignDetails.Tech_Team_Status;
                campaignDetails.CampaignType = _dbCampaignDetails.Campaign_Type;
                campaignDetails.DemoGrapicInformation = _dbCampaignDetails.DemoGrapic_Information;
                campaignDetails.TechTeamFileUploadedTime = _dbCampaignDetails.Tech_Team_File_Uploaded_Time;
                campaignDetails.Notes = _dbCampaignDetails.Notes;

            }

            return campaignDetails;

        }

        #endregion

        #region  Update Campaign Extend Date
        public bool UpdateCampaignExtendDate(CampaignDetails campaignDetails)
        {
            bool isReturnStatus = false;

            var _dbCampaignDetails = (from a in _context.Biz_Campaign_Details
                                      where a.Campaign_Details_Id == campaignDetails.CampaignDetailsId && !a.Is_Deleted && a.Admin_Project_Id > 0
                                      select a).SingleOrDefault();
            if (_dbCampaignDetails != null)
            {
                _dbCampaignDetails.Campaign_End_Date = campaignDetails.CampaignEndDate;
                _dbCampaignDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
                isReturnStatus = true;
            }

            return isReturnStatus;

        }
        #endregion

        #region Update Campaign Pause Status
        public bool UpdateCampaignPauseStatus(long campaignDetailsId, bool isPaused)
        {
            bool isReturnStatus = false;
            var _dbCampaignDetails = (from a in _context.Biz_Campaign_Details
                                      where a.Campaign_Details_Id == campaignDetailsId && !a.Is_Deleted && a.Admin_Project_Id > 0
                                      select a).SingleOrDefault();
            if (_dbCampaignDetails != null)
            {
                _dbCampaignDetails.Is_Paused = isPaused;
                _dbCampaignDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
                isReturnStatus = true;
            }
            return isReturnStatus;

        }
        #endregion

        #region Update Campaign Suspend Status
        public bool UpdateCampaignSuspendStatus(long campaignDetailsId, bool isSuspend)
        {
            bool isReturnStatus = false;
            var _dbCampaignDetails = (from a in _context.Biz_Campaign_Details
                                      where a.Campaign_Details_Id == campaignDetailsId && !a.Is_Deleted && a.Admin_Project_Id > 0
                                      select a).SingleOrDefault();
            if (_dbCampaignDetails != null)
            {
                _dbCampaignDetails.Is_Suspended = isSuspend;
                _dbCampaignDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
                isReturnStatus = true;
            }
            return isReturnStatus;

        }
        #endregion

        #region Delete Campaign
        public bool DeleteCampaign(long campaignDetailsId)
        {

            bool isReturnDeletedStatus = false;
            var _dbCampaignDetails = (from a in _context.Biz_Campaign_Details
                                      where a.Campaign_Details_Id == campaignDetailsId && !a.Is_Deleted && a.Admin_Project_Id > 0
                                      select a).SingleOrDefault();
            if (_dbCampaignDetails != null)
            {
                _dbCampaignDetails.Is_Deleted = true;
                _dbCampaignDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
                isReturnDeletedStatus = true;
            }
            return isReturnDeletedStatus;

        }
        #endregion

        #region Suspend Campaign
        public bool SuspendCampaign(CampaignDetails campaignDetails)
        {

            bool isReturnDeletedStatus = false;
            if (campaignDetails.CampaignDetailsId > 0)
            {
                var _dbCampaignDetails = (from a in _context.Biz_Campaign_Details
                                          where a.Campaign_Details_Id == campaignDetails.CampaignDetailsId && !a.Is_Deleted && a.Admin_Project_Id > 0
                                          select a).SingleOrDefault();
                if (_dbCampaignDetails != null)
                {
                    _dbCampaignDetails.Is_Suspended = true;
                    _dbCampaignDetails.SuspendReason = campaignDetails.SuspendReason;
                    _dbCampaignDetails.SuspendDate = DateTime.Now;
                    _dbCampaignDetails.Updated_Time_Stamp = DateTime.Now;
                    _context.SaveChanges();
                    isReturnDeletedStatus = true;
                }
            }
            return isReturnDeletedStatus;

        }
        #endregion

        #region Get Campaign Upload Details by  Admin User Id
        public List<CampaignDetails> GetCampaignUploadRequestDetails()
        {
            List<CampaignDetails> campaignDetailsList = new List<CampaignDetails>();
            var _dbCampaignDetailsList = (from a in _context.Biz_Campaign_Details
                                          where !a.Is_Deleted && a.Admin_Project_Id > 0 && !a.Is_Upload_File_Assign
                                          orderby a.Created_Time_Stamp descending
                                          select new
                                          {
                                              campaignDetails = a,
                                              campaignAssignedDetails = a.Biz_Campaign_Assigned_Details,
                                              campaignSupportUserDetails = a.Biz_Campaign_Support_User_Details,
                                          }).ToList();

            if (_dbCampaignDetailsList != null && _dbCampaignDetailsList.Count > 0)
            {
                foreach (var dynamicCampaign in _dbCampaignDetailsList)
                {
                    var _dbCampaignDetails = dynamicCampaign.campaignDetails;
                    var _dbCampaignSupportUserDetails = dynamicCampaign.campaignSupportUserDetails.Where(x => !x.Is_Deleted).ToList();

                    CampaignDetails campaignDetails = new CampaignDetails();
                    campaignDetails.CampaignDetailsId = _dbCampaignDetails.Campaign_Details_Id;
                    campaignDetails.AdminUserId = _dbCampaignDetails.Admin_User_Id;
                    campaignDetails.CampaignName = _dbCampaignDetails.Campaign_Name;
                    campaignDetails.CampaignFileName = _dbCampaignDetails.Campaign_File_Name;
                    campaignDetails.FilePath = _dbCampaignDetails.File_Path;
                    campaignDetails.Goals = _dbCampaignDetails.Goals;
                    campaignDetails.NoOfLeads = _dbCampaignDetails.No_Of_Leads;
                    campaignDetails.UniqueId = _dbCampaignDetails.Unique_Id;
                    campaignDetails.IsPaused = _dbCampaignDetails.Is_Paused;
                    campaignDetails.IsSuspended = _dbCampaignDetails.Is_Suspended;
                    campaignDetails.AdminProjectId = _dbCampaignDetails.Admin_Project_Id;
                    campaignDetails.CampaignStartDate = _dbCampaignDetails.Campaign_Start_Date;
                    campaignDetails.CampaignEndDate = _dbCampaignDetails.Campaign_End_Date;
                    campaignDetails.IsUploadFileAssign = _dbCampaignDetails.Is_Upload_File_Assign;
                    campaignDetails.CreateTimeStamp = _dbCampaignDetails.Created_Time_Stamp;
                    campaignDetails.TechTeamStatus = _dbCampaignDetails.Tech_Team_Status;
                    campaignDetails.CampaignType = _dbCampaignDetails.Campaign_Type;
                    campaignDetails.DemoGrapicInformation = _dbCampaignDetails.DemoGrapic_Information;
                    campaignDetails.TechTeamFileUploadedTime = _dbCampaignDetails.Tech_Team_File_Uploaded_Time;
                    campaignDetails.Notes = _dbCampaignDetails.Notes;
                    campaignDetails.TechTeamFileUploadedTime = _dbCampaignDetails.Tech_Team_File_Uploaded_Time;
                    campaignDetails.DemoGrapicInformation = _dbCampaignDetails.DemoGrapic_Information;

                    campaignDetails.CampaignSupportUserDetailsList = new List<CampaignSupportUserDetails>();

                    if (_dbCampaignSupportUserDetails != null && _dbCampaignSupportUserDetails.Count > 0)
                    {
                        foreach (var dbuserSupportDetails in _dbCampaignSupportUserDetails)
                        {
                            CampaignSupportUserDetails campaignSupportUserDetails = new CampaignSupportUserDetails();
                            campaignSupportUserDetails.CampaignSupportUserDetailId = dbuserSupportDetails.Campaign_Support_User_Detail_Id;
                            campaignSupportUserDetails.CampaignDetailsId = dbuserSupportDetails.Campaign_Details_Id ?? 0;
                            campaignSupportUserDetails.AdminUserId = dbuserSupportDetails.Admin_User_Id ?? 0;
                            campaignSupportUserDetails.SupportUserId = dbuserSupportDetails.Support_User_Id ?? 0;
                            campaignSupportUserDetails.NoOfUserAssigned = dbuserSupportDetails.No_Of_User_Assigned;
                            campaignSupportUserDetails.NoOfCompleted = dbuserSupportDetails.No_Of_Completed;
                            campaignSupportUserDetails.NoOfPending = dbuserSupportDetails.No_Of_Pending;
                            campaignSupportUserDetails.UserSkippedCount = dbuserSupportDetails.User_Skipped_Count;
                            campaignSupportUserDetails.IsViewed = dbuserSupportDetails.Is_Viewed;
                            campaignSupportUserDetails.AdminUserName = dbuserSupportDetails.Biz_Admin_Users1.Admin_User_Name;
                            campaignDetails.CampaignSupportUserDetailsList.Add(campaignSupportUserDetails);
                        }
                    }


                    campaignDetailsList.Add(campaignDetails);

                }
            }

            return campaignDetailsList;

        }

        #endregion

        #region  Update Discarded Request Reason
        public void UpdateDiscardedRequestReason(CampaignDetails campaignDetails)
        {
            var _dbCampaignDetails = (from a in _context.Biz_Campaign_Details
                                      where a.Campaign_Details_Id == campaignDetails.CampaignDetailsId && !a.Is_Deleted && a.Admin_Project_Id > 0
                                      select a).SingleOrDefault();
            if (_dbCampaignDetails != null)
            {
                _dbCampaignDetails.Is_Discarded_Request = campaignDetails.IsDiscardedRequest;
                _dbCampaignDetails.Discarded_Reason = campaignDetails.DiscardedReason;
                _dbCampaignDetails.Tech_Team_Status = campaignDetails.TechTeamStatus;
                _dbCampaignDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
            }
        }
        #endregion

        #region Save Tech Team FileU ploaded
        public void SaveTechTeamFileUploaded(CampaignDetails campaignDetails)
        {
            var _dbCampaignDetails = (from a in _context.Biz_Campaign_Details
                                      where a.Campaign_Details_Id == campaignDetails.CampaignDetailsId && !a.Is_Deleted && a.Admin_Project_Id > 0
                                      select a).SingleOrDefault();
            if (_dbCampaignDetails != null)
            {
                _dbCampaignDetails.Campaign_File_Name = campaignDetails.CampaignFileName;
                _dbCampaignDetails.File_Path = campaignDetails.FilePath;
                _dbCampaignDetails.No_Of_Leads = campaignDetails.NoOfLeads;
                _dbCampaignDetails.Notes = campaignDetails.Notes;
                _dbCampaignDetails.Tech_Team_Status = campaignDetails.TechTeamStatus;
                _dbCampaignDetails.Tech_Team_File_Uploaded_Time = campaignDetails.TechTeamFileUploadedTime;
                _dbCampaignDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
            }
        }
        #endregion

        #region Get Lead Details by  Campain Detail Id and Support User Id
        public LeadDetails GetLeadDetailsbyCampainDetailIdandSupportUserId(long campaignId, long supportUserId)
        {
            LeadDetails leadDetails = new LeadDetails();

            var dbSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
                                        where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                        select a).SingleOrDefault();

            var _dbLeadDetailslist = (from a in _context.Biz_Campaign_Support_User_Details
                                      where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId && a.Last_Lead_Id != null
                                      select a).FirstOrDefault();
            if (_dbLeadDetailslist != null)
            {
                var _dbLeadDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                      where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId && a.Campaign_Assigned_Details_Id == _dbLeadDetailslist.Last_Lead_Id
                                      orderby a.Created_Time_Stamp
                                      select a).SingleOrDefault();

                if (_dbLeadDetails != null)
                {
                    leadDetails.Name = _dbLeadDetails.Name;
                    leadDetails.BusinessName = _dbLeadDetails.Business_Name;
                    leadDetails.Phone = _dbLeadDetails.Phone_Number;
                    leadDetails.Email = _dbLeadDetails.Email_Address;
                    leadDetails.Address = _dbLeadDetails.Address;
                    leadDetails.UserType = _dbLeadDetails.User_Type;
                    leadDetails.LastFiled = _dbLeadDetails.Last_Filed_On ?? DateTime.Now;
                    leadDetails.NoofTrucks = _dbLeadDetails.No_Of_Trucks ?? 0;
                    leadDetails.CampaignAssignedDetailsId = _dbLeadDetails.Campaign_Assigned_Details_Id;
                    leadDetails.EIN = _dbLeadDetails.EIN;
                    leadDetails.ReturnNumber = _dbLeadDetails.ReturnNumber ?? 0;
                }
            }
            else
            {
                var _dbLeadDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                      where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                      orderby a.Created_Time_Stamp
                                      select a).Take(1).SingleOrDefault();

                if (_dbLeadDetails != null)
                {
                    leadDetails.Name = _dbLeadDetails.Name;
                    leadDetails.BusinessName = _dbLeadDetails.Business_Name;
                    leadDetails.Phone = _dbLeadDetails.Phone_Number;
                    leadDetails.Email = _dbLeadDetails.Email_Address;
                    leadDetails.Address = _dbLeadDetails.Address;
                    leadDetails.UserType = _dbLeadDetails.User_Type;
                    leadDetails.LastFiled = _dbLeadDetails.Last_Filed_On ?? DateTime.Now;
                    leadDetails.NoofTrucks = _dbLeadDetails.No_Of_Trucks ?? 0;
                    leadDetails.CampaignAssignedDetailsId = _dbLeadDetails.Campaign_Assigned_Details_Id;
                    leadDetails.EIN = _dbLeadDetails.EIN;
                    leadDetails.ReturnNumber = _dbLeadDetails.ReturnNumber ?? 0;
                }
            }

            //Get Header Name and  Header Values
            if (leadDetails.CampaignAssignedDetailsId > 0)
            {
                var dbAdditionalContList = (from a in _context.Biz_Campaign_Excel_Values
                                            join z in _context.Biz_Campaign_Excel_Headers on a.Campaign_Excel_Header_Id equals z.Campaign_Excel_Header_Id
                                            where a.Campaign_Assigned_Details_Id == leadDetails.CampaignAssignedDetailsId && !a.Is_Deleted
                                            select new
                                            {
                                                a.Campaign_Assigned_Details_Id,
                                                a.Campaign_Excel_Value_Id,
                                                a.Campaign_Excel_Header_Id,
                                                a.Header_Values,
                                                z.Header_Name,
                                            }).ToList();

                if (dbAdditionalContList != null && dbAdditionalContList.Any())
                {
                    leadDetails.ChampaignExcelValueDetailsList = new List<ChampaignExcelValueDetails>();
                    foreach (var item in dbAdditionalContList)
                    {
                        ChampaignExcelValueDetails ContDetails = new ChampaignExcelValueDetails();
                        ContDetails.ExcelHeaderValue = item.Header_Values;
                        ContDetails.CampaignAssignedDetailsId = item.Campaign_Assigned_Details_Id;
                        ContDetails.CampaignExcelValuesId = item.Campaign_Excel_Value_Id;
                        ContDetails.CampaignExcelHeaderId = item.Campaign_Excel_Header_Id;
                        ContDetails.ExcelHeaderName = item.Header_Name;
                        leadDetails.ChampaignExcelValueDetailsList.Add(ContDetails);
                    }
                }
            }

            //if (leadDetails.CampaignAssignedDetailsId > 0)
            //{
            //    var dbAdditionalContList = (from a in _context.Biz_Campaign_Additional_Contacts_Details
            //                                where a.Campaign_Assigned_Details_Id == leadDetails.CampaignAssignedDetailsId && !a.Is_Deleted
            //                                select new
            //                                {
            //                                    a.Additional_Contacts_Details_Id,
            //                                    a.Contacts_Name,
            //                                    a.Contacts_Phone_Number,
            //                                    a.Contacts_Title,
            //                                    a.Contacts_Email_Address
            //                                }).ToList();

            //    if(dbAdditionalContList != null && dbAdditionalContList.Any())
            //    {
            //        leadDetails.AdditionalContactsList = new List<AdditionalContacts>();
            //        foreach (var item in dbAdditionalContList)
            //        {
            //            AdditionalContacts ContDetails = new AdditionalContacts();
            //            ContDetails.AdditionalContactsDetailsId = item.Additional_Contacts_Details_Id;
            //            ContDetails.ContactName = item.Contacts_Name;
            //            ContDetails.ContactEmailAddress = item.Contacts_Email_Address;
            //            ContDetails.ContactPhone = item.Contacts_Phone_Number;
            //            ContDetails.ContactTitle = item.Contacts_Title;

            //            leadDetails.AdditionalContactsList.Add(ContDetails);
            //        }

            //    }
            //}

            if (dbSupportUserDetails != null)
            {
                leadDetails.TotalAssignedLeads = dbSupportUserDetails.No_Of_User_Assigned ?? 0;
            }

            var dbCampaignAssignedDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                             where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                             orderby a.Created_Time_Stamp
                                             select a.Campaign_Assigned_Details_Id).ToList();

            if (dbCampaignAssignedDetails != null && dbCampaignAssignedDetails.Any())
            {
                leadDetails.CurrentViewedLeads = dbCampaignAssignedDetails.IndexOf(leadDetails.CampaignAssignedDetailsId) + 1;
            }

            return leadDetails;
        }

        #endregion

        #region Get Lead Details By Skipped Record
        public LeadDetails GetLeadDetailsBySkippedRecord(long campaignId, long supportUserId)
        {
            LeadDetails leadDetails = new LeadDetails();

            var dynamicCampaign = (from a in _context.Biz_Campaign_Details
                                   where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Admin_Project_Id > 0
                                   select new
                                   {
                                       campaignDetails = a,
                                       campaignAssignedDetails = a.Biz_Campaign_Assigned_Details,
                                       campaignSupportUserDetails = a.Biz_Campaign_Support_User_Details,
                                       campaignLeadActivity = a.Biz_Campaign_Lead_Activity
                                   }).SingleOrDefault();

            if (dynamicCampaign != null)
            {
                var _dbCampaignSupportUserDetails = dynamicCampaign.campaignSupportUserDetails.Where(m => m.Support_User_Id == supportUserId && !m.Is_Deleted).ToList();
                var _dbCampaignAssignedDetails = dynamicCampaign.campaignAssignedDetails.Where(m => m.Support_User_Id == supportUserId && !m.Is_Deleted).ToList();
                var _dbConactedLeadDetailsIds = _dbCampaignAssignedDetails.Where(m => m.Is_Skip == true).Select(m => m.Campaign_Assigned_Details_Id).ToList();


                //var dbSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
                //                            where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                //                            select a).SingleOrDefault();


                var dbSupportUserDetails = (from a in _dbCampaignSupportUserDetails
                                            where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                            select a).SingleOrDefault();

                //var _dbLeadDetailslist = (from a in _context.Biz_Campaign_Support_User_Details
                //                          where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId && a.Last_Lead_Id != null
                //                          select a).FirstOrDefault();


                var _dbLeadDetailslist = (from a in _dbCampaignSupportUserDetails
                                          where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId && a.Last_Lead_Id != null
                                          select a).FirstOrDefault();



                if (_dbLeadDetailslist != null)
                {
                    var _dbLeadDetails = (from a in _dbCampaignAssignedDetails
                                          where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId && a.Campaign_Assigned_Details_Id == _dbLeadDetailslist.Last_Lead_Id
                                          orderby a.Created_Time_Stamp
                                          select a).SingleOrDefault();

                    if (_dbLeadDetails != null)
                    {
                        leadDetails.Name = _dbLeadDetails.Name;
                        leadDetails.BusinessName = _dbLeadDetails.Business_Name;
                        leadDetails.Phone = _dbLeadDetails.Phone_Number;
                        leadDetails.Email = _dbLeadDetails.Email_Address;
                        leadDetails.Address = _dbLeadDetails.Address;
                        leadDetails.UserType = _dbLeadDetails.User_Type;
                        leadDetails.LastFiled = _dbLeadDetails.Last_Filed_On ?? DateTime.Now;
                        leadDetails.NoofTrucks = _dbLeadDetails.No_Of_Trucks ?? 0;
                        leadDetails.CampaignAssignedDetailsId = _dbLeadDetails.Campaign_Assigned_Details_Id;
                        leadDetails.EIN = _dbLeadDetails.EIN;
                    }
                }
                else
                {
                    var _dbLeadDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                          where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                          && _dbConactedLeadDetailsIds.Contains(a.Campaign_Assigned_Details_Id)
                                          orderby a.Created_Time_Stamp
                                          select a).Take(1).SingleOrDefault();

                    if (_dbLeadDetails != null)
                    {
                        leadDetails.Name = _dbLeadDetails.Name;
                        leadDetails.BusinessName = _dbLeadDetails.Business_Name;
                        leadDetails.Phone = _dbLeadDetails.Phone_Number;
                        leadDetails.Email = _dbLeadDetails.Email_Address;
                        leadDetails.Address = _dbLeadDetails.Address;
                        leadDetails.UserType = _dbLeadDetails.User_Type;
                        leadDetails.LastFiled = _dbLeadDetails.Last_Filed_On ?? DateTime.Now;
                        leadDetails.NoofTrucks = _dbLeadDetails.No_Of_Trucks ?? 0;
                        leadDetails.CampaignAssignedDetailsId = _dbLeadDetails.Campaign_Assigned_Details_Id;
                        leadDetails.EIN = _dbLeadDetails.EIN;
                    }
                }

                //Get Header Name and  Header Values
                if (leadDetails.CampaignAssignedDetailsId > 0)
                {
                    var dbAdditionalContList = (from a in _context.Biz_Campaign_Excel_Values
                                                join z in _context.Biz_Campaign_Excel_Headers on a.Campaign_Excel_Header_Id equals z.Campaign_Excel_Header_Id
                                                where a.Campaign_Assigned_Details_Id == leadDetails.CampaignAssignedDetailsId && !a.Is_Deleted
                                                select new
                                                {
                                                    a.Campaign_Assigned_Details_Id,
                                                    a.Campaign_Excel_Value_Id,
                                                    a.Campaign_Excel_Header_Id,
                                                    a.Header_Values,
                                                    z.Header_Name,
                                                }).ToList();

                    if (dbAdditionalContList != null && dbAdditionalContList.Any())
                    {
                        leadDetails.ChampaignExcelValueDetailsList = new List<ChampaignExcelValueDetails>();
                        foreach (var item in dbAdditionalContList)
                        {
                            ChampaignExcelValueDetails ContDetails = new ChampaignExcelValueDetails();
                            ContDetails.ExcelHeaderValue = item.Header_Values;
                            ContDetails.CampaignAssignedDetailsId = item.Campaign_Assigned_Details_Id;
                            ContDetails.CampaignExcelValuesId = item.Campaign_Excel_Value_Id;
                            ContDetails.CampaignExcelHeaderId = item.Campaign_Excel_Header_Id;
                            ContDetails.ExcelHeaderName = item.Header_Name;
                            leadDetails.ChampaignExcelValueDetailsList.Add(ContDetails);
                        }
                    }
                }



                if (dbSupportUserDetails != null)
                {
                    leadDetails.TotalAssignedLeads = dbSupportUserDetails.No_Of_User_Assigned ?? 0;
                }

                var dbCampaignAssignedDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                                 where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                                 orderby a.Created_Time_Stamp
                                                 //  && !_dbConactedLeadDetailsIds.Contains(a.Campaign_Assigned_Details_Id)
                                                 select a.Campaign_Assigned_Details_Id).ToList();

                if (dbCampaignAssignedDetails != null && dbCampaignAssignedDetails.Any())
                {
                    leadDetails.CurrentViewedLeads = dbCampaignAssignedDetails.IndexOf(leadDetails.CampaignAssignedDetailsId) + 1;
                }
            }

            return leadDetails;
        }

        #endregion



        #region Get Lead Details by  Campain Detail Id and Support User Id
        public LeadDetails GetLeadDetailsForCommunicationByCampainDetailIdandSupportUserId(long campaignId, long supportUserId)
        {
            long campaignAssignedDetailId = 0;
            LeadDetails leadDetails = new LeadDetails();
            var _dbLeadDetailslist = (from a in _context.Biz_Campaign_Support_User_Details
                                      where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId && a.Last_Lead_Id != null
                                      select a).FirstOrDefault();
            if (_dbLeadDetailslist != null)
            {
                var _dbLeadDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                      where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId && a.Campaign_Assigned_Details_Id == _dbLeadDetailslist.Last_Lead_Id
                                      orderby a.Created_Time_Stamp
                                      select a).SingleOrDefault();

                if (_dbLeadDetails != null)
                {
                    leadDetails.Name = _dbLeadDetails.Name;
                    leadDetails.BusinessName = _dbLeadDetails.Business_Name;
                    leadDetails.Phone = _dbLeadDetails.Phone_Number;
                    leadDetails.Email = _dbLeadDetails.Email_Address;
                    leadDetails.Address = _dbLeadDetails.Address;
                    leadDetails.UserType = _dbLeadDetails.User_Type;
                    leadDetails.LastFiled = _dbLeadDetails.Last_Filed_On ?? DateTime.Now;
                    leadDetails.NoofTrucks = _dbLeadDetails.No_Of_Trucks ?? 0;
                    leadDetails.CampaignAssignedDetailsId = _dbLeadDetails.Campaign_Assigned_Details_Id;
                    campaignAssignedDetailId = _dbLeadDetails.Campaign_Assigned_Details_Id;
                }
            }
            else
            {
                var _dbLeadDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                      where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                      orderby a.Created_Time_Stamp
                                      select a).Take(1).SingleOrDefault();

                if (_dbLeadDetails != null)
                {
                    leadDetails.Name = _dbLeadDetails.Name;
                    leadDetails.BusinessName = _dbLeadDetails.Business_Name;
                    leadDetails.Phone = _dbLeadDetails.Phone_Number;
                    leadDetails.Email = _dbLeadDetails.Email_Address;
                    leadDetails.Address = _dbLeadDetails.Address;
                    leadDetails.UserType = _dbLeadDetails.User_Type;
                    leadDetails.LastFiled = _dbLeadDetails.Last_Filed_On ?? DateTime.Now;
                    leadDetails.NoofTrucks = _dbLeadDetails.No_Of_Trucks ?? 0;
                    leadDetails.CampaignAssignedDetailsId = _dbLeadDetails.Campaign_Assigned_Details_Id;
                    campaignAssignedDetailId = _dbLeadDetails.Campaign_Assigned_Details_Id;
                }
            }

            var _dbCampaignSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
                                                 where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                                 select a).SingleOrDefault();
            if (_dbCampaignSupportUserDetails != null)
            {
                _dbCampaignSupportUserDetails.Last_Lead_Id = campaignAssignedDetailId;
                _dbCampaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
            }



            return leadDetails;

        }

        #endregion

        #region Get Goals By Campaign Details Id
        public CampaignDetails GetGoalsByCampaignDetailsId(long campaignId)
        {
            CampaignDetails goalDetails = new CampaignDetails();

            if (campaignId > 0)
            {
                var _goalDetails = (from a in _context.Biz_Campaign_Details
                                    where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Admin_User_Id > 0
                                    select new
                                    {
                                        a.Goals,
                                        a.Campaign_Name,
                                        a.Campaign_Type,
                                        a.Admin_Project_Id
                                    }).SingleOrDefault();

                if (_goalDetails != null)
                {
                    goalDetails.Goals = _goalDetails.Goals;
                    goalDetails.CampaignName = _goalDetails.Campaign_Name;
                    goalDetails.CampaignType = _goalDetails.Campaign_Type;
                    goalDetails.ProductId = _goalDetails.Admin_Project_Id ?? 0;
                }
            }


            return goalDetails;
        }

        #endregion

        #region Save Campaign Communication

        public LeadCommunication SaveCampaignCommunication(LeadCommunication leadCommunication)
        {
            Biz_Campaign_Lead_Activity dbBizCampaignLeadActivity = null;
            dbBizCampaignLeadActivity = _context.Biz_Campaign_Lead_Activity.Where(m => m.Campaign_Lead_Activity_Id == leadCommunication.CampaignLeadActivityId).SingleOrDefault();
            bool IsRecordExists = false;
            if (dbBizCampaignLeadActivity != null && dbBizCampaignLeadActivity.Campaign_Lead_Activity_Id > 0)
            {
                IsRecordExists = true;
            }
            else
            {
                dbBizCampaignLeadActivity = new Biz_Campaign_Lead_Activity();
            }
            dbBizCampaignLeadActivity.Campaign_Assign_Detail_Id = leadCommunication.CampaignAssignDetailId;
            dbBizCampaignLeadActivity.Campaign_Detail_Id = leadCommunication.CampaignDetailId;
            dbBizCampaignLeadActivity.Support_User_Id = leadCommunication.SupportUserId;
            dbBizCampaignLeadActivity.Email_Address = leadCommunication.EmailAddress;
            dbBizCampaignLeadActivity.Method_Of_Contact = leadCommunication.MethodeofContract;
            dbBizCampaignLeadActivity.Type_Of_Call = leadCommunication.TypeOfCall;
            dbBizCampaignLeadActivity.Spoke_To = leadCommunication.Spoketo;
            dbBizCampaignLeadActivity.Comments = leadCommunication.Comments;
            dbBizCampaignLeadActivity.Is_Do_Not_Disturb = leadCommunication.DonotContactagain;
            dbBizCampaignLeadActivity.Reason = leadCommunication.Reason;
            dbBizCampaignLeadActivity.Is_Follow_Up = leadCommunication.IsFollowRequired;
            dbBizCampaignLeadActivity.Other_Reason = leadCommunication.OtherReason;
            dbBizCampaignLeadActivity.Is_Notified = false;
            if (dbBizCampaignLeadActivity.Is_Follow_Up)
            {
                dbBizCampaignLeadActivity.Lead_Status = LeadStatusflag.Pending.ToString();
            }
            else
            {
                dbBizCampaignLeadActivity.Lead_Status = LeadStatusflag.Resolved.ToString();
            }
            dbBizCampaignLeadActivity.Followup_Date = leadCommunication.FollowupDate;
            dbBizCampaignLeadActivity.Followup_Time = leadCommunication.FollowupTime;
            dbBizCampaignLeadActivity.Is_Deleted = false;
            dbBizCampaignLeadActivity.Update_Time_Stamp = DateTime.Now;
            if (!IsRecordExists)
            {
                dbBizCampaignLeadActivity.Create_Time_Stamp = DateTime.Now;
                _context.Biz_Campaign_Lead_Activity.Add(dbBizCampaignLeadActivity);
                UpdateLeadCount(leadCommunication);
            }
            if (leadCommunication.IsSaveNext)
            {
                UpdateLastLeadId(leadCommunication);
            }
            if (!string.IsNullOrWhiteSpace(leadCommunication.StateCode))
            {
                LeadDetailsSearchOption leadDetailsSearchOption = new LeadDetailsSearchOption();

                leadDetailsSearchOption.CampaignDetailId = leadCommunication.CampaignDetailId;
                leadDetailsSearchOption.SupportUserId = leadCommunication.SupportUserId;
                leadDetailsSearchOption.StateCode = leadCommunication.StateCode;
                leadDetailsSearchOption.LeadId = leadCommunication.CampaignAssignDetailId;
            }

            leadCommunication.isCampaignCommunicationStatus = true;
            _context.SaveChanges();

            var _dbCampaignAssignedDetails = _context.Biz_Campaign_Assigned_Details.Where(m => m.Campaign_Assigned_Details_Id == leadCommunication.CampaignAssignDetailId).SingleOrDefault();

            if (_dbCampaignAssignedDetails != null)
            {
                _dbCampaignAssignedDetails.Is_Skip = false;
                _dbCampaignAssignedDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
            }


            leadCommunication.CampaignLeadActivityId = dbBizCampaignLeadActivity.Campaign_Lead_Activity_Id;
            return leadCommunication;
        }
        #endregion

        #region Get Campaign And Support User And Assigned Details
        public CampaignDetails GetCampaignAndSupportUserAndAssignedDetails(long campaignId, long supportUserId)
        {
            CampaignDetails campaignDetails = new CampaignDetails();
            var dynamicCampaign = (from a in _context.Biz_Campaign_Details
                                   where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Admin_Project_Id > 0
                                   select new
                                   {
                                       campaignDetails = a,
                                       campaignAssignedDetails = a.Biz_Campaign_Assigned_Details,
                                       campaignSupportUserDetails = a.Biz_Campaign_Support_User_Details,
                                       campaignLeadActivity = a.Biz_Campaign_Lead_Activity
                                   }).SingleOrDefault();

            if (dynamicCampaign != null)
            {

                var _dbCampaignDetails = dynamicCampaign.campaignDetails;
                var _dbCampaignSupportUserDetails = dynamicCampaign.campaignSupportUserDetails.Where(m => m.Support_User_Id == supportUserId && !m.Is_Deleted).ToList();
                var _dbCampaignAssignedDetails = dynamicCampaign.campaignAssignedDetails.Where(m => m.Support_User_Id == supportUserId && !m.Is_Deleted).ToList();
                var _dbCampaignLeadActivityLst = dynamicCampaign.campaignLeadActivity.Where(m => m.Support_User_Id == supportUserId && !m.Is_Deleted).ToList();

                campaignDetails.CampaignDetailsId = _dbCampaignDetails.Campaign_Details_Id;
                campaignDetails.AdminUserId = _dbCampaignDetails.Admin_User_Id;
                campaignDetails.CampaignName = _dbCampaignDetails.Campaign_Name;
                campaignDetails.CampaignFileName = _dbCampaignDetails.Campaign_File_Name;
                campaignDetails.FilePath = _dbCampaignDetails.File_Path;
                campaignDetails.Goals = _dbCampaignDetails.Goals;
                campaignDetails.NoOfLeads = _dbCampaignDetails.No_Of_Leads;
                campaignDetails.UniqueId = _dbCampaignDetails.Unique_Id;
                campaignDetails.IsPaused = _dbCampaignDetails.Is_Paused;
                campaignDetails.IsSuspended = _dbCampaignDetails.Is_Suspended;
                campaignDetails.AdminProjectId = _dbCampaignDetails.Admin_Project_Id;
                campaignDetails.CampaignStartDate = _dbCampaignDetails.Campaign_Start_Date;
                campaignDetails.CampaignEndDate = _dbCampaignDetails.Campaign_End_Date;
                campaignDetails.IsUploadFileAssign = _dbCampaignDetails.Is_Upload_File_Assign;
                campaignDetails.CreateTimeStamp = _dbCampaignDetails.Created_Time_Stamp;
                campaignDetails.TechTeamStatus = _dbCampaignDetails.Tech_Team_Status;
                campaignDetails.IsDiscardedRequest = _dbCampaignDetails.Is_Discarded_Request;
                campaignDetails.DiscardedReason = _dbCampaignDetails.Discarded_Reason;
                campaignDetails.CampaignType = _dbCampaignDetails.Campaign_Type;
                campaignDetails.DemoGrapicInformation = _dbCampaignDetails.DemoGrapic_Information;
                campaignDetails.TechTeamFileUploadedTime = _dbCampaignDetails.Tech_Team_File_Uploaded_Time;
                campaignDetails.Notes = _dbCampaignDetails.Notes;


                campaignDetails.CampaignSupportUserDetailsList = new List<CampaignSupportUserDetails>();

                if (_dbCampaignSupportUserDetails != null && _dbCampaignSupportUserDetails.Count > 0)
                {
                    foreach (var dbuserSupportDetails in _dbCampaignSupportUserDetails)
                    {
                        CampaignSupportUserDetails campaignSupportUserDetails = new CampaignSupportUserDetails();
                        campaignSupportUserDetails.CampaignSupportUserDetailId = dbuserSupportDetails.Campaign_Support_User_Detail_Id;
                        campaignSupportUserDetails.CampaignDetailsId = dbuserSupportDetails.Campaign_Details_Id ?? 0;
                        campaignSupportUserDetails.AdminUserId = dbuserSupportDetails.Admin_User_Id ?? 0;
                        campaignSupportUserDetails.SupportUserId = dbuserSupportDetails.Support_User_Id ?? 0;
                        campaignSupportUserDetails.NoOfUserAssigned = dbuserSupportDetails.No_Of_User_Assigned;
                        campaignSupportUserDetails.NoOfCompleted = dbuserSupportDetails.No_Of_Completed;
                        campaignSupportUserDetails.NoOfPending = dbuserSupportDetails.No_Of_Pending;
                        campaignSupportUserDetails.UserSkippedCount = dbuserSupportDetails.User_Skipped_Count;
                        campaignSupportUserDetails.IsViewed = dbuserSupportDetails.Is_Viewed;
                        campaignSupportUserDetails.AdminUserName = dbuserSupportDetails.Biz_Admin_Users1.Admin_User_Name;
                        campaignDetails.CampaignSupportUserDetailsList.Add(campaignSupportUserDetails);
                    }
                }

                campaignDetails.CampaignAssignedDetailsList = new List<CampaignAssignedDetails>();

                if (_dbCampaignAssignedDetails != null && _dbCampaignAssignedDetails.Count > 0)
                {
                    foreach (var _dbCampaignAssigned in _dbCampaignAssignedDetails)
                    {
                        CampaignAssignedDetails campaignAssignedDetails = new CampaignAssignedDetails();

                        campaignAssignedDetails.CampaignDetailsId = _dbCampaignAssigned.Campaign_Details_Id;
                        campaignAssignedDetails.Name = _dbCampaignAssigned.Name;
                        campaignAssignedDetails.EmailAddress = _dbCampaignAssigned.Email_Address;
                        campaignAssignedDetails.EIN = _dbCampaignAssigned.EIN;
                        campaignAssignedDetails.BusinessName = _dbCampaignAssigned.Business_Name;
                        campaignAssignedDetails.PhoneNumber = _dbCampaignAssigned.Phone_Number;
                        campaignAssignedDetails.Address = _dbCampaignAssigned.Address;
                        campaignAssignedDetails.SignedUpOn = _dbCampaignAssigned.Signed_Up_On;
                        campaignAssignedDetails.ProductName = _dbCampaignAssigned.Product_Name;
                        campaignAssignedDetails.UserType = _dbCampaignAssigned.User_Type;
                        var noOfTracks = (_dbCampaignAssigned.No_Of_Trucks ?? 0).ToString();
                        campaignAssignedDetails.NoOfTrucks = Convert.ToInt32(noOfTracks);
                        campaignAssignedDetails.LastFiledOn = _dbCampaignAssigned.Last_Filed_On;
                        campaignAssignedDetails.Subscribed = _dbCampaignAssigned.Subscribed;
                        campaignAssignedDetails.SupportUserId = _dbCampaignAssigned.Support_User_Id;
                        campaignAssignedDetails.CampaignAssignedDetailsId = _dbCampaignAssigned.Campaign_Assigned_Details_Id;
                        campaignAssignedDetails.IsSkip = _dbCampaignAssigned.Is_Skip;
                        //  campaignAssignedDetails.LeadStatus = _dbCampaignAssigned.Lead_Status;
                        campaignAssignedDetails.Subscribed = _dbCampaignAssigned.Subscribed;
                        campaignAssignedDetails.ReturnNumber = _dbCampaignAssigned.ReturnNumber ?? 0;
                        campaignDetails.CampaignAssignedDetailsList.Add(campaignAssignedDetails);
                    }
                }

                campaignDetails.CampaignContactedDetailsList = new List<CampaignAssignedDetails>();

                var _dbConactedLeadDetailsIds = _dbCampaignAssignedDetails.Where(m => m.Is_Skip == true).Select(m => m.Campaign_Assigned_Details_Id).ToList();
                var _dbLeadActivityList = _dbCampaignLeadActivityLst.Select(m => m.Campaign_Assign_Detail_Id).ToList();

                if (campaignDetails.CampaignAssignedDetailsList != null && campaignDetails.CampaignAssignedDetailsList.Count > 0)
                {
                    campaignDetails.CampaignContactedDetailsList = campaignDetails.CampaignAssignedDetailsList.Where(m => _dbLeadActivityList.Contains(m.CampaignAssignedDetailsId)).ToList();
                }

                campaignDetails.CampaignSkippedDetailsList = new List<CampaignAssignedDetails>();

                if (campaignDetails.CampaignAssignedDetailsList != null && campaignDetails.CampaignAssignedDetailsList.Count > 0)
                {
                    campaignDetails.CampaignSkippedDetailsList = campaignDetails.CampaignAssignedDetailsList.Where(m => _dbConactedLeadDetailsIds.Contains(m.CampaignAssignedDetailsId)).ToList();
                }

                campaignDetails.LeadCommunicationLst = new List<LeadCommunication>();

                if (_dbCampaignLeadActivityLst != null && _dbCampaignLeadActivityLst.Count > 0)
                {
                    foreach (var _dbCampaignLeadActivity in _dbCampaignLeadActivityLst)
                    {
                        LeadCommunication leadCommunication = new LeadCommunication();

                        leadCommunication.CampaignDetailId = _dbCampaignLeadActivity.Campaign_Detail_Id;
                        leadCommunication.CampaignLeadActivityId = _dbCampaignLeadActivity.Campaign_Lead_Activity_Id;
                        leadCommunication.CampaignAssignDetailId = _dbCampaignLeadActivity.Campaign_Assign_Detail_Id;
                        leadCommunication.SupportUserId = _dbCampaignLeadActivity.Support_User_Id;
                        leadCommunication.EmailAddress = _dbCampaignLeadActivity.Email_Address;
                        leadCommunication.MethodeofContract = _dbCampaignLeadActivity.Method_Of_Contact;
                        leadCommunication.TypeOfCall = _dbCampaignLeadActivity.Type_Of_Call;
                        leadCommunication.Spoketo = _dbCampaignLeadActivity.Spoke_To;
                        leadCommunication.Comments = _dbCampaignLeadActivity.Comments;
                        leadCommunication.Reason = _dbCampaignLeadActivity.Reason;
                        leadCommunication.IsFollowRequired = _dbCampaignLeadActivity.Is_Follow_Up;
                        leadCommunication.FollowupDate = _dbCampaignLeadActivity.Followup_Date;
                        leadCommunication.FollowupTime = _dbCampaignLeadActivity.Followup_Time;
                        leadCommunication.CreateTimeStamp = _dbCampaignLeadActivity.Create_Time_Stamp;
                        campaignDetails.LeadCommunicationLst.Add(leadCommunication);
                    }
                }



            }

            return campaignDetails;

        }

        #endregion

        #region Get Campaign Lead Activity List By Campaign Detail Id
        public List<LeadDetails> GetCampaignLeadActivityListByCampaignDetailId(long campaignDetailId, long supportUserId)
        {
            List<LeadDetails> leadDetailsList = new List<LeadDetails>();

            var _dbLeadDetailsList = (from cla in _context.Biz_Campaign_Lead_Activity
                                      join cad in _context.Biz_Campaign_Assigned_Details on cla.Campaign_Assign_Detail_Id equals cad.Campaign_Assigned_Details_Id
                                      where cla.Campaign_Detail_Id == campaignDetailId && cla.Support_User_Id == supportUserId &&
                                      !cla.Is_Deleted && !cad.Is_Deleted
                                      select new
                                      {
                                          cla.Campaign_Lead_Activity_Id,
                                          cla.Campaign_Assign_Detail_Id,
                                          cla.Method_Of_Contact,
                                          cla.Comments,
                                          cad.Business_Name
                                      }).ToList();

            if (_dbLeadDetailsList != null && _dbLeadDetailsList.Any())
            {
                foreach (var item in _dbLeadDetailsList)
                {
                    LeadDetails leadDetails = new LeadDetails();
                    leadDetails.MethodOfContact = (MethodOfContact)Enum.Parse(typeof(MethodOfContact), item.Method_Of_Contact.ToString(), true);
                    leadDetails.BusinessName = item.Business_Name;
                    leadDetails.Comments = item.Comments;
                    leadDetails.CampaignAssignedDetailsId = item.Campaign_Assign_Detail_Id;
                    leadDetails.CampaignLeadActivityId = item.Campaign_Lead_Activity_Id;
                    leadDetailsList.Add(leadDetails);
                }
            }
            return leadDetailsList;
        }
        #endregion

        #region Update Lead Count
        public void UpdateLeadCount(LeadCommunication leadCommunication)
        {
            var dbBizCampaignLeadActivity = _context.Biz_Campaign_Lead_Activity.Where(m => m.Campaign_Detail_Id == leadCommunication.CampaignDetailId && m.Support_User_Id == leadCommunication.SupportUserId && m.Campaign_Assign_Detail_Id == leadCommunication.CampaignAssignDetailId);
            if (dbBizCampaignLeadActivity != null && dbBizCampaignLeadActivity.Count() == 0)
            {
                var _dbCampaignSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
                                                     where a.Campaign_Details_Id == leadCommunication.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadCommunication.SupportUserId
                                                     select a).SingleOrDefault();
                if (_dbCampaignSupportUserDetails != null)
                {
                    _dbCampaignSupportUserDetails.No_Of_Completed = ((_dbCampaignSupportUserDetails.No_Of_Completed == null) ? 0 : (_dbCampaignSupportUserDetails.No_Of_Completed)) + 1;
                    _dbCampaignSupportUserDetails.No_Of_Pending = _dbCampaignSupportUserDetails.No_Of_Pending - 1;
                    _dbCampaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
                    _context.SaveChanges();
                }
            }
        }
        #endregion

        #region Get All States
        public List<State> GetAllStates()
        {
            List<State> stateList = new List<State>();
            var dbState = (from s in _context.Static_Biz_Admin_States
                           where s.Is_Deleted == false && s.Country_Id == 1
                           select new
                           {
                               s.Country_Id,
                               s.State_Code,
                               s.State_Id,
                               s.State_Name
                           }).ToList();

            if (dbState != null && dbState.Any())
            {
                foreach (var item in dbState)
                {
                    State state = new State();
                    state.CountryId = item.Country_Id;
                    state.StateCode = item.State_Code;
                    state.StateId = item.State_Id;
                    state.StateName = item.State_Name;

                    stateList.Add(state);
                }
            }

            return stateList;
        }
        #endregion

        //#region Get Lead Details List By StateId
        //public LeadDetails GetLeadDetailsListByStateId(long campaignId, long supportUserId, string stateCode)
        //{
        //    LeadDetails leadDetails = new LeadDetails();
        //    var _dbLeadDetailslist = (from a in _context.Biz_Campaign_Support_User_Details
        //                              where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId && a.Last_Lead_Id != null
        //                              select a).FirstOrDefault();
        //    if (_dbLeadDetailslist != null)
        //    {
        //        bool IsFiltercon = (from a in _context.Biz_Campaign_Assigned_Details
        //                            where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                            && a.Address.Contains(", " + stateCode) && a.Campaign_Assigned_Details_Id == _dbLeadDetailslist.Last_Lead_Id
        //                            orderby a.Campaign_Assigned_Details_Id
        //                            select a.Campaign_Assigned_Details_Id).Any();
        //        if (IsFiltercon)
        //        {
        //            var _dbLeadDetails = (from a in _context.Biz_Campaign_Assigned_Details
        //                                  where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                  && a.Address.Contains(", " + stateCode) && a.Campaign_Assigned_Details_Id == _dbLeadDetailslist.Last_Lead_Id
        //                                  orderby a.Campaign_Assigned_Details_Id
        //                                  select a).Take(1).SingleOrDefault();
        //            if (_dbLeadDetails != null)
        //            {
        //                leadDetails.Name = _dbLeadDetails.Name;
        //                leadDetails.BusinessName = _dbLeadDetails.Business_Name;
        //                leadDetails.Phone = _dbLeadDetails.Phone_Number;
        //                leadDetails.Email = _dbLeadDetails.Email_Address;
        //                leadDetails.Address = _dbLeadDetails.Address;
        //                leadDetails.UserType = _dbLeadDetails.User_Type;
        //                leadDetails.LastFiled = _dbLeadDetails.Last_Filed_On ?? DateTime.Now;
        //                leadDetails.NoofTrucks = _dbLeadDetails.No_Of_Trucks ?? 0;
        //                leadDetails.CampaignAssignedDetailsId = _dbLeadDetails.Campaign_Assigned_Details_Id;
        //            }
        //        }
        //        else
        //        {
        //            var _dbLeadDetails = (from a in _context.Biz_Campaign_Assigned_Details
        //                                  where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                  && !a.Address.Contains(", " + stateCode) && a.Campaign_Assigned_Details_Id == _dbLeadDetailslist.Last_Lead_Id
        //                                  orderby a.Campaign_Assigned_Details_Id
        //                                  select a).Take(1).SingleOrDefault();
        //            if (_dbLeadDetails != null)
        //            {
        //                leadDetails.Name = _dbLeadDetails.Name;
        //                leadDetails.BusinessName = _dbLeadDetails.Business_Name;
        //                leadDetails.Phone = _dbLeadDetails.Phone_Number;
        //                leadDetails.Email = _dbLeadDetails.Email_Address;
        //                leadDetails.Address = _dbLeadDetails.Address;
        //                leadDetails.UserType = _dbLeadDetails.User_Type;
        //                leadDetails.LastFiled = _dbLeadDetails.Last_Filed_On ?? DateTime.Now;
        //                leadDetails.NoofTrucks = _dbLeadDetails.No_Of_Trucks ?? 0;
        //                leadDetails.CampaignAssignedDetailsId = _dbLeadDetails.Campaign_Assigned_Details_Id;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        bool IsFilter = (from a in _context.Biz_Campaign_Assigned_Details
        //                         where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                         && a.Address.Contains(", " + stateCode)
        //                         orderby a.Campaign_Assigned_Details_Id
        //                         select a.Campaign_Assigned_Details_Id).Any();
        //        if (IsFilter)
        //        {
        //            var _dbLeadDetails = (from a in _context.Biz_Campaign_Assigned_Details
        //                                  where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                  && a.Address.Contains(", " + stateCode)
        //                                  orderby a.Campaign_Assigned_Details_Id
        //                                  select a).Take(1).SingleOrDefault();
        //            if (_dbLeadDetails != null)
        //            {
        //                leadDetails.Name = _dbLeadDetails.Name;
        //                leadDetails.BusinessName = _dbLeadDetails.Business_Name;
        //                leadDetails.Phone = _dbLeadDetails.Phone_Number;
        //                leadDetails.Email = _dbLeadDetails.Email_Address;
        //                leadDetails.Address = _dbLeadDetails.Address;
        //                leadDetails.UserType = _dbLeadDetails.User_Type;
        //                leadDetails.LastFiled = _dbLeadDetails.Last_Filed_On ?? DateTime.Now;
        //                leadDetails.NoofTrucks = _dbLeadDetails.No_Of_Trucks ?? 0;
        //                leadDetails.CampaignAssignedDetailsId = _dbLeadDetails.Campaign_Assigned_Details_Id;
        //            }
        //        }
        //        else
        //        {
        //            var _dbLeadDetails = (from a in _context.Biz_Campaign_Assigned_Details
        //                                  where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                  && !a.Address.Contains(", " + stateCode)
        //                                  orderby a.Campaign_Assigned_Details_Id
        //                                  select a).Take(1).SingleOrDefault();
        //            if (_dbLeadDetails != null)
        //            {
        //                leadDetails.Name = _dbLeadDetails.Name;
        //                leadDetails.BusinessName = _dbLeadDetails.Business_Name;
        //                leadDetails.Phone = _dbLeadDetails.Phone_Number;
        //                leadDetails.Email = _dbLeadDetails.Email_Address;
        //                leadDetails.Address = _dbLeadDetails.Address;
        //                leadDetails.UserType = _dbLeadDetails.User_Type;
        //                leadDetails.LastFiled = _dbLeadDetails.Last_Filed_On ?? DateTime.Now;
        //                leadDetails.NoofTrucks = _dbLeadDetails.No_Of_Trucks ?? 0;
        //                leadDetails.CampaignAssignedDetailsId = _dbLeadDetails.Campaign_Assigned_Details_Id;
        //            }
        //        }
        //    }
        //    return leadDetails;

        //}
        //#endregion

        #region Get Lead Details List By StateId
        public LeadDetails GetLeadDetailsListByStateId(long campaignId, long supportUserId, string stateCode)
        {
            LeadDetails leadDetails = new LeadDetails();

            string dbStateName = GetStateNameByStateCode(stateCode);


            var _dbLeadDetailslist = (from a in _context.Biz_Campaign_Support_User_Details
                                      where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId && a.Last_Lead_Id != null
                                      select a.Last_Lead_Id).FirstOrDefault();

            //var _dbExistsCampaignAssignedDetails = (from a in _context.Biz_Campaign_Support_User_Details
            //                                        where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId && a.Last_Lead_Id != null
            //                                        select a).FirstOrDefault();

            if (_dbLeadDetailslist == null || _dbLeadDetailslist == 0)
            {
                _dbLeadDetailslist = (from a in _context.Biz_Campaign_Assigned_Details
                                      where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                     && a.Address != null && (a.Address.ToLower().Contains(", " + stateCode.ToLower()) || a.Address.ToLower().Contains("," + stateCode.ToLower()) || a.Address.ToLower().Contains(", " + dbStateName.ToLower()) || a.Address.ToLower().Contains("," + dbStateName.ToLower()))
                                      orderby a.Created_Time_Stamp
                                      select a.Campaign_Assigned_Details_Id).Take(1).FirstOrDefault();

                if (_dbLeadDetailslist > 0)
                {
                    var _dbCampaignSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
                                                         where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                                         select a).FirstOrDefault();

                    if (_dbCampaignSupportUserDetails != null && _dbCampaignSupportUserDetails.Campaign_Support_User_Detail_Id > 0)
                    {
                        _dbCampaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
                        _dbCampaignSupportUserDetails.Last_Lead_Id = _dbLeadDetailslist;
                        _context.SaveChanges();
                    }

                }



            }
            if (_dbLeadDetailslist > 0)
            {
                var leadBusinessDetail = (from a in _context.Biz_Campaign_Assigned_Details
                                          where a.Campaign_Assigned_Details_Id == _dbLeadDetailslist
                                          orderby a.Created_Time_Stamp
                                          select new
                                          {
                                              a.Name,
                                              a.Business_Name,
                                              a.Phone_Number,
                                              a.Email_Address,
                                              a.Address,
                                              a.EIN,
                                              a.User_Type,
                                              a.Last_Filed_On,
                                              a.No_Of_Trucks,
                                              a.Campaign_Assigned_Details_Id
                                          }).SingleOrDefault();
                leadDetails.Name = leadBusinessDetail.Name;
                leadDetails.BusinessName = leadBusinessDetail.Business_Name;
                leadDetails.Phone = leadBusinessDetail.Phone_Number;
                leadDetails.Email = leadBusinessDetail.Email_Address;
                leadDetails.Address = leadBusinessDetail.Address;
                leadDetails.UserType = leadBusinessDetail.User_Type;
                leadDetails.LastFiled = leadBusinessDetail.Last_Filed_On ?? DateTime.Now;
                leadDetails.NoofTrucks = leadBusinessDetail.No_Of_Trucks ?? 0;
                leadDetails.EIN = leadBusinessDetail.EIN;
                leadDetails.CampaignAssignedDetailsId = leadBusinessDetail.Campaign_Assigned_Details_Id;
            }

            var dbSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
                                        where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                        select a).FirstOrDefault();

            if (dbSupportUserDetails != null)
            {
                leadDetails.TotalAssignedLeads = dbSupportUserDetails.No_Of_User_Assigned ?? 0;
            }

            var dbCampaignAssignedDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                             where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                             orderby a.Created_Time_Stamp
                                             select a.Campaign_Assigned_Details_Id).ToList();

            if (dbCampaignAssignedDetails != null && dbCampaignAssignedDetails.Any())
            {
                leadDetails.CurrentViewedLeads = dbCampaignAssignedDetails.IndexOf(leadDetails.CampaignAssignedDetailsId) + 1;
            }

            return leadDetails;

        }
        #endregion

        #region Get Lead Details List By Timezone
        public LeadDetails GetLeadDetailsListByTimezone(long campaignId, long supportUserId, string timeZone)
        {
            LeadDetails leadDetails = new LeadDetails();
            if (!string.IsNullOrWhiteSpace(timeZone))
            {

                string dbState = _context.Static_Biz_Admin_States.Where(a => a.Time_Zone == timeZone).Select(a => a.State_Code).FirstOrDefault();


                var _dbLeadDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                      where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                      && a.Address.Contains(", " + dbState)
                                      orderby a.Created_Time_Stamp
                                      select a).Take(1).SingleOrDefault();

                if (_dbLeadDetails != null)
                {
                    leadDetails.Name = _dbLeadDetails.Name;
                    leadDetails.BusinessName = _dbLeadDetails.Business_Name;
                    leadDetails.Phone = _dbLeadDetails.Phone_Number;
                    leadDetails.Email = _dbLeadDetails.Email_Address;
                    leadDetails.Address = _dbLeadDetails.Address;
                    leadDetails.UserType = _dbLeadDetails.User_Type;
                    leadDetails.LastFiled = _dbLeadDetails.Last_Filed_On ?? DateTime.Now;
                    leadDetails.NoofTrucks = _dbLeadDetails.No_Of_Trucks ?? 0;
                    leadDetails.CampaignAssignedDetailsId = _dbLeadDetails.Campaign_Assigned_Details_Id;
                    leadDetails.EIN = _dbLeadDetails.EIN;
                }
            }
            return leadDetails;

        }
        #endregion

        #region  Update Last Lead Id
        public void UpdateLastLeadId(LeadCommunication leadCommunication)
        {

            long LastLeadId = 0;

            if (leadCommunication.IsSkip)
            {
                LastLeadId = (from a in _context.Biz_Campaign_Assigned_Details
                              where a.Campaign_Details_Id == leadCommunication.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadCommunication.SupportUserId && a.Is_Skip == true && a.Campaign_Assigned_Details_Id > leadCommunication.CampaignAssignDetailId
                              orderby a.Created_Time_Stamp
                              select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
            }
            else
            {
                LastLeadId = (from a in _context.Biz_Campaign_Assigned_Details
                              where a.Campaign_Details_Id == leadCommunication.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadCommunication.SupportUserId && a.Campaign_Assigned_Details_Id > leadCommunication.CampaignAssignDetailId
                              orderby a.Created_Time_Stamp
                              select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
            }

            if (LastLeadId == 0)
            {
                LastLeadId = leadCommunication.CampaignAssignDetailId;
            }

            var _dbCampaignSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
                                                 where a.Campaign_Details_Id == leadCommunication.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadCommunication.SupportUserId
                                                 select a).SingleOrDefault();
            if (_dbCampaignSupportUserDetails != null && LastLeadId > 0)
            {
                _dbCampaignSupportUserDetails.Last_Lead_Id = LastLeadId;
                _dbCampaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
            }
        }
        #endregion

        #region  UpdateLastLeadIdDuringBack
        public bool UpdateLastLeadIdDuringBack(long campaignId, long supportUserId)
        {
            bool isupdated = false;
            var _dbLeadDetailslist = (from a in _context.Biz_Campaign_Support_User_Details
                                      where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                      select a.Last_Lead_Id).Take(1).SingleOrDefault();
            var LastLeadId = (from a in _context.Biz_Campaign_Assigned_Details
                              where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId && a.Campaign_Assigned_Details_Id
                              < _dbLeadDetailslist
                              orderby a.Created_Time_Stamp descending
                              select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
            var _dbCampaignSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
                                                 where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                                 select a).SingleOrDefault();
            if (_dbCampaignSupportUserDetails != null)
            {
                _dbCampaignSupportUserDetails.Last_Lead_Id = LastLeadId;
                _dbCampaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
                isupdated = true;
            }
            return isupdated;
        }
        #endregion

        #region  UpdateLastLeadIdDuringSkip
        public bool UpdateLastLeadIdDuringSkip(long campaignId, long supportUserId)
        {
            bool isupdated = false;
            var _dbLeadDetailslist = (from a in _context.Biz_Campaign_Support_User_Details
                                      where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                      select a.Last_Lead_Id).Take(1).SingleOrDefault();

            if (_dbLeadDetailslist != null)
            {

                var LastLeadId = (from a in _context.Biz_Campaign_Assigned_Details
                                  where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId && a.Campaign_Assigned_Details_Id
                                  > _dbLeadDetailslist
                                  orderby a.Created_Time_Stamp
                                  select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
                var _dbCampaignSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
                                                     where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                                     select a).SingleOrDefault();
                if (_dbCampaignSupportUserDetails != null)
                {
                    _dbCampaignSupportUserDetails.Last_Lead_Id = LastLeadId;
                    _dbCampaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
                    _context.SaveChanges();
                    isupdated = true;
                }

            }
            else
            {
                var _dbCampaignAssignedDetailsId = (from a in _context.Biz_Campaign_Assigned_Details
                                                    where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                                    orderby a.Created_Time_Stamp
                                                    select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
                var LastLeadId = (from a in _context.Biz_Campaign_Assigned_Details
                                  where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId && a.Campaign_Assigned_Details_Id
                                  > _dbCampaignAssignedDetailsId
                                  orderby a.Created_Time_Stamp
                                  select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
                var _dbCampaignSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
                                                     where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                                     select a).SingleOrDefault();
                if (_dbCampaignSupportUserDetails != null)
                {
                    _dbCampaignSupportUserDetails.Last_Lead_Id = LastLeadId;
                    _dbCampaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
                    _context.SaveChanges();
                    isupdated = true;
                }

            }



            return isupdated;
        }
        #endregion

        #region Get Campaign Recent Activity List
        public List<LeadCommunication> GetCampaignRecentActivityList(long campaignAssignedDetailsId)
        {
            List<LeadCommunication> leadCommunicationList = new List<LeadCommunication>();
            //var _dbLeadDetailslist = (from a in _context.Biz_Campaign_Support_User_Details
            //                          where a.Campaign_Details_Id == campaignDetailId && !a.Is_Deleted && a.Support_User_Id == supportUserId && a.Last_Lead_Id != null
            //                          select a).FirstOrDefault();
            //if (_dbLeadDetailslist != null)
            //{
            //    CampaignAssignDetailId = _dbLeadDetailslist.Last_Lead_Id ?? 0;
            //    // CampaignAssignDetailId = 253;
            //}
            //else
            //{
            //    var _dbLeadDetails = (from a in _context.Biz_Campaign_Assigned_Details
            //                          where a.Campaign_Details_Id == campaignDetailId && !a.Is_Deleted && a.Support_User_Id == supportUserId
            //                          orderby a.Campaign_Assigned_Details_Id
            //                          select a).Take(1).SingleOrDefault();

            //    if (_dbLeadDetails != null)
            //    {
            //        CampaignAssignDetailId = _dbLeadDetails.Campaign_Assigned_Details_Id;
            //    }

            //}
            //var _dbLeadCommunicationList = (from cla in _context.Biz_Campaign_Lead_Activity
            //                                join Supportuser in _context.Biz_Admin_Users on cla.Support_User_Id equals Supportuser.Admin_User_Id
            //                                join CampaignDetail in _context.Biz_Campaign_Details on cla.Campaign_Detail_Id equals CampaignDetail.Campaign_Details_Id
            //                                where cla.Campaign_Detail_Id == campaignDetailId && cla.Support_User_Id == supportUserId && cla.Campaign_Assign_Detail_Id == CampaignAssignDetailId &&
            //                                !cla.Is_Deleted && !Supportuser.Is_Deleted
            //                                select new
            //                                {
            //                                    cla,
            //                                    Supportuser.Admin_User_Name,
            //                                    CampaignDetail.Campaign_Name
            //                                }).ToList();


            var _dbLeadCommunicationList = (from cla in _context.Biz_Campaign_Lead_Activity
                                            join Supportuser in _context.Biz_Admin_Users on cla.Support_User_Id equals Supportuser.Admin_User_Id
                                            where !cla.Is_Deleted && cla.Campaign_Assign_Detail_Id == campaignAssignedDetailsId
                                            orderby cla.Create_Time_Stamp descending
                                            select new
                                            {
                                                cla,
                                                cla.Biz_Campaign_Details.Campaign_Name,
                                                Supportuser.Admin_User_Name
                                            }).ToList();


            if (_dbLeadCommunicationList != null && _dbLeadCommunicationList.Any())
            {
                foreach (var item in _dbLeadCommunicationList)
                {
                    LeadCommunication leadCommunication = new LeadCommunication();
                    leadCommunication.CreatedDate = item.cla.Create_Time_Stamp;
                    leadCommunication.MethodOfContact = (MethodOfContact)Enum.Parse(typeof(MethodOfContact), item.cla.Method_Of_Contact.ToString(), true);
                    leadCommunication.FollowupDate = item.cla.Followup_Date;
                    leadCommunication.SupportUserName = item.Admin_User_Name;
                    leadCommunication.LeadStatus = item.cla.Lead_Status;
                    leadCommunication.CampaignLeadActivityId = item.cla.Campaign_Lead_Activity_Id;
                    leadCommunication.Comments = item.cla.Comments;
                    leadCommunication.FollowupTime = item.cla.Followup_Time;
                    leadCommunication.DonotContactagain = item.cla.Is_Do_Not_Disturb;
                    leadCommunication.CampaignName = item.Campaign_Name;
                    leadCommunication.Spoketo = item.cla.Spoke_To;
                    leadCommunication.Reason = item.cla.Reason;
                    leadCommunicationList.Add(leadCommunication);
                }
            }

            return leadCommunicationList;
        }
        #endregion

        #region  Get Campaign Details First Count
        public long GetCampaignDetailsFirstCount(LeadDetailsSearchOption leadDetailsSearchOption)
        {
            string stateCode = leadDetailsSearchOption.StateCode;
            string dbStateName = GetStateNameByStateCode(stateCode);
            long? LastLeadId = 0;

            var _dbLeadDetailslist = (from a in _context.Biz_Campaign_Support_User_Details
                                      where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId
                                      select a.Last_Lead_Id).Take(1).SingleOrDefault();

            if (_dbLeadDetailslist != null)
            {

                if (leadDetailsSearchOption.Skipped)
                {
                    var dbLeadActivity = (from a in _context.Biz_Campaign_Assigned_Details
                                          where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId && a.Is_Skip == true
                                          orderby a.Created_Time_Stamp
                                          select a.Campaign_Assigned_Details_Id).ToList();

                    if (dbLeadActivity != null && dbLeadActivity.Count > 0)
                    {
                        int startPosition = dbLeadActivity.IndexOf(Convert.ToInt64(_dbLeadDetailslist));

                        if (dbLeadActivity.Count > (startPosition - 1) && startPosition != 0)
                        {
                            LastLeadId = dbLeadActivity[startPosition - 1];
                        }

                    }


                }
                else if (!string.IsNullOrWhiteSpace(leadDetailsSearchOption.StateCode))
                {

                    var _dbCampaignAssignedDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                                      where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId
                                                      select a).OrderBy(a => a.Created_Time_Stamp).ToList();


                    var dbLeadActivityAddressFilter = (from a in _dbCampaignAssignedDetails
                                                       where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId
                                                       && a.Address != null && (a.Address.ToLower().Contains(", " + stateCode.ToLower()) || a.Address.ToLower().Contains("," + stateCode.ToLower()) || a.Address.ToLower().Contains(", " + dbStateName.ToLower()) || a.Address.ToLower().Contains("," + dbStateName.ToLower()))
                                                       select a.Campaign_Assigned_Details_Id).ToList();

                    var dbLeadActivityNotAddressFilter = (from a in _dbCampaignAssignedDetails
                                                          where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId
                                                          && !dbLeadActivityAddressFilter.Contains(a.Campaign_Assigned_Details_Id)
                                                          select a.Campaign_Assigned_Details_Id).ToList();

                    var dbLeadActivity = new List<long>();

                    dbLeadActivity.AddRange(dbLeadActivityAddressFilter);

                    dbLeadActivity.AddRange(dbLeadActivityNotAddressFilter);


                    if (dbLeadActivity != null && dbLeadActivity.Count > 0)
                    {
                        int startPosition = dbLeadActivity.IndexOf(Convert.ToInt64(_dbLeadDetailslist));
                        if (dbLeadActivity.Count > (startPosition - 1) && startPosition != 0)
                        {
                            LastLeadId = dbLeadActivity[startPosition - 1];
                        }
                    }


                }
                else
                {

                    var campaignAssignedDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                                   where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId
                                                   orderby a.Created_Time_Stamp
                                                   select a.Campaign_Assigned_Details_Id).ToList();

                    if (campaignAssignedDetails != null && campaignAssignedDetails.Count > 0)
                    {
                        int startPosition = campaignAssignedDetails.IndexOf(Convert.ToInt64(_dbLeadDetailslist));

                        if (campaignAssignedDetails.Count > (startPosition - 1) && startPosition != 0)
                        {
                            LastLeadId = campaignAssignedDetails[startPosition - 1];
                        }

                    }
                }

            }


            return LastLeadId ?? 0;
        }
        #endregion

        #region Get Campaign Previous Activity By Campaign Lead Activity Id
        public LeadCommunication GetCampaignPreviousActivityByCampaignLeadActivityId(long campaignLeadActivityId)
        {
            LeadCommunication leadCommunication = new LeadCommunication();
            var _dbLeadCommunicationList = (from cla in _context.Biz_Campaign_Lead_Activity
                                            where cla.Campaign_Lead_Activity_Id == campaignLeadActivityId &&
                                            !cla.Is_Deleted
                                            select new
                                            {
                                                cla
                                            }).SingleOrDefault();

            if (_dbLeadCommunicationList != null)
            {
                leadCommunication.CreatedDate = _dbLeadCommunicationList.cla.Create_Time_Stamp;
                leadCommunication.MethodOfContact = (MethodOfContact)Enum.Parse(typeof(MethodOfContact), _dbLeadCommunicationList.cla.Method_Of_Contact.ToString(), true);
                leadCommunication.FollowupDate = _dbLeadCommunicationList.cla.Followup_Date;
                leadCommunication.LeadStatus = _dbLeadCommunicationList.cla.Lead_Status;
                leadCommunication.CampaignLeadActivityId = _dbLeadCommunicationList.cla.Campaign_Lead_Activity_Id;
                leadCommunication.Comments = _dbLeadCommunicationList.cla.Comments;
                leadCommunication.Spoketo = _dbLeadCommunicationList.cla.Spoke_To;
                leadCommunication.DonotContactagain = _dbLeadCommunicationList.cla.Is_Do_Not_Disturb;
            }

            return leadCommunication;
        }
        #endregion

        #region Get Campaign Assigned Details by CampainDetailId and SupportUserId


        public List<CampaignAssignedDetails> GetCampaignAssignedDetailsbyCampainDetailIdandSupportUserId(long campaignDetailId, long supportUserId)
        {
            List<CampaignAssignedDetails> _campaingDetailList = new List<CampaignAssignedDetails>();
            var campaingDetailList = _context.Biz_Campaign_Assigned_Details.Where(x => x.Campaign_Details_Id == campaignDetailId && x.Support_User_Id == supportUserId && !x.Is_Deleted).ToList();
            if (campaingDetailList != null && campaingDetailList.Any())
            {
                foreach (var item in campaingDetailList)
                {
                    CampaignAssignedDetails campaignDetails = new CampaignAssignedDetails();
                    campaignDetails.CampaignAssignedDetailsId = item.Campaign_Assigned_Details_Id;
                    campaignDetails.CampaignDetailsId = item.Campaign_Details_Id;
                    _campaingDetailList.Add(campaignDetails);
                }
            }
            return _campaingDetailList;
        }

        #endregion

        #region Update Campaign Details

        public List<CampaignAssignedDetails> UpdateCampaignDetails(List<CampaignAssignedDetails> CampaignAssignedDetailsList)
        {
            var campaignAssignedDetailsId = CampaignAssignedDetailsList.Select(x => x.CampaignAssignedDetailsId).ToList();
            var campaignAssignDetails = _context.Biz_Campaign_Assigned_Details.Where(x => campaignAssignedDetailsId.Contains(x.Campaign_Assigned_Details_Id)).ToList();
            if (campaignAssignDetails != null && campaignAssignDetails.Count > 0)
            {
                foreach (var item in campaignAssignDetails)
                {
                    var assignedCampaignDetails = CampaignAssignedDetailsList.Where(m => m.CampaignAssignedDetailsId == item.Campaign_Assigned_Details_Id).SingleOrDefault();
                    if (assignedCampaignDetails != null)
                    {
                        item.Is_Skip = false;
                        item.Support_User_Id = assignedCampaignDetails.SupportUserId;
                    }
                    item.Updated_Time_Stamp = DateTime.Now;
                    item.Created_Time_Stamp = DateTime.Now;
                }
                _context.SaveChanges();

                long campaingDetailId = CampaignAssignedDetailsList.Select(x => x.CampaignDetailsId).FirstOrDefault();
                long adminUserId = CampaignAssignedDetailsList.Select(x => x.AdminUserId).FirstOrDefault();

                if (campaingDetailId > 0)
                {
                    var _dbCampaignSupportAssignedDetails = _context.Biz_Campaign_Support_User_Details.Where(m => m.Campaign_Details_Id == campaingDetailId).ToList();

                    if (_dbCampaignSupportAssignedDetails != null && _dbCampaignSupportAssignedDetails.Any())
                    {
                        foreach (var item in _dbCampaignSupportAssignedDetails)
                        {
                            item.No_Of_User_Assigned = _context.Biz_Campaign_Assigned_Details.Where(x => x.Support_User_Id == item.Support_User_Id && x.Campaign_Details_Id == campaingDetailId && !x.Is_Deleted).Count();
                            item.No_Of_Pending = (item.No_Of_User_Assigned ?? 0) - (item.No_Of_Completed ?? 0);
                            item.Updated_Time_Stamp = DateTime.Now;
                        }
                        _context.SaveChanges();
                    }

                }

                if (campaingDetailId > 0)
                {
                    var supportUserIds = CampaignAssignedDetailsList.Select(x => x.SupportUserId).Distinct().ToList();
                    if (supportUserIds != null && supportUserIds.Count > 0)
                    {
                        foreach (var item in supportUserIds)
                        {
                            var campaingSupportUserDetails = _context.Biz_Campaign_Support_User_Details.SingleOrDefault(x => x.Support_User_Id == item && x.Campaign_Details_Id == campaingDetailId && !x.Is_Deleted);
                            bool isRecordExist = false;
                            if (campaingSupportUserDetails != null)
                            {
                                isRecordExist = true;
                            }
                            if (!isRecordExist)
                            {
                                campaingSupportUserDetails = new Biz_Campaign_Support_User_Details();
                            }
                            campaingSupportUserDetails.Campaign_Details_Id = campaingDetailId;
                            campaingSupportUserDetails.Support_User_Id = item;
                            campaingSupportUserDetails.Admin_User_Id = adminUserId;
                            campaingSupportUserDetails.No_Of_User_Assigned = _context.Biz_Campaign_Assigned_Details.Where(x => x.Support_User_Id == item && x.Campaign_Details_Id == campaingDetailId && !x.Is_Deleted).Count();
                            campaingSupportUserDetails.No_Of_Pending = (campaingSupportUserDetails.No_Of_User_Assigned ?? 0) - (campaingSupportUserDetails.No_Of_Completed ?? 0);
                            campaingSupportUserDetails.Is_Deleted = false;
                            campaingSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
                            if (!isRecordExist)
                            {
                                campaingSupportUserDetails.Created_Time_Stamp = DateTime.Now;
                                _context.Biz_Campaign_Support_User_Details.Add(campaingSupportUserDetails);
                            }
                            _context.SaveChanges();
                        }

                    }

                    var dbCampaignSupportAssignedDetails = _context.Biz_Campaign_Support_User_Details.Where(x => x.Campaign_Details_Id == campaingDetailId && !x.Is_Deleted).ToList();

                    if (dbCampaignSupportAssignedDetails != null && dbCampaignSupportAssignedDetails.Count > 0)
                    {
                        foreach (var dbSupportDetails in dbCampaignSupportAssignedDetails)
                        {
                            int assignedDetails = _context.Biz_Campaign_Assigned_Details.Where(m => m.Campaign_Details_Id == campaingDetailId && m.Support_User_Id == dbSupportDetails.Support_User_Id && !m.Is_Deleted).Count();
                            if (assignedDetails == 0)
                            {
                                dbSupportDetails.Is_Deleted = true;
                                dbSupportDetails.Updated_Time_Stamp = DateTime.Now;
                                _context.SaveChanges();
                            }
                        }
                    }


                }

            }



            return CampaignAssignedDetailsList;
        }

        #endregion

        #region  Update Lead Status
        public bool UpdateLeadStatus(long campaignLeadActivityId, string leadStatus)
        {
            bool isupdated = false;

            var _dbCampaignSupportUserDetails = (from a in _context.Biz_Campaign_Lead_Activity
                                                 where a.Campaign_Lead_Activity_Id == campaignLeadActivityId && !a.Is_Deleted
                                                 select a).SingleOrDefault();
            if (_dbCampaignSupportUserDetails != null)
            {
                _dbCampaignSupportUserDetails.Lead_Status = leadStatus;
                _dbCampaignSupportUserDetails.Update_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
                isupdated = true;
            }
            return isupdated;
        }
        #endregion

        #region Get Campaign Previous Activity List
        public List<LeadCommunication> GetCampaignPreviousActivityList(long campaignAssignedDetailId)
        {
            List<LeadCommunication> leadCommunicationList = new List<LeadCommunication>();

            var _dbCampaignAssigneddetails = (from a in _context.Biz_Campaign_Assigned_Details
                                              where a.Campaign_Assigned_Details_Id == campaignAssignedDetailId && !a.Is_Deleted
                                              select a).SingleOrDefault();

            if (_dbCampaignAssigneddetails != null)
            {


                var _dbLeadCommunicationList = (from cla in _context.Biz_Campaign_Lead_Activity
                                                join CampaignAssignedDetail in _context.Biz_Campaign_Assigned_Details on cla.Campaign_Assign_Detail_Id equals CampaignAssignedDetail.Campaign_Assigned_Details_Id
                                                where !cla.Is_Deleted && !CampaignAssignedDetail.Is_Deleted
                                                && (!string.IsNullOrEmpty(CampaignAssignedDetail.Email_Address) && !string.IsNullOrEmpty(_dbCampaignAssigneddetails.Email_Address) && CampaignAssignedDetail.Email_Address.Contains(_dbCampaignAssigneddetails.Email_Address))
                                                || (!string.IsNullOrEmpty(CampaignAssignedDetail.Phone_Number) && CampaignAssignedDetail.Phone_Number.Replace("(", "").Replace(")", "").Replace(" ", "").Contains(_dbCampaignAssigneddetails.Phone_Number.Replace("(", "").Replace(")", "").Replace(" ", "")))
                                                orderby cla.Campaign_Lead_Activity_Id
                                                select new
                                                {
                                                    cla,
                                                    CampaignAssignedDetail.Biz_Admin_Users.Admin_User_Name,
                                                    CampaignAssignedDetail.Biz_Campaign_Details.Campaign_Name
                                                }).ToList();


                if (_dbLeadCommunicationList != null && _dbLeadCommunicationList.Count > 0)
                {



                    foreach (var item in _dbLeadCommunicationList)
                    {
                        LeadCommunication leadCommunication = new LeadCommunication();
                        leadCommunication.CreatedDate = item.cla.Create_Time_Stamp;
                        leadCommunication.MethodOfContact = (MethodOfContact)Enum.Parse(typeof(MethodOfContact), item.cla.Method_Of_Contact.ToString(), true);
                        leadCommunication.FollowupDate = item.cla.Followup_Date;
                        leadCommunication.SupportUserName = item.Admin_User_Name;
                        leadCommunication.LeadStatus = item.cla.Lead_Status;
                        leadCommunication.CampaignLeadActivityId = item.cla.Campaign_Lead_Activity_Id;
                        leadCommunication.Comments = item.cla.Comments;
                        leadCommunication.FollowupTime = item.cla.Followup_Time;
                        leadCommunication.DonotContactagain = item.cla.Is_Do_Not_Disturb;
                        leadCommunication.CampaignName = item.Campaign_Name;
                        leadCommunication.Spoketo = item.cla.Spoke_To;
                        leadCommunication.Reason = item.cla.Reason;
                        if (!string.IsNullOrWhiteSpace(item.cla.Type_Of_Call))
                        {
                            leadCommunication.TypeofCall = (TypeofCall)Enum.Parse(typeof(TypeofCall), item.cla.Type_Of_Call, true);
                            leadCommunication.TypeOfCall = item.cla.Type_Of_Call;
                        }
                        leadCommunication.IsFollowRequired = item.cla.Is_Follow_Up;
                        leadCommunicationList.Add(leadCommunication);
                    }
                }
            }


            //var _dbLeadCommunicationList = (from cla in _context.Biz_Campaign_Lead_Activity
            //                                join Supportuser in _context.Biz_Admin_Users on cla.Support_User_Id equals Supportuser.Admin_User_Id
            //                                join CampaignDetail in _context.Biz_Campaign_Details on cla.Campaign_Detail_Id equals CampaignDetail.Campaign_Details_Id
            //                                join CampaignAssignedDetail in _context.Biz_Campaign_Assigned_Details on cla.Campaign_Assign_Detail_Id equals CampaignAssignedDetail.Campaign_Assigned_Details_Id
            //                                where CampaignAssignedDetail.Email_Address.Contains(_dbCampaignAssigneddetails.Email_Address) || CampaignAssignedDetail.Phone_Number.Contains(_dbCampaignAssigneddetails.Phone_Number) &&
            //                                !cla.Is_Deleted && !Supportuser.Is_Deleted
            //                                orderby cla.Campaign_Lead_Activity_Id
            //                                select new
            //                                {
            //                                    cla,
            //                                    Supportuser.Admin_User_Name,
            //                                    CampaignDetail.Campaign_Name
            //                                }).ToList();


            return leadCommunicationList;
        }
        #endregion

        #region Get All Admin Support Users
        public List<AdminUser> GetAllAdminSupportUsers()
        {
            var dbAdminSupportUserRoles = (from roles in _context.Biz_Admin_User_Roles
                                           join users in _context.Biz_Admin_Users on roles.Admin_User_Id equals users.Admin_User_Id
                                           where !roles.Is_Deleted && !users.Is_Deleted
                                           select new AdminUser
                                           {
                                               UserId = roles.Admin_User_Id,
                                               UserName = users.Admin_User_Name,
                                               AdminLocation = users.Location
                                           }).Distinct().OrderBy(a => a.UserName).ToList();

            return dbAdminSupportUserRoles;
        }
        #endregion

        #region Get Campaign Details by  Admin User Id
        public List<CampaignDetails> GetCampaignDetailsByFilters(JQueryDataTableParamModel param)
        {

            AdminRoleType userRoles = AdminRoleType.Manager;

            var dbUsers = _context.Biz_Admin_Users.Where(m => !m.Is_Deleted && m.Admin_User_Id == param.AdminUserId).SingleOrDefault();


            if (dbUsers != null && dbUsers.Admin_User_Id > 0)
            {
                if (dbUsers.Admin_Role == (int)AdminRoleType.Administrator)
                {
                    userRoles = AdminRoleType.Administrator;
                }
            }


            var currentDate = DateTime.Now.Date;
            List<CampaignDetails> campaignDetailsList = new List<CampaignDetails>();
            var _dbCampaignDetailsList = (from a in _context.Biz_Campaign_Details
                                          where !a.Is_Deleted && (userRoles != AdminRoleType.Administrator ? (a.Admin_User_Id == param.AdminUserId) : true) &&
                                          (
                                          param.ProjectId > 0
                                          ?
                                          a.Admin_Project_Id == param.ProjectId
                                          :
                                          a.Admin_Project_Id > 0
                                          )
                                          &&
                                          (
                                          (param.CampaignStatus != null)
                                          ?
                                          (

                                                param.CampaignStatus == CampaignStatus.Draft.ToString()
                                                ? // Draft true condition
                                                a.Tech_Team_Status == TechTeamStatus.FILE_UPLOADED.ToString() && a.Campaign_Start_Date == null && a.Campaign_End_Date == null
                                                : // Draft false condition
                                                (
                                                    param.CampaignStatus == CampaignStatus.Suspended.ToString()
                                                    ? // Suspended true condition
                                                    a.Is_Suspended
                                                    : // Suspended false condition
                                                    (
                                                        param.CampaignStatus == CampaignStatus.Active.ToString()
                                                        ? // Active true condition
                                                        a.Campaign_Start_Date.HasValue && DbFunctions.TruncateTime(a.Campaign_Start_Date) <= DbFunctions.TruncateTime(currentDate)
                                                        && a.Campaign_End_Date.HasValue && DbFunctions.TruncateTime(a.Campaign_End_Date) >= DbFunctions.TruncateTime(currentDate) && a.Is_Suspended != true

                                                        : // Active false condition
                                                        (
                                                            param.CampaignStatus == CampaignStatus.Upcoming.ToString()
                                                            ? // Upcoming true condition
                                                            a.Campaign_Start_Date.HasValue && DbFunctions.TruncateTime(a.Campaign_Start_Date) > DbFunctions.TruncateTime(currentDate) && a.Is_Suspended != true
                                                            : // Upcoming false condition
                                                            (
                                                                param.CampaignStatus == CampaignStatus.Expired.ToString()
                                                                ? // Expired true condition
                                                                    DbFunctions.TruncateTime(a.Campaign_End_Date) < DbFunctions.TruncateTime(currentDate)
                                                                    && a.Is_Suspended != true
                                                                : // Expired false condition
                                                                true
                                                            )
                                                        )
                                                    )
                                                )

                                          )
                                          :
                                          true
                                          )

                                          orderby a.Created_Time_Stamp descending
                                          select new
                                          {
                                              campaignDetails = a,
                                              campaignAssignedDetails = a.Biz_Campaign_Assigned_Details,
                                              campaignSupportUserDetails = a.Biz_Campaign_Support_User_Details,
                                          }).Skip(param.SkipRecords).Take(param.TakeRecords).ToList();

            if (_dbCampaignDetailsList != null && _dbCampaignDetailsList.Count > 0)
            {
                foreach (var dynamicCampaign in _dbCampaignDetailsList)
                {
                    var _dbCampaignDetails = dynamicCampaign.campaignDetails;
                    var _dbCampaignSupportUserDetails = dynamicCampaign.campaignSupportUserDetails.Where(x => !x.Is_Deleted).ToList();

                    CampaignDetails campaignDetails = new CampaignDetails();
                    campaignDetails.CampaignDetailsId = _dbCampaignDetails.Campaign_Details_Id;
                    campaignDetails.AdminUserId = _dbCampaignDetails.Admin_User_Id;
                    campaignDetails.CampaignName = _dbCampaignDetails.Campaign_Name;
                    campaignDetails.CampaignFileName = _dbCampaignDetails.Campaign_File_Name;
                    campaignDetails.FilePath = _dbCampaignDetails.File_Path;
                    campaignDetails.Goals = _dbCampaignDetails.Goals;
                    campaignDetails.NoOfLeads = _dbCampaignDetails.No_Of_Leads;
                    campaignDetails.UniqueId = _dbCampaignDetails.Unique_Id;
                    campaignDetails.IsPaused = _dbCampaignDetails.Is_Paused;
                    campaignDetails.IsSuspended = _dbCampaignDetails.Is_Suspended;
                    campaignDetails.AdminProjectId = _dbCampaignDetails.Admin_Project_Id;
                    campaignDetails.CampaignStartDate = _dbCampaignDetails.Campaign_Start_Date;
                    campaignDetails.CampaignEndDate = _dbCampaignDetails.Campaign_End_Date;
                    campaignDetails.IsUploadFileAssign = _dbCampaignDetails.Is_Upload_File_Assign;
                    campaignDetails.CreateTimeStamp = _dbCampaignDetails.Created_Time_Stamp;
                    campaignDetails.TechTeamStatus = _dbCampaignDetails.Tech_Team_Status;
                    campaignDetails.IsDiscardedRequest = _dbCampaignDetails.Is_Discarded_Request;
                    campaignDetails.DiscardedReason = _dbCampaignDetails.Discarded_Reason;
                    campaignDetails.DemoGrapicInformation = _dbCampaignDetails.DemoGrapic_Information;
                    campaignDetails.TechTeamFileUploadedTime = _dbCampaignDetails.Tech_Team_File_Uploaded_Time;
                    campaignDetails.Notes = _dbCampaignDetails.Notes;
                    campaignDetails.SuspendDate = _dbCampaignDetails.SuspendDate ?? DateTime.Now;
                    campaignDetails.SuspendReason = _dbCampaignDetails.SuspendReason;
                    campaignDetails.CampaignType = _dbCampaignDetails.Campaign_Type;

                    campaignDetails.CampaignSupportUserDetailsList = new List<CampaignSupportUserDetails>();

                    if (_dbCampaignSupportUserDetails != null && _dbCampaignSupportUserDetails.Count > 0)
                    {
                        foreach (var dbuserSupportDetails in _dbCampaignSupportUserDetails)
                        {
                            CampaignSupportUserDetails campaignSupportUserDetails = new CampaignSupportUserDetails();
                            campaignSupportUserDetails.CampaignSupportUserDetailId = dbuserSupportDetails.Campaign_Support_User_Detail_Id;
                            campaignSupportUserDetails.CampaignDetailsId = dbuserSupportDetails.Campaign_Details_Id ?? 0;
                            campaignSupportUserDetails.AdminUserId = dbuserSupportDetails.Admin_User_Id ?? 0;
                            campaignSupportUserDetails.SupportUserId = dbuserSupportDetails.Support_User_Id ?? 0;
                            campaignSupportUserDetails.NoOfUserAssigned = dbuserSupportDetails.No_Of_User_Assigned;
                            campaignSupportUserDetails.NoOfCompleted = dbuserSupportDetails.No_Of_Completed;
                            campaignSupportUserDetails.NoOfPending = dbuserSupportDetails.No_Of_Pending;
                            campaignSupportUserDetails.UserSkippedCount = dbuserSupportDetails.User_Skipped_Count;
                            campaignSupportUserDetails.IsViewed = dbuserSupportDetails.Is_Viewed;
                            campaignSupportUserDetails.AdminUserName = dbuserSupportDetails.Biz_Admin_Users1.Admin_User_Name;
                            campaignDetails.CampaignSupportUserDetailsList.Add(campaignSupportUserDetails);
                        }
                    }


                    campaignDetailsList.Add(campaignDetails);

                }
            }

            return campaignDetailsList;

        }

        #endregion

        #region Update Campaign Assigned Details Business Name By Campaign Assigned Details Id
        public bool UpdateCampaignAssignedDetailsBusinessNameByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string businessName)
        {
            bool isUpdated = false;

            var _dbCampaignAssignedDetailsDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                                     where a.Campaign_Assigned_Details_Id == campaignAssignedDetailsId && !a.Is_Deleted
                                                     select a).SingleOrDefault();
            if (_dbCampaignAssignedDetailsDetails != null)
            {
                _dbCampaignAssignedDetailsDetails.Business_Name = businessName;
                _dbCampaignAssignedDetailsDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
                isUpdated = true;
            }
            return isUpdated;
        }
        #endregion

        #region Update Campaign Assigned Details Name By Campaign Assigned Details Id
        public bool UpdateCampaignAssignedDetailsNameByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string name)
        {
            bool isUpdated = false;

            var _dbCampaignAssignedDetailsDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                                     where a.Campaign_Assigned_Details_Id == campaignAssignedDetailsId && !a.Is_Deleted
                                                     select a).SingleOrDefault();
            if (_dbCampaignAssignedDetailsDetails != null)
            {
                _dbCampaignAssignedDetailsDetails.Name = name;
                _dbCampaignAssignedDetailsDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
                isUpdated = true;
            }
            return isUpdated;
        }
        #endregion

        #region Update Campaign Assigned Details Phone By Campaign Assigned Details Id
        public bool UpdateCampaignAssignedDetailsPhoneByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string phone)
        {
            bool isUpdated = false;

            var _dbCampaignAssignedDetailsDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                                     where a.Campaign_Assigned_Details_Id == campaignAssignedDetailsId && !a.Is_Deleted
                                                     select a).SingleOrDefault();
            if (_dbCampaignAssignedDetailsDetails != null)
            {
                _dbCampaignAssignedDetailsDetails.Phone_Number = phone;
                _dbCampaignAssignedDetailsDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
                isUpdated = true;
            }
            return isUpdated;
        }
        #endregion

        #region Update Campaign Assigned Details Email By Campaign Assigned Details Id
        public bool UpdateCampaignAssignedDetailsEmailByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string email)
        {
            bool isUpdated = false;

            var _dbCampaignAssignedDetailsDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                                     where a.Campaign_Assigned_Details_Id == campaignAssignedDetailsId && !a.Is_Deleted
                                                     select a).SingleOrDefault();
            if (_dbCampaignAssignedDetailsDetails != null)
            {
                _dbCampaignAssignedDetailsDetails.Email_Address = email;
                _dbCampaignAssignedDetailsDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
                isUpdated = true;
            }
            return isUpdated;
        }
        #endregion

        #region Update Campaign Assigned Details Address By Campaign Assigned Details Id
        public bool UpdateCampaignAssignedDetailsAddressByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string address)
        {
            bool isUpdated = false;

            var _dbCampaignAssignedDetailsDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                                     where a.Campaign_Assigned_Details_Id == campaignAssignedDetailsId && !a.Is_Deleted
                                                     select a).SingleOrDefault();
            if (_dbCampaignAssignedDetailsDetails != null)
            {
                _dbCampaignAssignedDetailsDetails.Address = address;
                _dbCampaignAssignedDetailsDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
                isUpdated = true;
            }
            return isUpdated;
        }
        #endregion

        #region Update Campaign Assigned Details No ot Trucks By Campaign Assigned Details Id
        public bool UpdateCampaignAssignedDetailsNofTrucksByCampaignAssignedDetailsId(long campaignAssignedDetailsId, long nofTrucks)
        {
            bool isUpdated = false;

            var _dbCampaignAssignedDetailsDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                                     where a.Campaign_Assigned_Details_Id == campaignAssignedDetailsId && !a.Is_Deleted
                                                     select a).SingleOrDefault();
            if (_dbCampaignAssignedDetailsDetails != null)
            {
                _dbCampaignAssignedDetailsDetails.No_Of_Trucks = nofTrucks;
                _dbCampaignAssignedDetailsDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
                isUpdated = true;
            }
            return isUpdated;
        }
        #endregion

        #region Get Campaign Details Count By Admin User Id

        public int GetCampaignDetailsCountByAdminUserId(JQueryDataTableParamModel param)
        {
            AdminRoleType userRoles = AdminRoleType.Manager;

            var dbUsers = _context.Biz_Admin_Users.Where(m => !m.Is_Deleted && m.Admin_User_Id == param.AdminUserId).SingleOrDefault();


            if (dbUsers != null && dbUsers.Admin_User_Id > 0)
            {
                if (dbUsers.Admin_Role == (int)AdminRoleType.Administrator)
                {
                    userRoles = AdminRoleType.Administrator;
                }
            }

            var _dbCampaignDetailsListCount = (from a in _context.Biz_Campaign_Details
                                               where !a.Is_Deleted && (userRoles != AdminRoleType.Administrator ? (a.Admin_User_Id == param.AdminUserId) : true) &&
                                               (
                                                  param.ProjectId > 0
                                                  ?
                                                  a.Admin_Project_Id == param.ProjectId
                                                  :
                                                  a.Admin_Project_Id > 0
                                               )
                                               &&
                                                  (
                                                  (param.CampaignStatus != null)
                                                  ?
                                                  (

                                                        param.CampaignStatus == CampaignStatus.Draft.ToString()
                                                        ? // Draft true condition
                                                        a.Tech_Team_Status == TechTeamStatus.FILE_UPLOADED.ToString() && a.Campaign_Start_Date == null && a.Campaign_End_Date == null
                                                        : // Draft false condition
                                                        (
                                                            param.CampaignStatus == CampaignStatus.Suspended.ToString()
                                                            ? // Suspended true condition
                                                            a.Is_Suspended
                                                            : // Suspended false condition
                                                            (
                                                                param.CampaignStatus == CampaignStatus.Active.ToString()
                                                                ? // Active true condition
                                                                a.Campaign_Start_Date.HasValue && a.Campaign_Start_Date >= DateTime.Now
                                                                && a.Campaign_End_Date.HasValue && a.Campaign_End_Date <= DateTime.Now

                                                                : // Active false condition
                                                                (
                                                                    param.CampaignStatus == CampaignStatus.Upcoming.ToString()
                                                                    ? // Upcoming true condition
                                                                    a.Campaign_Start_Date.HasValue && a.Campaign_Start_Date > DateTime.Now
                                                                    : // Upcoming false condition
                                                                    (
                                                                        param.CampaignStatus == CampaignStatus.Expired.ToString()
                                                                        ? // Expired true condition
                                                                            a.Campaign_Start_Date < DateTime.Now
                                                                        : // Expired false condition
                                                                        true
                                                                    )
                                                                )
                                                            )
                                                        )

                                                  )
                                                  :
                                                  true
                                                  )
                                               select new
                                               {
                                                   a.Campaign_Details_Id
                                               }).Count();

            return _dbCampaignDetailsListCount;
        }

        #endregion

        #region Get Campaign Details by  Admin User Id
        public List<CampaignDetails> GetCampaignSupportUserDetails(JQueryDataTableParamModel param)
        {
            List<CampaignDetails> campaignDetailsList = new List<CampaignDetails>();
            DateTime currentDate = DateTime.Now.Date;
            var _dbCampaignDetailsList = (from csd in _context.Biz_Campaign_Support_User_Details
                                          join cd in _context.Biz_Campaign_Details on csd.Campaign_Details_Id equals cd.Campaign_Details_Id
                                          where csd.Support_User_Id == param.AdminUserId && !csd.Is_Deleted && !cd.Is_Deleted
                                          && (param.ProjectId > 0 ? cd.Admin_Project_Id == param.ProjectId : cd.Admin_Project_Id > 0)
                                          && cd.Is_Paused == false
                                          && (param.CampaignStatus == LeadListStatus.Expired.ToString() ? (DbFunctions.TruncateTime(cd.Campaign_End_Date) < currentDate) :
                                              param.CampaignStatus == LeadListStatus.Active.ToString() ? (DbFunctions.TruncateTime(cd.Campaign_Start_Date) <= currentDate && DbFunctions.TruncateTime(cd.Campaign_End_Date) >= currentDate) :
                                             param.CampaignStatus == LeadListStatus.Upcoming.ToString() ? DbFunctions.TruncateTime(cd.Campaign_Start_Date) > currentDate : true)

                                          && cd.Is_Suspended == false
                                          orderby cd.Created_Time_Stamp descending
                                          select new
                                          {
                                              cd,
                                              csd.Campaign_Support_User_Detail_Id,
                                              csd.No_Of_Completed,
                                              csd.No_Of_Pending,
                                              csd.No_Of_User_Assigned,
                                              csd.Support_User_Id,
                                              csd.Last_Lead_Id,
                                          }).Skip(param.SkipRecords).Take(param.TakeRecords).ToList();
            if (_dbCampaignDetailsList != null && _dbCampaignDetailsList.Count > 0)
            {
                foreach (var dynamicCampaign in _dbCampaignDetailsList)
                {
                    var _dbCampaignDetails = dynamicCampaign.cd;
                    CampaignDetails campaignDetails = new CampaignDetails();
                    campaignDetails.CampaignDetailsId = _dbCampaignDetails.Campaign_Details_Id;
                    campaignDetails.AdminUserId = _dbCampaignDetails.Admin_User_Id;
                    campaignDetails.CampaignName = _dbCampaignDetails.Campaign_Name;
                    campaignDetails.CampaignFileName = _dbCampaignDetails.Campaign_File_Name;
                    campaignDetails.FilePath = _dbCampaignDetails.File_Path;
                    campaignDetails.Goals = _dbCampaignDetails.Goals;
                    campaignDetails.NoOfLeads = _dbCampaignDetails.No_Of_Leads;
                    campaignDetails.UniqueId = _dbCampaignDetails.Unique_Id;
                    campaignDetails.IsPaused = _dbCampaignDetails.Is_Paused;
                    campaignDetails.IsSuspended = _dbCampaignDetails.Is_Suspended;
                    campaignDetails.AdminProjectId = _dbCampaignDetails.Admin_Project_Id;
                    campaignDetails.CampaignStartDate = _dbCampaignDetails.Campaign_Start_Date;
                    campaignDetails.CampaignEndDate = _dbCampaignDetails.Campaign_End_Date;
                    campaignDetails.IsUploadFileAssign = _dbCampaignDetails.Is_Upload_File_Assign;
                    campaignDetails.CreateTimeStamp = _dbCampaignDetails.Created_Time_Stamp;
                    campaignDetails.TechTeamStatus = _dbCampaignDetails.Tech_Team_Status;
                    campaignDetails.IsDiscardedRequest = _dbCampaignDetails.Is_Discarded_Request;
                    campaignDetails.DiscardedReason = _dbCampaignDetails.Discarded_Reason;
                    campaignDetails.CampaignType = _dbCampaignDetails.Campaign_Type;
                    campaignDetails.DemoGrapicInformation = _dbCampaignDetails.DemoGrapic_Information;
                    campaignDetails.TechTeamFileUploadedTime = _dbCampaignDetails.Tech_Team_File_Uploaded_Time;
                    campaignDetails.Notes = _dbCampaignDetails.Notes;
                    campaignDetails.BatchStatus = _dbCampaignDetails.Batch_Status;
                    campaignDetails.CampaignType = _dbCampaignDetails.Campaign_Type;
                    campaignDetails.IsManagerFollowUp = _dbCampaignDetails.Is_Manager_Follow_Up;
                    campaignDetails.CampaignSupportUserDetails = new CampaignSupportUserDetails();
                    campaignDetails.CampaignSupportUserDetails.NoOfUserAssigned = dynamicCampaign.No_Of_User_Assigned;
                    campaignDetails.CampaignSupportUserDetails.NoOfCompleted = dynamicCampaign.No_Of_Completed;
                    campaignDetails.CampaignSupportUserDetails.NoOfPending = dynamicCampaign.No_Of_Pending;
                    campaignDetails.CampaignSupportUserDetails.SupportUserId = dynamicCampaign.Support_User_Id ?? 0;
                    campaignDetails.CampaignSupportUserDetails.LastLeadId = dynamicCampaign.Last_Lead_Id ?? 0;
                    campaignDetails.IsManagerFollowUp = _dbCampaignDetails.Is_Manager_Follow_Up;

                    if (dynamicCampaign.cd != null && dynamicCampaign.cd.Biz_Campaign_Assigned_Details != null)
                    {
                        var _dbCampaignAssignedDetails = dynamicCampaign.cd.Biz_Campaign_Assigned_Details.Where(m => m.Support_User_Id == param.AdminUserId && !m.Is_Deleted).ToList();

                        campaignDetails.SkippedCount = _dbCampaignAssignedDetails.Where(m => m.Is_Skip == true).Count();

                    }
                    if (campaignDetails.CampaignDetailsId > 0)
                    {
                        var leadActivityList = (from la in _context.Biz_Campaign_Lead_Activity
                                                where la.Campaign_Detail_Id == campaignDetails.CampaignDetailsId &&
                                                la.Support_User_Id == param.AdminUserId && !la.Is_Deleted
                                                select new
                                                {
                                                    la.Method_Of_Contact,
                                                    la.Is_Follow_Up
                                                }).ToList();
                        if (leadActivityList != null && leadActivityList.Any())
                        {
                            if (campaignDetails.CreateTimeStamp >= Utilities.DataUtility.GetDateTime(Utilities.DataUtility.GetAppSettings("CampaignCreateDate")))
                            {
                                campaignDetails.PhoneCount = leadActivityList.Where(a => a.Method_Of_Contact == MethodOfContact.CallBack.ToString() || a.Method_Of_Contact == MethodOfContact.LeftVM.ToString() || a.Method_Of_Contact == MethodOfContact.InvoiceSent.ToString() || a.Method_Of_Contact == MethodOfContact.NotInterested.ToString() || a.Method_Of_Contact == MethodOfContact.DoNotContact.ToString()).ToList().Count;
                            }
                            else
                            {
                                campaignDetails.PhoneCount = leadActivityList.Where(a => a.Method_Of_Contact == "Phone").ToList().Count;
                            }
                            //campaignDetails.EmailCount = leadActivityList.Where(a => a.Method_Of_Contact == MethodOfContact.Mail.ToString()).ToList().Count;
                            campaignDetails.FollowupCount = leadActivityList.Where(a => a.Is_Follow_Up == true).ToList().Count;
                        }
                    }
                    if (campaignDetails.AdminUserId > 0)
                    {
                        campaignDetails.AdminUserName = GetAdminNameByUserId(campaignDetails.AdminUserId);
                    }
                    campaignDetailsList.Add(campaignDetails);
                }
            }
            return campaignDetailsList;
        }
        #endregion

        #region Save Additional Contacts
        public AdditionalContacts SaveAdditionalContacts(AdditionalContacts ContDetails)
        {
            AdditionalContacts additionalContacts = new AdditionalContacts();
            if (ContDetails != null && ContDetails.CampaignAssignedDetailsId > 0)
            {
                using (var entities = new MainControlDB_Entities())
                {
                    Biz_Campaign_Additional_Contacts_Details dbAdditionalContacts = null;
                    bool isRecordExists = false;

                    if (ContDetails.AdditionalContactsDetailsId > 0)
                    {
                        dbAdditionalContacts = _context.Biz_Campaign_Additional_Contacts_Details.SingleOrDefault(a => a.Additional_Contacts_Details_Id == ContDetails.AdditionalContactsDetailsId && !a.Is_Deleted);
                    }

                    if (dbAdditionalContacts != null && dbAdditionalContacts.Additional_Contacts_Details_Id > 0)
                    {
                        isRecordExists = true;
                    }
                    else
                    {
                        dbAdditionalContacts = new Biz_Campaign_Additional_Contacts_Details();
                    }

                    dbAdditionalContacts.Campaign_Assigned_Details_Id = ContDetails.CampaignAssignedDetailsId;
                    dbAdditionalContacts.Contacts_Name = ContDetails.ContactName;
                    dbAdditionalContacts.Contacts_Title = ContDetails.ContactTitle;
                    dbAdditionalContacts.Contacts_Email_Address = ContDetails.ContactEmailAddress;
                    dbAdditionalContacts.Contacts_Phone_Number = ContDetails.ContactPhone;
                    dbAdditionalContacts.Updated_Time_Stamp = DateTime.Now;

                    if (!isRecordExists)
                    {
                        dbAdditionalContacts.Created_Time_Stamp = DateTime.Now;
                        _context.Biz_Campaign_Additional_Contacts_Details.Add(dbAdditionalContacts);
                    }

                    _context.SaveChanges();

                    if (dbAdditionalContacts.Additional_Contacts_Details_Id > 0)
                    {
                        ContDetails.AdditionalContactsDetailsId = dbAdditionalContacts.Additional_Contacts_Details_Id;
                        ContDetails.StatusType = StatusType.Success;
                    }
                }
            }

            return additionalContacts;
        }
        #endregion

        #region Get Additional Contact Details By Additional Contacts Details Id
        public AdditionalContacts GetAdditionalContactDetailsByAdditionalContactsDetailsId(long additionalContactsDetailsId)
        {
            AdditionalContacts ContDetails = new AdditionalContacts();
            if (additionalContactsDetailsId > 0)
            {
                var dbContDetails = (from a in _context.Biz_Campaign_Additional_Contacts_Details
                                     where a.Additional_Contacts_Details_Id == additionalContactsDetailsId && !a.Is_Deleted
                                     select new
                                     {
                                         a.Contacts_Name,
                                         a.Contacts_Phone_Number,
                                         a.Contacts_Title,
                                         a.Contacts_Email_Address,
                                         a.Additional_Contacts_Details_Id,
                                         a.Campaign_Assigned_Details_Id
                                     }).SingleOrDefault();


                if (dbContDetails != null)
                {
                    ContDetails.ContactName = dbContDetails.Contacts_Name;
                    ContDetails.ContactEmailAddress = dbContDetails.Contacts_Email_Address;
                    ContDetails.ContactPhone = dbContDetails.Contacts_Phone_Number;
                    ContDetails.ContactTitle = dbContDetails.Contacts_Title;
                    ContDetails.AdditionalContactsDetailsId = dbContDetails.Additional_Contacts_Details_Id;
                    ContDetails.CampaignAssignedDetailsId = dbContDetails.Campaign_Assigned_Details_Id;
                }
            }
            return ContDetails;
        }
        #endregion

        #region Delete Additional Contact By Additional Contacts Details Id
        public bool DeleteAdditionalContactByAdditionalContactsDetailsId(long additionalContactsDetailsId)
        {

            bool isReturnDeletedStatus = false;
            var _dbAdditionalContactDetails = (from a in _context.Biz_Campaign_Additional_Contacts_Details
                                               where a.Additional_Contacts_Details_Id == additionalContactsDetailsId && !a.Is_Deleted
                                               select a).SingleOrDefault();
            if (_dbAdditionalContactDetails != null)
            {
                _dbAdditionalContactDetails.Is_Deleted = true;
                _dbAdditionalContactDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
                isReturnDeletedStatus = true;
            }
            return isReturnDeletedStatus;

        }
        #endregion

        #region Get Campaign Details Last Count
        public long GetCampaignDetailsLastCount(LeadDetailsSearchOption leadDetailsSearchOption)
        {
            long? LastLeadId = 0;
            string stateCode = leadDetailsSearchOption.StateCode;
            string dbStateName = GetStateNameByStateCode(stateCode);


            var dbLeadDetailslist = (from a in _context.Biz_Campaign_Support_User_Details
                                     where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId
                                     select a.Last_Lead_Id).Take(1).SingleOrDefault();
            if (dbLeadDetailslist != null)
            {

                if (leadDetailsSearchOption.Skipped)
                {
                    var dbLeadActivity = (from a in _context.Biz_Campaign_Assigned_Details
                                          where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId && a.Is_Skip == true
                                          orderby a.Created_Time_Stamp
                                          select a.Campaign_Assigned_Details_Id).ToList();

                    if (dbLeadActivity != null && dbLeadActivity.Count > 0)
                    {
                        int startPosition = dbLeadActivity.IndexOf(Convert.ToInt64(dbLeadDetailslist));

                        if (dbLeadActivity.Count > (startPosition + 1))
                        {
                            LastLeadId = dbLeadActivity[startPosition + 1];
                        }

                    }

                }
                else if (!string.IsNullOrWhiteSpace(leadDetailsSearchOption.StateCode))
                {

                    var _dbCampaignAssignedDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                                      where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId
                                                      select a).OrderBy(a => a.Created_Time_Stamp).ToList();


                    var dbLeadActivityAddressFilter = (from a in _dbCampaignAssignedDetails
                                                       where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId
                                                       && a.Address != null && (a.Address.ToLower().Contains(", " + stateCode.ToLower()) || a.Address.ToLower().Contains("," + stateCode.ToLower()) || a.Address.ToLower().Contains(", " + dbStateName.ToLower()) || a.Address.ToLower().Contains("," + dbStateName.ToLower()))
                                                       select a.Campaign_Assigned_Details_Id).ToList();

                    var dbLeadActivityNotAddressFilter = (from a in _dbCampaignAssignedDetails
                                                          where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId
                                                          && !dbLeadActivityAddressFilter.Contains(a.Campaign_Assigned_Details_Id)
                                                          select a.Campaign_Assigned_Details_Id).ToList();

                    var dbLeadActivity = new List<long>();

                    dbLeadActivity.AddRange(dbLeadActivityAddressFilter);

                    dbLeadActivity.AddRange(dbLeadActivityNotAddressFilter);

                    if (dbLeadActivity != null && dbLeadActivity.Count > 0)
                    {
                        int startPosition = dbLeadActivity.IndexOf(Convert.ToInt64(dbLeadDetailslist));
                        if (dbLeadActivity.Count > (startPosition + 1))
                        {
                            LastLeadId = dbLeadActivity[startPosition + 1];
                        }
                    }


                }
                else
                {
                    var campaignAssignedDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                                   where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId
                                                   orderby a.Created_Time_Stamp
                                                   select a.Campaign_Assigned_Details_Id).ToList();

                    if (campaignAssignedDetails != null && campaignAssignedDetails.Count > 0)
                    {
                        int startPosition = campaignAssignedDetails.IndexOf(Convert.ToInt64(dbLeadDetailslist));

                        if (campaignAssignedDetails.Count > (startPosition + 1))
                        {
                            LastLeadId = campaignAssignedDetails[startPosition + 1];
                        }

                    }

                }

            }
            else
            {
                var dbCampaignAssignedDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                                 where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId
                                                 orderby a.Created_Time_Stamp
                                                 select a).Take(1).SingleOrDefault();
                if (dbCampaignAssignedDetails != null)
                {
                    if (leadDetailsSearchOption.Skipped)
                    {
                        var dbLeadActivity = (from a in _context.Biz_Campaign_Lead_Activity
                                              where a.Campaign_Detail_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId
                                              select a.Campaign_Assign_Detail_Id).ToList();

                        LastLeadId = (from a in _context.Biz_Campaign_Assigned_Details
                                      where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId && a.Campaign_Assigned_Details_Id
                                      > dbCampaignAssignedDetails.Campaign_Assigned_Details_Id && !dbLeadActivity.Contains(a.Campaign_Assigned_Details_Id)
                                      orderby a.Created_Time_Stamp descending
                                      select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();


                    }
                    else if (!string.IsNullOrWhiteSpace(leadDetailsSearchOption.StateCode))
                    {
                        var dbLeadActivity = (from a in _context.Biz_Campaign_Lead_Activity
                                              where a.Campaign_Detail_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId
                                              select a.Campaign_Assign_Detail_Id).ToList();

                        LastLeadId = (from a in _context.Biz_Campaign_Assigned_Details
                                      where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId && a.Campaign_Assigned_Details_Id
                                      > dbCampaignAssignedDetails.Campaign_Assigned_Details_Id && !dbLeadActivity.Contains(a.Campaign_Assigned_Details_Id)
                                      orderby a.Created_Time_Stamp descending
                                      select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
                    }
                    else
                    {

                        LastLeadId = (from a in _context.Biz_Campaign_Assigned_Details
                                      where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId && a.Campaign_Assigned_Details_Id
                                      > dbCampaignAssignedDetails.Campaign_Assigned_Details_Id
                                      orderby a.Created_Time_Stamp descending
                                      select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
                    }
                }
            }

            return LastLeadId ?? 0;
        }
        #endregion

        #region Get Additional Contact List By Campaign Assigned Details Id
        public LeadDetails GetAdditionalContactListByCampaignAssignedDetailsId(long campaignAssignedDetailsId)
        {
            LeadDetails leadDetails = new LeadDetails();
            if (campaignAssignedDetailsId > 0)
            {
                var dbAdditionalContList = (from a in _context.Biz_Campaign_Additional_Contacts_Details
                                            where a.Campaign_Assigned_Details_Id == campaignAssignedDetailsId && !a.Is_Deleted
                                            select new
                                            {
                                                a.Additional_Contacts_Details_Id,
                                                a.Contacts_Name,
                                                a.Contacts_Phone_Number,
                                                a.Contacts_Title,
                                                a.Contacts_Email_Address
                                            }).ToList();

                if (dbAdditionalContList != null && dbAdditionalContList.Any())
                {
                    leadDetails.AdditionalContactsList = new List<AdditionalContacts>();
                    foreach (var item in dbAdditionalContList)
                    {
                        AdditionalContacts ContDetails = new AdditionalContacts();
                        ContDetails.AdditionalContactsDetailsId = item.Additional_Contacts_Details_Id;
                        ContDetails.ContactName = item.Contacts_Name;
                        ContDetails.ContactEmailAddress = item.Contacts_Email_Address;
                        ContDetails.ContactPhone = item.Contacts_Phone_Number;
                        ContDetails.ContactTitle = item.Contacts_Title;

                        leadDetails.AdditionalContactsList.Add(ContDetails);
                    }

                }
            }
            return leadDetails;
        }
        #endregion

        #region Get Followup By Support User Id
        public List<LeadCommunication> GetFollowupBySupportUserId(long supportUserId, string followUpFilter)
        {
            List<LeadCommunication> leadCommunicationList = new List<LeadCommunication>();
            if (followUpFilter == FollowupFilter.All.ToString())
            {
                var _dbLeadCommunicationList = (from cla in _context.Biz_Campaign_Lead_Activity
                                                join CampaignDetail in _context.Biz_Campaign_Details on cla.Campaign_Detail_Id equals CampaignDetail.Campaign_Details_Id
                                                join CampaignAssignedDetail in _context.Biz_Campaign_Assigned_Details on cla.Campaign_Assign_Detail_Id equals CampaignAssignedDetail.Campaign_Assigned_Details_Id
                                                where cla.Support_User_Id == supportUserId && cla.Is_Follow_Up && cla.Lead_Status == LeadStatusflag.Pending.ToString() && !cla.Is_Deleted && !CampaignDetail.Is_Deleted && !CampaignAssignedDetail.Is_Deleted
                                                orderby cla.Campaign_Lead_Activity_Id
                                                select new
                                                {
                                                    cla,
                                                    CampaignDetail.Campaign_Name,
                                                    CampaignAssignedDetail.Email_Address,
                                                    CampaignAssignedDetail.Phone_Number,
                                                    CampaignAssignedDetail.Name
                                                }).ToList();

                if (_dbLeadCommunicationList != null && _dbLeadCommunicationList.Any())
                {
                    foreach (var item in _dbLeadCommunicationList)
                    {
                        LeadCommunication leadCommunication = new LeadCommunication();
                        leadCommunication.CreatedDate = item.cla.Create_Time_Stamp;
                        leadCommunication.MethodOfContact = (MethodOfContact)Enum.Parse(typeof(MethodOfContact), item.cla.Method_Of_Contact.ToString(), true);
                        leadCommunication.FollowupDate = item.cla.Followup_Date;
                        leadCommunication.LeadStatus = item.cla.Lead_Status;
                        leadCommunication.CampaignLeadActivityId = item.cla.Campaign_Lead_Activity_Id;
                        leadCommunication.Comments = item.cla.Comments;
                        leadCommunication.FollowupTime = item.cla.Followup_Time;
                        leadCommunication.DonotContactagain = item.cla.Is_Do_Not_Disturb;
                        leadCommunication.CampaignName = item.Campaign_Name;
                        leadCommunication.Spoketo = item.cla.Spoke_To;
                        leadCommunication.TypeOfCall = item.cla.Type_Of_Call;
                        leadCommunication.Reason = item.cla.Reason;
                        leadCommunication.IsFollowRequired = item.cla.Is_Follow_Up;
                        leadCommunication.LeadName = item.Name;
                        leadCommunication.LogDate = item.cla.Create_Time_Stamp;
                        leadCommunication.EmailAddress = item.Email_Address;
                        leadCommunication.Phone = item.Phone_Number;
                        leadCommunication.CampaignDetailId = item.cla.Campaign_Detail_Id;
                        leadCommunication.SupportUserId = item.cla.Support_User_Id;
                        leadCommunication.CampaignAssignDetailId = item.cla.Campaign_Assign_Detail_Id;
                        leadCommunicationList.Add(leadCommunication);
                    }
                }
            }
            else if (followUpFilter == FollowupFilter.Today.ToString())
            {
                var _dbLeadCommunicationList = (from cla in _context.Biz_Campaign_Lead_Activity
                                                join CampaignDetail in _context.Biz_Campaign_Details on cla.Campaign_Detail_Id equals CampaignDetail.Campaign_Details_Id
                                                join CampaignAssignedDetail in _context.Biz_Campaign_Assigned_Details on cla.Campaign_Assign_Detail_Id equals CampaignAssignedDetail.Campaign_Assigned_Details_Id
                                                where cla.Support_User_Id == supportUserId && cla.Is_Follow_Up && cla.Lead_Status == LeadStatusflag.Pending.ToString() && cla.Followup_Date.Value.Year == DateTime.Now.Year && cla.Followup_Date.Value.Month == DateTime.Now.Month && cla.Followup_Date.Value.Day == DateTime.Now.Day && !cla.Is_Deleted && !CampaignDetail.Is_Deleted && !CampaignAssignedDetail.Is_Deleted
                                                orderby cla.Campaign_Lead_Activity_Id
                                                select new
                                                {
                                                    cla,
                                                    CampaignDetail.Campaign_Name,
                                                    CampaignAssignedDetail.Email_Address,
                                                    CampaignAssignedDetail.Phone_Number,
                                                    CampaignAssignedDetail.Name
                                                }).ToList();

                if (_dbLeadCommunicationList != null && _dbLeadCommunicationList.Any())
                {
                    foreach (var item in _dbLeadCommunicationList)
                    {
                        LeadCommunication leadCommunication = new LeadCommunication();
                        leadCommunication.CreatedDate = item.cla.Create_Time_Stamp;
                        leadCommunication.MethodOfContact = (MethodOfContact)Enum.Parse(typeof(MethodOfContact), item.cla.Method_Of_Contact.ToString(), true);
                        leadCommunication.FollowupDate = item.cla.Followup_Date;
                        leadCommunication.LeadStatus = item.cla.Lead_Status;
                        leadCommunication.CampaignLeadActivityId = item.cla.Campaign_Lead_Activity_Id;
                        leadCommunication.Comments = item.cla.Comments;
                        leadCommunication.FollowupTime = item.cla.Followup_Time;
                        leadCommunication.DonotContactagain = item.cla.Is_Do_Not_Disturb;
                        leadCommunication.CampaignName = item.Campaign_Name;
                        leadCommunication.Spoketo = item.cla.Spoke_To;
                        leadCommunication.Reason = item.cla.Reason;
                        leadCommunication.IsFollowRequired = item.cla.Is_Follow_Up;
                        leadCommunication.LeadName = item.Name;
                        leadCommunication.TypeOfCall = item.cla.Type_Of_Call;
                        leadCommunication.LogDate = item.cla.Create_Time_Stamp;
                        leadCommunication.EmailAddress = item.Email_Address;
                        leadCommunication.Phone = item.Phone_Number;
                        leadCommunication.CampaignDetailId = item.cla.Campaign_Detail_Id;
                        leadCommunication.SupportUserId = item.cla.Support_User_Id;
                        leadCommunication.CampaignAssignDetailId = item.cla.Campaign_Assign_Detail_Id;
                        leadCommunicationList.Add(leadCommunication);
                    }
                }

            }
            else if (followUpFilter == FollowupFilter.PastDue.ToString())
            {
                var _dbLeadCommunicationList = (from cla in _context.Biz_Campaign_Lead_Activity
                                                join CampaignDetail in _context.Biz_Campaign_Details on cla.Campaign_Detail_Id equals CampaignDetail.Campaign_Details_Id
                                                join CampaignAssignedDetail in _context.Biz_Campaign_Assigned_Details on cla.Campaign_Assign_Detail_Id equals CampaignAssignedDetail.Campaign_Assigned_Details_Id
                                                where cla.Support_User_Id == supportUserId && cla.Is_Follow_Up && cla.Lead_Status == LeadStatusflag.Pending.ToString() &&
                                                (cla.Followup_Date != null &&
                                           (cla.Followup_Date.Value.Year < DateTime.Now.Year ? (cla.Followup_Date.Value.Day >= DateTime.Now.Day || cla.Followup_Date.Value.Day <= DateTime.Now.Day) :
                                          (cla.Followup_Date.Value.Year == DateTime.Now.Year ? (cla.Followup_Date.Value.Month == DateTime.Now.Month ? cla.Followup_Date.Value.Day < DateTime.Now.Day :
                                          cla.Followup_Date.Value.Month < DateTime.Now.Month ? (cla.Followup_Date.Value.Day >= DateTime.Now.Day || cla.Followup_Date.Value.Day <= DateTime.Now.Day) :
                                          cla.Followup_Date.Value.Month > DateTime.Now.Month ? false : false) :
                                          cla.Followup_Date.Value.Year > DateTime.Now.Year ? false : false))) && !cla.Is_Deleted && !CampaignDetail.Is_Deleted && !CampaignAssignedDetail.Is_Deleted
                                                orderby cla.Campaign_Lead_Activity_Id
                                                select new
                                                {
                                                    cla,
                                                    CampaignDetail.Campaign_Name,
                                                    CampaignAssignedDetail.Email_Address,
                                                    CampaignAssignedDetail.Phone_Number,
                                                    CampaignAssignedDetail.Name
                                                }).ToList();

                if (_dbLeadCommunicationList != null && _dbLeadCommunicationList.Any())
                {
                    foreach (var item in _dbLeadCommunicationList)
                    {
                        LeadCommunication leadCommunication = new LeadCommunication();
                        leadCommunication.CreatedDate = item.cla.Create_Time_Stamp;
                        leadCommunication.MethodOfContact = (MethodOfContact)Enum.Parse(typeof(MethodOfContact), item.cla.Method_Of_Contact.ToString(), true);
                        leadCommunication.FollowupDate = item.cla.Followup_Date;
                        leadCommunication.LeadStatus = item.cla.Lead_Status;
                        leadCommunication.CampaignLeadActivityId = item.cla.Campaign_Lead_Activity_Id;
                        leadCommunication.Comments = item.cla.Comments;
                        leadCommunication.FollowupTime = item.cla.Followup_Time;
                        leadCommunication.DonotContactagain = item.cla.Is_Do_Not_Disturb;
                        leadCommunication.CampaignName = item.Campaign_Name;
                        leadCommunication.Spoketo = item.cla.Spoke_To;
                        leadCommunication.Reason = item.cla.Reason;
                        leadCommunication.IsFollowRequired = item.cla.Is_Follow_Up;
                        leadCommunication.LeadName = item.Name;
                        leadCommunication.TypeOfCall = item.cla.Type_Of_Call;
                        leadCommunication.LogDate = item.cla.Create_Time_Stamp;
                        leadCommunication.EmailAddress = item.Email_Address;
                        leadCommunication.Phone = item.Phone_Number;
                        leadCommunication.CampaignDetailId = item.cla.Campaign_Detail_Id;
                        leadCommunication.SupportUserId = item.cla.Support_User_Id;
                        leadCommunication.CampaignAssignDetailId = item.cla.Campaign_Assign_Detail_Id;
                        leadCommunicationList.Add(leadCommunication);
                    }
                }
            }
            else
            {
                var _dbLeadCommunicationList = (from cla in _context.Biz_Campaign_Lead_Activity
                                                join CampaignDetail in _context.Biz_Campaign_Details on cla.Campaign_Detail_Id equals CampaignDetail.Campaign_Details_Id
                                                join CampaignAssignedDetail in _context.Biz_Campaign_Assigned_Details on cla.Campaign_Assign_Detail_Id equals CampaignAssignedDetail.Campaign_Assigned_Details_Id
                                                where cla.Support_User_Id == supportUserId && cla.Is_Follow_Up && cla.Lead_Status == LeadStatusflag.Pending.ToString() && (cla.Followup_Date != null &&
                                          (cla.Followup_Date.Value.Year > DateTime.Now.Year ? (cla.Followup_Date.Value.Day >= DateTime.Now.Day || cla.Followup_Date.Value.Day <= DateTime.Now.Day) :
                                          cla.Followup_Date.Value.Year < DateTime.Now.Year ? false : (cla.Followup_Date.Value.Month > DateTime.Now.Month ? (cla.Followup_Date.Value.Day > DateTime.Now.Day || cla.Followup_Date.Value.Day < DateTime.Now.Day) :
                                          cla.Followup_Date.Value.Month == DateTime.Now.Month ? cla.Followup_Date.Value.Day > DateTime.Now.Day
                                          : cla.Followup_Date.Value.Month < DateTime.Now.Month ? false : false))) && !cla.Is_Deleted && !CampaignDetail.Is_Deleted && !CampaignAssignedDetail.Is_Deleted
                                                orderby cla.Campaign_Lead_Activity_Id
                                                select new
                                                {
                                                    cla,
                                                    CampaignDetail.Campaign_Name,
                                                    CampaignAssignedDetail.Email_Address,
                                                    CampaignAssignedDetail.Phone_Number,
                                                    CampaignAssignedDetail.Name
                                                }).ToList();

                if (_dbLeadCommunicationList != null && _dbLeadCommunicationList.Any())
                {
                    foreach (var item in _dbLeadCommunicationList)
                    {
                        LeadCommunication leadCommunication = new LeadCommunication();
                        leadCommunication.CreatedDate = item.cla.Create_Time_Stamp;
                        leadCommunication.MethodOfContact = (MethodOfContact)Enum.Parse(typeof(MethodOfContact), item.cla.Method_Of_Contact.ToString(), true);
                        leadCommunication.FollowupDate = item.cla.Followup_Date;
                        leadCommunication.LeadStatus = item.cla.Lead_Status;
                        leadCommunication.CampaignLeadActivityId = item.cla.Campaign_Lead_Activity_Id;
                        leadCommunication.Comments = item.cla.Comments;
                        leadCommunication.FollowupTime = item.cla.Followup_Time;
                        leadCommunication.DonotContactagain = item.cla.Is_Do_Not_Disturb;
                        leadCommunication.CampaignName = item.Campaign_Name;
                        leadCommunication.Spoketo = item.cla.Spoke_To;
                        leadCommunication.Reason = item.cla.Reason;
                        leadCommunication.TypeOfCall = item.cla.Type_Of_Call;
                        leadCommunication.IsFollowRequired = item.cla.Is_Follow_Up;
                        leadCommunication.LeadName = item.Name;
                        leadCommunication.LogDate = item.cla.Create_Time_Stamp;
                        leadCommunication.EmailAddress = item.Email_Address;
                        leadCommunication.Phone = item.Phone_Number;
                        leadCommunication.CampaignDetailId = item.cla.Campaign_Detail_Id;
                        leadCommunication.SupportUserId = item.cla.Support_User_Id;
                        leadCommunication.CampaignAssignDetailId = item.cla.Campaign_Assign_Detail_Id;
                        leadCommunicationList.Add(leadCommunication);
                    }
                }

            }
            return leadCommunicationList;
        }
        #endregion

        #region Get Campaign Details by  Admin User Id
        public int GetCampaignSupportUserDetailsCount(JQueryDataTableParamModel param)
        {
            DateTime currentDate = DateTime.Now.Date;
            var _dbCampaignDetailsListCount = (from csd in _context.Biz_Campaign_Support_User_Details
                                               join cd in _context.Biz_Campaign_Details on csd.Campaign_Details_Id equals cd.Campaign_Details_Id
                                               where csd.Support_User_Id == param.AdminUserId && !csd.Is_Deleted && !cd.Is_Deleted
                                               && (param.ProjectId > 0 ? cd.Admin_Project_Id == param.ProjectId : cd.Admin_Project_Id > 0)
                                               && cd.Is_Paused == false &&
                                            (param.CampaignStatus == LeadListStatus.Expired.ToString() ? (DbFunctions.TruncateTime(cd.Campaign_End_Date) < currentDate) :
                                              param.CampaignStatus == LeadListStatus.Active.ToString() ? (DbFunctions.TruncateTime(cd.Campaign_Start_Date) <= currentDate && DbFunctions.TruncateTime(cd.Campaign_End_Date) >= currentDate) :
                                             param.CampaignStatus == LeadListStatus.Upcoming.ToString() ? DbFunctions.TruncateTime(cd.Campaign_Start_Date) > currentDate : true)
                                              && cd.Is_Suspended == false
                                               orderby cd.Created_Time_Stamp descending
                                               select new
                                               {
                                                   csd.Campaign_Support_User_Detail_Id,
                                               }).Count();

            return _dbCampaignDetailsListCount;
        }

        #endregion

        #region Get Campaign Lead Activity List By Campaign Detail Id
        public List<LeadDetails> GetAllLeadListBySupportUserId(long supportUserId, string searchBy, string value)
        {
            List<LeadDetails> leadDetailsList = new List<LeadDetails>();

            var _dbLeadDetailsList = (from cad in _context.Biz_Campaign_Assigned_Details
                                      where cad.Support_User_Id == supportUserId && !cad.Is_Deleted &&
                                      (searchBy == "Name" ? cad.Name != null && cad.Name.Contains(value) : (searchBy == "Phone" ? cad.Phone_Number != null && cad.Phone_Number.Replace("-", "").Replace(")", "").Replace("(", "").Replace(" ", "").Contains(value) :
                                      (searchBy == "Email" ? cad.Email_Address != null && cad.Email_Address.Contains(value) : (searchBy == "BusinessName" ? cad.Business_Name != null && cad.Business_Name.Contains(value) : false))))
                                      select new
                                      {
                                          cad.Biz_Campaign_Details,
                                          cad.Business_Name,
                                          cad.Email_Address,
                                          cad.Phone_Number,
                                          cad.Name,
                                          cad.Campaign_Assigned_Details_Id,
                                          cad.Campaign_Details_Id

                                      }).ToList();

            if (_dbLeadDetailsList != null && _dbLeadDetailsList.Any())
            {
                LeadDetails leadDetails = new LeadDetails();
                foreach (var item in _dbLeadDetailsList)
                {
                    leadDetails.BusinessName = item.Business_Name;
                    leadDetails.Email = item.Email_Address;
                    leadDetails.Phone = item.Phone_Number;
                    leadDetails.Name = item.Name;
                    leadDetails.CampaignAssignedDetailsId = item.Campaign_Assigned_Details_Id;
                    leadDetails.CampaignDetailsId = item.Campaign_Details_Id;
                    if (leadDetails.CampaignDetailsId > 0)
                    {
                        leadDetails.CampaignName = item.Biz_Campaign_Details.Campaign_Name;
                    }
                    leadDetails.SupportUserId = supportUserId;
                    leadDetailsList.Add(leadDetails);
                }
            }
            return leadDetailsList;
        }
        #endregion

        #region  Update Last Lead Id By Campaign Assigned Details Id
        public bool UpdateLastLeadIdByCampaignAssignedDetailsId(LeadCommunication leadCommunication)
        {
            bool Isupdated = false;
            var _dbCampaignSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
                                                 where a.Campaign_Details_Id == leadCommunication.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadCommunication.SupportUserId
                                                 select a).SingleOrDefault();
            if (_dbCampaignSupportUserDetails != null)
            {
                _dbCampaignSupportUserDetails.Last_Lead_Id = leadCommunication.CampaignAssignDetailId;
                _dbCampaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
                Isupdated = true;
            }
            return Isupdated;
        }
        #endregion

        //#region  Update Previous Lead Id As Last Lead Id by Statecode
        //public bool UpdatePreviousLeadIdAsLastLeadIdByStatecode(long campaignId, long supportUserId, string stateCode)
        //{
        //    bool isupdated = false;
        //    using (var _context = new MainControlDB_Entities())
        //    {

        //        long lastCampaignAssignedDetails = (from a in _context.Biz_Campaign_Assigned_Details
        //                                            where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                            && a.Address.Contains(", " + stateCode)
        //                                            orderby a.Campaign_Assigned_Details_Id descending
        //                                            select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
        //        long firstElement = (from a in _context.Biz_Campaign_Assigned_Details
        //                             where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                             && !a.Address.Contains(", " + stateCode)
        //                             orderby a.Campaign_Assigned_Details_Id
        //                             select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
        //        var _dbLeadDetailslist = (from a in _context.Biz_Campaign_Support_User_Details
        //                                  where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                  select a.Last_Lead_Id).Take(1).SingleOrDefault();
        //        if (_dbLeadDetailslist != null)
        //        {

        //            bool IsFilter = (from a in _context.Biz_Campaign_Assigned_Details
        //                             where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                             && !a.Address.Contains(", " + stateCode) && a.Campaign_Assigned_Details_Id == _dbLeadDetailslist
        //                             orderby a.Campaign_Assigned_Details_Id
        //                             select a.Campaign_Assigned_Details_Id).Any();
        //            if (IsFilter)
        //            {
        //                if (_dbLeadDetailslist == firstElement)
        //                {
        //                    var _dbCampaignSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
        //                                                         where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                                         select a).SingleOrDefault();
        //                    if (_dbCampaignSupportUserDetails != null)
        //                    {
        //                        _dbCampaignSupportUserDetails.Last_Lead_Id = lastCampaignAssignedDetails;
        //                        _dbCampaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
        //                        _context.SaveChanges();
        //                        isupdated = true;
        //                    }
        //                }
        //                else
        //                {
        //                    var LastLeadId = (from a in _context.Biz_Campaign_Assigned_Details
        //                                      where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                      && !a.Address.Contains(", " + stateCode) && a.Campaign_Assigned_Details_Id < _dbLeadDetailslist
        //                                      orderby a.Campaign_Assigned_Details_Id descending
        //                                      select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();

        //                    var _dbCampaignSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
        //                                                         where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                                         select a).SingleOrDefault();
        //                    if (_dbCampaignSupportUserDetails != null)
        //                    {
        //                        _dbCampaignSupportUserDetails.Last_Lead_Id = LastLeadId;
        //                        _dbCampaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
        //                        _context.SaveChanges();
        //                        isupdated = true;
        //                    }

        //                }
        //            }
        //            else
        //            {
        //                bool IsFilterapllied = (from a in _context.Biz_Campaign_Assigned_Details
        //                                        where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                        && a.Address.Contains(", " + stateCode) && a.Campaign_Assigned_Details_Id == _dbLeadDetailslist
        //                                        orderby a.Campaign_Assigned_Details_Id
        //                                        select a.Campaign_Assigned_Details_Id).Any();
        //                if (IsFilterapllied)
        //                {
        //                    var LastLeadId = (from a in _context.Biz_Campaign_Assigned_Details
        //                                      where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                      && a.Address.Contains(", " + stateCode) && a.Campaign_Assigned_Details_Id < _dbLeadDetailslist
        //                                      orderby a.Campaign_Assigned_Details_Id descending
        //                                      select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
        //                    var _dbCampaignSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
        //                                                         where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                                         select a).SingleOrDefault();
        //                    if (_dbCampaignSupportUserDetails != null)
        //                    {
        //                        _dbCampaignSupportUserDetails.Last_Lead_Id = LastLeadId;
        //                        _dbCampaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
        //                        _context.SaveChanges();
        //                        isupdated = true;
        //                    }
        //                }
        //                else
        //                {
        //                    var LastLeadId = (from a in _context.Biz_Campaign_Assigned_Details
        //                                      where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                      && !a.Address.Contains(", " + stateCode) && a.Campaign_Assigned_Details_Id < _dbLeadDetailslist
        //                                      orderby a.Campaign_Assigned_Details_Id descending
        //                                      select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
        //                    var _dbCampaignSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
        //                                                         where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                                         select a).SingleOrDefault();
        //                    if (_dbCampaignSupportUserDetails != null)
        //                    {
        //                        _dbCampaignSupportUserDetails.Last_Lead_Id = LastLeadId;
        //                        _dbCampaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
        //                        _context.SaveChanges();
        //                        isupdated = true;
        //                    }

        //                }


        //            }

        //        }
        //    }
        //    return isupdated;
        //}
        //#endregion

        #region  Update Previous Lead Id As Last Lead Id by Statecode
        public bool UpdatePreviousLeadIdAsLastLeadIdByStatecode(LeadDetailsSearchOption leadDetailsSearchOption)
        {
            bool isupdated = false;
            long _leadId = 0;
            int index = 0;
            string stateCode = leadDetailsSearchOption.StateCode;
            string dbStateName = GetStateNameByStateCode(stateCode);
            using (var _context = new MainControlDB_Entities())
            {
                var dynamicCampaign = (from a in _context.Biz_Campaign_Details
                                       where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Admin_Project_Id > 0
                                       select new
                                       {
                                           campaignDetails = a,
                                           campaignAssignedDetails = a.Biz_Campaign_Assigned_Details,
                                           campaignSupportUserDetails = a.Biz_Campaign_Support_User_Details,
                                           campaignLeadActivity = a.Biz_Campaign_Lead_Activity
                                       }).SingleOrDefault();

                if (dynamicCampaign != null)
                {
                    var _dbCampaignDetails = dynamicCampaign.campaignDetails;

                    var _dbCampaignSupportUserDetails = dynamicCampaign.campaignSupportUserDetails.Where(m => m.Support_User_Id == leadDetailsSearchOption.SupportUserId && !m.Is_Deleted).ToList();

                    var _dbCampaignAssignedDetails = dynamicCampaign.campaignAssignedDetails.Where(m => m.Support_User_Id == leadDetailsSearchOption.SupportUserId && !m.Is_Deleted).OrderBy(m => m.Created_Time_Stamp).ToList();

                    var _dbCampaignLeadActivityLst = dynamicCampaign.campaignLeadActivity.Where(m => m.Support_User_Id == leadDetailsSearchOption.SupportUserId && !m.Is_Deleted).ToList();

                    var _dbConactedLeadDetailsIds = new List<long>();

                    if (leadDetailsSearchOption.Skipped)
                    {
                        _dbConactedLeadDetailsIds = _dbCampaignAssignedDetails.Where(m => m.Is_Skip == true).Select(m => m.Campaign_Assigned_Details_Id).ToList();
                    }
                    else if (!string.IsNullOrWhiteSpace(stateCode))
                    {
                        _dbConactedLeadDetailsIds = _dbCampaignAssignedDetails.Where(a => a.Address != null && (a.Address.ToLower().Contains(", " + stateCode.ToLower()) || a.Address.ToLower().Contains("," + stateCode.ToLower()) || a.Address.ToLower().Contains(", " + dbStateName.ToLower()) || a.Address.ToLower().Contains("," + dbStateName.ToLower()))).Select(m => m.Campaign_Assigned_Details_Id).ToList();

                        var notExistsStateCodeLeadIds = _dbCampaignAssignedDetails.Where(m => !_dbConactedLeadDetailsIds.Contains(m.Campaign_Assigned_Details_Id)).Select(m => m.Campaign_Assigned_Details_Id).ToList();

                        _dbConactedLeadDetailsIds.AddRange(notExistsStateCodeLeadIds);

                    }
                    else
                    {
                        _dbConactedLeadDetailsIds = _dbCampaignAssignedDetails.Select(m => m.Campaign_Assigned_Details_Id).ToList();
                    }

                    var fullList = (from a in _dbCampaignAssignedDetails
                                    where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId && _dbConactedLeadDetailsIds.Contains(a.Campaign_Assigned_Details_Id)
                                    select new LeadDetails { CampaignAssignedDetailsId = a.Campaign_Assigned_Details_Id, Address = a.Address }).ToList();

                    var _dbLeadDetailslist = new List<LeadDetails>();


                    if (_dbConactedLeadDetailsIds != null && _dbConactedLeadDetailsIds.Count > 0)
                    {
                        foreach (var item in _dbConactedLeadDetailsIds)
                        {
                            var assignObj = fullList.Where(m => m.CampaignAssignedDetailsId == item).SingleOrDefault();
                            _dbLeadDetailslist.Add(assignObj);

                        }
                    }


                    if (_dbLeadDetailslist != null)
                    {
                        index = _dbLeadDetailslist.FindIndex(a => a.CampaignAssignedDetailsId == leadDetailsSearchOption.LeadId);
                        _leadId = _dbLeadDetailslist.Skip(index - 1).Take(1).FirstOrDefault().CampaignAssignedDetailsId;
                    }

                    var campaignSupportDetails = (from a in _dbCampaignSupportUserDetails
                                                  where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId
                                                  select a).Take(1).SingleOrDefault();
                    if (campaignSupportDetails != null)
                    {
                        campaignSupportDetails.Last_Lead_Id = _leadId;
                        campaignSupportDetails.Updated_Time_Stamp = DateTime.Now;
                        _context.SaveChanges();
                        isupdated = true;
                    }
                }
            }

            return isupdated;
        }
        #endregion

        //#region  Update Next Lead Id As Last Lead Id By Statecode
        //public bool UpdateNextLeadIdAsLastLeadIdByStatecode(long campaignId, long supportUserId, string stateCode)
        //{
        //    bool isupdated = false;
        //    using (var _context = new MainControlDB_Entities())
        //    {
        //        long lastCampaignAssignedDetails = (from a in _context.Biz_Campaign_Assigned_Details
        //                                            where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                            && a.Address.Contains(", " + stateCode)
        //                                            orderby a.Campaign_Assigned_Details_Id descending
        //                                            select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
        //        long firstElement = (from a in _context.Biz_Campaign_Assigned_Details
        //                             where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                             && !a.Address.Contains(", " + stateCode)
        //                             orderby a.Campaign_Assigned_Details_Id
        //                             select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
        //        var _dbLeadDetailslist = (from a in _context.Biz_Campaign_Support_User_Details
        //                                  where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                  select a.Last_Lead_Id).Take(1).SingleOrDefault();
        //        if (_dbLeadDetailslist != null)
        //        {
        //            bool IsFilter = (from a in _context.Biz_Campaign_Assigned_Details
        //                             where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                             && !a.Address.Contains(", " + stateCode) && a.Campaign_Assigned_Details_Id == _dbLeadDetailslist
        //                             orderby a.Campaign_Assigned_Details_Id
        //                             select a.Campaign_Assigned_Details_Id).Any();
        //            if (IsFilter || _dbLeadDetailslist == lastCampaignAssignedDetails)
        //            {
        //                if (_dbLeadDetailslist == lastCampaignAssignedDetails)
        //                {
        //                    var _dbCampaignSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
        //                                                         where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                                         select a).SingleOrDefault();
        //                    if (_dbCampaignSupportUserDetails != null)
        //                    {
        //                        _dbCampaignSupportUserDetails.Last_Lead_Id = firstElement;
        //                        _dbCampaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
        //                        _context.SaveChanges();
        //                        isupdated = true;
        //                    }
        //                }
        //                else
        //                {
        //                    var LastLeadId = (from a in _context.Biz_Campaign_Assigned_Details
        //                                      where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                      && !a.Address.Contains(", " + stateCode) && a.Campaign_Assigned_Details_Id > _dbLeadDetailslist
        //                                      orderby a.Campaign_Assigned_Details_Id
        //                                      select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
        //                    var _dbCampaignSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
        //                                                         where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                                         select a).SingleOrDefault();
        //                    if (_dbCampaignSupportUserDetails != null)
        //                    {
        //                        _dbCampaignSupportUserDetails.Last_Lead_Id = LastLeadId;
        //                        _dbCampaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
        //                        _context.SaveChanges();
        //                        isupdated = true;
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                var LastLeadId = (from a in _context.Biz_Campaign_Assigned_Details
        //                                  where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                  && a.Address.Contains(", " + stateCode) && a.Campaign_Assigned_Details_Id > _dbLeadDetailslist
        //                                  orderby a.Campaign_Assigned_Details_Id
        //                                  select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
        //                var _dbCampaignSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
        //                                                     where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                                     select a).SingleOrDefault();
        //                if (_dbCampaignSupportUserDetails != null)
        //                {
        //                    _dbCampaignSupportUserDetails.Last_Lead_Id = LastLeadId;
        //                    _dbCampaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
        //                    _context.SaveChanges();
        //                    isupdated = true;
        //                }

        //            }
        //        }
        //        else
        //        {
        //            bool IsFilterapllied = (from a in _context.Biz_Campaign_Assigned_Details
        //                                    where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                    && a.Address.Contains(", " + stateCode)
        //                                    orderby a.Campaign_Assigned_Details_Id
        //                                    select a.Campaign_Assigned_Details_Id).Any();
        //            if (IsFilterapllied)
        //            {
        //                var _dbCampaignAssignedDetailsId = (from a in _context.Biz_Campaign_Assigned_Details
        //                                                    where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                                    && a.Address.Contains(", " + stateCode)
        //                                                    orderby a.Campaign_Assigned_Details_Id
        //                                                    select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
        //                var LastLeadId = (from a in _context.Biz_Campaign_Assigned_Details
        //                                  where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                  && a.Address.Contains(", " + stateCode) && a.Campaign_Assigned_Details_Id > _dbCampaignAssignedDetailsId
        //                                  orderby a.Campaign_Assigned_Details_Id
        //                                  select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
        //                var _dbCampaignSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
        //                                                     where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                                     select a).SingleOrDefault();
        //                if (_dbCampaignSupportUserDetails != null)
        //                {
        //                    _dbCampaignSupportUserDetails.Last_Lead_Id = LastLeadId;
        //                    _dbCampaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
        //                    _context.SaveChanges();
        //                    isupdated = true;
        //                }
        //            }
        //            else
        //            {
        //                var _dbCampaignAssignedDetailsId = (from a in _context.Biz_Campaign_Assigned_Details
        //                                                    where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                                    && !a.Address.Contains(", " + stateCode)
        //                                                    orderby a.Campaign_Assigned_Details_Id
        //                                                    select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
        //                var LastLeadId = (from a in _context.Biz_Campaign_Assigned_Details
        //                                  where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                  && !a.Address.Contains(", " + stateCode) && a.Campaign_Assigned_Details_Id > _dbCampaignAssignedDetailsId
        //                                  orderby a.Campaign_Assigned_Details_Id
        //                                  select a.Campaign_Assigned_Details_Id).Take(1).SingleOrDefault();
        //                var _dbCampaignSupportUserDetails = (from a in _context.Biz_Campaign_Support_User_Details
        //                                                     where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                                     select a).SingleOrDefault();
        //                if (_dbCampaignSupportUserDetails != null)
        //                {
        //                    _dbCampaignSupportUserDetails.Last_Lead_Id = LastLeadId;
        //                    _dbCampaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
        //                    _context.SaveChanges();
        //                    isupdated = true;
        //                }
        //            }

        //        }
        //    }
        //    return isupdated;
        //}
        //#endregion

        #region  Update Next Lead Id As Last Lead Id By Statecode
        public bool UpdateNextLeadIdAsLastLeadIdByStatecode(LeadDetailsSearchOption leadDetailsSearchOption)
        {
            bool isupdated = false;
            long _leadId = 0;
            int index = 0;

            string stateCode = leadDetailsSearchOption.StateCode;
            string dbStateName = GetStateNameByStateCode(stateCode);

            using (var _context = new MainControlDB_Entities())
            {

                var dynamicCampaign = (from a in _context.Biz_Campaign_Details
                                       where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Admin_Project_Id > 0
                                       select new
                                       {
                                           campaignDetails = a,
                                           campaignAssignedDetails = a.Biz_Campaign_Assigned_Details,
                                           campaignSupportUserDetails = a.Biz_Campaign_Support_User_Details,
                                           campaignLeadActivity = a.Biz_Campaign_Lead_Activity
                                       }).SingleOrDefault();

                if (dynamicCampaign != null)
                {
                    var _dbCampaignDetails = dynamicCampaign.campaignDetails;
                    var _dbCampaignSupportUserDetails = dynamicCampaign.campaignSupportUserDetails.Where(m => m.Support_User_Id == leadDetailsSearchOption.SupportUserId && !m.Is_Deleted).ToList();
                    var _dbCampaignAssignedDetails = dynamicCampaign.campaignAssignedDetails.Where(m => m.Support_User_Id == leadDetailsSearchOption.SupportUserId && !m.Is_Deleted).OrderBy(m => m.Created_Time_Stamp).ToList();

                    var _dbCampaignLeadActivityLst = dynamicCampaign.campaignLeadActivity.Where(m => m.Support_User_Id == leadDetailsSearchOption.SupportUserId && !m.Is_Deleted).ToList();

                    var _dbConactedLeadDetailsIds = new List<long>();

                    if (leadDetailsSearchOption.Skipped)
                    {
                        _dbConactedLeadDetailsIds = _dbCampaignAssignedDetails.Where(m => m.Is_Skip == true).Select(m => m.Campaign_Assigned_Details_Id).ToList();
                    }
                    else if (!string.IsNullOrWhiteSpace(stateCode))
                    {
                        _dbConactedLeadDetailsIds = _dbCampaignAssignedDetails.Where(a => a.Address != null && (a.Address.ToLower().Contains(", " + stateCode.ToLower()) || a.Address.ToLower().Contains("," + stateCode.ToLower()) || a.Address.ToLower().Contains(", " + dbStateName.ToLower()) || a.Address.ToLower().Contains("," + dbStateName.ToLower()))).Select(m => m.Campaign_Assigned_Details_Id).ToList();

                        var notExistsStateCodeLeadIds = _dbCampaignAssignedDetails.Where(m => !_dbConactedLeadDetailsIds.Contains(m.Campaign_Assigned_Details_Id)).Select(m => m.Campaign_Assigned_Details_Id).ToList();

                        _dbConactedLeadDetailsIds.AddRange(notExistsStateCodeLeadIds);
                    }
                    else
                    {
                        _dbConactedLeadDetailsIds = _dbCampaignAssignedDetails.Select(m => m.Campaign_Assigned_Details_Id).ToList();
                    }


                    var fullList = (from a in _dbCampaignAssignedDetails
                                    where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId && _dbConactedLeadDetailsIds.Contains(a.Campaign_Assigned_Details_Id)
                                    select new LeadDetails { CampaignAssignedDetailsId = a.Campaign_Assigned_Details_Id, Address = a.Address }).ToList();

                    var _dbLeadDetailslist = new List<LeadDetails>();


                    if (_dbConactedLeadDetailsIds != null && _dbConactedLeadDetailsIds.Count > 0)
                    {
                        foreach (var item in _dbConactedLeadDetailsIds)
                        {
                            var assignObj = fullList.Where(m => m.CampaignAssignedDetailsId == item).SingleOrDefault();
                            _dbLeadDetailslist.Add(assignObj);

                        }
                    }

                    //_dbLeadDetailslist.AddRange(fullList.Where(a => !a.Address.Contains(", " + leadDetailsSearchOption.StateCode)).OrderBy(a => a.CampaignAssignedDetailsId).ToList());

                    var dbCampaignAssignedDetails = dynamicCampaign.campaignAssignedDetails.Where(m => m.Campaign_Assigned_Details_Id == leadDetailsSearchOption.LeadId && !m.Is_Deleted).SingleOrDefault();

                    var _existsLeadActivity = _dbCampaignLeadActivityLst.Where(m => m.Campaign_Assign_Detail_Id == leadDetailsSearchOption.LeadId).Count();

                    if (_existsLeadActivity == 0)
                    {
                        dbCampaignAssignedDetails.Is_Skip = true;
                        dbCampaignAssignedDetails.Updated_Time_Stamp = DateTime.Now;
                        _context.SaveChanges();
                    }


                    if (_dbLeadDetailslist != null && _dbLeadDetailslist.Count > 0)
                    {
                        index = _dbLeadDetailslist.FindIndex(a => a.CampaignAssignedDetailsId == leadDetailsSearchOption.LeadId);
                        _leadId = _dbLeadDetailslist.Skip(index + 1).Take(1).FirstOrDefault().CampaignAssignedDetailsId;
                    }

                    var campaignSupportUserDetails = (from a in _dbCampaignSupportUserDetails
                                                      where a.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId && !a.Is_Deleted && a.Support_User_Id == leadDetailsSearchOption.SupportUserId
                                                      select a).Take(1).SingleOrDefault();

                    if (campaignSupportUserDetails != null)
                    {
                        campaignSupportUserDetails.Last_Lead_Id = _leadId;
                        campaignSupportUserDetails.Updated_Time_Stamp = DateTime.Now;
                        _context.SaveChanges();
                        isupdated = true;
                    }
                }
            }
            return isupdated;
        }
        #endregion

        //#region  Get Campaign Details First CountBy StateCode
        //public long GetCampaignDetailsFirstCountByStateCode(long campaignId, long supportUserId, string stateCode)
        //{
        //    long Count = 0;
        //    var CampaignasigndetailswithCondition = (from a in _context.Biz_Campaign_Assigned_Details
        //                                             where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                             && a.Address.Contains(", " + stateCode)
        //                                             orderby a.Campaign_Assigned_Details_Id
        //                                             select new LeadDetails
        //                                             {
        //                                                 Name = a.Name,
        //                                                 BusinessName = a.Business_Name,
        //                                                 Phone = a.Phone_Number,
        //                                                 Email = a.Email_Address,
        //                                                 Address = a.Address,
        //                                                 UserType = a.User_Type,
        //                                                 LastFiled = a.Last_Filed_On ?? DateTime.Now,
        //                                                 NoofTrucks = a.No_Of_Trucks ?? 0,
        //                                                 CampaignAssignedDetailsId = a.Campaign_Assigned_Details_Id
        //                                             }).ToList();
        //    var CampaignasigndetailswithoutCondition = (from a in _context.Biz_Campaign_Assigned_Details
        //                                                where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                                && !a.Address.Contains(", " + stateCode)
        //                                                orderby a.Campaign_Assigned_Details_Id
        //                                                select new LeadDetails
        //                                                {
        //                                                    Name = a.Name,
        //                                                    BusinessName = a.Business_Name,
        //                                                    Phone = a.Phone_Number,
        //                                                    Email = a.Email_Address,
        //                                                    Address = a.Address,
        //                                                    UserType = a.User_Type,
        //                                                    LastFiled = a.Last_Filed_On ?? DateTime.Now,
        //                                                    NoofTrucks = a.No_Of_Trucks ?? 0,
        //                                                    CampaignAssignedDetailsId = a.Campaign_Assigned_Details_Id
        //                                                }).ToList();

        //    List<LeadDetails> Arrangedlist = new List<LeadDetails>();
        //    Arrangedlist = CampaignasigndetailswithCondition;
        //    Arrangedlist.AddRange(CampaignasigndetailswithoutCondition);
        //    var _dbLeadDetailslist = (from a in _context.Biz_Campaign_Support_User_Details
        //                              where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                              select a.Last_Lead_Id).Take(1).SingleOrDefault();
        //    if (_dbLeadDetailslist != null)
        //    {
        //        var LastLeadId = (Arrangedlist.Where(a => a.CampaignAssignedDetailsId < _dbLeadDetailslist).OrderByDescending(a => a.CampaignAssignedDetailsId).Select(a => a.CampaignAssignedDetailsId).Take(1).SingleOrDefault());
        //        Count = LastLeadId;
        //    }
        //    return Count;
        //}
        //#endregion

        //#region Get Campaign Details Last Count By StateCode
        //public long GetCampaignDetailsLastCountByStateCode(long campaignId, long supportUserId, string stateCode)
        //{
        //    long Count = 0;
        //    var CampaignasigndetailswithCondition = (from a in _context.Biz_Campaign_Assigned_Details
        //                                             where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                             && a.Address.Contains(", " + stateCode)
        //                                             orderby a.Campaign_Assigned_Details_Id
        //                                             select new LeadDetails
        //                                             {
        //                                                 Name = a.Name,
        //                                                 BusinessName = a.Business_Name,
        //                                                 Phone = a.Phone_Number,
        //                                                 Email = a.Email_Address,
        //                                                 Address = a.Address,
        //                                                 UserType = a.User_Type,
        //                                                 LastFiled = a.Last_Filed_On ?? DateTime.Now,
        //                                                 NoofTrucks = a.No_Of_Trucks ?? 0,
        //                                                 CampaignAssignedDetailsId = a.Campaign_Assigned_Details_Id
        //                                             }).ToList();
        //    var CampaignasigndetailswithoutCondition = (from a in _context.Biz_Campaign_Assigned_Details
        //                                                where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                                                && !a.Address.Contains(", " + stateCode)
        //                                                orderby a.Campaign_Assigned_Details_Id
        //                                                select new LeadDetails
        //                                                {
        //                                                    Name = a.Name,
        //                                                    BusinessName = a.Business_Name,
        //                                                    Phone = a.Phone_Number,
        //                                                    Email = a.Email_Address,
        //                                                    Address = a.Address,
        //                                                    UserType = a.User_Type,
        //                                                    LastFiled = a.Last_Filed_On ?? DateTime.Now,
        //                                                    NoofTrucks = a.No_Of_Trucks ?? 0,
        //                                                    CampaignAssignedDetailsId = a.Campaign_Assigned_Details_Id
        //                                                }).ToList();

        //    List<LeadDetails> Arrangedlist = new List<LeadDetails>();
        //    Arrangedlist = CampaignasigndetailswithCondition;
        //    Arrangedlist.AddRange(CampaignasigndetailswithoutCondition);
        //    var dbLeadDetailslist = (from a in _context.Biz_Campaign_Support_User_Details
        //                             where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
        //                             select a.Last_Lead_Id).Take(1).SingleOrDefault();
        //    if (dbLeadDetailslist != null)
        //    {
        //        var LastLeadId = (Arrangedlist.Where(a => a.CampaignAssignedDetailsId > dbLeadDetailslist).OrderByDescending(a => a.CampaignAssignedDetailsId).Select(a => a.CampaignAssignedDetailsId).Take(1).SingleOrDefault());
        //        Count = LastLeadId;
        //    }
        //    else
        //    {
        //        var dbCampaignAssignedDetails = (Arrangedlist.Select(a => a.CampaignAssignedDetailsId).Take(1).SingleOrDefault());
        //        var LastLeadId = (Arrangedlist.Where(a => a.CampaignAssignedDetailsId > dbCampaignAssignedDetails).OrderByDescending(a => a.CampaignAssignedDetailsId).Select(a => a.CampaignAssignedDetailsId).Take(1).SingleOrDefault());
        //        Count = LastLeadId;
        //    }
        //    return Count;
        //}
        //#endregion

        #region Get Campaign Details Count By StateCode
        public bool GetCampaignDetailsCountByStateCode(long campaignId, long supportUserId, string stateCode)
        {
            string dbStateName = GetStateNameByStateCode(stateCode);
            bool IsExsits = (from a in _context.Biz_Campaign_Assigned_Details
                             where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                            && a.Address != null && (a.Address.ToLower().Contains(", " + stateCode.ToLower()) || a.Address.ToLower().Contains("," + stateCode.ToLower()) || a.Address.ToLower().Contains(", " + dbStateName.ToLower()) || a.Address.ToLower().Contains("," + dbStateName.ToLower()))
                             orderby a.Created_Time_Stamp
                             select a.Campaign_Assigned_Details_Id).Any();
            return IsExsits;
        }
        #endregion

        #region Get StateCode By StateName
        public string GetStateNameByStateCode(string stateCode)
        {
            string stateName = string.Empty;
            var _dbStateObj = _context.Static_Biz_Admin_States.Where(m => !m.Is_Deleted && m.State_Code == stateCode).SingleOrDefault();
            if (_dbStateObj != null)
            {
                stateName = _dbStateObj.State_Name;
            }
            return stateName;
        }
        #endregion

        #region Get Campaign Headers Details By CampaignDetailsId
        //Get Campaign Headers Details By CampaignDetailsId
        public List<ChampaignExcelHeaderDetails> GetCampaignHeadersDetails(long? campaignDetailsId)
        {
            List<ChampaignExcelHeaderDetails> campaignHeaderList = new List<ChampaignExcelHeaderDetails>();
            var _dbCampaignHeaderList = (from a in _context.Biz_Campaign_Excel_Headers
                                         where !a.Is_Deleted && a.Campaign_Details_Id == campaignDetailsId
                                         orderby a.Created_Time_Stamp descending
                                         select new
                                         {
                                             a.Campaign_Excel_Header_Id,
                                             a.Header_Name,
                                             a.Campaign_Details_Id,
                                         }).ToList();

            if (_dbCampaignHeaderList != null && _dbCampaignHeaderList.Count > 0)
            {
                foreach (var item in _dbCampaignHeaderList)
                {
                    ChampaignExcelHeaderDetails campaignDetails = new ChampaignExcelHeaderDetails();
                    campaignDetails.CampaignExcelHeaderId = item.Campaign_Excel_Header_Id;
                    campaignDetails.HeaderName = item.Header_Name;
                    campaignDetails.CampaignDetailsId = item.Campaign_Details_Id;
                    campaignHeaderList.Add(campaignDetails);
                }
            }
            return campaignHeaderList;
        }
        #endregion

        #region Get Campaign Details by  Campain Detail Id and Support User Id
        public List<LeadDetails> GetCampaignDetailsbyCampainDetailIdandSupportUserId(long campaignId, long supportUserId)
        {
            List<LeadDetails> _leadDetailsList = new List<LeadDetails>();

            var _dbLeadDetailsList = (from a in _context.Biz_Campaign_Assigned_Details
                                      where a.Campaign_Details_Id == campaignId && !a.Is_Deleted && a.Support_User_Id == supportUserId
                                      orderby a.Created_Time_Stamp
                                      select a).ToList();

            if (_dbLeadDetailsList != null && _dbLeadDetailsList.Any())
            {
                foreach (var item in _dbLeadDetailsList)
                {
                    LeadDetails leadDetails = new LeadDetails();
                    leadDetails.Name = item.Name;
                    leadDetails.BusinessName = item.Business_Name;
                    leadDetails.Phone = item.Phone_Number;
                    leadDetails.Email = item.Email_Address;
                    leadDetails.Address = item.Address;
                    leadDetails.CampaignAssignedDetailsId = item.Campaign_Assigned_Details_Id;
                    _leadDetailsList.Add(leadDetails);
                }
            }
            return _leadDetailsList;
        }
        #endregion

        #region Remove All Non Static IPs
        /// <summary>
        /// Remove All Non Static IPs
        /// </summary>
        /// <returns></returns>
        public bool RemoveAllNonStaticIPs()
        {
            using (var entities = new MainControlDB_Entities())
            {
                DateTime removeIpAddressDate = DateTime.Now.AddDays(-Utilities.DataUtility.GetDouble(Utilities.DataUtility.GetAppSettings("RemoveNonStaticIPsDayCount")));
                var dbBizIpAddress = entities.Biz_Ip_Address.Where(du => du.Create_Time_Stamp <= removeIpAddressDate && du.Is_Static != true && !du.Is_Deleted).ToList();
                if (dbBizIpAddress != null && dbBizIpAddress.Any())
                {
                    foreach (var item in dbBizIpAddress)
                    {
                        item.Is_Deleted = true;
                        item.Update_Time_Stamp = DateTime.Now;
                    }
                    entities.SaveChanges();
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region Update Campaign Assigned Details EIN By Campaign Assigned Details Id
        public bool UpdateCampaignAssignedDetailsEINByCampaignAssignedDetailsId(long campaignAssignedDetailsId, string ein)
        {
            bool isUpdated = false;

            var _dbCampaignAssignedDetailsDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                                     where a.Campaign_Assigned_Details_Id == campaignAssignedDetailsId && !a.Is_Deleted
                                                     select a).SingleOrDefault();
            if (_dbCampaignAssignedDetailsDetails != null)
            {
                _dbCampaignAssignedDetailsDetails.EIN = ein;
                _dbCampaignAssignedDetailsDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
                isUpdated = true;
            }
            return isUpdated;
        }
        #endregion

        #region regassign check exists Lead Id
        public void ReAssignedLeads(long campaignDetailId, long supportUserId)
        {

            var _dbLeadDetailslist = (from a in _context.Biz_Campaign_Support_User_Details
                                      where a.Campaign_Details_Id == campaignDetailId && !a.Is_Deleted && a.Support_User_Id == supportUserId && a.Last_Lead_Id != null
                                      select a).FirstOrDefault();

            if (_dbLeadDetailslist != null && _dbLeadDetailslist.Last_Lead_Id > 0)
            {
                var _dbCampaignAssignedDetails = _context.Biz_Campaign_Assigned_Details.Where(m => !m.Is_Deleted && m.Campaign_Details_Id == campaignDetailId && m.Support_User_Id == supportUserId && m.Campaign_Assigned_Details_Id == _dbLeadDetailslist.Last_Lead_Id).SingleOrDefault();

                if (_dbCampaignAssignedDetails == null)
                {
                    _dbLeadDetailslist.Last_Lead_Id = null;
                    _dbLeadDetailslist.Updated_Time_Stamp = DateTime.Now;
                    _context.SaveChanges();
                }
            }


            if (_dbLeadDetailslist != null && _dbLeadDetailslist.Last_Lead_Id == 0)
            {
                _dbLeadDetailslist.Last_Lead_Id = null;
                _dbLeadDetailslist.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
            }


        }
        #endregion


        #region regassign check exists Lead Id
        public void UpdateReassignDetailsReset(LeadDetailsSearchOption leadDetailsSearchOption)
        {
            string stateCode = leadDetailsSearchOption.StateCode;
            string dbStateName = GetStateNameByStateCode(stateCode);

            var _dbLeadSupportDetails = _context.Biz_Campaign_Support_User_Details.Where(m => !m.Is_Deleted && m.Support_User_Id == leadDetailsSearchOption.SupportUserId && m.Campaign_Details_Id == leadDetailsSearchOption.CampaignDetailId).SingleOrDefault();

            if (_dbLeadSupportDetails != null && _dbLeadSupportDetails.Last_Lead_Id > 0)
            {
                var _dbLastLeadDetails = _context.Biz_Campaign_Assigned_Details.Where(m => m.Campaign_Assigned_Details_Id == _dbLeadSupportDetails.Last_Lead_Id && !m.Is_Deleted).SingleOrDefault();

                if (_dbLastLeadDetails == null)
                {
                    _dbLeadSupportDetails.Last_Lead_Id = null;
                    _dbLeadSupportDetails.Updated_Time_Stamp = DateTime.Now;
                    _context.SaveChanges();
                }
                else if (leadDetailsSearchOption.Skipped && _dbLastLeadDetails != null && !_dbLastLeadDetails.Is_Skip)
                {
                    _dbLeadSupportDetails.Last_Lead_Id = null;
                    _dbLeadSupportDetails.Updated_Time_Stamp = DateTime.Now;
                    _context.SaveChanges();
                }
                else if (_dbLastLeadDetails != null && !string.IsNullOrWhiteSpace(leadDetailsSearchOption.StateCode) && !(_dbLastLeadDetails.Address != null && (_dbLastLeadDetails.Address.ToLower().Contains(", " + stateCode.ToLower()) || _dbLastLeadDetails.Address.ToLower().Contains("," + stateCode.ToLower()) || _dbLastLeadDetails.Address.ToLower().Contains(", " + dbStateName.ToLower()) || _dbLastLeadDetails.Address.ToLower().Contains("," + dbStateName.ToLower()))))
                {
                    _dbLeadSupportDetails.Last_Lead_Id = null;
                    _dbLeadSupportDetails.Updated_Time_Stamp = DateTime.Now;
                    _context.SaveChanges();
                }

            }
        }
        #endregion

        #region Get Campaign Uploaded Time Details
        public CampaignDetails GetCampaignUploadedTimeDetails(CampaignDetails campaignDetails)
        {
            var _dbCampaignDetails = _context.Biz_Campaign_Details.Where(m => !m.Is_Deleted && m.Campaign_Details_Id == campaignDetails.CampaignDetailsId).SingleOrDefault();

            if (_dbCampaignDetails != null)
            {
                campaignDetails.NoOfLeads = _dbCampaignDetails.No_Of_Leads;
                campaignDetails.UploadedCount = _dbCampaignDetails.Biz_Campaign_Assigned_Details.Where(m => !m.Is_Deleted).Count();
            }

            return campaignDetails;
        }
        #endregion

        #region Get User Verification Code Type
        public CampaignDetails GetUserVerificationCodeType(CampaignDetails campaignDetails)
        {
            var _dbCampaignDetails = _context.Biz_Campaign_Details.Where(m => !m.Is_Deleted && m.Campaign_Details_Id == campaignDetails.CampaignDetailsId).SingleOrDefault();

            if (_dbCampaignDetails != null)
            {
                campaignDetails.NoOfLeads = _dbCampaignDetails.No_Of_Leads;
                campaignDetails.UploadedCount = _dbCampaignDetails.Biz_Campaign_Assigned_Details.Where(m => !m.Is_Deleted).Count();
            }

            return campaignDetails;
        }
        #endregion

        #region Update User Verification Code Type
        /// <summary>
        /// Update User Verification Code Type
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <param name="verificationCodeType"></param>
        /// <returns></returns>
        public bool UpdateUserVerificationCodeType(long adminUserId)
        {
            bool _isUpdateStatus = false;
            if (adminUserId > 0)
            {
                var _dbAdminUser = (from a in _context.Biz_Admin_Users
                                    where a.Admin_User_Id == adminUserId && !a.Is_Deleted
                                    select a).SingleOrDefault();
                if (_dbAdminUser != null && _dbAdminUser.Admin_User_Id > 0)
                {
                    _dbAdminUser.Verification_Code_Type = !string.IsNullOrEmpty(_dbAdminUser.Verification_Code_Type) ? (_dbAdminUser.Verification_Code_Type == VerificationCodeType.SMS.ToString() ? VerificationCodeType.EMAIL.ToString() : VerificationCodeType.SMS.ToString()) : VerificationCodeType.EMAIL.ToString();
                    _dbAdminUser.Update_Time_Stamp = DateTime.Now;
                    _context.SaveChanges();
                    _isUpdateStatus = true;
                }
            }
            return _isUpdateStatus;
        }
        #endregion

        #region Get Campaign Lead Activity Details
        public LeadCommunication GetCampaignLeadActivityDetails(long campaignLeadActivityId)
        {
            LeadCommunication leadCommunication = new LeadCommunication();

            var _dbLeadActivityDetails = _context.Biz_Campaign_Lead_Activity.Where(m => !m.Is_Deleted && m.Campaign_Lead_Activity_Id == campaignLeadActivityId).SingleOrDefault();

            if (_dbLeadActivityDetails != null)
            {
                leadCommunication.CampaignLeadActivityId = _dbLeadActivityDetails.Campaign_Lead_Activity_Id;
                leadCommunication.CampaignDetailId = _dbLeadActivityDetails.Campaign_Detail_Id;
                leadCommunication.SupportUserId = _dbLeadActivityDetails.Support_User_Id;
                leadCommunication.EmailAddress = _dbLeadActivityDetails.Email_Address;
                leadCommunication.MethodeofContract = _dbLeadActivityDetails.Method_Of_Contact;
                leadCommunication.TypeOfCall = _dbLeadActivityDetails.Type_Of_Call;
                leadCommunication.Spoketo = _dbLeadActivityDetails.Spoke_To;
                leadCommunication.Comments = _dbLeadActivityDetails.Comments;
                leadCommunication.DonotContactagain = _dbLeadActivityDetails.Is_Do_Not_Disturb;
                leadCommunication.Reason = _dbLeadActivityDetails.Reason;
                leadCommunication.OtherReason = _dbLeadActivityDetails.Other_Reason;
                leadCommunication.IsFollowRequired = _dbLeadActivityDetails.Is_Follow_Up;
                leadCommunication.FollowupDate = _dbLeadActivityDetails.Followup_Date;
                leadCommunication.FollowupTime = _dbLeadActivityDetails.Followup_Time;
            }

            return leadCommunication;

        }
        #endregion

        #region Get Recent Returns
        public List<RecentReturns> GetRecentReturns(Guid userId)
        {
            List<RecentReturns> form2290ReturnDetailList = new List<RecentReturns>();
            if (userId != null && userId != Guid.Empty)
            {
                //var campaignFollowUpId = (from a in _context.Biz_Campaign_Assigned_Details
                //                          where a.Campaign_Assigned_Details_Id == assignedDetailId && !a.Is_Deleted
                //                          orderby a.Campaign_Follow_Up_Id
                //                          select a.Campaign_Follow_Up_Id).SingleOrDefault();
                //if (campaignFollowUpId > 0)
                //{
                //    var followUpDetails = _context.Biz_Campaign_Manager_Follow_Up_Details.Where(f => f.Campaign_Follow_Up_Id == campaignFollowUpId && !f.Is_Deleted).SingleOrDefault();
                //    if (followUpDetails.User_Id > 0)
                //    {
                //        userId = followUpDetails.User_Id;
                //    }
                //}
                //if (userId > 0)
                //{

                using (var reqClient = new ETTAPIClient())
                {
                    string requestTsnaUri = "/Form2290/GetRecentReturnDetails/?userId=" + userId;
                    ETTAPIRequestUtility.GenerateAuthHeader(reqClient, requestTsnaUri, "GET");
                    var _settingresponse = reqClient.GetAsync(requestTsnaUri).Result;
                    if (_settingresponse != null && _settingresponse.IsSuccessStatusCode)
                    {
                        form2290ReturnDetailList = _settingresponse.Content.ReadAsAsync<List<RecentReturns>>().Result;
                    }
                }
                //}
            }
            return form2290ReturnDetailList;
        }
        #endregion

        #region Get FollowUp Details
        public RecentReturns GetFollowUpDetails(long assignedDetailId)
        {
            RecentReturns followUpDetails = new RecentReturns();
            if (assignedDetailId > 0)
            {
                var campaignFollowUpId = (from a in _context.Biz_Campaign_Assigned_Details
                                          where a.Campaign_Assigned_Details_Id == assignedDetailId && !a.Is_Deleted
                                          orderby a.Campaign_Follow_Up_Id
                                          select a.Campaign_Follow_Up_Id).SingleOrDefault();
                if (campaignFollowUpId > 0)
                {
                    var managerFollowUpDetails = _context.Biz_Campaign_Manager_Follow_Up_Details.Where(f => f.Campaign_Follow_Up_Id == campaignFollowUpId && !f.Is_Deleted).SingleOrDefault();
                    if (managerFollowUpDetails.Admin_User_Id > 0)
                    {
                        followUpDetails.AdminUserName = _context.Biz_Admin_Users.Where(a => a.Admin_User_Id == managerFollowUpDetails.Admin_User_Id && !a.Is_Deleted).Select(n => n.Admin_User_Name).SingleOrDefault();
                    }
                    followUpDetails.Comments = !string.IsNullOrWhiteSpace(managerFollowUpDetails.Comments) ? managerFollowUpDetails.Comments : string.Empty;
                }
            }
            return followUpDetails;
        }
        #endregion

        #region Update FollowUp Details
        private void UpdateFollowUpDetails(int campFollowUpId, string status)
        {
            int campaignFollowUpId = campFollowUpId;
            if (campaignFollowUpId > 0)
            {
                var _followUpDetails = _context.Biz_Campaign_Manager_Follow_Up_Details.Where(q => q.Campaign_Follow_Up_Id == campaignFollowUpId && !q.Is_Deleted).SingleOrDefault();
                if (_followUpDetails != null)
                {
                    _followUpDetails.Campaign_Status = status;
                    _followUpDetails.Updated_Time_Stamp = DateTime.Now;
                    _context.SaveChanges();
                }
            }
        }
        #endregion

        #region Get Business Details By User Id
        public List<BusinessDetails> GetBusinessDetailsByUserId(int userId)
        {
            List<BusinessDetails> businessDetailsList = new List<BusinessDetails>();
            if (userId > 0)
            {
                using (var reqClient = new ETTAPIClient())
                {
                    string requestUri = "Form2290/GetBusinessDetailsByUserId/?userId=" + userId;
                    ETTAPIRequestUtilities.GenerateAuthHeader(reqClient, requestUri, "GET");
                    var _settingresponse = reqClient.GetAsync(requestUri).Result;
                    if (_settingresponse != null && _settingresponse.IsSuccessStatusCode)
                    {
                        businessDetailsList = _settingresponse.Content.ReadAsAsync<List<BusinessDetails>>().Result;
                    }
                }
            }
            return businessDetailsList;
        }
        #endregion

        #region For download
        /// <summary>
        /// For 
        /// </summary>
        /// <param name="optionSelected"></param>
        /// <returns></returns>
        public List<GroupMembers> GetReportBySelectedOption(string optionSelected, long adminUserId, long campaignDetailId)
        {
            //long supportUserId = (from a in _context.Biz_Admin_Users where a.Admin_First_Name )
            List<GroupMembers> recordList = new List<GroupMembers>();
            if (!string.IsNullOrWhiteSpace(optionSelected))
            {
                //if (optionSelected == "DoNotContact")
                //{
                //    recordList = (from c in _context.Biz_Campaign_Assigned_Details
                //                  join a in _context.Biz_Campaign_Lead_Activity on c.Campaign_Assigned_Details_Id
                //                  equals a.Campaign_Assign_Detail_Id
                //                  where a.Support_User_Id == adminUserId && a.Is_Do_Not_Disturb && !a.Is_Deleted && !c.Is_Deleted
                //                  select new GroupMembers
                //                  {
                //                      Contact_Name = c.Name,
                //                      EmailAddress = c.Email_Address,
                //                      PhoneNumber = c.Phone_Number
                //                  }
                //                 ).ToList();
                //}
                //else
                //{
                recordList = (from c in _context.Biz_Campaign_Assigned_Details
                              join a in _context.Biz_Campaign_Lead_Activity on c.Campaign_Assigned_Details_Id
                              equals a.Campaign_Assign_Detail_Id
                              where a.Campaign_Detail_Id == campaignDetailId && a.Support_User_Id == adminUserId && a.Method_Of_Contact == optionSelected && !a.Is_Deleted && !c.Is_Deleted
                              select new GroupMembers
                              {
                                  Contact_Name = c.Name,
                                  EmailAddress = c.Email_Address,
                                  PhoneNumber = c.Phone_Number
                              }
                             ).ToList();
                // }
            }
            return recordList;
        }
        #endregion

        #region Get Transaction Report Details
        public List<TransactionReportDetails> GetTransactionReportDetails(TransactionReport transactionReport)
        {
            List<TransactionReportDetails> transactionReportDetails = new List<TransactionReportDetails>();
            var query = @"exec Span_Get_Payment_Transation_Details @paymentProcessorId,@productId,@fromDate,@toDate,@transactionType";
            using (SqlConnection con = new SqlConnection(transactionReport.ConnectionString))
            {
                transactionReportDetails = con.Query<TransactionReportDetails>(query, new { @productId = transactionReport.ProductId, @paymentProcessorId = transactionReport.PaymentProcessorid, @fromDate = transactionReport.FromDate, @toDate = transactionReport.ToDate, @transactionType = transactionReport.TransactionType }).ToList();
            }
            List<string> isVoidOrRefundTransactionRefIds = new List<string>();
            var isCheckVoidOrRefundQuery = @"select  Transaction_Reference_Id as TransactionReferenceId  from SPAN_PAYMENT_REFUND_LOG WHERE Paid_Date <= @EndDate and Paid_Date >= @BeginDate and Product_Id = @ProductId  and Is_Deleted = 0";
            //Get the Void or refund transaction ids to compare original transaction ref ids 
            using (SqlConnection con = new SqlConnection(transactionReport.ConnectionString))
            {
                isVoidOrRefundTransactionRefIds = con.Query<string>(isCheckVoidOrRefundQuery, new { @productId = transactionReport.ProductId, @BeginDate = transactionReport.FromDate, @EndDate = transactionReport.ToDate }).ToList();
            }
            if(isVoidOrRefundTransactionRefIds != null && isVoidOrRefundTransactionRefIds.Count > 0)
            {
                transactionReportDetails.Where(a => isVoidOrRefundTransactionRefIds.Contains(a.TransactionReferenceId)).ToList().ForEach(a => a.IsRefundOrVoid = true);
            }
            if (transactionReport != null)
            {
                string productCode = string.Empty;
                var productCodeQuery = @"SELECT [Product_Code] ProductName FROM span_products WHERE [Is_Deleted] = 0 and [Span_product_id] in (@ProductId)";
                using (SqlConnection con1 = new SqlConnection(transactionReport.ConnectionString))
                {
                    productCode = con1.Query<string>(productCodeQuery, new { @ProductId = transactionReport.ProductId }).SingleOrDefault();
                }
                if (transactionReportDetails != null && transactionReportDetails.Count > 0)
                {
                    transactionReportDetails.Where(a => a.ApiCallStatus == "Refunded").ToList().ForEach(a => a.ApiCallStatus = "Success");

                    transactionReportDetails.Where(a => a.ProductCode == null).ToList().ForEach(a => a.ProductCode = productCode);
                }

                string chargeBackType = "REFUND"; //Bug #39819- We changed paid amount to refund amount and Bug #39708 -We changed paid date to refund date
                var refundQuery = string.Empty;
                if(transactionReport.PaymentProcessorid > 0)
                {
                    refundQuery = @"select Product_Id as ProductId, Transaction_Reference_Id as TransactionReferenceId, Refund_Amount as OrderAmount,Payment_Processor_Id as PaymentProcessorType,Email_Address as EmailAddress,Refund_Void_Date as PaymentDate from SPAN_PAYMENT_REFUND_LOG WHERE Charge_Back_Type ='" + chargeBackType + "' AND Payment_Processor_Id =@paymentProcessorId AND REFUND_VOID_date <= @EndDate and REFUND_VOID_date >= @BeginDate and Product_Id = @ProductId  and Is_Deleted = 0 ";
                }
                else
                {
                    refundQuery = @"select Product_Id as ProductId, Transaction_Reference_Id as TransactionReferenceId, Refund_Amount as OrderAmount,Payment_Processor_Id as PaymentProcessorType,Email_Address as EmailAddress,Refund_Void_Date as PaymentDate from SPAN_PAYMENT_REFUND_LOG WHERE Charge_Back_Type ='" + chargeBackType + "' AND REFUND_VOID_date <= @EndDate and REFUND_VOID_date >= @BeginDate and Product_Id = @ProductId  and Is_Deleted = 0 ";
                }
               
                using (SqlConnection con = new SqlConnection(transactionReport.ConnectionString))
                {
                    var refundDbDetails= con.Query<TransactionReportDetails>(refundQuery, new { @BeginDate = transactionReport.FromDate, @EndDate = transactionReport.ToDate, @ProductId = transactionReport.ProductId, @paymentProcessorId = transactionReport.PaymentProcessorid }).ToList();
                    if (refundDbDetails != null && refundDbDetails.Count > 0)
                    {
                        foreach (var refundDetails in refundDbDetails)
                        {
                            if (refundDetails != null)
                            {
                                TransactionReportDetails refundTransactionDetails = new TransactionReportDetails();
                                refundTransactionDetails.ProductId = refundDetails.ProductId;
                                refundTransactionDetails.ProductCode = productCode;
                                refundTransactionDetails.TransactionReferenceId = refundDetails.TransactionReferenceId;
                                refundTransactionDetails.EmailAddress = refundDetails.EmailAddress;
                                refundTransactionDetails.OrderAmount = Main.Control.Services.Utilities.Utilities.GetDecimal2Digits(refundDetails.OrderAmount);
                                refundTransactionDetails.PaymentProcessorType = refundDetails.PaymentProcessorType;
                                refundTransactionDetails.PaymentApprovalCode = "";
                                refundTransactionDetails.ApiCallStatus = "Refunded";
                                refundTransactionDetails.ResponseS3FilePath = "";
                                refundTransactionDetails.TransactionLogText = "";
                                refundTransactionDetails.UrlPath = "";
                                refundTransactionDetails.PaymentDate = refundDetails.PaymentDate;
                                transactionReportDetails.Add(refundTransactionDetails);
                            }
                        }
                    }
                }
            }
            return transactionReportDetails;
        }
        #endregion

        #region Get Span Products
        public List<SpanProducts> GetSpanProducts(string connectionString)
        {
            List<SpanProducts> spanProducts = new List<SpanProducts>();
            var query = @"SELECT [Span_Product_Id] ProductId ,[Product_Code] ProductName FROM span_products WHERE [Is_Deleted] = 0 and [Span_product_id] in (8,1,12,4,14,11,3,16,15,2,17,18,19)";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                spanProducts = con.Query<SpanProducts>(query).ToList();
            }
            return spanProducts;
        }
        #endregion
     
        #region Get S3 upload URL
        public List<TransactionReportDetails> GetS3UploadURL(string transactionId, string connectionString, int paymentProcessorType)
        {
            var query = "SELECT P.Request_Response_S3_Path ResponseS3FilePath,P.Span_Payment_Processor_Id PaymentProcessorType FROM SPAN_API_REQUEST_RESPONSE_LOG P WHERE P.Transaction_Reference_Id = '" + transactionId + "' AND P.Span_Payment_Processor_Id = " + paymentProcessorType + " AND P.IS_DELETED = 0";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                List<TransactionReportDetails> logFiles = con.Query<TransactionReportDetails>(query).ToList();
                return logFiles;
            }
        }
        #endregion

        #region Get Span Products
        public List<PaymentProcessors> GetSpanPaymentProcessors(string connectionString)
        {
            List<PaymentProcessors> spanProducts = new List<PaymentProcessors>();
            var query = @"SELECT [Span_Payment_Processor_Id] PaymentProcessorId, [Processor_Name] PaymentProcessorName FROM Span_Payment_Processors  where Span_Payment_Processor_Id !=4 and  [Is_Deleted] = 0";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                spanProducts = con.Query<PaymentProcessors>(query).ToList();
            }
            return spanProducts;
        }
        #endregion

        #region Get all user payments

        public List<UserPayments> GetAllUserPayments()
        {
            List<UserPayments> userPayments = new List<UserPayments>();
            using (MainControlDB_Entities dbEntities = new MainControlDB_Entities())
            {
                userPayments = (from up in dbEntities.Biz_User_Payments
                                where !up.Is_Deleted
                                orderby up.Update_Time_Stamp descending
                                select new UserPayments
                                {
                                    Address1 = up.Address_1,
                                    Address2 = up.Address_2,
                                    BusinessName = up.Business_Name,
                                    CardNo = up.Card_No,
                                    CountryId = up.Country_Id,
                                    FromEmail = up.From_Email,
                                    MailSentTime = up.Mail_Sent_Time,
                                    MailBodyS3Path = up.Mail_Body_S3_Path,
                                    MessageId = up.Message_Id,
                                    NameOnCard = up.Name_On_Card,
                                    PaidTime = up.Paid_Time,
                                    PaymentAmount = up.Payment_Amount ?? 0,
                                    PaymentApprovalCode = up.Payment_Approval_Code,
                                    PaymentProcessor = up.Payment_Processor,
                                    PaymentStatus = up.Payment_Status,
                                    PostalZipCode = up.Postal_Zip_Code,
                                    Projectid = up.Project_id,
                                    StateId = up.State_Id ?? 0,
                                    ToEmail = up.To_Email,
                                    UserPaymentId = up.User_Payment_Id,
                                    InvoiceNo = up.Invoice_No,
                                    ReceiptS3Path = up.Receipt_S3_Path
                                }).ToList();
            }
            return userPayments;
        }

        #endregion

        #region Get user payment by user payment id

        public UserPayments GetUserPaymentDetailByTokenId(Guid tokenId)
        {
            UserPayments userPayment = new UserPayments();
            using (MainControlDB_Entities dbEntities = new MainControlDB_Entities())
            {
                userPayment = dbEntities.Biz_User_Payments.Where(x => x.User_Payment_Id == tokenId && !x.Is_Deleted).Select(x => new UserPayments
                {
                    Address1 = x.Address_1,
                    Address2 = x.Address_2,
                    BusinessName = x.Business_Name,
                    CardNo = x.Card_No,
                    CountryId = x.Country_Id,
                    FromEmail = x.From_Email,
                    MailSentTime = x.Mail_Sent_Time,
                    MailBodyS3Path = x.Mail_Body_S3_Path,
                    MessageId = x.Message_Id,
                    NameOnCard = x.Name_On_Card,
                    PaidTime = x.Paid_Time,
                    PhoneNo = x.Phone_No,
                    PaymentAmount = x.Payment_Amount ?? 0,
                    PaymentApprovalCode = x.Payment_Approval_Code,
                    PaymentProcessor = x.Payment_Processor,
                    PaymentStatus = x.Payment_Status,
                    PostalZipCode = x.Postal_Zip_Code,
                    Projectid = x.Project_id,
                    StateId = x.State_Id ?? 0,
                    ToEmail = x.To_Email,
                    UserPaymentId = x.User_Payment_Id,
                    CardExpiry = x.Card_Expiry,
                    OrderDescription = x.Order_Description,
                    City = x.City,
                    FailureMsg = x.Mail_Fail_Msg,
                    InvoiceNo = x.Invoice_No,
                    ReceiptS3Path = x.Receipt_S3_Path,
                }).SingleOrDefault();
            }
            return userPayment;
        }

        #endregion

        #region Save User Payments

        public UserPayments SaveUserPayments(UserPayments userPayments)
        {
            UserPayments userPayment = new UserPayments();
            using (MainControlDB_Entities dbEntities = new MainControlDB_Entities())
            {
                Biz_User_Payments dbUserPayments = null;

                bool IsRecordExists = false;

                if (userPayments.UserPaymentId != Guid.Empty)
                {
                    dbUserPayments = _context.Biz_User_Payments.SingleOrDefault(au => au.User_Payment_Id == userPayments.UserPaymentId && !au.Is_Deleted);
                }
                if (dbUserPayments != null && dbUserPayments.User_Payment_Id != Guid.Empty)
                {
                    IsRecordExists = true;
                }
                else
                {
                    dbUserPayments = new Biz_User_Payments();
                    dbUserPayments.User_Payment_Id = Guid.NewGuid();
                }

                dbUserPayments.Address_1 = userPayments.Address1;
                dbUserPayments.Address_2 = userPayments.Address2;
                dbUserPayments.Business_Name = userPayments.BusinessName;
                dbUserPayments.Card_No = userPayments.CardNo;
                dbUserPayments.Country_Id = userPayments.CountryId == 0 ? null : userPayments.CountryId;
                dbUserPayments.From_Email = userPayments.FromEmail;
                dbUserPayments.Phone_No = userPayments.PhoneNo;
                dbUserPayments.Mail_Sent_Time = userPayments.MailSentTime;
                dbUserPayments.Mail_Body_S3_Path = userPayments.MailBodyS3Path;
                dbUserPayments.Message_Id = userPayments.MessageId;
                dbUserPayments.Name_On_Card = userPayments.NameOnCard;
                dbUserPayments.Paid_Time = userPayments.PaidTime;
                dbUserPayments.Payment_Amount = userPayments.PaymentAmount;
                dbUserPayments.Payment_Approval_Code = userPayments.PaymentApprovalCode;
                dbUserPayments.Payment_Processor = userPayments.PaymentProcessor;
                dbUserPayments.Payment_Status = userPayments.PaymentStatus;
                dbUserPayments.Postal_Zip_Code = userPayments.PostalZipCode;
                dbUserPayments.Project_id = userPayments.Projectid;
                dbUserPayments.State_Id = userPayments.StateId == 0 ? null : userPayments.StateId;
                dbUserPayments.To_Email = userPayments.ToEmail;
                dbUserPayments.Card_Expiry = userPayments.CardExpiry;
                dbUserPayments.Order_Description = userPayments.OrderDescription;
                dbUserPayments.City = userPayments.City;
                dbUserPayments.Mail_Subject = userPayments.MailSubject;
                dbUserPayments.Mail_Fail_Msg = userPayments.FailureMsg;
                dbUserPayments.Product_User_Id = userPayments.ProductUserId;
                if (string.IsNullOrEmpty(dbUserPayments.Invoice_No))
                {
                    dbUserPayments.Invoice_No = "INV" + DateTime.Now.ToString("yyddmmhhmmss");
                }

                if (!IsRecordExists)
                {
                    dbUserPayments.Is_Deleted = false;
                    dbUserPayments.Create_Time_Stamp = DateTime.Now;
                    _context.Biz_User_Payments.Add(dbUserPayments);
                }
                dbUserPayments.Update_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
                userPayments.UserPaymentId = dbUserPayments.User_Payment_Id;
                return userPayments;
            }
        }

        #endregion

        #region Get all Countries

        public List<Country> GetCountries()
        {
            List<Country> countryList = new List<Country>();
            var dbCntry = (from s in _context.Static_Biz_Admin_Countries
                           where s.Is_Deleted == false
                           select new
                           {
                               s.Country_Id,
                               s.Country_name,
                               s.Country_Code
                           }).ToList();

            if (dbCntry != null && dbCntry.Any())
            {
                foreach (var item in dbCntry)
                {
                    Country country = new Country();
                    country.CountryId = item.Country_Id;
                    country.CountryCode = item.Country_Code;
                    country.CountryName = item.Country_name;
                    countryList.Add(country);
                }
            }

            return countryList;
        }

        #endregion

        #region Get Payment Template By Template Id

        public PaymentTemplate GetPaymentTemplateByTemplateId(int templateId)
        {
            PaymentTemplate template = new PaymentTemplate();
            using (MainControlDB_Entities entities = new MainControlDB_Entities())
            {
                var dbMailTemplate = entities.Static_Biz_Payment_Templates.Where(x => x.Payment_Template_Id == templateId && !x.Is_Deleted).SingleOrDefault();
                if (dbMailTemplate != null)
                {
                    template.MailTemplateHtml = dbMailTemplate.Template_Html;
                    template.MailSubject = dbMailTemplate.Mail_Subject;
                }
            }
            return template;
        }

        #endregion

        #region Get Transaction Report Details
        public List<SpanLibraryProductDetails> GetAllSpanLibrProducts(string spanLibConnStr)
        {
            List<SpanLibraryProductDetails> productDetails = new List<SpanLibraryProductDetails>();
            var query = @"select product_code ProductCode,Access_Key AccessKey,Secret_Key SecretKey From span_products where is_deleted=0";
            using (SqlConnection con = new SqlConnection(spanLibConnStr))
            {
                productDetails = con.Query<SpanLibraryProductDetails>(query).ToList();
            }
            return productDetails;
        }
        #endregion

        #region Save User Payment Log

        public void SaveUserPaymentLog(UserPaymentLog paymentLog)
        {
            using (MainControlDB_Entities _entities = new MainControlDB_Entities())
            {
                Biz_User_Payments_Logs userPaymentLog = new Biz_User_Payments_Logs();
                userPaymentLog.Acitivity_Msg = paymentLog.AcitivityMsg;
                if (paymentLog.AdminUserId > 0)
                {
                    userPaymentLog.Admin_User_Id = paymentLog.AdminUserId;
                }
                userPaymentLog.Actity_Type = paymentLog.ActivityType;
                userPaymentLog.Is_Deleted = false;
                userPaymentLog.Create_Time_Stamp = DateTime.Now;
                userPaymentLog.Update_Time_Stamp = DateTime.Now;
                userPaymentLog.User_Payment_Id = paymentLog.UserPaymentId;
                userPaymentLog.User_Payment_Id = paymentLog.UserPaymentId;
                _entities.Biz_User_Payments_Logs.Add(userPaymentLog);
                _entities.SaveChanges();

            }
        }
        #endregion

        #region Get all payment logs

        public List<UserPaymentLog> GetPaymentLogs(Guid paymentId, PaymentActivityType activityType)
        {
            List<UserPaymentLog> paymentLogs = new List<UserPaymentLog>();
            using (MainControlDB_Entities _entities = new MainControlDB_Entities())
            {
                paymentLogs = _entities.Biz_User_Payments_Logs.Where(x => x.User_Payment_Id == paymentId && x.Actity_Type == activityType.ToString() && !x.Is_Deleted).Select(x => new UserPaymentLog
                {
                    AcitivityMsg = x.Acitivity_Msg,
                    ActivityType = x.Actity_Type,
                    AdminUserId = x.Admin_User_Id ?? 0,
                    UserPaymentId = x.User_Payment_Id,
                    ActivityTimeStamp = x.Create_Time_Stamp
                }).ToList();

                if (paymentLogs != null)
                {
                    paymentLogs.ForEach(x =>
                    {
                        if (x.AdminUserId > 0) x.AdminUserName = GetAdminUserById(x.AdminUserId).AdminUserName;
                    });

                }
            }
            return paymentLogs;
        }

        #endregion

        #region Update Receipt S3 path in payments

        public void UpdateReceiptS3Path(Guid userPaymentId, string receiptS3Path)
        {
            using (MainControlDB_Entities _entities = new MainControlDB_Entities())
            {
                var dbUserPayment = _entities.Biz_User_Payments.Where(x => !x.Is_Deleted && x.User_Payment_Id == userPaymentId).SingleOrDefault();
                if (dbUserPayment != null)
                {
                    dbUserPayment.Receipt_S3_Path = receiptS3Path;
                    dbUserPayment.Update_Time_Stamp = DateTime.Now;
                    _entities.SaveChanges();
                }
            }
        }

        #endregion

        #region Search Email By Product

        public List<string> SearchEmailByProduct(string emailAddress, int project)
        {
            List<string> emails = new List<string>();
            using (SqlConnection sqlConnection = new SqlConnection())
            {
                string qry = string.Empty;
                if (project == (int)Project.ExpressTruckTax)
                {
                    sqlConnection.ConnectionString = Utilities.DataUtility.GetAppSettings("ETTConnStr");
                    qry = "SELECT TOP 5 convert(varchar(100),USER_ID)+'|'+EMAIL_ADDRESS+'|'+Contact_Name +'|' + Phone_Number FROM EE_USERS WHERE EMAIL_ADDRESS LIKE '%" + emailAddress + "%' AND IS_DELETED=0";
                }
                if (project == (int)Project.ExpressTaxFilings)
                {
                    sqlConnection.ConnectionString = Utilities.DataUtility.GetAppSettings("TBSConnStr");
                    qry = "SELECT TOP 5 convert(varchar(50),USER_ID)+'|'+EMAIL_ADDRESS+'|'+Contact_Name +'|' + Phone_Number FROM ETF_HUB_USERS WHERE EMAIL_ADDRESS LIKE '%" + emailAddress + "%' AND IS_DELETED=0";

                }
                if (!string.IsNullOrWhiteSpace(sqlConnection.ConnectionString))
                {
                    sqlConnection.Open();
                    emails = sqlConnection.Query<string>(qry).ToList();
                    sqlConnection.Close();
                }
            }
            return emails;
        }

        #endregion

        #region Save Void Refund
        public void SaveVoidRefund(PaymentRefundLog voidRefundRequest, string connectionString)
        {
            var query = @"insert into span_payment_refund_log(Product_Id,Payment_Processor_Id,Paid_Amount,Refund_Amount,Comments,Email_Address,Paid_Date,Refund_Void_Date,Admin_User_Name,Transaction_Reference_Id,Charge_Back_Type) values(@Product_Id,@Payment_Processor_Id,@Paid_Amount,@Refund_Amount,@Comments,@Email_Address,@Paid_Date,@Refund_Void_Date,@Admin_User_Name,@TransactionReferenceId,@ChargeBackType)";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Query(query, new { @Product_Id = voidRefundRequest.SpanProductId, @Payment_Processor_Id = voidRefundRequest.PaymentProcessorId, @Paid_Amount = voidRefundRequest.PaidAmount, @Refund_Amount = voidRefundRequest.RefundAmount, @Comments = voidRefundRequest.Comments, @Email_Address = voidRefundRequest.EmailAddress, @Paid_Date = voidRefundRequest.PaidDate, @Refund_Void_Date = voidRefundRequest.VoidRefundDate, @Admin_User_Name = voidRefundRequest.AdminUserName, @TransactionReferenceId = voidRefundRequest.TransactionReferenceId, @ChargeBackType = voidRefundRequest.ChargeBackType });
            }
            if (voidRefundRequest.ChargeBackType == "VOID")
            {
                var deleteTransactionDetailQuery = @"update span_api_request_response_log set Api_Call_status='Voided' where transaction_reference_id = '" + voidRefundRequest.TransactionReferenceId + "'";
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Query(deleteTransactionDetailQuery);
                }
            }
        }
        #endregion

        #region Get Transaction Report Details
        public List<PaymentRefundLog> GetPaymentRefundDetails(PaymentRefundLog transactionReport, string connectionString)
        {
            List<PaymentRefundLog> transactionReportDetails = new List<PaymentRefundLog>();
            var query = @"select Payment_Refund_Id RefundId,Product_Id ProductId,Payment_Processor_Id PaymentProcessorId,Paid_Amount PaidAmount,Refund_Amount RefundAmount,Comments,Email_Address EmailAddress,Paid_Date PaidDate,Refund_Void_Date VoidRefundDate,Admin_User_Name AdminUserName,Transaction_Reference_Id TransactionReferenceId,Charge_Back_Type ChargeBackType from SPAN_PAYMENT_REFUND_LOG WHERE REFUND_VOID_date <= @EndDate and REFUND_VOID_date >= @BeginDate and Product_Id = @ProductId and Is_Deleted = 0";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                transactionReportDetails = con.Query<PaymentRefundLog>(query, new { @BeginDate = transactionReport.BeginDate, @EndDate = transactionReport.EndDate, @ProductId = transactionReport.SpanProductId }).ToList();
            }
            return transactionReportDetails;
        }
        #endregion

        #region Get total refund amount 

        public decimal GetRefundAmount(PaymentRefundLog transactionReport, string connectionString, string chargeBackType)
        {
            decimal amount = 0;
            var query = @"select SUM(REFUND_AMOUNT) from SPAN_PAYMENT_REFUND_LOG WHERE Charge_Back_Type ='" + chargeBackType + "' AND REFUND_VOID_date <= @EndDate and REFUND_VOID_date >= @BeginDate and Product_Id = @ProductId  and Is_Deleted = 0";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                amount = con.Query<decimal?>(query, new { @BeginDate = transactionReport.BeginDate, @EndDate = transactionReport.EndDate, @ProductId = transactionReport.SpanProductId }).SingleOrDefault() ?? 0;
            }
            return amount;
        }
        #endregion

        #region GetTransactionReport
        public TransactionReportDetails GetTransactionDetail(string transactionId, string connectionString)
        {
            var query = "SELECT P.Request_Response_S3_Path ResponseS3FilePath,P.Span_Payment_Processor_Id PaymentProcessorType,Span_Product_Id ProductId,Api_Call_Status ApiCallStatus,Transaction_Amount OrderAmount,Transaction_Reference_Id TransactionReferenceId,Email_Address EmailAddress,Update_Time_stamp PaymentDate FROM SPAN_API_REQUEST_RESPONSE_LOG P WHERE P.Transaction_Reference_Id = '" + transactionId + "' AND P.Is_deleted = 0";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                TransactionReportDetails logFiles = con.Query<TransactionReportDetails>(query).SingleOrDefault();
                return logFiles;
            }
        }
        #endregion

        #region Get Campaign Assigned details
        /// <summary>
        /// Get Campaign Assigned details
        /// </summary>
        /// <param name="campaignDetailId"></param>
        /// <param name="supportUserId"></param>
        /// <param name="campaignAssignedDetailId"></param>
        /// <returns></returns>
        public LeadDetails GetCampaignAssignedDetails(long campaignDetailId, long supportUserId, long campaignAssignedDetailId)
        {
            LeadDetails leaddetails = new LeadDetails();

            if (campaignDetailId > 0 && supportUserId > 0 && campaignAssignedDetailId > 0)
            {
                var _dbLeadDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                      where a.Campaign_Details_Id == campaignDetailId && !a.Is_Deleted && a.Support_User_Id == supportUserId && a.Campaign_Assigned_Details_Id == campaignAssignedDetailId
                                      select a).SingleOrDefault();

                if (_dbLeadDetails != null)
                {
                    leaddetails.Email = _dbLeadDetails.Email_Address;
                }
            }
            return leaddetails;
        }
        #endregion

        #region Get AdminUser Details By AdminUserId 
        /// <summary>
        /// Get AdminUser Details By AdminUserId 
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <returns></returns>
        public AdminUser GetAdminUserUthenticatorDetailsByAdminUserId(long adminUserId)
        {
            AdminUser adminUser = new AdminUser();
            if (adminUserId > 0)
            {
                var details = (from a in _context.Biz_Admin_Users
                              where a.Admin_User_Id == adminUserId && a.Is_Deleted == false
                              select a).SingleOrDefault();
                if (details != null)
                {
                    adminUser.IsEnabledAuthenticator = details.Is_Enabled_Authenticator != null ? details.Is_Enabled_Authenticator : false;
                    adminUser.EmailAddress = details.Admin_Email_Address;
                }
            }
            return adminUser;
        }
        #endregion

        #region Update Authentication For AdminUser
        /// <summary>
        /// Update Authentication For AdminUser
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <returns></returns>
        public bool UpdateAuthenticationForAdminUser(long adminUserId)
        {
            bool isAuthenticationEnabled = false;
            if (adminUserId > 0)
            {
                using (var entities = new MainControlDB_Entities())
                {
                    var dbAUthenticationEnabled = entities.Biz_Admin_Users.SingleOrDefault(bau => bau.Admin_User_Id == adminUserId && !bau.Is_Deleted);
                    if (dbAUthenticationEnabled != null && dbAUthenticationEnabled.Admin_User_Id > 0)
                    {
                        dbAUthenticationEnabled.Is_Enabled_Authenticator = true;
                        dbAUthenticationEnabled.Update_Time_Stamp = DateTime.Now;
                        entities.SaveChanges();
                        isAuthenticationEnabled = true;
                    }
                }
            }
            return isAuthenticationEnabled;
        }
        #endregion

        #region Reset Authentication
        /// <summary>
        /// Reset Authentication
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool ResetAuthentication(long Id)
        {
            bool _isUpdateStatus = false;
            string team = AdminRoleType.Team.ToString();
            if (Id > 0)
            {
                using (var entities = new MainControlDB_Entities())
                {
                    var dbAUthenticationEnabled = entities.Biz_Admin_Users.SingleOrDefault(bau => bau.Admin_User_Id == Id && !bau.Is_Deleted);
                    if (dbAUthenticationEnabled != null && dbAUthenticationEnabled.Admin_User_Id > 0)
                    {
                        dbAUthenticationEnabled.Is_Enabled_Authenticator = false;
                        dbAUthenticationEnabled.Update_Time_Stamp = DateTime.Now;
                        entities.SaveChanges();
                        _isUpdateStatus = true;
                    }
                }
            }
            return _isUpdateStatus;
        }
        #endregion

        #region Get App Settings
        /// <summary>
        /// Get the appsetting value from the web.config
        /// </summary>
        /// <param name="appKey">appsettings key</param>
        /// <returns></returns>
        public static string GetAppSettings(string appKey)
        {
            object value = null;
            value = ConfigurationManager.AppSettings[appKey];
            if (value != null)
            {
                return value.ToString();
            }
            return string.Empty;
        }
        #endregion

        #region Get App Reconcilation Payment
        public List<ReturnCountReport> GetAppReconcilationPayment(DateTime BeginDate, DateTime EndDate, int ProductId)
        {
            List<ReturnCountReport> returnCountReport = new List<ReturnCountReport>();
            string connectionString = string.Empty;

            if (ProductId == Constants.ProductIdLive || ProductId == Constants.ProductIdSprint)
            {
                connectionString = GetAppSettings(Constants.TBSAppConnStr);
            }
            else if (ProductId == Constants.EEProductIdLive || ProductId == Constants.EEProductIdSprint)
            {
                connectionString = GetAppSettings(Constants.EEConnStr);

            }
            else
            {
                connectionString = GetAppSettings(Constants.ETTConnStr);
            }
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                if (ProductId == Constants.ProductIdLive || ProductId == Constants.ProductIdSprint)
                {
                    returnCountReport = con.Query<ReturnCountReport>(SQLQueryConstants.GetAppReconcilationPayment, new { @CurrentDateTimeStart = BeginDate, @CurrentDateTimeEnd = EndDate }).ToList();
                }
                else if (ProductId == Constants.EEProductIdLive || ProductId == Constants.EEProductIdSprint)
                {
                    returnCountReport = con.Query<ReturnCountReport>(SQLQueryConstants.GetEEAppReconcilationPayment, new { @CurrentDateTimeStart = BeginDate, @CurrentDateTimeEnd = EndDate }).ToList();
                }
                else
                {
                    returnCountReport = con.Query<ReturnCountReport>(SQLQueryConstants.GetETTAppReconcilationPayment, new { @CurrentDateTimeStart = BeginDate, @CurrentDateTimeEnd = EndDate }).ToList();
                }
                if (returnCountReport != null)
                {
                    foreach (var item in returnCountReport)
                    {
                        item.AppTxnId = item.AppTxnId.Replace("-", "");
                    }
                }
                
            }
            return returnCountReport;
        }
        #endregion

        #region Get API Reconcilation Payment
        public List<ReturnCountReport> GetAPIReconcilationPayment(DateTime BeginDate, DateTime EndDate, int ProductId)
        {
            List<ReturnCountReport> returnCountReport = new List<ReturnCountReport>();
            string connectionString = GetAppSettings(Constants.PaymentLibConnStr);

            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                returnCountReport = conn.Query<ReturnCountReport>(SQLQueryConstants.GetAPIReconcilationPayment, new { @CurrentDateTimeStart = BeginDate, @CurrentDateTimeEnd = EndDate, @ProductId = ProductId }).ToList();

                foreach (var item in returnCountReport)
                {
                    item.ApiTxnId = item.ApiTxnId.Replace("-", "");
                }

            }
            return returnCountReport;
        }
        #endregion


        #region Get Charge Back Type In Payment Refund Log
        public ReturnCountReport GetChargeBackTypeInPaymentRefundLog(string TransactionRefId)
        {
            ReturnCountReport returnCountReport = new ReturnCountReport();
            string connectionString = GetAppSettings(Constants.PaymentLibConnStr);
            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                returnCountReport = conn.Query<ReturnCountReport>(SQLQueryConstants.GetChargeBackTypeInPaymentRefundLog, new { @TransactionRefId = TransactionRefId }).SingleOrDefault();
            }

            return returnCountReport;
        }

        #endregion

        #region Get All Stripe Credentials
        public List<StripeCredential> GetAllStripeCredentials()
        {
            List<StripeCredential> lstdbAuthCredentials = new List<StripeCredential>();
            string spanlibconn = GetAppSettings(Constants.PaymentLibConnStr);
            var query = @" SELECT sp.Span_Payment_Processor_Id StripeProcessorId, sp.Stripe_Secret_Key StripeLoginKey,p.Product_Code ProductCode  from Span_Product_Payment_Processor sp
                                           join Span_Products p
                                           on sp.Span_Product_Id = p.Span_Product_Id
                                           where sp.Is_Deleted = 0 and p.Is_deleted=0 and sp.Stripe_Secret_Key is not null and sp.Stripe_Secret_Key !=''";
            using (SqlConnection con = new SqlConnection(spanlibconn))
            {
                //get stripe report credentials with productCode
                lstdbAuthCredentials = con.Query<StripeCredential>(query).ToList();
            }
            return lstdbAuthCredentials;
        }
        #endregion

        #region Update Campaign Assigned Details Return Number By Campaign Assigned Details Id
        public bool UpdateCampaignAssignedDetailsReturnNumberByCampaignAssignedDetailsId(long campaignAssignedDetailsId, long returnNumber)
        {
            bool isUpdated = false;

            var _dbCampaignAssignedDetailsDetails = (from a in _context.Biz_Campaign_Assigned_Details
                                                     where a.Campaign_Assigned_Details_Id == campaignAssignedDetailsId && !a.Is_Deleted
                                                     select a).SingleOrDefault();
            if (_dbCampaignAssignedDetailsDetails != null)
            {
                _dbCampaignAssignedDetailsDetails.ReturnNumber = returnNumber;
                _dbCampaignAssignedDetailsDetails.Updated_Time_Stamp = DateTime.Now;
                _context.SaveChanges();
                isUpdated = true;
            }
            return isUpdated;
        }
        #endregion
    }
}
