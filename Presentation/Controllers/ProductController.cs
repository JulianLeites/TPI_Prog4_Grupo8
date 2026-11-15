using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "waiter,admin")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products); 
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetById([FromRoute] int id)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID debe ser mayor que cero."
            );

        var product = await _productService.GetByIdAsync(id);

        if (product is null)
            return Problem(
                statusCode: 404,
                title: "Producto no encontrado",
                detail: "No se encontró el producto solicitado."
            );

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] ProductDto dto)
    {
        var product = await _productService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product
        );
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update([FromRoute]int id, [FromBody] ProductDto dto)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID debe ser mayor que cero."
            );

        var product = await _productService.UpdateAsync(id, dto);

        if (product is null)
            return Problem(
                statusCode: 404,
                title: "Producto no encontrado",
                detail: "No se encontró el producto solicitado."
            );

        return Ok(product);
    }

    [HttpPatch("{id:int}/price")]
    public async Task<ActionResult> ChangePrice([FromRoute]int id, [FromBody] ProductPriceDto dto)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID debe ser mayor que cero."
            );

        var product = await _productService.ChangePriceAsync(id, dto);

        if (product is null)
            return Problem(
                statusCode: 404,
                title: "Producto no encontrado",
                detail: "No se encontró el producto solicitado."
            );

        return Ok(product);

    }

    [HttpGet("ProductCategory/{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetProductsByCategoryId(int id)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID de categoría debe ser mayor que cero."
            );

        var products = await _productService.GetProductsByCategoryIdAsync(id);
        return Ok(products); 
    }

    [HttpGet("search/{terms}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetProductsByName(string terms)
    {
        var products = await _productService.GetProductsByNameAsync(terms);
        return Ok(products); 
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID debe ser mayor que cero."
            );

        var deleted = await _productService.DeleteAsync(id);

        if (!deleted)
            return Problem(
                statusCode: 404,
                title: "Producto no encontrado",
                detail: "No se encontró el producto solicitado."
            );

        return NoContent();
    }
}