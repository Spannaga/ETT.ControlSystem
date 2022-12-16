using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Main.Control.Core.Models
{
   public class SPAddress
    {
        [XmlAttribute()]
        public string Address1 { get; set; }

        [XmlAttribute()]
        public string Address2 { get; set; }

        [XmlAttribute()]
        public string City { get; set; }

        [XmlAttribute()]
        public string State { get; set; }

        [XmlAttribute()]
        public string Country { get; set; }

        [XmlAttribute()]
        public string Zip { get; set; }

        public long CountryId { get; set; }

        public string PhoneNumber { get; set; }
    }
}
