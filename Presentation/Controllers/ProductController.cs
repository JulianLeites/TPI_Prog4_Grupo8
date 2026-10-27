using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class ProudctController : ControllerBase
{
    private readonly IProductService _productService;

    public ProudctController(IProductService productService)
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
        var product = await _productService.GetByIdAsync(id);
        if(product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]

    public async Task<ActionResult> Create(ProductDto dto)
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

    [HttpPut]

    public async Task<ActionResult> Update(int id, ProductDto dto)
    {
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

    [HttpDelete]

    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _productService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }

}