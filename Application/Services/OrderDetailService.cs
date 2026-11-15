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
    private readonly IOrderGroupRepository _orderGroupRepository;
    private readonly IProductRepository _productRepository;
    private readonly ITableRepository _tableRepository;

    public OrderDetailService( IOrderDetailRepository orderDetailRepository,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ITableRepository tableRepository,
        IOrderGroupRepository orderGroupRepository)
    {
        _orderDetailRepository = orderDetailRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _tableRepository = tableRepository;
        _orderGroupRepository = orderGroupRepository;
    } 

    public async Task<IEnumerable<OrderDetailDto>> GetAllAsync()
    {
        var details = await _orderDetailRepository.GetAllAsync();

        return details.Select(MapToDto);
    }

    public async Task<OrderDetailDto?> GetByIdAsync(int id)
    {
        var detail = await _orderDetailRepository.GetByIdAsync(id);

        return detail is null ? null : MapToDto(detail);
    }

    public async Task<IEnumerable<OrderDetailDto>> GetDetailsByOrderGroupIdAsync(int orderGroupId)
    {
        var details = await _orderDetailRepository
        .GetDetailsByOrderGroupIdAsync(orderGroupId);

        return details.Select(MapToDto);
    }

    public async Task<IEnumerable<OrderDetailDto>> GetDetailsByProductIdAsync(int id)
    {
        var details = await _orderDetailRepository.GetDetailsByProductIdAsync(id);

        return details.Select(MapToDto);
    }

    public async Task<OrderDetailDto?> ChangeQuantityAsync(int id, OrderDetailQuantityDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var detail = await _orderDetailRepository.GetByIdAsync(id);

        if (detail is null)
            return null;

        var group = await ValidateOrderGroupAsync(detail.OrderGroup_id);

        if (dto.Quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.");

        detail.Quantity = dto.Quantity;
        detail.Subtotal = detail.CalculateSubtotal();

        await _orderDetailRepository.UpdateAsync(detail);

        await UpdateOrderTotalAsync(group.OrderId);

        return MapToDto(detail);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var detail = await _orderDetailRepository.GetByIdAsync(id);

        if (detail is null)
            return false;

        var group = await ValidateOrderGroupAsync(detail.OrderGroup_id);

        bool isLastDetail = group.Details.Count == 1;

        await _orderDetailRepository.DeleteAsync(detail);

        if (isLastDetail)
        {
            await _orderGroupRepository.DeleteAsync(group);
        }

        await UpdateOrderTotalAsync(group.OrderId);

        return true;
    }

    public async Task<OrderDetailDto> CreateAsync(CreateOrderDetailDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.Quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.");

        var group = await ValidateOrderGroupAsync(dto.OrderGroup_id);

        var product = await _productRepository.GetByIdAsync(dto.Product_id);

        if (product is null)
            throw new ArgumentException($"El producto con id {dto.Product_id} no existe.");

        var existingDetail = await _orderDetailRepository.GetExistingDetailAsync(
            dto.OrderGroup_id,
            dto.Product_id,
            product.Price);

        if (existingDetail is not null)
        {
            existingDetail.Quantity += dto.Quantity;
            existingDetail.Subtotal = existingDetail.CalculateSubtotal();

            await _orderDetailRepository.UpdateAsync(existingDetail);

            await UpdateOrderTotalAsync(group.OrderId);

            return MapToDto(existingDetail);
        }

        var detail = new OrderDetail
        {
            Quantity = dto.Quantity,
            UnitPrice = product.Price,
            Product_id = dto.Product_id,
            OrderGroup_id = dto.OrderGroup_id
        };

        detail.Subtotal = detail.CalculateSubtotal();

        await _orderDetailRepository.AddAsync(detail);

        await UpdateOrderTotalAsync(group.OrderId);

        return MapToDto(detail);
    }

    private static OrderDetailDto MapToDto(OrderDetail detail)
    {
        return new OrderDetailDto
        {
            Id = detail.Id,
            Quantity = detail.Quantity,
            UnitPrice = detail.UnitPrice,
            Subtotal = detail.Subtotal,
            Product_id = detail.Product_id,
            OrderGroup_id = detail.OrderGroup_id
        };
    }

    private async Task<OrderGroup> ValidateOrderGroupAsync(int orderGroupId)
    {
        var group = await _orderGroupRepository.GetByIdWithDetailsAsync(orderGroupId);

        if (group is null)
            throw new ArgumentException($"El grupo con id {orderGroupId} no existe.");

        if (group.order is null)
            throw new InvalidOperationException("El grupo no tiene una orden asociada.");

        if (group.order.State != OrderState.Active)
            throw new InvalidOperationException("No se pueden modificar detalles de una orden cerrada.");

        if (group.State != OrderGroupState.InPreparation)
            throw new InvalidOperationException("Solo se pueden modificar grupos en preparación.");

        var table = await _tableRepository.GetByIdAsync(group.order.Table_id);

        if (table is null || table.State != TableState.Occupied)
            throw new InvalidOperationException("La mesa asociada no está ocupada.");

        return group;
    }

    private async Task UpdateOrderTotalAsync(int orderId)
    {
        var groups = await _orderGroupRepository.GetByOrderIdAsync(orderId);

        var total = groups
            .SelectMany(group => group.Details)
            .Sum(detail => detail.Subtotal);

        await _orderRepository.UpdateTotalAsync(orderId, total);
    }
}