using Domain.Entities;

namespace Domain.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByTableIdAsync(int tableId);

    Task<IEnumerable<Order>> GetOrdersInPreparationAsync();

}