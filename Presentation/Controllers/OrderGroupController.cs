using Application.Interfaces;
using Application.Models;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]

public class OrderGroupController : ControllerBase
{
    private readonly IOrderGroupService _orderGroupService;

    public OrderGroupController(IOrderGroupService orderGroupService)
    {
        _orderGroupService = orderGroupService;
    }

    [HttpGet]
    [Authorize(Roles = "waiter,admin,kitchen")]
    public async Task<IActionResult> GetAll()
    {
        var groups = await _orderGroupService.GetByStateAsync(OrderGroupState.InPreparation);

        return Ok(groups);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador Invalido",
                detail: "El ID del grupo debe ser mayor que cero"
            );

        var group = await _orderGroupService.GetByIdAsync(id);

        if (group is null)
            return Problem(
                statusCode: 404,
                title: "Grupo no encontrado",
                detail: "No se encontro el grupo"
            );

        return Ok(group);
    }

    [HttpGet("Order/{orderId:int}")]
    [Authorize(Roles = "waiter,admin,user")]
    public async Task<IActionResult> GetByOrderId(int orderId)
    {
        if (orderId <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID de la orden debe ser mayor que cero."
            );

        var groups = await _orderGroupService.GetByOrderIdAsync(orderId);

        return Ok(groups);
    }

    [HttpGet("State/{state}")]
    [Authorize(Roles = "waiter,admin,kitchen")]
    public async Task<IActionResult> GetByState(OrderGroupState state)
    {
        if (!Enum.IsDefined(state))
            return Problem(
                statusCode: 400,
                title: "Estado inválido",
                detail: "El estado del grupo indicado no es válido."
            );

        var groups = await _orderGroupService.GetByStateAsync(state);

        return Ok(groups);
    }

    [HttpPost("Order/{orderId:int}")]
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> Create(int orderId)
    {
        if (orderId <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID de la orden debe ser mayor que cero."
            );

        var group = await _orderGroupService.CreateAsync(orderId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = group.Id },
            group
        );
    }

    [HttpPatch("{id:int}/State")]
    [Authorize(Roles = "kitchen,admin")]
    public async Task<IActionResult> ChangeState(
       int id,
       [FromBody] OrderGroupState newState)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID del grupo debe ser mayor que cero."
            );

        if (!Enum.IsDefined(newState))
            return Problem(
                statusCode: 400,
                title: "Estado inválido",
                detail: "El estado indicado no es válido."
            );

        var group = await _orderGroupService.ChangeStateAsync(id, newState);

        if (group is null)
            return Problem(
                statusCode: 404,
                title: "Grupo no encontrado",
                detail: "No se encontró el grupo solicitado."
            );

        return Ok(group);
    }
}
