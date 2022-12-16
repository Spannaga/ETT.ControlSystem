using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Main.Control.Core.Models;

namespace Main.Control.Web.Models
{
    public class AdminUserRoleUI
    {
        public List<AdminRole> AdminRoleList { get; set; }
        public List<AdminUserRole> AdminUserRoleList { get; set; }
        public List<AdminCategory> AdminCategoryList { get; set; }


        public List<AdminProject> AdminProjectList { get; set; }
        public long AdminUserId { get; set; }
        public long ProjectId { get; set; }
        public long Roles { get; set; }
        public long AdminCategoryId { get; set; }
        public long AdminUserRoleId { get; set; }
    }
}