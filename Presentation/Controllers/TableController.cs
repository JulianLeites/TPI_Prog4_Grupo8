using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TableController : ControllerBase
{
    private readonly ITableService _tableService;

    public TableController(ITableService tableService)
    {
        _tableService = tableService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tables = await _tableService.GetAllAsync();
        return Ok(tables);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var table = await _tableService.GetByIdAsync(id);

        if (table is null)
            return NotFound();

        return Ok(table);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TableUpdateDto dto)
    {
        try
        {
            var table = await _tableService.UpdateAsync(id, dto);

            if (table is null)
                return NotFound();

            return Ok(table);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/state")]
    public async Task<IActionResult> ChangeState(
        int id,
        ChangeTableStateDto dto)
    {
        try
        {
            var table = await _tableService.ChangeStateAsync(id, dto);

            if (table is null)
                return NotFound();

            return Ok(table);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _tableService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> Create(TableCreateDto dto)
    {
        try
        {
            var table = await _tableService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = table.Id },
                table
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
