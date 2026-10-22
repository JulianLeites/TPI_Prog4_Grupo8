using System;
using System.Collections.Generic;
using System.Text;

using Domain.Entities;

namespace Domain.Interfaces;

public interface IProductCategoryRepository
{
    Task<IEnumerable<ProductCategory>> GetAllAsync();
    Task<ProductCategory?> GetByIdAsync(int id);
    Task<ProductCategory> AddAsync(ProductCategory category);
    Task UpdateAsync(ProductCategory category);
    Task DeleteAsync(int id);
}