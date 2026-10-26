using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using System.Linq;

namespace Application.Services;

public class OrderDetailService : IOrderDetailService
{
    private readonly IOrderDetailRepository _orderDetailRepository; 
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
 

    public OrderDetailService( IOrderDetailRepository repository, IOrderRepository repository1, IProductRepository repository2)
    {
        _orderDetailRepository = repository;
        _orderRepository = repository1;
        _productRepository = repository2;
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
            Product_id = orderDetail.Product_id,
            Order_id = orderDetail.Order_id 
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
            Product_id = orderDetail.Product_id,
            Order_id = orderDetail.Order_id 
        };
    }

    public async Task<IEnumerable<OrderDetailDto>> GetDetailsByOrderIdAsync(int id)
    {
        var orderDetails = await _orderDetailRepository.GetDetailsByOrderIdAsync(id);

        return orderDetails.Select(orderDetail => new OrderDetailDto
        {
            Id = orderDetail.Id,
            Quantity = orderDetail.Quantity,
            UnitPrice = orderDetail.UnitPrice,
            Subtotal = orderDetail.Subtotal,
            Product_id = orderDetail.Product_id,
            Order_id = orderDetail.Order_id 
        });
    }

    public async Task<IEnumerable<OrderDetailDto>> GetDetailsByProductIdAsync(int id)
    {
        var orderDetails = await _orderDetailRepository.GetDetailsByProductIdAsync(id);

        return orderDetails.Select(orderDetail => new OrderDetailDto
        {
            Id = orderDetail.Id,
            Quantity = orderDetail.Quantity,
            UnitPrice = orderDetail.UnitPrice,
            Subtotal = orderDetail.Subtotal,
            Product_id = orderDetail.Product_id,
            Order_id = orderDetail.Order_id 
        });
    }
    public async Task<OrderDetailDto?> UpdateAsync(int id, CreateOrderDetailDto dto)
    {
        var orderDetail = await _orderDetailRepository.GetByIdAsync(id);

        if (orderDetail is null)
            return null;

        var order = await _orderRepository.GetByIdAsync(orderDetail.Order_id);

        orderDetail.Quantity = dto.Quantity;
        orderDetail.Subtotal = orderDetail.CalculateSubtotal();

        await _orderDetailRepository.UpdateAsync(orderDetail);
        order.Details = await _orderDetailRepository.GetDetailsByOrderIdAsync(order.Id);

        order.Total = order.CalculateTotal();

        await _orderRepository.UpdateAsync(order);
        return new OrderDetailDto
        {
            Id = orderDetail.Id,
            Quantity = orderDetail.Quantity,
            UnitPrice = orderDetail.UnitPrice,
            Subtotal = orderDetail.Subtotal,
            Product_id = orderDetail.Product_id,
            Order_id = orderDetail.Order_id
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var orderDetail = await _orderDetailRepository.GetByIdAsync(id);
        if (orderDetail is null)
            return false;
            
        var order = await _orderRepository.GetByIdAsync(orderDetail.Order_id);

        await _orderDetailRepository.DeleteAsync(orderDetail);
        order.Details = await _orderDetailRepository.GetDetailsByOrderIdAsync(order.Id);

        order.Total = order.CalculateTotal();

        await _orderRepository.UpdateAsync(order);
        return true;
    }

    public async Task<OrderDetailDto> CreateAsync(CreateOrderDetailDto dto)
    {

        var product = await _productRepository.GetByIdAsync(dto.Product_id);
        if(product is null){
            throw new ArgumentException($"El producto con id {dto.Product_id} no existe");
        }
        var order = await _orderRepository.GetByIdAsync(dto.Order_id);
        if(order is null){
            throw new ArgumentException($"La orden con id {dto.Order_id} no existe");
        }

        var orderDetail = new OrderDetail
        {
            Quantity = dto.Quantity,
            UnitPrice = product.Price,
            Product_id = dto.Product_id,
            Order_id = dto.Order_id 
        };

        orderDetail.Subtotal = orderDetail.CalculateSubtotal();

        await _orderDetailRepository.AddAsync(orderDetail);

        order.Details = await _orderDetailRepository.GetDetailsByOrderIdAsync(order.Id);

        order.Total = order.CalculateTotal();

        await _orderRepository.UpdateAsync(order);

        return new OrderDetailDto
        {
            Id = orderDetail.Id,
            Quantity = orderDetail.Quantity,
            UnitPrice = orderDetail.UnitPrice,
            Subtotal = orderDetail.Subtotal,
            Product_id = orderDetail.Product_id,
            Order_id = orderDetail.Order_id
        };
    }
}