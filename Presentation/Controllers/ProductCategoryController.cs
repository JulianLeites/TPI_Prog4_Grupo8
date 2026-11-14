using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class ProductCategoriesController : ControllerBase
{
    private readonly IProductCategoryService _service;

    public ProductCategoriesController(IProductCategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductCategoryDto>>> GetAll()
    {
        var categories = await _service.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductCategoryDto>> GetById(int id)
    {
        var category = await _service.GetByIdAsync(id);
        if (category == null) return NotFound();

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<ProductCategoryDto>> Create([FromBody] CreateProductCategoryDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCategoryDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (!result) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }

        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}/with-products")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductCategoryWithProductsDto>> GetCategoryWithProducts(int id)
    {
        var category = await _service.GetCategoryWithProductsAsync(id);
        if (category is null) return NotFound();

        return Ok(category);
    }
}