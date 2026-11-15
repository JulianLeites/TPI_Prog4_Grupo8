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
    public OrderService(IOrderRepository repository, ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
        _orderRepository = repository;
    } 
    public async Task<IEnumerable<OrderDto>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllAsync();

        return orders.Select(MapToDto);
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
            return null;

        return MapToDto(order);
    }

    public async Task<OrderDto> CreateAsync(OrderCreateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var table = await _tableRepository.GetByIdAsync(dto.Table_id);

        if(table is null){
            throw new ArgumentException($"La mesa con id {dto.Table_id} no existe");
        }

        if (table.State != TableState.Occupied)
            throw new InvalidOperationException("No se pueden crear pedidos en mesas desocupadas");

        var existingOrder = await _orderRepository.GetActiveOrderByTableIdAsync(dto.Table_id);

        if (existingOrder is not null)
            throw new InvalidOperationException("La mesa ya tiene una orden activa");

        var order = new Order
        {
            Table_id = dto.Table_id,
            State = OrderState.Active,
            Total = 0
        };

        await _orderRepository.AddAsync(order);

        return MapToDto(order);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByTableIdAsync(int tableId)
    {
        var orders = await _orderRepository.GetOrdersByTableIdAsync(tableId);

        return orders.Select(MapToDto);
    }

    public async Task<OrderDto?> ChangeStateAsync(int id, ChangeOrderStateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
            return null;

        if (order.State == OrderState.Closed)
            throw new InvalidOperationException(
                "La orden ya se encuentra cerrada.");

        if (dto.State != OrderState.Closed)
            throw new InvalidOperationException(
                "La única transición permitida es Active → Closed.");

        if (order.OrderGroups.Any(
            group => group.State != OrderGroupState.Delivered))
        {
            throw new InvalidOperationException(
                "No se puede cerrar la orden mientras existan grupos pendientes de entrega.");
        }

        var table = await _tableRepository.GetByIdAsync(order.Table_id);

        if (table is null)
            throw new InvalidOperationException(
                "No se encontró la mesa asociada a la orden.");

        await _orderRepository.CloseOrderAndFreeTableAsync(order, table);

        return MapToDto(order);
    }

    private static OrderDto MapToDto(Order order)
    {
        var details = order.OrderGroups
            .SelectMany(group => group.Details)
            .GroupBy(detail => new
            {
                detail.Product_id,
                detail.UnitPrice
            })
            .Select(group => new OrderProductSummaryDto
            {
                Product_id = group.Key.Product_id,
                UnitPrice = group.Key.UnitPrice,
                Quantity = group.Sum(detail => detail.Quantity),
                Subtotal = group.Sum(detail => detail.Subtotal)
            })
            .ToList();

        return new OrderDto
        {
            Id = order.Id,
            Table_id = order.Table_id,
            State = order.State,
            Total = details.Sum(detail => detail.Subtotal),
            Details = details
        };
    }
}