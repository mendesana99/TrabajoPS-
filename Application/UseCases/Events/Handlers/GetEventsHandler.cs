using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Events.Queries;

namespace Application.UseCases.Events.Handlers
{
    public class GetEventsHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEventsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<EventResponse>> HandleAsync(GetEventsQuery query)
        {
            var events = await _unitOfWork.Events.GetAllAsync();
            
            return events.Select(e => new EventResponse
            {
                Id = e.Id,
                Name = e.Name,
                Venue = e.Venue,
                EventDate = e.EventDate,
                Status = e.Status.ToString()
            });
        }
    }
}
