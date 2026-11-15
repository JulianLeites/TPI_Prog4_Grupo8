using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "admin")]
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
            return Problem(
                statusCode: 404,
                title: "Rol no encontrado",
                detail: "No se encontró el rol solicitado."
            );

        return Ok(role);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, RoleUpdateDto dto)
    {
        var role = await _roleService.UpdateAsync(id, dto);

        if (role is null)
            return Problem(
                statusCode: 404,
                title: "Rol no encontrado",
                detail: "No se encontró el rol solicitado."
            );

        return Ok(role);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _roleService.DeleteAsync(id);

        if (!deleted)
            return Problem(
                statusCode: 404,
                title: "Rol no encontrado",
                detail: "No se encontró el rol solicitado."
            );

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> Create(RoleCreateDto dto)
    {
        var role = await _roleService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = role.Id },
            role
        );
    }
}
