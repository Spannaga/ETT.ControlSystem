using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Main.Control.Core.Models
{
    [Serializable()]

    public enum LeadStatusDetails
    {
        Lead = 1,
        Interested = 2,
        InNegotiation = 3,
        Client = 4,
        NotInterested = 5,
        Inactive=6
    }

    public class LeadStatus
    {
        [XmlAttribute()]
        public string Status { get; set; }
        [XmlAttribute()]
        public int Count { get; set; }

    }

  

}
