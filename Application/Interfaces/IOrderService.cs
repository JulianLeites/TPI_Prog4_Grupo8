using Application.Models;

namespace Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllAsync();
    Task<OrderDto?> GetByIdAsync(int id);
    Task<OrderDto?> UpdateAsync(int id, OrderCreateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<OrderDto> CreateAsync(OrderCreateDto dto);
    Task<OrderDto?> ChangeStateAsync(int id, ChangeOrderStateDto dto);
}