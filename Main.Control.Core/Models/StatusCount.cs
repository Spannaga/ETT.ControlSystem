using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Main.Control.Core.Models
{
    [Serializable()]
    public class StatusCount
    {

        [XmlAttribute()]
        public string Status { get; set; }
        [XmlAttribute()]
        public int Count { get; set; }
    }
}
