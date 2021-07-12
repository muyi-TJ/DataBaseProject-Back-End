using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class StayLabel
    {
        public int StayId { get; set; }
        public string LabelName { get; set; }

        public virtual Label LabelNameNavigation { get; set; }
        public virtual Stay Stay { get; set; }
    }
}
