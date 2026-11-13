using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories;

public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
{
    public ReservationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(r => r.Tables)
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        return await _dbSet
            .Include(r => r.Tables)
            .Include(r => r.User)
            .Where(r => r.Date >= startDate && r.Date <= endDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByStateAsync(ReservationState state)
    {
        return await _dbSet
            .Include(r => r.Tables)
            .Include(r => r.User)
            .Where(r => r.State == state)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Reservation>> GetAllAsync()
    {
        return await _dbSet
            .Include(r => r.Tables)
            .ToListAsync();
    }

    public override async Task<Reservation?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(r => r.Tables)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<int>> GetOccupiedTableIdsAsync(DateOnly date, TimeOnly time, int marginMinutes = 60, int? excludeReservationId = null)
    {
        var startTime = time.AddMinutes(-marginMinutes);
        var endTime = time.AddMinutes(marginMinutes);

        var query = _dbSet
            .Where(r => r.Date == date &&
                        r.State == ReservationState.Confirmed &&
                        r.Time > startTime &&
                        r.Time < endTime);

        if (excludeReservationId.HasValue)
        {
            query = query.Where(r => r.Id != excludeReservationId.Value);
        }

        return await query
            .SelectMany(r => r.Tables.Select(t => t.Id))
            .Distinct()
            .ToListAsync();
    }

}