using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Models;

public class ProductCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateProductCategoryDto
{
    [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres")]
    public string Name { get; set; } = string.Empty;
}

public class UpdateProductCategoryDto
{
    [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres")]
    public string Name { get; set; } = string.Empty;
}
public class ProductCategoryWithProductsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<ProductResponseDto> Products { get; set; } = new List<ProductResponseDto>();
}