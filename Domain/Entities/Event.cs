using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Venue { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Sector> Sectors { get; set; } = new List<Sector>();
    }
}
