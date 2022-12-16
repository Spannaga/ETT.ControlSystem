using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Main.Control.Core.Models
{
   public class SPModules
    {

        [XmlAttribute()]
        public long ModuleId { get; set; }
        [XmlAttribute()]
        public string ModuleName { get; set; }
        [XmlAttribute()]
        public string Description { get; set; }
        [XmlAttribute()]
        public int ParentModuleId { get; set; }
        [XmlAttribute()]
        public string SKUIds { get; set; }
    }
}
