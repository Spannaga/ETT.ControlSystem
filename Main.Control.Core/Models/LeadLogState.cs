using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Main.Control.Core.Models
{
    [Serializable()]
    public class LeadLogState
    {
        [XmlAttribute]
        public long StateId { get; set; }
        [XmlAttribute]
        public string StateName { get; set; }
        [XmlAttribute()]
        public long CountryId { get; set; }
        [XmlAttribute()]
        public string StateCode { get; set; }
        [XmlAttribute()]
        public bool IgnoreCatchBlock { get; set; }
        [XmlAttribute]
        public long PositionId { get; set; }
        [XmlAttribute]
        public string PositionName { get; set; }
        [XmlAttribute()]
        public string CountryName { get; set; }
        [XmlAttribute()]
        public string CountryCode { get; set; }
        [XmlAttribute()]
        public bool IsStateAvailable { get; set; }
    }
}
