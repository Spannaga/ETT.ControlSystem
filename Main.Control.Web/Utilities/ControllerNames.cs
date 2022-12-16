using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Main.Control.Web.Utilities
{
    public sealed class ControllerNames
    {
        public const string Admin = "Admin";
        public const string Home = "Home";
        public const string Error = "Error";

        //UnitWise
        public const string UWFeature = "UWFeature";
        public const string UWDashboard = "UWDashboard";

        //Campaign
        public static string Campaign { get; set; }
    }
}