using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Main.Control.Core.Models
{
   public class AdminIpAddress
    {
       public long IpAddressId { get; set; }

       public int? ProjectId { get; set; }

       public string ProjectName { get; set; }

       [Required(ErrorMessage = "Ip Address is required")]
       [RegularExpression(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b",ErrorMessage="Enter valid Ip Address")]
       public string IpAddress { get; set; }

       public string EmailAddress { get; set; }

       public string IpName { get; set; }

       [Display(Name = "Is Static")]
       public bool IsStatic { get; set; }

    }
}
