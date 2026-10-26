using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderDetailController : ControllerBase
{
    private readonly IOrderDetailService _orderDetailService;

    public OrderDetailController(IOrderDetailService orderDetailService)
    {
        _orderDetailService = orderDetailService;

    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orderDetails = await _orderDetailService.GetAllAsync();
        return Ok(orderDetails);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var orderDetail = await _orderDetailService.GetByIdAsync(id);

        if (orderDetail is null)
            return NotFound();

        return Ok(orderDetail);
    }
        
    [HttpGet("Order/{id:int}")]
    public async Task<IActionResult> GetDetailsByOrderId(int id)
    {
        var orderDetails = await _orderDetailService.GetDetailsByOrderIdAsync(id);

        return Ok(orderDetails);
    }

    [HttpGet("Product/{id:int}")]
    public async Task<IActionResult> GetDetailsByProductId(int id)
    {
        var orderDetails = await _orderDetailService.GetDetailsByProductIdAsync(id);

        return Ok(orderDetails);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CreateOrderDetailDto dto)
    {
        try
        {
            var orderDetail = await _orderDetailService.UpdateAsync(id, dto);

            if (orderDetail is null)
                return NotFound();

            return Ok(orderDetail);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _orderDetailService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderDetailDto dto)
    {
        try
        {
            var orderDetail = await _orderDetailService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = orderDetail.Id },
                orderDetail
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
