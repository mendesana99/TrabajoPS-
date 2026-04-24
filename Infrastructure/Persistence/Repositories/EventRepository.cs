using Domain.Entities;
using Infrastructure.Persistence;
using Application.Interfaces;

namespace Infrastructure.Persistence.Repositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(AppDbContext context) : base(context)
        {
        }
    }
}
