using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "waiter,admin")]
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
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID debe ser mayor que cero."
            );

        var category = await _service.GetByIdAsync(id);

        if (category is null)
            return Problem(
                statusCode: 404,
                title: "Categoría no encontrada",
                detail: "No se encontró la categoría solicitada."
            );

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<ProductCategoryDto>> Create([FromBody] CreateProductCategoryDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCategoryDto dto)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID debe ser mayor que cero."
            );

        var result = await _service.UpdateAsync(id, dto);

        if (!result)
            return Problem(
                statusCode: 404,
                title: "Categoría no encontrada",
                detail: "No se encontró la categoría solicitada."
            );

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID debe ser mayor que cero."
            );

        var result = await _service.DeleteAsync(id);

        if (!result)
            return Problem(
                statusCode: 404,
                title: "Categoría no encontrada",
                detail: "No se encontró la categoría solicitada."
            );

        return NoContent();
    }

    [HttpGet("{id:int}/with-products")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductCategoryWithProductsDto>> GetCategoryWithProducts(int id)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID debe ser mayor que cero."
            );

        var category = await _service.GetCategoryWithProductsAsync(id);

        if (category is null)
            return Problem(
                statusCode: 404,
                title: "Categoría no encontrada",
                detail: "No se encontró la categoría solicitada."
            );

        return Ok(category);
    }
}