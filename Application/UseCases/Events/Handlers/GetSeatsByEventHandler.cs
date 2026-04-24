using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Events.Queries;

namespace Application.UseCases.Events.Handlers
{
    public class GetSeatsByEventHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSeatsByEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SeatDto>> HandleAsync(GetSeatsByEventQuery query)
        {
            var seats = await _unitOfWork.Seats
                .FindAsync(s => s.Sector != null && s.Sector.EventId == query.EventId);
                
            return seats.Select(s => new SeatDto
            {
                Id = s.Id,
                SectorName = s.Sector?.Name ?? string.Empty,
                RowIdentifier = s.RowIdentifier,
                SeatNumber = s.SeatNumber,
                Status = s.Status,
                Price = s.Sector?.Price ?? 0
            });
        }
    }
}
