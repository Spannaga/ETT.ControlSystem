using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class ApiLogEntry
    {
        public long ApiLogEntryId { get; set; }             // The (database) ID for the API log entry.
        public string Application { get; set; }             // The application that made the request.
        public string User { get; set; }                    // The user that made the request.
        public string Machine { get; set; }                 // The machine that made the request.
        public string RequestIpAddress { get; set; }        // The IP address that made the request.
        public string RequestContentType { get; set; }      // The request content type.
        public string RequestContentBody { get; set; }      // The request content body.
        public string RequestUri { get; set; }              // The request URI.
        public string RequestMethod { get; set; }           // The request method (GET, POST, etc).
        public string RequestRouteTemplate { get; set; }    // The request route template.
        public string RequestRouteData { get; set; }        // The request route data.
        public string RequestHeaders { get; set; }          // The request headers.
        public DateTime? RequestTimestamp { get; set; }     // The request timestamp.
        public string ResponseContentType { get; set; }     // The response content type.
        public string ResponseContentBody { get; set; }     // The response content body.
        public int? ResponseStatusCode { get; set; }        // The response status code.
        public string ResponseHeaders { get; set; }         // The response headers.
        public string ControllerName { get; set; }          // The request Controller name.
        public string ActionName { get; set; }              // The request ActionName.
        public long HealingCenterId { get; set; }          // The request Header healing center id.
        public long UserId { get; set; }                   // The request Header UserId.
        public int AdminUserId { get; set; }                   // The request Header Admin UserId.
        public string UserName { get; set; }                    // The username that made the request.
        public string RequestFrom { get; set; }                    // The user that made the request.
        public string ActivityLog { get; set; }
        public string ActionType { get; set; }
        public DateTime? ResponseTimestamp { get; set; }    // The response timestamp.
    }
}
