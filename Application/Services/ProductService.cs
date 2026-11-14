using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IGenericRepository<ProductCategory> _productCategoryRepository;

    public ProductService(
        IProductRepository repository,
        IGenericRepository<ProductCategory> productCategoryRepository)
    {
        _productRepository = repository;
        _productCategoryRepository = productCategoryRepository;
    }

    private async Task<ProductCategory> ValidateProductAsync(ProductDto dto)
    {
        if(dto is null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("El nombre del producto es obligatorio");

        var name = dto.Name.Trim();

        if (name.Length < 2 || name.Length > 100)
            throw new ArgumentException("El nombre debe tener entre 2 y 100 caracteres");

        if (dto.Price <= 0)
            throw new ArgumentException("El precio debe ser mayor que cero");

        var description = dto.Description?.Trim();

        if (description?.Length > 500)
            throw new ArgumentException("La descripcion no debe superar los 500 caracteres");

        if (string.IsNullOrWhiteSpace(dto.ImageUrl))
            throw new ArgumentException("La URL de la imagen es obligatoria");

        if (dto.ProductCategoryId <= 0)
            throw new ArgumentException("El ID de la categoria debe ser mayor a cero");

        var category = await _productCategoryRepository.GetByIdAsync(dto.ProductCategoryId);

        if (category is null)
            throw new ArgumentException("La categoria seleccionada no existe");

        return category;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(product => new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            ProductCategoryId = product.ProductCategoryId,
            ProductCategoryName = product.ProductCategory.Name
        });
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if(product is null)
        {
            return null;
        }
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            ProductCategoryId = product.ProductCategoryId,
            ProductCategoryName = product.ProductCategory.Name
        };
    }

    public async Task<ProductResponseDto> CreateAsync(ProductDto dto)
    {
        var category = await ValidateProductAsync(dto);

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Price = dto.Price,
            Description = dto.Description?.Trim(),
            ImageUrl = dto.ImageUrl.Trim(),
            ProductCategoryId = dto.ProductCategoryId,
            ProductCategory = category
        };

        await _productRepository.AddAsync(product);

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            ProductCategoryId = product.ProductCategoryId,
            ProductCategoryName = category.Name
        };
    }

    public async Task<ProductResponseDto?> UpdateAsync(int Id, ProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(Id);

        if(product is null)
        {
            return null;
        }

        var category = await ValidateProductAsync(dto);

        product.Name = dto.Name.Trim();
        product.Price = dto.Price;
        product.Description = dto.Description?.Trim();
        product.ImageUrl = dto.ImageUrl.Trim();
        product.ProductCategoryId = dto.ProductCategoryId;
        product.ProductCategory = category;

        await _productRepository.UpdateAsync(product);

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            ProductCategoryId = product.ProductCategoryId,
            ProductCategoryName = category.Name
        };
    }

    public async Task<ProductResponseDto?> ChangePriceAsync(int id, ProductPriceDto dto)
    {
        if (dto is null)
            throw new ArgumentNullException(nameof(dto));

        if (dto.Price <= 0)
            throw new ArgumentException("El precio debe ser mayor a 0");

        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
            return null;
        
        product.Price = dto.Price;

        await _productRepository.UpdateAsync(product);

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            ProductCategoryId = product.ProductCategoryId,
            ProductCategoryName = product.ProductCategory.Name
        };
    }

    public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryIdAsync(int categoryId)
    {
        var products = await _productRepository.GetProductsByCategoryIdAsync(categoryId);

        return products.Select(product => new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            ProductCategoryId = product.ProductCategoryId,
            ProductCategoryName = product.ProductCategory.Name
        });
    }

    public async Task<IEnumerable<ProductResponseDto>> GetProductsByNameAsync(string term)
    {
        var products = await _productRepository.GetProductsByNameAsync(term);

        return products.Select(product => new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            ProductCategoryId = product.ProductCategoryId,
            ProductCategoryName = product.ProductCategory.Name
        });
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if(product is null)
        {
            return false;
        };
        await _productRepository.DeleteAsync(product);

        return true;
    }
}