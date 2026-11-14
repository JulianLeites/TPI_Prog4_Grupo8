using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services;

public class OrderDetailService : IOrderDetailService
{
    private readonly IOrderDetailRepository _orderDetailRepository; 
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ITableRepository _tableRepository;

    public OrderDetailService( IOrderDetailRepository orderDetailRepository, IOrderRepository orderRepository, IProductRepository productRepository, ITableRepository tableRepository)
    {
        _orderDetailRepository = orderDetailRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _tableRepository = tableRepository;
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
    
    public async Task<OrderDetailDto?> UpdateAsync(int id, OrderDetailUpdateDto dto)
    {
        var orderDetail = await _orderDetailRepository.GetByIdAsync(id);

        if (orderDetail is null)
            return null;

        var product = await _productRepository.GetByIdAsync(dto.Product_id);
        var order = await _orderRepository.GetByIdAsync(orderDetail.Order_id);

        if (order is null)
            throw new InvalidOperationException($"El pedido con id {orderDetail.Order_id} no existe");

        await ValidateOrderTableAsync(order);

        if (product is null)
            throw new ArgumentException($"El producto con id {dto.Product_id} no existe");
        
        if (dto.Quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a 0");

        orderDetail.Product_id = dto.Product_id;
        orderDetail.Quantity = dto.Quantity;
        orderDetail.UnitPrice = product.Price;
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

    public async Task<OrderDetailDto?> ChangeQuantityAsync(int id, OrderDetailQuantityDto dto)
    {
        var orderDetail = await _orderDetailRepository.GetByIdAsync(id);

        if (orderDetail is null)
            return null;
        if (dto.Quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a 0");

        var order = await _orderRepository.GetByIdAsync(orderDetail.Order_id);

        if (order is null)
            throw new InvalidOperationException($"El pedido con id {orderDetail.Order_id} no existe");

        await ValidateOrderTableAsync(order);

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

        if (order is null)
            return false;

        var details = await _orderDetailRepository.GetDetailsByOrderIdAsync(order.Id);

        await _orderDetailRepository.DeleteAsync(orderDetail);

        if(details.Count == 1)
        {
            await _orderRepository.DeleteAsync(order);
            return true;
        }

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

        await ValidateOrderTableAsync(order);
        
        if (dto.Quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a 0");

        var existingDetail = await _orderDetailRepository.GetExistingDetailAsync(dto.Order_id, dto.Product_id);
        
        if(existingDetail != null){
            existingDetail.Quantity += dto.Quantity;
            existingDetail.Subtotal = existingDetail.CalculateSubtotal();

            await _orderDetailRepository.UpdateAsync(existingDetail);

            order.Details = await _orderDetailRepository.GetDetailsByOrderIdAsync(order.Id);
            order.Total = order.CalculateTotal();

            await _orderRepository.UpdateAsync(order);

            return new OrderDetailDto
            {
                Id = existingDetail.Id,
                Quantity = existingDetail.Quantity,
                UnitPrice = existingDetail.UnitPrice,
                Subtotal = existingDetail.Subtotal,
                Product_id = existingDetail.Product_id,
                Order_id = existingDetail.Order_id
            };
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

    private async Task ValidateOrderTableAsync(Order order)
    {
        var table = await _tableRepository.GetByIdAsync(order.Table_id);

        if (table is null)
            throw new InvalidOperationException($"La mesa con id {order.Table_id} no existe");

        if (table.State != TableState.Occupied)
            throw new InvalidOperationException("No se pueden modificar detalles de un pedido cuya mesa no está ocupada");
    }
}