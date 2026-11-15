using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrderDetailController : ControllerBase
{
    private readonly IOrderDetailService _orderDetailService;

    public OrderDetailController(IOrderDetailService orderDetailService)
    {
        _orderDetailService = orderDetailService;

    }

    [HttpGet]
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> GetAll()
    {
        var orderDetails = await _orderDetailService.GetAllAsync();
        return Ok(orderDetails);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID debe ser mayor que cero."
            );

        var orderDetail = await _orderDetailService.GetByIdAsync(id);

        if (orderDetail is null)
            return Problem(
                statusCode: 404,
                title: "Detalle no encontrado",
                detail: "No se encontró el detalle de pedido solicitado."
            );

        return Ok(orderDetail);
    }
        
    [HttpGet("Order/{id:int}")]
    [Authorize(Roles = "waiter,admin,user")]
    public async Task<IActionResult> GetDetailsByOrderGroupId(int id)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID del grupo de pedido debe ser mayor que cero."
            );

        var orderDetails = await _orderDetailService.GetDetailsByOrderGroupIdAsync(id);

        return Ok(orderDetails);
    }

    [HttpGet("Product/{id:int}")]
    public async Task<IActionResult> GetDetailsByProductId(int id)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID del producto debe ser mayor que cero."
            );

        var orderDetails = await _orderDetailService.GetDetailsByProductIdAsync(id);

        return Ok(orderDetails);
    }

    [HttpPatch("{id:int}/Quantity")]
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> ChangeQuantity(int id, OrderDetailQuantityDto dto)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID debe ser mayor que cero."
            );

        var orderDetail = await _orderDetailService.ChangeQuantityAsync(id, dto);

        if (orderDetail is null)
            return Problem(
                statusCode: 404,
                title: "Detalle no encontrado",
                detail: "No se encontró el detalle de pedido solicitado."
            );

        return Ok(orderDetail);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return Problem(
                statusCode: 400,
                title: "Identificador inválido",
                detail: "El ID debe ser mayor que cero."
            );

        var deleted = await _orderDetailService.DeleteAsync(id);

        if (!deleted)
            return Problem(
                statusCode: 404,
                title: "Detalle no encontrado",
                detail: "No se encontró el detalle de pedido solicitado."
            );

        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "waiter,admin")]
    public async Task<IActionResult> Create(CreateOrderDetailDto dto)
    {
        var orderDetail = await _orderDetailService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = orderDetail.Id },
            orderDetail
        );
    }
}
