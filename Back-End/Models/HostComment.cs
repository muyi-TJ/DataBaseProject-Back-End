using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class HostComment
    {
        public int OrderId { get; set; }
        public DateTime CommentTime { get; set; }
        public string HostComment1 { get; set; }
        public decimal CustomerStars { get; set; }

        public virtual Order Order { get; set; }
    }
}
