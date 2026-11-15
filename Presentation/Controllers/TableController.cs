using Application.Interfaces;
using Application.Models;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
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
            return Problem(
                statusCode: 400,
                title: "Estado inválido",
                detail: "El estado de mesa indicado no es válido."
            );

        var tables = await _tableService.GetAllAsync(state);
        return Ok(tables);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID debe ser mayor que cero."
            );

        var table = await _tableService.GetByIdAsync(id);

        if (table is null)
            return Problem(
                statusCode: 404,
                title: "Mesa no encontrada",
                detail: "No se encontró la mesa solicitada."
            );

        return Ok(table);
    }

    [HttpGet("number/{number:int}")]
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> GetByNumber([FromRoute] int number)
    {
        if (number <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El número de mesa debe ser mayor que cero."
            );

        var table = await _tableService.GetByNumberAsync(number);

        if (table is null)
            return Problem(
                statusCode: 404,
                title: "Mesa no encontrada",
                detail: "No se encontró la mesa solicitada."
            );

        return Ok(table);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TableUpdateDto dto)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID debe ser mayor que cero."
            );

        var table = await _tableService.UpdateAsync(id, dto);

        if (table is null)
            return Problem(
                statusCode: 404,
                title: "Mesa no encontrada",
                detail: "No se encontró la mesa solicitada."
            );

        return Ok(table);
    }

    [HttpPatch("{id:int}/state")]
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> ChangeState([FromRoute] int id,[FromBody] ChangeTableStateDto dto)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID debe ser mayor que cero."
            );

        var table = await _tableService.ChangeStateAsync(id, dto);

        if (table is null)
            return Problem(
                statusCode: 404,
                title: "Mesa no encontrada",
                detail: "No se encontró la mesa solicitada."
            );

        return Ok(table);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID debe ser mayor que cero."
            );

        var deleted = await _tableService.DeleteAsync(id);

        if (!deleted)
            return Problem(
                statusCode: 404,
                title: "Mesa no encontrada",
                detail: "No se encontró la mesa solicitada."
            );


        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] TableCreateDto dto)
    {
        var table = await _tableService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = table.Id },
            table
        );
    }
}
