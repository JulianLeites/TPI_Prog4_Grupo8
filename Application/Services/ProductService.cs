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
        var category = await _productCategoryRepository.GetByIdAsync(dto.ProductCategoryId);

        if (category is null)
            throw new ArgumentException("La categoria no existe");

        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            ProductCategoryId = dto.ProductCategoryId,
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
            ProductCategoryName = product.ProductCategory.Name
        };
    }

    public async Task<ProductResponseDto?> UpdateAsync(int Id, ProductDto dto)
    {
        var category = await _productCategoryRepository.GetByIdAsync(dto.ProductCategoryId);

        if (category is null)
            throw new ArgumentException("La categoria no existe");

        var product = await _productRepository.GetByIdAsync(Id);

        if(product is null)
        {
            return null;
        }

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.Description = dto.Description;
        product.ImageUrl = dto.ImageUrl;
        product.ProductCategoryId = dto.ProductCategoryId;

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