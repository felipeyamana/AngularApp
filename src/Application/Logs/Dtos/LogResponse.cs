using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logs.Dtos
{
    public class LogResponse
    {
        /// <summary>
        /// Current page number (1-based).
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Number of items per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of records matching the query (before pagination).
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// List of logs for the current page.
        /// </summary>
        public List<LogResponseDto> Items { get; set; } = new();
    }

}
