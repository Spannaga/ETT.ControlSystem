using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Main.Control.Utilities
{
    public class LogUtility
    {
        public static string BuildExceptionMessage(Exception x)
        {
            Exception logException = x;
            if (x.InnerException != null)
                logException = x.InnerException;

            string strErrorMsg = Environment.NewLine + "Error in Path :" + System.Web.HttpContext.Current.Request.Path;

            //Get the QueryString along with the Virtual Path
            strErrorMsg += Environment.NewLine + "Raw Url :" + System.Web.HttpContext.Current.Request.RawUrl;

            //Get the error data
            if (logException.Data != null)
            {
                if (logException.Data["MachineName"] != null)
                {
                    strErrorMsg += Environment.NewLine + "Machine Name :" + logException.Data["MachineName"].ToString();
                }
                if (logException.Data["ClientMachineName"] != null)
                {
                    strErrorMsg += Environment.NewLine + "Client's Machine Name :" + logException.Data["ClientMachineName"].ToString();
                }
                if (logException.Data["ClientIP"] != null)
                {
                    strErrorMsg += Environment.NewLine + "Client's IP address :" + logException.Data["ClientIP"].ToString();
                }

            }

            // Get the error message
            strErrorMsg += Environment.NewLine + "Message :" + logException.Message;

            // Source of the message
            strErrorMsg += Environment.NewLine + "Source :" + logException.Source;

            // Stack Trace of the error

            strErrorMsg += Environment.NewLine + "Stack Trace :" + logException.StackTrace;

            // Method where the error occurred
            strErrorMsg += Environment.NewLine + "TargetSite :" + logException.TargetSite;
            return strErrorMsg;
        }
    }
}
