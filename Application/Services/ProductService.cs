using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository repository)
    {
        _productRepository = repository;
    }

    public async Task<IEnumerable<ProductCreateDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(product => new ProductCreateDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl
        });
    }

    public async Task<ProductCreateDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if(product is null)
        {
            return null;
        }
        return new ProductCreateDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl
        };
    }
    public async Task<ProductCreateDto> CreateAsync(ProductCreateDto dto)
    {
        var product = new Product
        {
            Id = dto.Id,
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl        
        };

        await _productRepository.AddAsync(product);

        return new ProductCreateDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl      
        };
    }

    public async Task<ProductCreateDto?> UpdateAsync(int Id, ProductCreateDto dto)
    {
        var product = await _productRepository.GetByIdAsync(Id);
        if(product is null)
        {
            return null;
        }

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.Description = dto.Description;
        product.ImageUrl = dto.ImageUrl;
        await _productRepository.UpdateAsync(product);
        return new ProductCreateDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl      
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