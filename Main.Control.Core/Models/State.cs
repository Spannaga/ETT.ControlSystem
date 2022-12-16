using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Main.Control.Core.Models
{
   public class State
    {
        [XmlAttribute()]
        public int StateId { get; set; }

        [XmlAttribute()]
        public long CountryId { get; set; }

        [XmlAttribute()]
        public string StateName { get; set; }

        [XmlAttribute()]
        public string StateCode { get; set; }

        [XmlAttribute()]
        public string TimeZone { get; set; }

        [XmlAttribute()]
        public string StateNoValid { get; set; }

        [XmlAttribute()]
        public string StateNoError { get; set; }

    }
}
