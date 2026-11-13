using System.ComponentModel.DataAnnotations;

namespace Application.Models;

public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int ProductCategoryId { get; set; }
    public string ProductCategoryName { get; set; } = string.Empty;

}

public class ProductDto
{
    [Required(ErrorMessage = "El nombre del producto es obligatorio")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
    [RegularExpression(@".*\S.*",
        ErrorMessage = "El nombre no pude tener solo espacios")]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999999",
        ErrorMessage = "El precio debe ser mayor que cero")]
    public decimal Price { get; set; }

    [MaxLength(500, ErrorMessage = "La descripcion no puede superar los 500 caracteres")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "La URL de la imagen es obligatoria")]
    [Url(ErrorMessage = "La URL no tiene un formato valido")]
    public string ImageUrl { get; set; } = string.Empty;

    [Range(1, int.MaxValue,
        ErrorMessage = "Debe seleccionar una categoria valida")]
    public int ProductCategoryId { get; set; }
}

public class ProductPriceDto
{
    [Range(typeof(decimal), "0.01", "999999999",
        ErrorMessage = "El precio debe ser mayor que cero")]
    public decimal Price { get; set; }
}