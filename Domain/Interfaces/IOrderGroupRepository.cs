using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces;

public interface IOrderGroupRepository : IGenericRepository<OrderGroup>
{
    Task<IEnumerable<OrderGroup>> GetByOrderIdAsync(int orderId);
    Task<IEnumerable<OrderGroup>> GetByStateAsync(OrderGroupState state);
    Task<OrderGroup?> GetByIdWithDetailsAsync(int id);
}
