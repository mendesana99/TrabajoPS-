using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Seat
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int SectorId { get; set; }
        public string RowIdentifier { get; set; } = string.Empty;
        public int SeatNumber { get; set; }
        public string Status { get; set; } = string.Empty;

        [ConcurrencyCheck]
        public int Version { get; set; } // Optimistic concurrency

        // Navigation properties
        public Sector? Sector { get; set; }
    }
}
