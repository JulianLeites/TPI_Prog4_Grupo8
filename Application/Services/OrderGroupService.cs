using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services;

public class OrderGroupService : IOrderGroupService
{
    private readonly IOrderGroupRepository _orderGroupRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ITableRepository _tableRepository;

    public OrderGroupService(
        IOrderGroupRepository orderGroupRepository,
        IOrderRepository orderRepository,
        ITableRepository tableRepository)
    {
        _orderGroupRepository = orderGroupRepository;
        _orderRepository = orderRepository;
        _tableRepository = tableRepository;
    }

    public async Task<IEnumerable<OrderGroupDto>> GetByOrderIdAsync(int orderId)
    {
        var groups = await _orderGroupRepository.GetByOrderIdAsync(orderId);

        return groups.Select(MapToDto);
    }
    public async Task<IEnumerable<OrderGroupDto>> GetByStateAsync(OrderGroupState? state)
    {
        if (state.HasValue && !Enum.IsDefined(state.Value))
            throw new ArgumentException("El estado indicado no es valido");

        var groups = state.HasValue
            ? await _orderGroupRepository.GetByStateAsync(state.Value)
            : await _orderGroupRepository.GetAllAsync();

        return groups.Select(MapToDto);
    }
    public async Task<OrderGroupDto?> GetByIdAsync(int id)
    {
        var group = await _orderGroupRepository.GetByIdWithDetailsAsync(id);

        if (group is null)
            return null;

        return MapToDto(group);
    }
    public async Task<OrderGroupDto> CreateAsync(int orderId)
    {
        if(orderId <= 0)
            throw new ArgumentException("El Id de la orden debe ser mayor que cero");

        var order = await _orderRepository.GetByIdAsync(orderId);

        if(order is null)
            throw new ArgumentException($"La orden con ID {orderId} no existe.");

        if (order.State != OrderState.Active)
            throw new InvalidOperationException("No se pueden crear grupos en una orden cerrada.");
    
        var table = await _tableRepository.GetByIdAsync(order.Table_id);

        if (table is null || table.State != TableState.Occupied)
            throw new InvalidOperationException("No se pueden crear grupos si la mesa no está ocupada.");

        var group = new OrderGroup
        {
            OrderId = orderId,
            State = OrderGroupState.InPreparation
        };

        await _orderGroupRepository.AddAsync(group);

        return MapToDto(group);
    }
    public async Task<OrderGroupDto?> ChangeStateAsync(int id, OrderGroupState newState)
    {
        if(id <= 0)
            throw new ArgumentException("El Id del grupo debe ser mayor que cero");

        if (!Enum.IsDefined(newState))
            throw new ArgumentException("El estado indicado no es valido");

        var group = await _orderGroupRepository.GetByIdWithDetailsAsync(id);

        if (group is null)
            return null;

        if (group.order is null)
            throw new InvalidOperationException("El grupo no tiene una orden asociada.");

        if (group.order.State != OrderState.Active)
            throw new InvalidOperationException("No se puede modificar el estado de un grupo de una orden cerrada.");

        if (newState == OrderGroupState.Ready && !group.Details.Any())
            throw new InvalidOperationException("No se puede marcar como listo un grupo sin detalles.");

        bool validTransition =
            (group.State == OrderGroupState.InPreparation &&
            newState == OrderGroupState.Ready) ||
            (group.State == OrderGroupState.Ready &&
            newState == OrderGroupState.Delivered);

        if (!validTransition)
            throw new InvalidOperationException("La transicion de estado no esta permitida");

        group.State = newState;

        await _orderGroupRepository.UpdateAsync(group);

        return MapToDto(group);
    }
    private static OrderGroupDto MapToDto(OrderGroup group)
    {
        return new OrderGroupDto
        {
            Id = group.Id,
            OrderId = group.OrderId,
            State = group.State,
            Details = group.Details.Select(detail => new OrderDetailDto
            {
                Id = detail.Id,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice,
                Subtotal = detail.Subtotal,
                Product_id = detail.Product_id,
                OrderGroup_id = detail.OrderGroup_id
            }).ToList()
        };
    }
}
