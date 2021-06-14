using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class CustomerComment
    {
        public int OrderId { get; set; }
        public DateTime CommentTime { get; set; }
        public string CustomerComment1 { get; set; }
        public decimal HouseStars { get; set; }

        public virtual Order Order { get; set; }
    }
}
