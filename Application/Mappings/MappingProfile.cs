using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;

namespace Application.Mappings
{
    public static class MappingProfile
    {
        public static SeatDto ToDto(this Seat seat)
        {
            return new SeatDto
            {
                Id = seat.Id,
                SectorName = seat.Sector?.Name ?? string.Empty,
                RowIdentifier = seat.RowIdentifier,
                SeatNumber = seat.SeatNumber,
                Status = seat.Status.ToString(),
                Price = seat.Sector?.Price ?? 0
            };
        }

        public static IEnumerable<SeatDto> ToDtoList(this IEnumerable<Seat> seats)
        {
            return seats.Select(s => s.ToDto());
        }
    }
}
