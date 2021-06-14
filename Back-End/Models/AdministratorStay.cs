using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class AdministratorStay
    {
        public int AdminId { get; set; }
        public int StayId { get; set; }
        public decimal ValidateResult { get; set; }
        public string ValidateReply { get; set; }
        public DateTime ValCreateTime { get; set; }
        public DateTime ValReplyTime { get; set; }

        public virtual Administrator Admin { get; set; }
        public virtual Stay Stay { get; set; }
    }
}
