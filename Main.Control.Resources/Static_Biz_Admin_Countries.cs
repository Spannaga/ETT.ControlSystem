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
    
    public partial class Static_Biz_Admin_Countries
    {
        public Static_Biz_Admin_Countries()
        {
            this.Biz_User_Payments = new HashSet<Biz_User_Payments>();
            this.Static_Biz_Admin_States = new HashSet<Static_Biz_Admin_States>();
        }
    
        public long Country_Id { get; set; }
        public string Country_name { get; set; }
        public string Country_Code { get; set; }
        public bool Is_Deleted { get; set; }
        public System.DateTime Create_Time_Stamp { get; set; }
        public System.DateTime Update_Time_Stamp { get; set; }
        public Nullable<bool> Is_State_Available { get; set; }
    
        public virtual ICollection<Biz_User_Payments> Biz_User_Payments { get; set; }
        public virtual ICollection<Static_Biz_Admin_States> Static_Biz_Admin_States { get; set; }
    }
}
