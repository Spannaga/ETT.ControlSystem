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
    
    public partial class Static_Biz_Admin_Project
    {
        public Static_Biz_Admin_Project()
        {
            this.Biz_Campaign_Details = new HashSet<Biz_Campaign_Details>();
            this.Biz_Ip_Address = new HashSet<Biz_Ip_Address>();
            this.Biz_User_Payments = new HashSet<Biz_User_Payments>();
            this.Static_Biz_Payment_Templates = new HashSet<Static_Biz_Payment_Templates>();
        }
    
        public int Project_Id { get; set; }
        public string Project_Name { get; set; }
        public string Project_Description { get; set; }
        public Nullable<bool> Is_Deleted { get; set; }
        public Nullable<System.DateTime> Create_Time_Stamp { get; set; }
        public Nullable<System.DateTime> Update_Time_Stamp { get; set; }
        public string Technical_Team_Email { get; set; }
        public string Payment_From_Email { get; set; }
        public string Payment_Cc_Address { get; set; }
        public string Payment_Bcc_Address { get; set; }
        public string Payment_Url { get; set; }
        public string WS_SMS_Notify_UserIds { get; set; }
    
        public virtual ICollection<Biz_Campaign_Details> Biz_Campaign_Details { get; set; }
        public virtual ICollection<Biz_Ip_Address> Biz_Ip_Address { get; set; }
        public virtual ICollection<Biz_User_Payments> Biz_User_Payments { get; set; }
        public virtual ICollection<Static_Biz_Payment_Templates> Static_Biz_Payment_Templates { get; set; }
    }
}
