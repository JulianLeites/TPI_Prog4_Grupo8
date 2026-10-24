using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class OrderDetailService : IOrderDetailService
{
    private readonly IOrderDetailRepository _orderDetailRepository;

    public OrderDetailService( IOrderDetailRepository repository)
    {
        _orderDetailRepository = repository;
    } 

    public async Task<IEnumerable<OrderDetailDto>> GetAllAsync()
    {
        var tables = await _orderDetailRepository.GetAllAsync();

        return tables.Select(table => new Order
        {
            Id = table.Id,
            Number = table.Number,
            Capacity = table.Capacity,
            State = table.State
        });
    }

    public async Task<OrderDetailDto?> GetByIdAsync(int id)
    {
        var table = await _orderDetailRepository.GetByIdAsync(id);

        if (table is null)
            return null;

        return new OrderDetailDto
        {
            Id = table.Id,
            Number = table.Number,
            Capacity = table.Capacity,
            State = table.State
        };
    }

    public async Task<OrderDetailDto?> UpdateAsync(int id, OrderDetailDto dto)
    {
        var table = await _orderDetailRepository.GetByIdAsync(id);

        if (table is null)
            return null;

        table.Number = dto.Number;
        table.Capacity = dto.Capacity;

        await _orderDetailRepository.UpdateAsync(table);

        return new OrderDetailDto
        {
            Id = table.Id,
            Number = table.Number,
            Capacity = table.Capacity,
            State = table.State
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var table = await _orderDetailRepository.GetByIdAsync(id);

        if (table is null)
            return false;

        await _orderDetailRepository.DeleteAsync(table);
        return true;
    }

    public async Task<OrderDetailDto> CreateAsync(TableCreateDto dto)
    {
        var table = new OrderDetailDto
        {
            Number = dto.Number,
            Capacity = dto.Capacity
        };

        await _orderDetailRepository.AddAsync(table);

        return new OrderDetailDto
        {
            Id = table.Id,
            Number = table.Number,
            Capacity = table.Capacity,
            State = table.State
        };
    }

    public async Task<OrderDetailDto?> ChangeStateAsync(int id, ChangeTableStateDto dto)
    {
        var table = await _orderDetailRepository.GetByIdAsync(id);

        if (table is null)
            return null;

        table.State = dto.State;

        await _orderDetailRepository.UpdateAsync(table);

        return new OrderDetailDto
        {
            Id = table.Id,
            Number = table.Number,
            Capacity = table.Capacity,
            State = table.State
        };
    }
}