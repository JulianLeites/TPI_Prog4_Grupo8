using System.Security.Cryptography.X509Certificates;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IGenericRepository<ProductCategory> _productCategoryRepository;
    private readonly IDolarService _dolarService;
    public ProductService(
        IProductRepository repository,
        IGenericRepository<ProductCategory> productCategoryRepository,
        IDolarService dolarService)
    {
        _productRepository = repository;
        _productCategoryRepository = productCategoryRepository;
        _dolarService = dolarService;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        var dolarOficial = await _dolarService.GetDolarOficialVentaAsync();
        return products.Select(product => new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            PriceUsd = product.Price / dolarOficial,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            ProductCategoryId = product.ProductCategoryId,
            ProductCategoryName = product.ProductCategory.Name
        });
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        var dolarOficial = await _dolarService.GetDolarOficialVentaAsync();
        if (product is null)
        {
            return null;
        }
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            PriceUsd = product.Price / dolarOficial,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            ProductCategoryId = product.ProductCategoryId,
            ProductCategoryName = product.ProductCategory.Name
        };
    }
    public async Task<ProductResponseDto> CreateAsync(ProductDto dto)
    {
        var category = await _productCategoryRepository.GetByIdAsync(dto.ProductCategoryId);
        var dolarOficial = await _dolarService.GetDolarOficialVentaAsync();
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
            PriceUsd = product.Price / dolarOficial,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            ProductCategoryId = product.ProductCategoryId,
            ProductCategoryName = product.ProductCategory.Name
        };
    }

    public async Task<ProductResponseDto?> UpdateAsync(int Id, ProductDto dto)
    {
        var category = await _productCategoryRepository.GetByIdAsync(dto.ProductCategoryId);
        var dolarOficial = await _dolarService.GetDolarOficialVentaAsync();
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
            PriceUsd = product.Price / dolarOficial,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            ProductCategoryId = product.ProductCategoryId,
            ProductCategoryName = product.ProductCategory.Name
        };
    }

    public async Task<ProductResponseDto?> ChangePriceAsync(int id, ProductPriceDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        var dolarOficial = await _dolarService.GetDolarOficialVentaAsync();

        if (product is null)
            return null;
        if (dto.Price <= 0)
            throw new ArgumentException("El precio debe ser mayor a 0");

        
        product.Price = dto.Price;

        await _productRepository.UpdateAsync(product);

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            PriceUsd = product.Price / dolarOficial,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            ProductCategoryId = product.ProductCategoryId,
            ProductCategoryName = product.ProductCategory.Name
        };
    }

    public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryIdAsync(int categoryId)
    {
        var products = await _productRepository.GetProductsByCategoryIdAsync(categoryId);
        var dolarOficial = await _dolarService.GetDolarOficialVentaAsync();
        return products.Select(product => new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            PriceUsd = product.Price / dolarOficial,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            ProductCategoryId = product.ProductCategoryId,
            ProductCategoryName = product.ProductCategory.Name
        });
    }

    public async Task<IEnumerable<ProductResponseDto>> GetProductsByNameAsync(string term)
    {
        var products = await _productRepository.GetProductsByNameAsync(term);
        var dolarOficial = await _dolarService.GetDolarOficialVentaAsync();

        return products.Select(product => new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            PriceUsd = product.Price / dolarOficial,
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