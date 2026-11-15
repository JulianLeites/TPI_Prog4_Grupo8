using Domain.Entities;

namespace Domain.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByTableIdAsync(int tableId);
    Task<Order?> GetActiveOrderByTableIdAsync(int tableId);
    Task CloseOrderAndFreeTableAsync(Order order, Table table);
    Task UpdateTotalAsync(int orderId, decimal total);
}