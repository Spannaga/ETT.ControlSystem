using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    [DataContract]
    public class GroupMembers
    {
        [DataMember]
        public long GroupMemberId { get; set; }
        [DataMember]
        public long GroupNameId { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public string Contact_Name { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public string PhoneNumber { get; set; }
        [DataMember]
        public DateTime Updated_Time_Stamp { get; set; }
        [DataMember]
        public string lastActivity { get; set; }
        [DataMember]
        public int CheckedOrUnchecked { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
    }
}
