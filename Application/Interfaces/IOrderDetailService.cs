using Application.Models;

namespace Application.Interfaces;

public interface IOrderDetailService
{
    Task<IEnumerable<OrderDetailDto>> GetAllAsync();
    Task<OrderDetailDto?> GetByIdAsync(int id);
    Task<IEnumerable<OrderDetailDto>> GetDetailsByOrderIdAsync(int orderId);
    Task<IEnumerable<OrderDetailDto>> GetDetailsByProductIdAsync(int productId);
    Task<OrderDetailDto?> UpdateAsync(int id, OrderDetailUpdateDto dto);

    Task<OrderDetailDto?> ChangeQuantityAsync(int id, OrderDetailQuantityDto dto);
    Task<bool> DeleteAsync(int id);
    Task<OrderDetailDto> CreateAsync(CreateOrderDetailDto dto);
}
