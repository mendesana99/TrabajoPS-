
namespace Domain.Entities
{
    public class Seat
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int SectorId { get; set; }
        public string RowIdentifier { get; set; } = string.Empty;
        public int SeatNumber { get; set; }
        public string Status { get; set; } = string.Empty; // Available, Reserved, Sold
        public int Version { get; set; } // Para control de concurrencia

        // Navigation properties
        public Sector? Sector { get; set; }
    }
}
