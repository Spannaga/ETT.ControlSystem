using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Control.Core.Models;

namespace Main.Control.Core.Services
{
    public interface ISpanControlService
    {
        AdminUser GetAdminUserById(long Id);
        AdminUser GetAdminUserByIdAndProduct(long Id, long productId);
        AdminUser GetAdminUserByIdETF(long Id, long productId);
        AdminUser AdminSignIn(AdminUser admin);
        AdminUser AdminSignInETF(AdminUser admin);
        List<AdminUserRole> GetAllAdminProjectRole(long userId);
        string GetAdminRoleByProjectIdAndAdminUserId(long projectId, long adminUserId);
        string GetAdminUserNameByUserId(long userId);
        AdminUser AdminSignInETL(AdminUser admin);

        AdminUser AdminSignInE990(AdminUser admin);
        AdminUser AdminSignInEXTN(AdminUser admin);
        AdminUser AdminSignInSTE(AdminUser admin);

        AdminUser AdminSignInTSNA(AdminUser admin);

        List<AdminUser> GetAdminUserNameList();

        AdminUser ValidateUser(AdminUser pTaxUser);
        AdminUser GetUserDetailsByUserName(string userName);
        List<string> GetAllValidIpAddress(long projectId);
        void RemoveProjectCache(string key);

    }
}
