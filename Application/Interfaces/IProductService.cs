using Application.Models;

namespace Application.Interfaces;
public interface IProductService
{
    Task<IEnumerable<ProductResponseDto>> GetAllAsync();
    Task<ProductResponseDto?> GetByIdAsync(int id);
    Task<ProductResponseDto> CreateAsync(ProductDto dto);
    Task<ProductResponseDto?> UpdateAsync(int id, ProductDto dto);
    Task<bool> DeleteAsync(int id);
}