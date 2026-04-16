using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Event> Events { get; }
        IRepository<Sector> Sectors { get; }
        IRepository<Seat> Seats { get; }
        IRepository<User> Users { get; }
        IRepository<Reservation> Reservations { get; }
        IRepository<AuditLog> AuditLogs { get; }

        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
