using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Main.Control.Core.Models
{
    public class CookieSessionItems
    {
        //Admin
        public string AdminUserId { get; set; }
        public string AdminUserName { get; set; }
        public string AdminRole { get; set; }
        public string RememberMe { get; set; }
        public string AdminSKUType { get; set; }
        public string ProjectType { get; set; }
        public string IsAdmin { get; set; }
        public string AdminDisplayName { get; set; }
        public List<string> IpAddress { get; set; }
        public string AdminEmailAddress { get; set; }
    }
}
