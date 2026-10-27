using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _service;

    public ReservationsController(IReservationService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Waiter,Admin")]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetAll()
    {
        var reservations = await _service.GetAllAsync();
        return Ok(reservations);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Waiter,Admin,User")]
    public async Task<ActionResult<ReservationDto>> GetById(int id)
    {
        var reservation = await _service.GetByIdAsync(id);
        if (reservation == null) return NotFound();

        return Ok(reservation);
    }

    [HttpPost]
    [Authorize(Roles = "Waiter,Admin,User")]
    public async Task<ActionResult<ReservationDto>> Create([FromBody] CreateReservationDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Waiter,Admin,User")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateReservationDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        if (!result) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result) return NotFound();

        return NoContent();
    }

    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Waiter,Admin,User")]
    public async Task<IActionResult> ChangeState(
        int id,
        ChangeReservationStateDto dto)
    {
        try
        {
            var reservation = await _service.ChangeStateAsync(id, dto);

            if (reservation is null)
                return NotFound();

            return Ok(reservation);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}