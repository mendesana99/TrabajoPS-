using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Events.Queries;

namespace Application.UseCases.Events.Handlers
{
    public class GetSectorsByEventHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSectorsByEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SectorResponse>> HandleAsync(GetSectorsByEventQuery query)
        {
            var sectors = await _unitOfWork.Sectors.FindAsync(s => s.EventId == query.EventId);
            
            return sectors.Select(s => new SectorResponse
            {
                Id = s.Id,
                EventId = s.EventId,
                Name = s.Name,
                Price = s.Price,
                Capacity = s.Capacity
            });
        }
    }
}
