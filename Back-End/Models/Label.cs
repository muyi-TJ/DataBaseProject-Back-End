using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class Label
    {
        public Label()
        {
            StayLabels = new HashSet<StayLabel>();
        }

        public string LabelName { get; set; }
        public decimal? LabelType { get; set; }

        public virtual ICollection<StayLabel> StayLabels { get; set; }
    }
}
