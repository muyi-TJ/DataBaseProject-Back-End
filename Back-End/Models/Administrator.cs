using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class Administrator
    {
        public Administrator()
        {
            AdministratorStays = new HashSet<AdministratorStay>();
            Reports = new HashSet<Report>();
        }

        public int AdminId { get; set; }
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }
        public string AdminAvatar { get; set; }
        public DateTime AdminCreateTime { get; set; }
        public string AdminTel { get; set; }

        public virtual ICollection<AdministratorStay> AdministratorStays { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}
