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


    public async Task<List<OrderDetail>> GetDetailsByOrderIdAsync(int orderId)
    {
        return await _context.OrderDetails
            .Where(od => od.Order_id == orderId)
            .ToListAsync();
    }

    public async Task<List<OrderDetail>> GetDetailsByProductIdAsync(int productId)
    {
        return await _context.OrderDetails
            .Where(od => od.Product_id == productId)
            .ToListAsync();
    }
} 