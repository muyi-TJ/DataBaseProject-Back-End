using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class Stay
    {
        public Stay()
        {
            AdministratorStays = new HashSet<AdministratorStay>();
            Collects = new HashSet<Collect>();
            Nears = new HashSet<Near>();
            Rooms = new HashSet<Room>();
        }

        public int StayId { get; set; }
        public int? HostId { get; set; }
        public string StayName { get; set; }
        public int? AreaId { get; set; }
        public string StayType { get; set; }
        public string PeripheralRoad { get; set; }
        public string DetailedAddress { get; set; }
        public byte StayCapacity { get; set; }
        public byte RoomNum { get; set; }
        public byte BedNum { get; set; }
        public decimal PublicBathroom { get; set; }
        public decimal PublicToilet { get; set; }
        public decimal NonBarrierFacility { get; set; }
        public string Characteristic { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public byte DaysMin { get; set; }
        public byte DaysMax { get; set; }
        public decimal StayStatus { get; set; }

        public virtual Area Area { get; set; }
        public virtual Host Host { get; set; }
        public virtual StayType StayTypeNavigation { get; set; }
        public virtual ICollection<AdministratorStay> AdministratorStays { get; set; }
        public virtual ICollection<Collect> Collects { get; set; }
        public virtual ICollection<Near> Nears { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
