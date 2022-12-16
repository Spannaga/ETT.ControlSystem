using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Control.Core.Repositories;
using Main.Control.Core.Models;

namespace Main.Control.Resources
{
    public class SpanControlRepository : ISpanControlRepository
    {
        #region Declaration
        MainControlDB_Entities _entities = new MainControlDB_Entities();
        #endregion

        #region ISpanControlRepository Members

        #region Get Admin Details By Id
        /// <summary>
        /// Get Admin Details By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public AdminUser GetAdminUserById(long Id)
        {
            AdminUser _adminUser = new AdminUser();
            var _dbAdminUser = (from a in _entities.Biz_Admin_Users
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
                //_adminUser.IsApproved = _dbAdminUser.Is_Approved ?? true;
                _adminUser.IsAdmin = CheckForAdminRole(_dbAdminUser.Admin_User_Id);
            }

            return _adminUser;
        }
        #endregion

        #region Get Admin Details By Id
        /// <summary>
        /// Get Admin Details By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public AdminUser GetAdminUserByIdAndProduct(long Id, long projectId)
        {
            AdminUser _adminUser = new AdminUser();

            var _dbAdminUser = (from a in _entities.Biz_Admin_Users
                                join b in _entities.Biz_Admin_User_Roles
                                on a.Admin_User_Id equals b.Admin_User_Id
                                where a.Admin_User_Id == Id && b.Admin_Project_Id == projectId
                                && !a.Is_Deleted && !b.Is_Deleted
                                select new { a, b.Admin_Role_Id }).SingleOrDefault();

            if (_dbAdminUser != null && _dbAdminUser.a.Admin_User_Id > 0)
            {
                _adminUser.AdminUserId = _dbAdminUser.a.Admin_User_Id;
                _adminUser.AdminUserName = _dbAdminUser.a.Admin_User_Name;
                _adminUser.AdminFirstName = _dbAdminUser.a.Admin_First_Name;
                _adminUser.AdminLastName = _dbAdminUser.a.Admin_Last_Name;
                _adminUser.AdminEmailAddress = _dbAdminUser.a.Admin_Email_Address;
                _adminUser.AdminRoleId = _dbAdminUser.Admin_Role_Id > 0 ? _dbAdminUser.Admin_Role_Id : _dbAdminUser.a.Admin_Role;
                _adminUser.AdminRoles = GetAdminRoleById(_adminUser.AdminRoleId);
                _adminUser.ProjectType = _dbAdminUser.a.Projects;
                //_adminUser.IsApproved = _dbAdminUser.Is_Approved ?? true;
                _adminUser.IsAdmin = CheckForAdminRole(_dbAdminUser.a.Admin_User_Id);
            }

            return _adminUser;
        }
        #endregion

        #region Get Admin Details By Id
        /// <summary>
        /// Get Admin Details By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public AdminUser GetAdminUserByIdETF(long Id, long projectId)
        {
            AdminUser _adminUser = new AdminUser();
            var _dbAdminUser = (from a in _entities.Biz_Admin_Users
                                join b in _entities.Biz_Admin_User_Roles
                              on a.Admin_User_Id equals b.Admin_User_Id
                                where a.Admin_User_Id == Id && !a.Is_Deleted && !b.Is_Deleted
                                 && b.Admin_Project_Id == projectId
                                select new
                                {
                                    a,
                                    b.Admin_Role_Id,
                                    b.Admin_Project_Id
                                }).SingleOrDefault();

            if (_dbAdminUser != null && _dbAdminUser.a.Admin_User_Id > 0)
            {
                _adminUser.AdminUserId = _dbAdminUser.a.Admin_User_Id;
                _adminUser.AdminUserName = _dbAdminUser.a.Admin_User_Name;
                _adminUser.AdminFirstName = _dbAdminUser.a.Admin_First_Name;
                _adminUser.AdminLastName = _dbAdminUser.a.Admin_Last_Name;
                _adminUser.AdminEmailAddress = _dbAdminUser.a.Admin_Email_Address;
                _adminUser.AdminRoleId = _dbAdminUser.a.Admin_Role;
                _adminUser.AdminRoles = GetAdminRoleById(_dbAdminUser.Admin_Role_Id);
                _adminUser.ProjectType = _dbAdminUser.a.Projects;
                //_adminUser.IsApproved = _dbAdminUser.Is_Approved ?? true;
                _adminUser.IsAdmin = CheckForAdminRole(_dbAdminUser.a.Admin_User_Id);
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
                var _dbadmin = (from a in _entities.Biz_Admin_Users
                                where a.Admin_User_Name == adminUser.AdminUserName && (a.Is_Approved == true || a.Is_Approved == null) && !a.Is_Deleted
                                select new
                                {
                                    a.Admin_User_Id,
                                    a.Admin_User_Name,
                                    a.Admin_Password,
                                    a.Admin_Salt,
                                    a.Admin_Role,
                                    a.Projects
                                }).SingleOrDefault();

                if (_dbadmin != null)
                {
                    //Assign the admin details to admin model entity from the above _dbadmin object
                    adminUser.AdminUserName = _dbadmin.Admin_User_Name;
                    adminUser.AdminPassword = _dbadmin.Admin_Password;
                    string _adminRole = GetAdminRoleById(_dbadmin.Admin_Role);
                    adminUser.AdminRoles = _adminRole;
                    adminUser.AdminSalt = _dbadmin.Admin_Salt;
                    adminUser.AdminUserId = _dbadmin.Admin_User_Id;
                    adminUser.ProjectType = _dbadmin.Projects;
                    adminUser.IsAdmin = CheckForAdminRole(_dbadmin.Admin_User_Id);
                    adminUser.AdminSKUType = _adminRole;
                }
            }
            //return the admin details.
            return adminUser;

        }
        #endregion

