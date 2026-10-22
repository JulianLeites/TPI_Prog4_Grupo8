using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _roleService.GetAllAsync();
        return Ok(roles);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var role = await _roleService.GetByIdAsync(id);

        if (role is null)
            return NotFound();

        return Ok(role);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, RoleUpdateDto dto)
    {
        try
        {
            var role = await _roleService.UpdateAsync(id, dto);

            if (role is null)
                return NotFound();
            return Ok(role);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _roleService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> Create(RoleCreateDto dto)
    {
        try
        {
            var role = await _roleService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = role.Id },
                role
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message});
        }
    }
}
