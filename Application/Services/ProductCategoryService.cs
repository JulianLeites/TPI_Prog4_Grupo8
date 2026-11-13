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
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("El nombre de la categoría es obligatorio");

        var name = dto.Name.Trim();

        var exists = await _repository.ExistsByNameAsync(name);
        if (exists)
        {
            throw new ArgumentException($"Ya existe una categoría con el nombre '{name}'");
        }

        var entity = new ProductCategory
        {
            Name = name
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
        if (category is null) 
            return false;

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("El nombre de la categoría es obligatorio");

        var name = dto.Name.Trim();

        var exists = await _repository.ExistsByNameAsync(name);

        if (exists && !string.Equals(category.Name, name, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Ya existe una categoria con ese nombre");

        category.Name = name;
        await _repository.UpdateAsync(category);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _repository.GetCategoryWithProductsAsync(id);
        if (category == null) return false;

        if (category.Products != null && category.Products.Any())
        {
            throw new InvalidOperationException("No se puede eliminar la categoría porque tiene productos asociados.");
        }

        await _repository.DeleteAsync(category);
        return true;
    }

    public async Task<ProductCategoryWithProductsDto?> GetCategoryWithProductsAsync(int categoryId)
    {
        var category = await _repository.GetCategoryWithProductsAsync(categoryId);
        if (category is null) return null;

        return new ProductCategoryWithProductsDto
        {
            Id = category.Id,
            Name = category.Name,
            Products = category.Products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                ProductCategoryId = p.ProductCategoryId,
                ProductCategoryName = category.Name
            }).ToList()
        };
    }
}