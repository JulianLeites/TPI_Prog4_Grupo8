using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderGroupRepository
    : GenericRepository<OrderGroup>, IOrderGroupRepository
{
    public OrderGroupRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public override async Task<IEnumerable<OrderGroup>> GetAllAsync()
    {
        return await _dbSet
            .Include(og => og.Details)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderGroup>> GetByOrderIdAsync(int orderId)
    {
        return await _context.OrderGroups
            .Where(og => og.OrderId == orderId)
            .Include(og => og.Details)
                .ThenInclude(d => d.product)
            .ToListAsync();
    }
    public async Task<IEnumerable<OrderGroup>> GetByStateAsync(OrderGroupState state)
    {
        return await _context.OrderGroups
            .Where(og => og.State == state)
            .Include(og => og.Details)
                .ThenInclude(d => d.product)
            .ToListAsync();
    }

    public async Task<OrderGroup?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.OrderGroups
            .Include(og => og.order)
            .Include(og => og.Details)
                .ThenInclude(d => d.product)
            .FirstOrDefaultAsync(og => og.Id == id);
    }
}
