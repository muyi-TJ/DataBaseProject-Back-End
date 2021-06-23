using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class Favoritestay
    {
        public int FavoriteId { get; set; }
        public int StayId { get; set; }

        public virtual Favorite Favorite { get; set; }
        public virtual Stay Stay { get; set; }
    }
}
