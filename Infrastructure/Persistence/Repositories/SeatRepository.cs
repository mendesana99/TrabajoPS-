using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Persistence.Repositories
{
    public class SeatRepository : Repository<Seat>, ISeatRepository
    {
        public SeatRepository(AppDbContext context) : base(context)
        {
        }
    }
}
