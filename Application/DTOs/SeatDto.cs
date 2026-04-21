using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class SeatDto
    {
        public Guid Id { get; set; }
        public string SectorName { get; set; } = string.Empty;
        public string RowIdentifier { get; set; } = string.Empty;
        public int SeatNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
