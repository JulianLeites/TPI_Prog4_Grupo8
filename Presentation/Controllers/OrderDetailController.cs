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

    public OrderDetailController(IOrderDetailService orderService)
    {
        _orderDetailService = orderService;
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
