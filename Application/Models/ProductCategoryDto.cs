using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models;

public class ProductCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateProductCategoryDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateProductCategoryDto
{
    public string Name { get; set; } = string.Empty;
}