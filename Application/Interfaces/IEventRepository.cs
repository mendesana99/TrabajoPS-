using Domain.Entities;

namespace Application.Interfaces
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<(IEnumerable<Event> Data, int Total)> GetPaginatedAsync(int page, int pageSize);
    }
}
