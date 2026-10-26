using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

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

        return orders.Select(order => new OrderDto
        {
            Id = order.Id,
            Total = order.Total,
            Table_id = order.Table_id,
            State = order.State,
            Details = order.Details
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
            State = order.State
        };
    }

    public async Task<OrderDto?> UpdateAsync( int id, OrderCreateDto dto)
    {
        var table = await _tableRepository.GetByIdAsync(dto.Table_id);
        if(table is null){
            throw new ArgumentException("La mesa no existe");
        }
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
            return null;

        order.Table_id = dto.Table_id;
        

        await _orderRepository.UpdateAsync(order);

        return new OrderDto
        {
            Id = order.Id,
            Total = order.Total,
            Table_id = order.Table_id,
            State = order.State
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
            return false;

        await _orderRepository.DeleteAsync(order);
        return true;
    }

    public async Task<OrderDto> CreateAsync(OrderCreateDto dto)
    {

        var table = await _tableRepository.GetByIdAsync(dto.Table_id);
        if(table is null){
            throw new ArgumentException($"La mesa con id {dto.Table_id} no existe");
        }
        var order = new Order
        {
            Table_id = dto.Table_id,
        };

        order.CalculateTotal();

        await _orderRepository.AddAsync(order);

        return new OrderDto
        {
            Id = order.Id,
            Total = order.Total,
            Table_id = order.Table_id,
            State = order.State
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
}