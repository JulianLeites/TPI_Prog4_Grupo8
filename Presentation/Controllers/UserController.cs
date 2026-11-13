using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll( [FromQuery] int? roleId)
    {
        if (roleId.HasValue)
        {
            if (roleId.Value <= 0)
                return BadRequest("Id de rol Invalido");

            var usersByRole = await _userService.GetByRoleIdAsync(roleId.Value);
            return Ok(usersByRole);
        }

        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim is null)
            return Unauthorized();

        if (!int.TryParse(userIdClaim.Value, out int userId))
            return Unauthorized();

        var user = await _userService.GetByIdAsync(userId);

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, UserUpdateDto dto)
    {
        try
        {
            var user = await _userService.UpdateAsync(id, dto);

            if (user is null)
                return NotFound();

            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message =  ex.Message });
        }
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UserUpdateDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim is null ||
            !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized();
        }

        try
        {
            var user = await _userService.UpdateAsync(userId, dto);

            if (user is null)
                return NotFound();

            return Ok(user);
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
        var deleted = await _userService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim is null ||
            !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized();
        }

        var deleted = await _userService.DeleteAsync(userId);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create(UserCreateDto dto)
    {
        try
        {
            var user = await _userService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = user.Id},
                user
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("me/password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim is null ||
            !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized();
        }

        var changed = await _userService.ChangePasswordAsync(userId, dto);

        if (!changed)
            return BadRequest(new
            {
                message = "No se pudo cambiar la contraseña. Verificá el usuario y la contraseña actual"
            });

        return NoContent();
    }

    [HttpPatch("{id}/role")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ChangeRole(int id, ChangeRoleDto dto)
    {
        try
        {
            var user = await _userService.ChangeRoleAsync(id, dto);

            if (user is null)
                return NotFound();

            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
