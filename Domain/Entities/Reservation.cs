using System;
using Domain.Enums;

namespace Domain.Entities
{
    public class Reservation : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int UserId { get; set; }
        public Guid SeatId { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime ReservedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Seat? Seat { get; set; }
    }
}
