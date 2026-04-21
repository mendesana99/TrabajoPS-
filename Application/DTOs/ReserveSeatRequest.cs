using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ReserveSeatRequest
    {
        public Guid SeatId { get; set; }
        public int UserId { get; set; }
    }
}
