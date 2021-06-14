using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class Report
    {
        public int OrderId { get; set; }
        public DateTime ReportTime { get; set; }
        public string Reason { get; set; }
        public decimal? IsDealed { get; set; }
        public int? AdminId { get; set; }
        public DateTime? DealTime { get; set; }
        public string Reply { get; set; }

        public virtual Administrator Admin { get; set; }
        public virtual Order Order { get; set; }
    }
}
