using Domain.Entities;

namespace Domain.Interfaces;

public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
{
    Task<List<OrderDetail>> GetDetailsByOrderGroupIdAsync(int orderGroupId);

    Task<List<OrderDetail>> GetDetailsByProductIdAsync(int productId);

    Task<OrderDetail?> GetExistingDetailAsync(int orderGroupId, int productId, decimal unitPrice);
}