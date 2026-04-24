using System;

namespace Application.DTOs
{
    public class EventResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
