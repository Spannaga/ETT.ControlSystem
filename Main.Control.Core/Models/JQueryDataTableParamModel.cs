using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Core.Models
{
    public class JQueryDataTableParamModel
    {
        /// <summary>
        /// Request sequence number sent by DataTable, same value must be returned in response
        /// </summary>       
        public string sEcho { get; set; }

        /// <summary>
        /// Text used for filtering
        /// </summary>
        public string sSearch { get; set; }

        /// <summary>
        /// Number of records that should be shown in table
        /// </summary>
        public int iDisplayLength { get; set; }

        /// <summary>
        /// First record that should be shown(used for paging)
        /// </summary>
        public int iDisplayStart { get; set; }

        /// <summary>
        /// Number of columns in table
        /// </summary>
        public int iColumns { get; set; }

        /// <summary>
        /// Number of columns that are used in sorting
        /// </summary>
        public int iSortingCols { get; set; }

        /// <summary>
        /// Comma separated list of column names
        /// </summary>
        public string sColumns { get; set; }

        public string ContactType { get; set; }

        /// <summary>
        /// SortType
        /// </summary>
        public string SortType { get; set; }

        /// <summary>
        /// SortColumn
        /// </summary>
        public int SortColumn { get; set; }

        public string Searchyear { get; set; }

        public string SearchType { get; set; }

        public int TotalRecordCount { get; set; }

        public decimal TotalAmount { get; set; }

        public string sInfoEmpty { get; set; }

        public string FilterType { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string SortChar { get; set; }
        public int totalRecords { get; set; }
        public int? SortColumnIndex { get; set; }
        public string sortDirection { get; set; }
        public long AdminUserId { get; set; }
        public long ProjectId { get; set; }
        public string CampaignStatus { get; set; }
        public int SkipRecords { get; set; }
        public int TakeRecords { get; set; }

    }
}
