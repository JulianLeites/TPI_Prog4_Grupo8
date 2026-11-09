using System;
using System.Collections.Generic;
using System.Text;

using Domain.Entities;

namespace Domain.Interfaces;
public interface IProductCategoryRepository : IGenericRepository<ProductCategory>
{
    Task<ProductCategory?> GetCategoryWithProductsAsync(int categoryId);
    Task<bool> ExistsByNameAsync(string name);
}