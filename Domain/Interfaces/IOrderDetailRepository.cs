using Domain.Entities;

namespace Domain.Interfaces;

public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
{
    Task<List<OrderDetail>> GetDetailsByOrderIdAsync(int orderId);

    Task<List<OrderDetail>> GetDetailsByProductIdAsync(int productId);
}