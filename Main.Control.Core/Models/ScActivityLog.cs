using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Main.Control.Core.Models
{
    [Serializable()]
    public class ScActivityLog : SPEntityBase
    {
        [XmlAttribute()]
        public long ActivityLogId { get; set; }
        [XmlAttribute()]
        public long ProjectId { get; set; }
        [XmlAttribute()]
        public string ActionType { get; set; }
        [XmlAttribute()]
        public string DisplayName { get; set; }
        [XmlAttribute()]
        public string ControllerName { get; set; }
        [XmlAttribute()]
        public string ActionName { get; set; }
        [XmlAttribute()]
        public string IPAddress { get; set; }
        [XmlAttribute()]
        public string Activity { get; set; }
        [XmlAttribute()]
        public string Memo { get; set; }
        [XmlAttribute()]
        public DateTime ActivityDate { get; set; }
        [XmlAttribute()]
        public string ProjectName { get; set; }
    }
}
