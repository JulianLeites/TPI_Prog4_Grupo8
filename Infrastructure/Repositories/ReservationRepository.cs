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
        var target = date.ToDateTime(time);
        var fromDate = date.AddDays(-1);
        var toDate = date.AddDays(1);

        var query = _dbSet
            .Include(r => r.Tables)
            .Where(r =>
                r.Date >= fromDate &&
                r.Date <= toDate &&
                r.State != ReservationState.Cancelled);

        if (excludeReservationId.HasValue)
            query = query.Where(r => r.Id != excludeReservationId.Value);

        var reservations = await query.ToListAsync();

        return reservations
            .Where(r => Math.Abs((r.Date.ToDateTime(r.Time) - target).TotalMinutes) <= marginMinutes)
            .SelectMany(r => r.Tables.Select(t => t.Id))
            .Distinct()
            .ToList();
    }

}