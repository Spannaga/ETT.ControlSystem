using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class SpancontrolContactGroup
    {
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public long GroupNameId { get; set; }
        public List<SpancontrolContactGroup> GetGroupsList { get; set; }
        public List<GroupMembers> GetGroupMembersList { get; set; }
        public bool IsAffiliateGroup { get; set; }
        public object Group_Id { get; set; }
    }
}
