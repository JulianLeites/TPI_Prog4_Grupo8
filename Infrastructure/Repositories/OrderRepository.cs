using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
        
    }
    public override async Task<Order?> GetByIdAsync(int id)
    {
    return await _context.Orders
        .Include(o => o.Details)
        .FirstOrDefaultAsync(o => o.Id == id);
    }
    public override async Task<IEnumerable<Order>> GetAllAsync()
    {
    return await _context.Orders
        .Include(o => o.Details)
            .ThenInclude(d => d.product)
        .ToListAsync();
    }
    public async Task<IEnumerable<Order>> GetOrdersByTableIdAsync(int tableId)
    {
        return await _context.Orders
            .Include(o => o.Details)
            .Where(od => od.Table_id == tableId)
            .ToListAsync();
    }
    public async Task<IEnumerable<Order>> GetOrdersInPreparationAsync()
    {
        return await _context.Orders
            .Include (o => o.Details)
            .Where(od => od.State == 0)
            .ToListAsync();
    }
    public async Task DeleteWithDetailsAsync(Order order)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            _context.OrderDetails.RemoveRange(order.Details);
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}