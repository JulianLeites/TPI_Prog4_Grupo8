using System;
using System.Collections.Generic;
using System.Text;

using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private static readonly List<ProductCategory> _categories = new()
    {
        new ProductCategory { Id = 1, Name = "Bebidas" },
        new ProductCategory { Id = 2, Name = "Entradas" }
    };

    public Task<IEnumerable<ProductCategory>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<ProductCategory>>(_categories);
    }

    public Task<ProductCategory?> GetByIdAsync(int id)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(category);
    }

    public Task<ProductCategory> AddAsync(ProductCategory category)
    {
        category.Id = _categories.Any() ? _categories.Max(c => c.Id) + 1 : 1;
        _categories.Add(category);
        return Task.FromResult(category);
    }

    public Task UpdateAsync(ProductCategory category)
    {
        var index = _categories.FindIndex(c => c.Id == category.Id);
        if (index != -1)
        {
            _categories[index] = category;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id);
        if (category != null)
        {
            _categories.Remove(category);
        }
        return Task.CompletedTask;
    }
}