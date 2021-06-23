using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class Favorite
    {
        public Favorite()
        {
            Favoritestays = new HashSet<Favoritestay>();
        }

        public int FavoriteId { get; set; }
        public int? CustomerId { get; set; }
        public string Name { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<Favoritestay> Favoritestays { get; set; }
    }
}
