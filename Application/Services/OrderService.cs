using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Enums;

namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ITableRepository _tableRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderDetailRepository _orderDetailRepository;
    public OrderService(IOrderRepository repository, ITableRepository tableRepository, IProductRepository productRepository, IOrderDetailRepository orderDetailRepository)
    {
        _tableRepository = tableRepository;
        _orderRepository = repository;
        _productRepository = productRepository;
        _orderDetailRepository = orderDetailRepository;
    } 
    public async Task<IEnumerable<OrderDto>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllAsync();

        return orders.Select(order => new OrderDto
        {
            Id = order.Id,
            Total = order.Total,
            Table_id = order.Table_id,
            State = order.State,
            Details = order.Details.Select(d => new OrderDetailDto
            {
                Id = d.Id,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Subtotal = d.Subtotal,
                Product_id = d.Product_id,
                Order_id = d.Order_id
            }).ToList()
        });
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
            return null;

        return new OrderDto
        {
            Id = order.Id,
            Total = order.Total,
            Table_id = order.Table_id,
            State = order.State,
            Details = order.Details.Select(d => new OrderDetailDto
            {
                Id = d.Id,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Subtotal = d.Subtotal,
                Product_id = d.Product_id,
                Order_id = d.Order_id
            }).ToList()
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
            return false;

        await _orderRepository.DeleteWithDetailsAsync(order);

        return true;
    }

    public async Task<OrderDto> CreateAsync(OrderCreateDto dto)
    {
        if (dto is null)
            throw new ArgumentNullException(nameof(dto));

        if (dto.Quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero");

        var table = await _tableRepository.GetByIdAsync(dto.Table_id);

        if(table is null){
            throw new ArgumentException($"La mesa con id {dto.Table_id} no existe");
        }

        if (table.State != TableState.Occupied)
            throw new ArgumentException("No se pueden crear pedidos en mesas desocupadas");

        var product = await _productRepository.GetByIdAsync(dto.Product_id);

        if(product is null)
            throw new ArgumentException($"El producto con id {dto.Product_id} no existe");

        var detail = new OrderDetail
        {
            Quantity = dto.Quantity,
            UnitPrice = product.Price,
            Product_id = dto.Product_id,
        };

        detail.Subtotal = detail.CalculateSubtotal();

        var order = new Order
        {
            Table_id = dto.Table_id,
            Details = new List<OrderDetail> { detail }
        };

        order.Total = order.CalculateTotal();

        await _orderRepository.AddAsync(order);

        return new OrderDto
        {
            Id = order.Id,
            Total = order.Total,
            Table_id = order.Table_id,
            State = order.State,
            Details = new List<OrderDetailDto>
            {
                new OrderDetailDto
                {
                    Id = detail.Id,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    Subtotal = detail.Subtotal,
                    Product_id = detail.Product_id,
                    Order_id = detail.Order_id
                }
            }
        };
    }

    public async Task<OrderDto?> ChangeStateAsync(int id, ChangeOrderStateDto dto)
    {
        
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null){
            return null;
        }

        order.State = dto.State;

        await _orderRepository.UpdateAsync(order);

        return new OrderDto
        {
            Id = order.Id,
            Total = order.Total,
            Table_id = order.Table_id,
            State = order.State
        };
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByTableIdAsync(int tableId)
    {
        var orders = await _orderRepository.GetOrdersByTableIdAsync(tableId);

        return orders.Select(order => new OrderDto
        {
            Id = order.Id,
            Total = order.Total,
            Table_id = order.Table_id,
            State = order.State,
            Details = order.Details.Select(d => new OrderDetailDto
            {
                Id = d.Id,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Subtotal = d.Subtotal,
                Product_id = d.Product_id,
                Order_id = d.Order_id
            }).ToList()
        });
    }
    public async Task<IEnumerable<OrderDto>> GetOrdersInPreparationAsync()
    {
        var orders = await _orderRepository.GetOrdersInPreparationAsync();

        return orders.Select(order => new OrderDto
        {
            Id = order.Id,
            Total = order.Total,
            Table_id = order.Table_id,
            State = order.State,
            Details = order.Details.Select(d => new OrderDetailDto
            {
                Id = d.Id,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Subtotal = d.Subtotal,
                Product_id = d.Product_id,
                Order_id = d.Order_id
            }).ToList()
        });
    }
}