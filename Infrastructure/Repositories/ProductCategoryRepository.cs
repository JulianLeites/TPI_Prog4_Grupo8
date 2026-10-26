using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories;

public class ProductCategoryRepository
    : GenericRepository<ProductCategory>,
      IProductCategoryRepository
{
    public ProductCategoryRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}