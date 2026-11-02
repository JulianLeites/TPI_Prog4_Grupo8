using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories;

public class TableRepository
    : GenericRepository<Table>, ITableRepository
{
    public TableRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Table>> GetByStateAsync(TableState state)
    {
        return await _dbSet
            .Where(t => t.State == state)
            .ToListAsync();
    }

    public async Task<bool> ExistsByNumberAsync(int number, int? excludeId = null)
    {
        var query = _dbSet.Where(t => t.Number == number);

        if (excludeId.HasValue)
            query = query.Where(t => t.Id != excludeId.Value);

        return await query.AnyAsync();
    }
    
    public async Task<Table?> GetByNumberAsync(int number)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Number == number);
    }

    public async Task<bool> HasFutureConfirmedReservationsAsync(int tableId, DateOnly date, TimeOnly time)
    {
        return await _context.Reservations
            .AnyAsync(r =>
            r.Tables.Any(t => t.Id == tableId) &&
            r.State == ReservationState.Confirmed &&
            (
                r.Date > date ||
                (r.Date == date && r.Time > time)
            ));
    }
}
