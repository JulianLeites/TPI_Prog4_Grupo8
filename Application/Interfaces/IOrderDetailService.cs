using Application.Models;

namespace Application.Interfaces;

public interface IOrderDetailService
{
    Task<IEnumerable<OrderDetailDto>> GetAllAsync();
    Task<OrderDetailDto?> GetByIdAsync(int id);
    Task<OrderDetailDto?> UpdateAsync(int id, CreateOrderDetailDto dto);
    Task<bool> DeleteAsync(int id);
    Task<OrderDetailDto> CreateAsync(CreateOrderDetailDto dto);
}
