using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Entities
{
    public class Event : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Venue { get; set; } = string.Empty;
        public EventStatus Status { get; set; }

        // Navigation properties
        public ICollection<Sector> Sectors { get; set; } = new List<Sector>();
    }
}
