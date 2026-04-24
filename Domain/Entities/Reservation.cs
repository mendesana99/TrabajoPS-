using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Reservation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int UserId { get; set; }
        public Guid SeatId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime ReservedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Seat? Seat { get; set; }
    }
}