        #region
        private bool CheckForAdminRole(long _adminUserId)
        {
            var _dbAdminRoleList = (from a in _entities.Biz_Admin_User_Roles
                                    where !a.Is_Deleted && a.Admin_User_Id == _adminUserId && a.Admin_Role_Id == 1
                                    select a).ToList();


            if (_dbAdminRoleList != null && _dbAdminRoleList.Count > 0)
            {
                return true;
            }
            return false;
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

            var _dbAdminRole = (from a in _entities.Biz_Admin_Roles
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

            var _dbAdminRole = (from a in _entities.Biz_Admin_User_Roles
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

        #region Get All Admin Project Role
        public List<AdminUserRole> GetAllAdminProjectRole(long userId)
        {
            //var _dbAdminProjectRoleList = (from a in _context.Biz_Admin_User_Roles
            //                               join b in _context.Static_Biz_Admin_Project on a.Admin_Project_Id equals b.Admin_Project_Id
            //                               join c in _context.Biz_Admin_Roles on a.Admin_Role_Id equals c.Admin_Role_Id
            //                               join d in _context.Biz_Admin_Users on a.Admin_User_Id equals d.Admin_User_Id
            //                               where !a.Is_Deleted  && !b.Is_Deleted && !c.Is_Deleted && !d.Is_Deleted && a.Admin_User_Id == userId
            //                               select new {

            //                                   a.Admin_User_Id,
            //                                   a.Admin_Role_Id,a.Admin_User_Role_Id,
            //                                   a.Admin_Project_Id,
            //                                   b.Project_Name,
            //                                   c.Admin_Role,
            //                                   d.Admin_User_Name
            //                               });



            var _dbAdminProjectRoleList = (from a in _entities.Biz_Admin_User_Roles
                                           join b in _entities.Static_Biz_Admin_Project on a.Admin_Project_Id equals b.Project_Id
                                           join c in _entities.Biz_Admin_Roles on a.Admin_Role_Id equals c.Admin_Role_Id
                                           join d in _entities.Biz_Admin_Users on a.Admin_User_Id equals d.Admin_User_Id

                                           join e in _entities.Static_Biz_Admin_Category_Project on a.Admin_Project_Id equals e.Project_Id


                                           where !a.Is_Deleted && a.Admin_User_Id == userId
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
                                           });
            List<AdminUserRole> lstAdminRole = new List<AdminUserRole>();
            foreach (var temp in _dbAdminProjectRoleList)
            {
                AdminUserRole _adminRole = new AdminUserRole();
                _adminRole.AdminUserRoleId = temp.Admin_User_Role_Id;
                _adminRole.AdminUserId = temp.Admin_User_Id;
                _adminRole.AdminRoleId = temp.Admin_Role_Id;
                _adminRole.AdminProjectId = temp.Admin_Project_Id;
                _adminRole.ProjectName = temp.Project_Name;
                _adminRole.Role = temp.Admin_Role;
                _adminRole.AdminUserName = temp.Admin_User_Name;
                _adminRole.AdminCategoryId = temp.Category_Id ?? 0;

                lstAdminRole.Add(_adminRole);
            }
            return lstAdminRole;
        }
        #endregion

        #region get admin user name by admin user id
        public string GetAdminUserNameByUserId(long _userID)
        {
            string _AdminName = string.Empty;

            var _dbAdminName = (from a in _entities.Biz_Admin_Users
                                where a.Admin_User_Id == _userID && !a.Is_Deleted
                                select new
                                {
                                    a.Admin_First_Name,
                                    a.Admin_Last_Name
                                }).SingleOrDefault();
            if (_dbAdminName != null)
            {

                _AdminName = _dbAdminName.Admin_First_Name + " " + _dbAdminName.Admin_Last_Name;
            }
            return _AdminName;
        }
        #endregion

        #region Get Admin Details By Admin User Name
        /// <summary>
        /// Get Admin Details By AdminUserName
        /// </summary>
        /// <param name="admin"></param>
        /// <returns>Admin Details</returns>
        public AdminUser GetAdminDetailsByUserNameAndProduct(AdminUser adminUser, long projectId)
        {
            if (adminUser.AdminUserName != null)
            {
                //Get the Admin Details based on the AdminUserName         
                var _dbadmin = (from a in _entities.Biz_Admin_Users
                                join b in _entities.Biz_Admin_User_Roles
                               on a.Admin_User_Id equals b.Admin_User_Id
                                where a.Admin_User_Name == adminUser.AdminUserName && (a.Is_Approved == true || a.Is_Approved == null) && !a.Is_Deleted && !b.Is_Deleted
                                && b.Admin_Project_Id == projectId
                                select new
                                {
                                    a.Admin_User_Id,
                                    a.Admin_User_Name,
                                    a.Admin_First_Name,
                                    a.Admin_Last_Name,
                                    a.Admin_Password,
                                    a.Admin_Salt,
                                    a.Admin_Role,
                                    a.Projects,
                                    b.Admin_Role_Id,
                                    b.Admin_Project_Id,
                                    a.Admin_Email_Address
                                }).SingleOrDefault();

                if (_dbadmin != null)
                {
                    //Assign the admin details to admin model entity from the above _dbadmin object
                    adminUser.AdminUserName = _dbadmin.Admin_User_Name;
                    adminUser.AdminPassword = _dbadmin.Admin_Password;
                    string _adminRole = GetAdminRoleById(_dbadmin.Admin_Role_Id);
                    adminUser.AdminRoles = _adminRole;
                    adminUser.AdminSalt = _dbadmin.Admin_Salt;
                    adminUser.AdminUserId = _dbadmin.Admin_User_Id;
                    adminUser.ProjectType = _dbadmin.Projects;
                    adminUser.IsAdmin = CheckForAdminRole(_dbadmin.Admin_User_Id);
                    adminUser.AdminSKUType = _adminRole;
                    adminUser.AdminFirstName = _dbadmin.Admin_First_Name;
                    adminUser.AdminLastName = _dbadmin.Admin_Last_Name;
                    adminUser.AdminEmailAddress = _dbadmin.Admin_Email_Address;
                }
            }
            //return the admin details.
            return adminUser;

        }

        #endregion

        #region Get Admin UserName List
        public List<AdminUser> GetAdminUserNameList()
        {
            List<AdminUser> _AdminUserNameList = new List<AdminUser>();

            var _dbAdminUser = (from a in _entities.Biz_Admin_Users
                                where !a.Is_Deleted
                                select new { a.Admin_User_Id, a.Admin_First_Name, a.Admin_Last_Name }).ToList();

            if (_dbAdminUser != null && _dbAdminUser.Count > 0)
            {
                foreach (var item in _dbAdminUser)
                {

                    AdminUser _adminUser = new AdminUser();
                    _adminUser.AdminUserId = item.Admin_User_Id;
                    _adminUser.AdminUserName = item.Admin_First_Name + " " + item.Admin_Last_Name;
                    _AdminUserNameList.Add(_adminUser);

                }

            }
            return _AdminUserNameList;
        }

        #endregion


        #endregion

        #region Get User Details By Email Address

        public AdminUser GetUserDetailsByUserName(string userName)
        {
            var _dbUser = (from u in _entities.Biz_Admin_Users
                           where u.Admin_User_Name == userName && !u.Is_Deleted
                           select new
                           {
                               u.Admin_User_Id,
                               u.Admin_Email_Address,
                               u.Admin_Password,
                           }).SingleOrDefault();

            var _adminUser = new AdminUser();
            if (_dbUser != null)
            {
                _adminUser.AdminUserId = _dbUser.Admin_User_Id;
                _adminUser.AdminEmailAddress = _dbUser.Admin_Email_Address;
                _adminUser.AdminPassword = _dbUser.Admin_Password;


            }
            return _adminUser;
        }

        #endregion

        #region Get All Valid Ip Address
        /// <summary>
        /// GetAllValidIpAddress
        /// </summary>
        /// <returns></returns>
        public List<AdminIpAddress> GetAllValidIpAddress()
        {
            return (from a in _entities.Biz_Ip_Address
                    where !a.Is_Deleted
                    select new AdminIpAddress
                    {
                        ProjectId = a.Project_id,
                        IpAddress = a.Ip_Address
                    }).ToList();
        }
        #endregion
    }
}
