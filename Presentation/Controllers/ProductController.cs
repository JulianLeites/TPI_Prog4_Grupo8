using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "admin")]
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
            return BadRequest(new { message = "El Id debe ser mayor que cero" });

        var product = await _productService.GetByIdAsync(id);

        if(product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] ProductDto dto)
    {
        try
        {
            var product = await _productService.CreateAsync(dto);


            return CreatedAtAction(
                nameof(GetById),
                new { id = product.Id},
                product
            );

        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message});
        }

    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update([FromRoute]int id, [FromBody] ProductDto dto)
    {
        if (id <= 0)
            return BadRequest(new { message = "El ID debe ser mayor que cero." });

        try
        {
            var product = await _productService.UpdateAsync(id, dto);

            if(product is null)
            {
                return NotFound();
            }

            return Ok(product);


        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message});
        }

    }

    [HttpPatch("{id:int}/price")]
    public async Task<ActionResult> ChangePrice([FromRoute]int id, [FromBody] ProductPriceDto dto)
    {
        if (id <= 0)
            return BadRequest(new { message = "El ID debe ser mayor que cero." });

        try
        {
            var product = await _productService.ChangePriceAsync(id, dto);

            if(product is null)
                return NotFound();

            return Ok(product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

    }

    [HttpGet("ProductCategory/{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetProductsByCategoryId(int id)
    {
        var products = await _productService.GetProductsByCategoryIdAsync(id);
        return Ok(products); 
    }

    [HttpGet("search/{terms}")]
    public async Task<ActionResult> GetProductsByName(string terms)
    {
        var products = await _productService.GetProductsByNameAsync(terms);
        return Ok(products); 
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "El ID debe ser mayor que cero." });

        var deleted = await _productService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

}