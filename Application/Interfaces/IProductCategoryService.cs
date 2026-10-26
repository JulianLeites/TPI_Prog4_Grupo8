using System;
using System.Collections.Generic;
using System.Text;

using Application.Models;

namespace Application.Interfaces;

public interface IProductCategoryService
{
    Task<IEnumerable<ProductCategoryDto>> GetAllAsync();
    Task<ProductCategoryDto?> GetByIdAsync(int id);
    Task<ProductCategoryDto> CreateAsync(CreateProductCategoryDto dto);
    Task<bool> UpdateAsync(int id, UpdateProductCategoryDto dto);
    Task<bool> DeleteAsync(int id);
}