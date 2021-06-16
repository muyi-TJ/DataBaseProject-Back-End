using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class Host
    {
        public Host()
        {
            Stays = new HashSet<Stay>();
        }

        public int HostId { get; set; }
        public string HostUsername { get; set; }
        public string HostPassword { get; set; }
        public string HostPhone { get; set; }
        public string HostEmail { get; set; }
        public string HostPrephone { get; set; }
        public string HostAvatar { get; set; }
        public DateTime? HostCreateTime { get; set; }
        public string HostIdnumber { get; set; }
        public string HostRealname { get; set; }
        public string HostGender { get; set; }
        public decimal HostScore { get; set; }
        public byte? HostLevel { get; set; }

        public virtual HostGroup HostLevelNavigation { get; set; }
        public virtual ICollection<Stay> Stays { get; set; }
    }
}
