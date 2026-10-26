using System;
using System.Collections.Generic;
using System.Text;

using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class ProductCategoryService : IProductCategoryService
{
    private readonly IProductCategoryRepository _repository;

    public ProductCategoryService(IProductCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProductCategoryDto>> GetAllAsync()
    {
        var categories = await _repository.GetAllAsync();

        return categories.Select(c => new ProductCategoryDto
        {
            Id = c.Id,
            Name = c.Name
        });
    }

    public async Task<ProductCategoryDto?> GetByIdAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null) return null;

        return new ProductCategoryDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public async Task<ProductCategoryDto> CreateAsync(CreateProductCategoryDto dto)
    {
        var entity = new ProductCategory
        {
            Name = dto.Name
        };

        await _repository.AddAsync(entity);

        return new ProductCategoryDto
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }

    public async Task<bool> UpdateAsync(int id, UpdateProductCategoryDto dto)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null) return false;

        category.Name = dto.Name;
        await _repository.UpdateAsync(category);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null) return false;

        await _repository.DeleteAsync(category);

        return true;
    }
}