using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Events = new Repository<Event>(context) as IRepository<Event>;
            Sectors = new Repository<Sector>(context) as IRepository<Sector>;
            Seats = new Repository<Seat>(context) as IRepository<Seat>;
            Users = new Repository<User>(context) as IRepository<User>;
            Reservations = new Repository<Reservation>(context) as IRepository<Reservation>;
            AuditLogs = new Repository<AuditLog>(context) as IRepository<AuditLog>;
        }

        public IRepository<Event> Events { get; }
        public IRepository<Sector> Sectors { get; }
        public IRepository<Seat> Seats { get; }
        public IRepository<User> Users { get; }
        public IRepository<Reservation> Reservations { get; }
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
