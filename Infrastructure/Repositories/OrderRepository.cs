using Domain.Entities;
using Domain.Enums;
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
        .Include(o => o.OrderGroups)
            .ThenInclude(og => og.Details)
                .ThenInclude(d => d.product)
        .FirstOrDefaultAsync(o => o.Id == id);
    }
    public override async Task<IEnumerable<Order>> GetAllAsync()
    {
    return await _context.Orders
        .Include(o => o.OrderGroups)
            .ThenInclude(og => og.Details)
                .ThenInclude(d => d.product)
        .ToListAsync();
    }
    public async Task<IEnumerable<Order>> GetOrdersByTableIdAsync(int tableId)
    {
        return await _context.Orders
            .Include(o => o.OrderGroups)
                .ThenInclude(og => og.Details)
                    .ThenInclude(d => d.product)
            .Where(o => o.Table_id == tableId)
            .ToListAsync();
    }
    public async Task<Order?> GetActiveOrderByTableIdAsync(int tableId)
    {
        return await _context.Orders
            .FirstOrDefaultAsync(o =>
                o.Table_id == tableId &&
                o.State == OrderState.Active);
    }
    public async Task CloseOrderAndFreeTableAsync(Order order, Table table)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            order.State = OrderState.Closed;
            table.State = TableState.Unoccupied;

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task UpdateTotalAsync(int orderId, decimal total)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

        if(order is null)
            throw new InvalidOperationException($"No se encontró la orden con ID {orderId}.");

        order.Total = total;

        await _context.SaveChangesAsync();
    }
}