using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderDetailRepository : GenericRepository<OrderDetail> , IOrderDetailRepository
{   
    public OrderDetailRepository (ApplicationDbContext context) : base(context)
    { 
    }

    public async Task<List<OrderDetail>> GetDetailsByOrderGroupIdAsync(int orderGroupId)
    {
        return await _context.OrderDetails
            .Where(od => od.OrderGroup_id == orderGroupId)
            .ToListAsync();
    }

    public async Task<List<OrderDetail>> GetDetailsByProductIdAsync(int productId)
    {
        return await _context.OrderDetails
            .Where(od => od.Product_id == productId)
            .ToListAsync();
    }

    public async Task<OrderDetail?> GetExistingDetailAsync(int orderGroupId, int productId, decimal unitPrice)
    {
        return await _context.OrderDetails
            .FirstOrDefaultAsync(orderDetail =>
            orderDetail.OrderGroup_id == orderGroupId &&
            orderDetail.Product_id == productId &&
            orderDetail.UnitPrice == unitPrice);

    }
} 