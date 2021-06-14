using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class HostGroup
    {
        public HostGroup()
        {
            Hosts = new HashSet<Host>();
        }

        public byte HostLevel { get; set; }
        public string HostLevelName { get; set; }
        public int HostLevelDegree { get; set; }

        public virtual ICollection<Host> Hosts { get; set; }
    }
}
