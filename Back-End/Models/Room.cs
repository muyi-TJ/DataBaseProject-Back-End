using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class Room
    {
        public Room()
        {
            Generates = new HashSet<Generate>();
            RoomBeds = new HashSet<RoomBed>();
            RoomPhotos = new HashSet<RoomPhoto>();
        }

        public int StayId { get; set; }
        public int RoomId { get; set; }
        public int Price { get; set; }
        public decimal? RoomArea { get; set; }
        public byte? BathroomNum { get; set; }

        public virtual Stay Stay { get; set; }
        public virtual ICollection<Generate> Generates { get; set; }
        public virtual ICollection<RoomBed> RoomBeds { get; set; }
        public virtual ICollection<RoomPhoto> RoomPhotos { get; set; }
    }
}
