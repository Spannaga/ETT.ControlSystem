using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Main.Control.Core.Models
{
    [Serializable()]
   public class AdminUserRole
    {
        [XmlAttribute()]
        public long AdminRoleId { get; set; }
        [XmlAttribute()]
        public long AdminUserId { get; set; }
        [XmlAttribute()]
        public long AdminProjectId { get; set; }

        [XmlAttribute()]
        public string ProjectName { get; set; }

        [XmlAttribute()]
        public string AdminUserName { get; set; }

        [XmlAttribute()]
        public string Status { get; set; }
        [XmlAttribute()]
        public long AdminUserRoleId { get; set; }
        [XmlAttribute()]
        public string Role { get; set; }

        [XmlAttribute()]
        public long AdminCategoryId { get; set; }

    }

    public class AdminCategory
    {
        [XmlAttribute()]
        public long AdminCategoryId { get; set; }

        [XmlAttribute()]
        public string AdminCategoryName { get; set; }

    }
}
