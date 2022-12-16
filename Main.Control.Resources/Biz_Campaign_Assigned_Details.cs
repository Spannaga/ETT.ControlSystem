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
    
    public partial class Biz_Campaign_Assigned_Details
    {
        public Biz_Campaign_Assigned_Details()
        {
            this.Biz_Campaign_Additional_Contacts_Details = new HashSet<Biz_Campaign_Additional_Contacts_Details>();
            this.Biz_Campaign_Excel_Values = new HashSet<Biz_Campaign_Excel_Values>();
            this.Biz_Campaign_Follow_Up_Log = new HashSet<Biz_Campaign_Follow_Up_Log>();
            this.Biz_Campaign_Lead_Activity = new HashSet<Biz_Campaign_Lead_Activity>();
        }
    
        public long Campaign_Assigned_Details_Id { get; set; }
        public long Campaign_Details_Id { get; set; }
        public string Name { get; set; }
        public string Email_Address { get; set; }
        public string Business_Name { get; set; }
        public string EIN { get; set; }
        public string Phone_Number { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> Signed_Up_On { get; set; }
        public string Product_Name { get; set; }
        public string User_Type { get; set; }
        public Nullable<long> No_Of_Trucks { get; set; }
        public Nullable<System.DateTime> Last_Filed_On { get; set; }
        public string Subscribed { get; set; }
        public Nullable<long> Support_User_Id { get; set; }
        public bool Is_Deleted { get; set; }
        public System.DateTime Created_Time_Stamp { get; set; }
        public System.DateTime Updated_Time_Stamp { get; set; }
        public bool Is_Skip { get; set; }
        public Nullable<int> Campaign_Follow_Up_Id { get; set; }
        public Nullable<long> ReturnNumber { get; set; }
    
        public virtual Biz_Admin_Users Biz_Admin_Users { get; set; }
        public virtual ICollection<Biz_Campaign_Additional_Contacts_Details> Biz_Campaign_Additional_Contacts_Details { get; set; }
        public virtual Biz_Campaign_Manager_Follow_Up_Details Biz_Campaign_Manager_Follow_Up_Details { get; set; }
        public virtual ICollection<Biz_Campaign_Excel_Values> Biz_Campaign_Excel_Values { get; set; }
        public virtual ICollection<Biz_Campaign_Follow_Up_Log> Biz_Campaign_Follow_Up_Log { get; set; }
        public virtual ICollection<Biz_Campaign_Lead_Activity> Biz_Campaign_Lead_Activity { get; set; }
        public virtual Biz_Campaign_Details Biz_Campaign_Details { get; set; }
    }
}