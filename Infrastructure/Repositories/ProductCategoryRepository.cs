using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories;

public class ProductCategoryRepository : GenericRepository<ProductCategory>, IProductCategoryRepository
{
    public ProductCategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<ProductCategory>> GetAllAsync()
    {
        return await _dbSet
            .Where(c => !c.IsDeleted)
            .ToListAsync();
    }
    public override async Task<ProductCategory?> GetByIdAsync(int id)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
    }

    public async Task<ProductCategory?> GetCategoryWithProductsAsync(int categoryId)
    {
        return await _dbSet
            .Where(c => c.Id == categoryId && !c.IsDeleted)
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == categoryId);
    }
    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _dbSet
            .AnyAsync(c => c.Name.ToLower() == name.Trim().ToLower());
    }
    public async Task<bool> SoftDeleteAsync(int categoryId)
    {
        var category = await _dbSet
            .FirstOrDefaultAsync(c => c.Id == categoryId && !c.IsDeleted);

        if (category is null)
            return false;

        category.IsDeleted = true;

        await _context.SaveChangesAsync();

        return true;
    }
}