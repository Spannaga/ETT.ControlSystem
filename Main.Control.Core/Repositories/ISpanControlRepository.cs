using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Control.Core.Models;

namespace Main.Control.Core.Repositories
{
    public interface ISpanControlRepository
    {
        AdminUser GetAdminUserById(long Id);
        AdminUser GetAdminUserByIdAndProduct(long Id, long productId);
        AdminUser GetAdminUserByIdETF(long Id, long productId);

        AdminUser GetAdminDetailsByUserName(AdminUser adminUser);
        AdminUser GetAdminDetailsByUserNameAndProduct(AdminUser adminUser, long projectId);
        List<AdminUserRole> GetAllAdminProjectRole(long userId);
        string GetAdminRoleByProjectIdAndAdminUserId(long projectId, long adminUserId);
        string GetAdminUserNameByUserId(long userId);
        List<AdminUser> GetAdminUserNameList();

        AdminUser GetUserDetailsByUserName(string userName);
        List<AdminIpAddress> GetAllValidIpAddress();

    }
}
