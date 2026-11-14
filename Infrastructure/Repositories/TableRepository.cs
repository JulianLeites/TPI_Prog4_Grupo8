using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TableRepository
    : GenericRepository<Table>, ITableRepository
{
    public TableRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<Table>> GetAllAsync()
    {
        return await _dbSet
            .Where(t => !t.IsDeleted)
            .ToListAsync();
    }

    public override async Task<Table?> GetByIdAsync(int id)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
    }

    public async Task<IEnumerable<Table>> GetByStateAsync(TableState state)
    {
        return await _dbSet
            .Where(t => t.State == state && !t.IsDeleted)
            .ToListAsync();
    }

    public async Task<bool> ExistsByNumberAsync(int number, int? excludeId = null)
    {
        var query = _dbSet.Where(t => t.Number == number && !t.IsDeleted);

        if (excludeId.HasValue)
            query = query.Where(t => t.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<Table?> GetByNumberAsync(int number)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Number == number && !t.IsDeleted);
    }

    public async Task<bool> HasFutureConfirmedReservationsAsync(int tableId, DateOnly date, TimeOnly time)
    {
        return await _context.Reservations
            .AnyAsync(r =>
            r.Tables.Any(t => t.Id == tableId) &&
            r.State != ReservationState.Cancelled &&
            (
                r.Date > date ||
                (r.Date == date && r.Time > time)
            ));
    }
    
    public async Task<bool> SoftDeleteAsync(int tableId)
    {
        var table = await _dbSet
            .FirstOrDefaultAsync(t => t.Id == tableId && !t.IsDeleted);

        if (table is null)
            return false;

        table.IsDeleted = false;

        await _context.SaveChangesAsync();

        return true;
    }
}