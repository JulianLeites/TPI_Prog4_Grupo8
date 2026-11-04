using Application.Interfaces;
using Application.Models;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TableController : ControllerBase
{
    private readonly ITableService _tableService;

    public TableController(ITableService tableService)
    {
        _tableService = tableService;
    }

    [HttpGet]
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> GetAll([FromQuery] TableState? state)
    {
        if (state.HasValue && !Enum.IsDefined(state.Value))
            return BadRequest(new { message = "Estado de mesa invalido" });

        var tables = await _tableService.GetAllAsync(state);
        return Ok(tables);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var table = await _tableService.GetByIdAsync(id);

        if (table is null)
            return NotFound();

        return Ok(table);
    }

    [HttpGet("number/{number:int}")]
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> GetByNumber(int number)
    {
        var table = await _tableService.GetByNumberAsync(number);

        if (table is null)
            return NotFound();

        return Ok(table);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin")]
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
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> ChangeState(int id,ChangeTableStateDto dto)
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
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _tableService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
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
