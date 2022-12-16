using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class AdditionalContacts
    {
        public long AdditionalContactsDetailsId { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string ContactEmailAddress { get; set; }
        public string ContactPhone { get; set; }
        public long CampaignAssignedDetailsId { get; set; }
        public StatusType StatusType { get; set; }
    }
}
