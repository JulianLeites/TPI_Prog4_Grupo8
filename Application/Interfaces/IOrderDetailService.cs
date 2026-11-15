using Application.Models;

namespace Application.Interfaces;

public interface IOrderDetailService
{
    Task<IEnumerable<OrderDetailDto>> GetAllAsync();
    Task<OrderDetailDto?> GetByIdAsync(int id);
    Task<IEnumerable<OrderDetailDto>> GetDetailsByOrderGroupIdAsync(int orderGroupId);
    Task<IEnumerable<OrderDetailDto>> GetDetailsByProductIdAsync(int productId);
    Task<OrderDetailDto?> ChangeQuantityAsync(int id, OrderDetailQuantityDto dto);
    Task<bool> DeleteAsync(int id);
    Task<OrderDetailDto> CreateAsync(CreateOrderDetailDto dto);
}
