using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Sector : BaseEntity
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Capacity { get; set; }

        // Navigation properties
        public Event? Event { get; set; }
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}
