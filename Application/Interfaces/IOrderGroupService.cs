using Application.Models;
using Domain.Enums;

namespace Application.Interfaces;

public interface IOrderGroupService
{
    Task<IEnumerable<OrderGroupDto>> GetByOrderIdAsync(int orderId);

    Task<IEnumerable<OrderGroupDto>> GetByStateAsync(OrderGroupState state);

    Task<OrderGroupDto?> GetByIdAsync(int id);

    Task<OrderGroupDto> CreateAsync(int orderId);

    Task<OrderGroupDto?> ChangeStateAsync(int id, OrderGroupState newState);
}