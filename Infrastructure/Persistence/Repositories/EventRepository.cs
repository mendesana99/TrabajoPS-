using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class EventRepository : IEventRepository
    {
        protected readonly AppDbContext _context;   
        protected readonly DbSet<Event> _dbSet;

        public EventRepository(AppDbContext context) 
        { 
            _context = context;
        }

        public Task AddEventAsync(Event entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteEventAsync(Event id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Event>> FindEventAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Event> GetEventByIdAsync(Event id)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateEventAsync(Event entity)
        {
            throw new NotImplementedException();
        }
    }
}
