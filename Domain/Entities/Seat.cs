using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Seat : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int SectorId { get; set; }
        public string RowIdentifier { get; set; } = string.Empty;
        public int SeatNumber { get; set; }
        public SeatStatus Status { get; set; }

        [Timestamp]
        public byte[]? Version { get; set; } // Optimistic concurrency

        // Navigation properties
        public Sector? Sector { get; set; }
    }

    public enum SeatStatus
    {
        Available,
        Reserved,
        Sold
    }


}
