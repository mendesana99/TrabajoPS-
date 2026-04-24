using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces; 

namespace Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Events = new EventRepository(context);
            Sectors = new Repository<Sector>(context);
            Seats = new SeatRepository(context);
            Users = new Repository<User>(context);
            Reservations = new ReservationRepository(context);
            AuditLogs = new Repository<AuditLog>(context);
        }

        public IEventRepository Events { get; }
        public IRepository<Sector> Sectors { get; }
        public ISeatRepository Seats { get; }
        public IRepository<User> Users { get; }
        public IReservationRepository Reservations { get; }
        public IRepository<AuditLog> AuditLogs { get; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
                await _transaction.CommitAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
