using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository (ApplicationDbContext context) : base(context)
    {   
    }
    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbSet
            .Where(product => !product.IsDeleted)
            .Include(product => product.ProductCategory)
            .ToListAsync();
    }
    public override async Task<Product?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Where(product => !product.IsDeleted)
            .Include(product => product.ProductCategory)
            .FirstOrDefaultAsync(product => product.Id == id);
    }
    public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId)
    {
        return await _context.Products
            .Where(product => product.ProductCategoryId == categoryId && !product.IsDeleted)
            .Include(product => product.ProductCategory)        
            .ToListAsync();
            
    }
    public async Task<IEnumerable<Product>> GetProductsByNameAsync(string term)
    {
        return await _context.Products
            .Where(p =>
                !p.IsDeleted &&
                EF.Functions.Like(p.Name, $"%{term}%"))
            .Include(p => p.ProductCategory)
            .ToListAsync();
    }
    public async Task<bool> SoftDeleteAsync(int productId)
    {
        var product = await _dbSet
            .FirstOrDefaultAsync(product => product.Id == productId && !product.IsDeleted);

        if (product is null)
            return false;

        product.IsDeleted = true;

        await _context.SaveChangesAsync();

        return true;
    }
}