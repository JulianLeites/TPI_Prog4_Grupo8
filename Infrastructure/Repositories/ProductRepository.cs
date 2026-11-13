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
            .Include(product => product.ProductCategory)
            .ToListAsync();
    }

    public override async Task<Product?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(product => product.ProductCategory)
            .FirstOrDefaultAsync(product => product.Id == id);
    }
    public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId)
    {
        return await _context.Products
            .Include(p => p.ProductCategory)        
            .Where(p => p.ProductCategoryId == categoryId)
            .ToListAsync();
            
    }

    public async Task<IEnumerable<Product>> GetProductsByNameAsync(string term)
    {
        return await _context.Products
            .Include(p => p.ProductCategory)
            .Where(p => EF.Functions.Like(p.Name, $"%{term}%"))
            .ToListAsync();
    }

    public async Task<Product?> GetExistingProductAsync(string name)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Name == name);
    }
}