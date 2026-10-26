using Application.Models;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("{id:int}")]
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _userService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpPost]
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

    [HttpPatch("{id:int}/password")]
    public async Task<IActionResult> ChangePassword(
        int id,
        ChangePasswordDto dto)
    {
        var changed = await _userService.ChangePasswordAsync(id, dto);

        if (!changed)
            return BadRequest(new
            {
                message = "No se pudo cambiar la contraseña. Verificá el usuario y la contraseña actual"
            });

        return NoContent();
    }
}
