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
    
    public partial class Biz_Campaign_Support_User_Details
    {
        public long Campaign_Support_User_Detail_Id { get; set; }
        public Nullable<long> Campaign_Details_Id { get; set; }
        public Nullable<long> Admin_User_Id { get; set; }
        public Nullable<long> Support_User_Id { get; set; }
        public Nullable<int> No_Of_User_Assigned { get; set; }
        public Nullable<int> No_Of_Completed { get; set; }
        public Nullable<int> No_Of_Pending { get; set; }
        public Nullable<int> User_Skipped_Count { get; set; }
        public bool Is_Viewed { get; set; }
        public bool Is_Deleted { get; set; }
        public System.DateTime Created_Time_Stamp { get; set; }
        public System.DateTime Updated_Time_Stamp { get; set; }
        public Nullable<long> Last_Lead_Id { get; set; }
    
        public virtual Biz_Admin_Users Biz_Admin_Users { get; set; }
        public virtual Biz_Admin_Users Biz_Admin_Users1 { get; set; }
        public virtual Biz_Campaign_Details Biz_Campaign_Details { get; set; }
        public virtual Biz_Campaign_Support_User_Details Biz_Campaign_Support_User_Details1 { get; set; }
        public virtual Biz_Campaign_Support_User_Details Biz_Campaign_Support_User_Details2 { get; set; }
    }
}
