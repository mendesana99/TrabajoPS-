namespace Application.DTOs
{
    public class SectorResponse
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Capacity { get; set; }
    }
}
