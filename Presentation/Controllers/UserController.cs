using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

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
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll( [FromQuery, Range(1, int.MaxValue)] int? roleId)
    {
        if (roleId.HasValue)
        {
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
            return Problem(
                statusCode: 404,
                title: "Recurso no encontrado",
                detail: "No se encontró el usuario solicitado."
            );

        return Ok(user);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out int userId))
            return Unauthorized();

        var user = await _userService.GetByIdAsync(userId);

        if (user is null)
            return Problem(
                statusCode: 404,
                title: "Recurso no encontrado",
                detail: "No se encontró el usuario solicitado."
            );

        return Ok(user);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, UserUpdateDto dto)
    {
        var user = await _userService.UpdateAsync(id, dto);

        if (user is null)
            return Problem(
                statusCode: 404,
                title: "Recurso no encontrado",
                detail: "No se encontró el usuario solicitado."
            );

        return Ok(user);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UserUpdateDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out int userId))
            return Unauthorized();

        var user = await _userService.UpdateAsync(userId, dto);

        if (user is null)
            return Problem(
                statusCode: 404,
                title: "Recurso no encontrado",
                detail: "No se encontró el usuario solicitado."
            );

        return Ok(user);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _userService.DeleteAsync(id);

        if (!deleted)
            return Problem(
                statusCode: 404,
                title: "Recurso no encontrado",
                detail: "No se encontró el usuario solicitado."
            );

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

        try
        {
            var deleted = await _userService.DeleteAsync(userId);

        if (!deleted)
            return Problem(
                statusCode: 404,
                title: "Recurso no encontrado",
                detail: "No se encontró el usuario solicitado."
            );

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create(UserCreateDto dto)
    {
        var user = await _userService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = user.Id},
            user
        );
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
            return Problem(
                statusCode: 400,
                title: "No se pudo cambiar la contraseña",
                detail: "Verifica la contraseña actual y volve a intentarlo"
            );

        return NoContent();
    }

    [HttpPatch("{id}/role")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ChangeRole(int id, ChangeRoleDto dto)
    {
        var user = await _userService.ChangeRoleAsync(id, dto);

        if (user is null)
            return Problem(
                statusCode: 404,
                title: "Recurso no encontrado",
                detail: "No se encontró el usuario solicitado."
            );

        return Ok(user);
    }
}
