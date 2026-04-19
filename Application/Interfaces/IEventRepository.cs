using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IEventRepository
    {
        Task<Event> GetEventByIdAsync(Event id);
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<IEnumerable<Event>> FindEventAsync();
        Task AddEventAsync(Event entity);
        Task UpdateEventAsync(Event entity);
        Task DeleteEventAsync(Event id);
        Task<int> SaveChangesAsync();

    }
}
