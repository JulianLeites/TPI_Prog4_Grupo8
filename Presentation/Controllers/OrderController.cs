using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _orderService.GetAllAsync();
        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "waiter,admin,user")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID del pedido debe ser mayor que cero."
            );

        var order = await _orderService.GetByIdAsync(id);

        if (order is null)
            return Problem(
                statusCode: 404,
                title: "Pedido no encontrado",
                detail: "No se encontró el pedido solicitado."
            );

        return Ok(order);
    }

    [HttpPatch("{id:int}/state")]
    [Authorize(Roles = "waiter,admin,kitchen")]
    public async Task<IActionResult> ChangeState(int id, ChangeOrderStateDto dto)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID del pedido debe ser mayor que cero."
            );

        var order = await _orderService.ChangeStateAsync(id, dto);

        if (order is null)
            return Problem(
                statusCode: 404,
                title: "Pedido no encontrado",
                detail: "No se encontró el pedido solicitado."
            );

        return Ok(order);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID del pedido debe ser mayor que cero."
            );

        var deleted = await _orderService.DeleteAsync(id);

        if (!deleted)
            return Problem(
                statusCode: 404,
                title: "Pedido no encontrado",
                detail: "No se encontró el pedido solicitado."
            );

        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> Create(OrderCreateDto dto)
    {
        var order = await _orderService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = order.Id },
            order
        );
    }

    [HttpGet("Table/{id:int}")]
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> GetOrdersByTableId(int id)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID de la mesa debe ser mayor que cero."
            );

        var orderDetails = await _orderService.GetOrdersByTableIdAsync(id);
        
        return Ok(orderDetails);
    }
    [HttpGet("State/InPreparation")]
    [Authorize(Roles = "waiter,admin,kitchen")]
    public async Task<IActionResult> GetOrdersInPreparation()
    {
        var orderDetails = await _orderService.GetOrdersInPreparationAsync();

        return Ok(orderDetails);
    }
}