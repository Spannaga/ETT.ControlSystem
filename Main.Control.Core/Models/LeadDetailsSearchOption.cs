using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class LeadDetailsSearchOption
    {
        public long CampaignDetailId { get; set; }
        public long SupportUserId { get; set; }
        public string StateCode { get; set; }
        public long LeadId { get; set; }
        public bool Skipped { get; set; }


    }
}
