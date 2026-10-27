using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Authorize(Roles = "Waiter,Admin")]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _orderService.GetAllAsync();
        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Waiter,Admin,User")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orderService.GetByIdAsync(id);

        if (order is null)
            return NotFound();

        return Ok(order);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Waiter,Admin")]
    public async Task<IActionResult> Update(int id, OrderCreateDto dto)
    {
        try
        {
            var order = await _orderService.UpdateAsync(id, dto);

            if (order is null)
                return NotFound();

            return Ok(order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/state")]
    [Authorize(Roles = "Waiter,Admin,Kitchen")]
    public async Task<IActionResult> ChangeState(int id, ChangeOrderStateDto dto)
    {
        try
        {
            var order = await _orderService.ChangeStateAsync(id, dto);

            if (order is null)
                return NotFound();

            return Ok(order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Waiter,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _orderService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "Waiter,Admin")]
    public async Task<IActionResult> Create(OrderCreateDto dto)
    {
        try
        {
            var order = await _orderService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = order.Id },
                order
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("Table/{id:int}")]
    [Authorize(Roles = "Waiter,Admin")]
    public async Task<IActionResult> GetOrdersByTableId(int id)
    {
        var orderDetails = await _orderService.GetOrdersByTableIdAsync(id);

        return Ok(orderDetails);
    }
    [HttpGet("State/InPreparation")]
    [Authorize(Roles = "Waiter,Admin,Kitchen")]
    public async Task<IActionResult> GetOrdersInPreparation()
    {
        var orderDetails = await _orderService.GetOrdersInPreparationAsync();

        return Ok(orderDetails);
    }
}