using Domain.Entities;
namespace Domain.Interfaces;

public interface IProductRepository: IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId); 
    Task<IEnumerable<Product>> GetProductsByNameAsync(string term);

    Task<Product?> GetExistingProductAsync(string name);
}