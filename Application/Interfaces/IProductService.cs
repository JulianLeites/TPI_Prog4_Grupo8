using Application.Models;

namespace Application.Interfaces;
public interface IProductService
{
    Task<IEnumerable<ProductCreateDto>> GetAllAsync();
    Task<ProductCreateDto?> GetByIdAsync(int id);
    Task<ProductCreateDto> CreateAsync(ProductCreateDto dto);
    Task<ProductCreateDto?> UpdateAsync(int id, ProductCreateDto dto);
    Task<bool> DeleteAsync(int id);

}