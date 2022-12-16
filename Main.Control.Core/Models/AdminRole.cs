using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Main.Control.Core.Models
{
   [Serializable()]
    public class AdminRole
    {
        [XmlAttribute()]
        public long AdminRoleId { get; set; }
        [XmlAttribute()]
        public string Role { get; set; }
        [XmlAttribute()]
        public string Description { get; set; }

        
       
       
    }

   [Serializable()]
   public class AdminProject
   {
       [XmlAttribute()]
       public long AdminProjectId { get; set; }
       [XmlAttribute()]
       public string ProjectName { get; set; }

       [XmlAttribute()]
       public string TechnicalTeamEmail { get; set; }
        [XmlAttribute()]
        public string PaymentLink { get; set; }
        [XmlAttribute()]
        public string PaymentCcAddress { get; set; }
        [XmlAttribute()]
        public string PaymentBccAddress { get; set; }
        [XmlAttribute()]
        public string PaymentFromEmail { get; set; }

    }

  
}
