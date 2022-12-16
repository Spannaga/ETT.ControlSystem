using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Main.Control.Core.Models
{
    public class SPSKU
    {
        [XmlAttribute()]
        public int SKUId { get; set; }
        [XmlAttribute()]
        public string SKUName { get; set; }
        [XmlAttribute()]
        public string SKUDescription { get; set; }
        [XmlAttribute()]
        public string SKUNote { get; set; }
        [XmlAttribute()]
        public string ResourceMessageField { get; set; }
        [XmlAttribute()]
        public SKUType SKUType { get; set; }
        [XmlAttribute()]
        public decimal BasePrice { get; set; }
        [XmlAttribute()]
        public decimal QuarterlyPrice { get; set; }
        [XmlAttribute()]
        public decimal HalfYearlyPrice { get; set; }
        [XmlAttribute()]
        public decimal YearlyPrice { get; set; }
        [XmlAttribute()]
        public bool IsTrail { get; set; }
        [XmlAttribute()]
        public long OrderId { get; set; }
        [XmlAttribute()]
        public DateTime NextDueDate { get; set; }
    }
}
