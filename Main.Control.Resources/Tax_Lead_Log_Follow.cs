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
    
    public partial class Tax_Lead_Log_Follow
    {
        public long LeadLog_Follow_Id { get; set; }
        public long LeadLog_Id { get; set; }
        public string FolowUp_Message { get; set; }
        public System.DateTime FollowUp_date { get; set; }
        public string Support_Type { get; set; }
        public bool Is_Deleted { get; set; }
        public Nullable<System.DateTime> Create_Time_Stamp { get; set; }
        public Nullable<System.DateTime> Update_Time_Stamp { get; set; }
        public bool Is_Dont_Follow_Up { get; set; }
        public string Lead_Status { get; set; }
        public Nullable<long> Admin_User_Id { get; set; }
        public string Admin_User_Name { get; set; }
        public Nullable<long> Lead_Status_SubcategoryId { get; set; }
        public Nullable<long> Lead_Status_Id { get; set; }
    
        public virtual Tax_Lead_Log Tax_Lead_Log { get; set; }
    }
}
