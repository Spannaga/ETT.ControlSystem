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
    
    public partial class Biz_Admin_Users
    {
        public Biz_Admin_Users()
        {
            this.Biz_Admin_User_Roles = new HashSet<Biz_Admin_User_Roles>();
            this.Biz_Campaign_Assigned_Details = new HashSet<Biz_Campaign_Assigned_Details>();
            this.Biz_Campaign_Details = new HashSet<Biz_Campaign_Details>();
            this.Biz_Campaign_Follow_Up_Log = new HashSet<Biz_Campaign_Follow_Up_Log>();
            this.Biz_Campaign_Follow_Up_Log1 = new HashSet<Biz_Campaign_Follow_Up_Log>();
            this.Biz_Campaign_Support_User_Details = new HashSet<Biz_Campaign_Support_User_Details>();
            this.Biz_Campaign_Support_User_Details1 = new HashSet<Biz_Campaign_Support_User_Details>();
            this.Biz_User_Payments_Logs = new HashSet<Biz_User_Payments_Logs>();
            this.Biz_Windows_Service_Activity_Log = new HashSet<Biz_Windows_Service_Activity_Log>();
            this.Mobile_Verification = new HashSet<Mobile_Verification>();
            this.Sc_Activity_Log = new HashSet<Sc_Activity_Log>();
        }
    
        public long Admin_User_Id { get; set; }
        public long Admin_Role { get; set; }
        public string Admin_User_Name { get; set; }
        public string Admin_Password { get; set; }
        public string Admin_Salt { get; set; }
        public string Admin_First_Name { get; set; }
        public string Admin_Last_Name { get; set; }
        public string Admin_Email_Address { get; set; }
        public string Projects { get; set; }
        public Nullable<long> Created_By { get; set; }
        public Nullable<long> Last_Updated_By { get; set; }
        public bool Is_Deleted { get; set; }
        public System.DateTime Create_Time_Stamp { get; set; }
        public System.DateTime Update_Time_Stamp { get; set; }
        public Nullable<bool> Is_Approved { get; set; }
        public string Phone_Number { get; set; }
        public string Admin_Initial { get; set; }
        public string Location { get; set; }
        public string Verification_Code_Type { get; set; }
        public string Alternate_Admin_Email_Address { get; set; }
        public string Title { get; set; }
        public string Support_Security_Code { get; set; }
        public Nullable<int> Security_Code_Count { get; set; }
        public Nullable<System.DateTime> Code_Generated_On { get; set; }
        public Nullable<bool> Is_Enabled_Authenticator { get; set; }
    
        public virtual Biz_Admin_Roles Biz_Admin_Roles { get; set; }
        public virtual ICollection<Biz_Admin_User_Roles> Biz_Admin_User_Roles { get; set; }
        public virtual ICollection<Biz_Campaign_Assigned_Details> Biz_Campaign_Assigned_Details { get; set; }
        public virtual ICollection<Biz_Campaign_Details> Biz_Campaign_Details { get; set; }
        public virtual ICollection<Biz_Campaign_Follow_Up_Log> Biz_Campaign_Follow_Up_Log { get; set; }
        public virtual ICollection<Biz_Campaign_Follow_Up_Log> Biz_Campaign_Follow_Up_Log1 { get; set; }
        public virtual ICollection<Biz_Campaign_Support_User_Details> Biz_Campaign_Support_User_Details { get; set; }
        public virtual ICollection<Biz_Campaign_Support_User_Details> Biz_Campaign_Support_User_Details1 { get; set; }
        public virtual ICollection<Biz_User_Payments_Logs> Biz_User_Payments_Logs { get; set; }
        public virtual ICollection<Biz_Windows_Service_Activity_Log> Biz_Windows_Service_Activity_Log { get; set; }
        public virtual ICollection<Mobile_Verification> Mobile_Verification { get; set; }
        public virtual ICollection<Sc_Activity_Log> Sc_Activity_Log { get; set; }
    }
}
