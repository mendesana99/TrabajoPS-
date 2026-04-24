using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IEventRepository Events { get; }
        IRepository<Sector> Sectors { get; }
        ISeatRepository Seats { get; }
        IRepository<User> Users { get; }
        IReservationRepository Reservations { get; }
        IRepository<AuditLog> AuditLogs { get; }

        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
