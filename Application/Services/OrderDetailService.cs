using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class OrderDetailService : IOrderDetailService
{
    private readonly IOrderDetailRepository _orderDetailRepository;
/* 
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
 */

    public OrderDetailService( IOrderDetailRepository repository)
    {
        _orderDetailRepository = repository;
    } 

    public async Task<IEnumerable<OrderDetailDto>> GetAllAsync()
    {
        var ordersDetails = await _orderDetailRepository.GetAllAsync();

        return ordersDetails.Select(orderDetail => new OrderDetailDto
        {
            Id = orderDetail.Id,
            Quantity = orderDetail.Quantity,
            UnitPrice = orderDetail.UnitPrice,
            Subtotal = orderDetail.Subtotal,
/*          Product_id = orderDetail.Product_id,
            Order_id = orderDetail.Order_id */
        });
    }

    public async Task<OrderDetailDto?> GetByIdAsync(int id)
    {
        var orderDetail = await _orderDetailRepository.GetByIdAsync(id);

        if (orderDetail is null)
            return null;

        return new OrderDetailDto
        {
            Id = orderDetail.Id,
            Quantity = orderDetail.Quantity,
            UnitPrice = orderDetail.UnitPrice,
            Subtotal = orderDetail.Subtotal,
/*          Product_id = orderDetail.Product_id,
            Order_id = orderDetail.Order_id */
        };
    }

    public async Task<OrderDetailDto?> UpdateAsync(int id, CreateOrderDetailDto dto)
    {
        var orderDetail = await _orderDetailRepository.GetByIdAsync(id);

        if (orderDetail is null)
            return null;

        orderDetail.Quantity = dto.Quantity;
        orderDetail.UnitPrice = dto.UnitPrice;
        orderDetail.Subtotal = dto.UnitPrice;
/*      orderDetail.Product_id = dto.Product_id;
        orderDetail.Order_id = dto.Order_id; */


        await _orderDetailRepository.UpdateAsync(orderDetail);

        return new OrderDetailDto
        {
            Id = orderDetail.Id,
            Quantity = orderDetail.Quantity,
            UnitPrice = orderDetail.UnitPrice,
            Subtotal = orderDetail.Subtotal,
/*          Product_id = orderDetail.Product_id,
            Order_id = orderDetail.Order_id */
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var orderDetail = await _orderDetailRepository.GetByIdAsync(id);

        if (orderDetail is null)
            return false;

        await _orderDetailRepository.DeleteAsync(orderDetail);
        return true;
    }

    public async Task<OrderDetailDto> CreateAsync(CreateOrderDetailDto dto)
    {
        var orderDetail = new OrderDetail
        {
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice,
            Subtotal = dto.Subtotal,
/*             Product_id = dto.Product_id,
            Order_id = dto.Order_id */
        };

        await _orderDetailRepository.AddAsync(orderDetail);

        return new OrderDetailDto
        {
            Id = orderDetail.Id,
            Quantity = orderDetail.Quantity,
            UnitPrice = orderDetail.UnitPrice,
            Subtotal = orderDetail.Subtotal,
/*          Product_id = orderDetail.Product_id,
            Order_id = orderDetail.Order_id */
        };
    }
}