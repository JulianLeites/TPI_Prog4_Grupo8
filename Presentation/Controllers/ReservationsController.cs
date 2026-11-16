using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    [Authorize(Roles = "waiter,admin")]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetAll()
    {
        var reservations = await _service.GetAllAsync();
        return Ok(reservations);
    }

    [HttpGet("mine")]
    [Authorize(Roles = "user")]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetMine()
    {
        var loggedUserId = GetLoggedUserId();
        if (loggedUserId is null)
            return Unauthorized();

        var reservations = await _service.GetReservationsByUserIdAsync(loggedUserId.Value);
        return Ok(reservations);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "waiter,admin")]
    public async Task<ActionResult<ReservationDto>> GetById(int id)
    {
        var reservation = await _service.GetByIdAsync(id);

        if (reservation == null) 
            return NotFound();

        return Ok(reservation);
    }
    
    [HttpGet("available-tables")]
    [Authorize(Roles = "waiter,admin,user")]
    public async Task<ActionResult<IEnumerable<TableDto>>> GetAvailableTables(
        [FromQuery] DateOnly date,
        [FromQuery] TimeOnly time,
        [FromQuery] int diners = 1)
    {
        try
        {
            var tables = await _service.GetAvailableTablesAsync(date, time, diners);
            return Ok(tables);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "waiter,admin,user")]
    public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
    {
        if (User.IsInRole("user"))
        {
            var loggedUserId = GetLoggedUserId();
            if (loggedUserId is null)
                return Unauthorized();

            dto.UserId = loggedUserId.Value;
            dto.TableIds = new List<int>();
        }

        try
        {
            var created = await _service.CreateAsync(dto);

            if (User.IsInRole("user"))
                return Ok(created);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result) return NotFound();

        return NoContent();
    }

    [HttpPatch("{id:int}")]
    [Authorize(Roles = "waiter,admin,user")]
    public async Task<IActionResult> ChangeState(int id, ChangeReservationStateDto dto)
    {
        var existingReservation = await _service.GetByIdAsync(id);

        if (existingReservation is null)
            return NotFound();

        if (User.IsInRole("user"))
        {
            var loggedUserId = GetLoggedUserId();

            if (loggedUserId is null)
                return Unauthorized();

            if (existingReservation.UserId != loggedUserId.Value)
                return NotFound();

            if (dto.State != ReservationState.Cancelled)
                return Forbid();
        }

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
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("user/{userId:int}")]
    [Authorize(Roles = "waiter,admin")]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetByUserId(int userId)
    {
        var reservations = await _service.GetReservationsByUserIdAsync(userId);
        return Ok(reservations);
    }

    [HttpGet("range")]
    [Authorize(Roles = "waiter,admin")]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetByDateRange([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
    {
        try
        {
            var reservations = await _service.GetReservationsByDateRangeAsync(startDate, endDate);
            return Ok(reservations);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("state/{state}")]
    [Authorize(Roles = "waiter,admin")]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetByState(string state)
    {
        try
        {
            var reservations = await _service.GetReservationsByStateAsync(state);
            return Ok(reservations);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private int? GetLoggedUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out int userId))
            return null;

        return userId;
    }

    [HttpPost("walk-in")]
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> CreateWalkIn([FromBody] WalkInReservationDto dto)
    {
        var userId = GetLoggedUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            var created = await _service.CreateWalkInAsync(dto, userId.Value);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

}