using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    [Serializable()]
    [DataContract]
    public class UserProfileComments
    {
        [DataMember]
        public long UserCommentsId { get; set; }
        [DataMember]
        public int UserProfileId { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public string Comments { get; set; }
        [DataMember]
        public string AdminUserName { get; set; }
        [DataMember]
        public DateTime CreateTimeStamp { get; set; }
        [DataMember]
        public DateTime UpdateTimeStamp { get; set; }
    }
}
