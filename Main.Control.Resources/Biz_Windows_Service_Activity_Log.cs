//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Main.Control.Resources
{
    using System;
    using System.Collections.Generic;
    
    public partial class Biz_Windows_Service_Activity_Log
    {
        public long Service_Log_Id { get; set; }
        public string Command { get; set; }
        public long Service_Id { get; set; }
        public long Admin_User_Id { get; set; }
        public bool Is_Deleted { get; set; }
        public System.DateTime Create_Time_Stamp { get; set; }
        public System.DateTime Update_Time_Stamp { get; set; }
    
        public virtual Biz_Admin_Users Biz_Admin_Users { get; set; }
        public virtual Biz_Windows_Services Biz_Windows_Services { get; set; }
    }
}
