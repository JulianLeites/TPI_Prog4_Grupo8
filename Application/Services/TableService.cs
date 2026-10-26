using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;

    public TableService(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<IEnumerable<TableDto>> GetAllAsync()
    {
        var tables = await _tableRepository.GetAllAsync();

        return tables.Select(table => new TableDto
        {
            Id = table.Id,
            Number = table.Number,
            Capacity = table.Capacity,
            State = table.State
        });
    }

    public async Task<TableDto?> GetByIdAsync(int id)
    {
        var table = await _tableRepository.GetByIdAsync(id);

        if (table is null)
            return null;

        return new TableDto
        {
            Id = table.Id,
            Number = table.Number,
            Capacity = table.Capacity,
            State = table.State
        };
    }

    public async Task<TableDto?> UpdateAsync(
        int id,
        TableUpdateDto dto)
    {
        var table = await _tableRepository.GetByIdAsync(id);

        if (table is null)
            return null;

        table.Number = dto.Number;
        table.Capacity = dto.Capacity;

        await _tableRepository.UpdateAsync(table);

        return new TableDto
        {
            Id = table.Id,
            Number = table.Number,
            Capacity = table.Capacity,
            State = table.State
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var table = await _tableRepository.GetByIdAsync(id);

        if (table is null)
            return false;

        await _tableRepository.DeleteAsync(table);
        return true;
    }

    public async Task<TableDto> CreateAsync(TableCreateDto dto)
    {
        var table = new Table
        {
            Number = dto.Number,
            Capacity = dto.Capacity
        };

        await _tableRepository.AddAsync(table);

        return new TableDto
        {
            Id = table.Id,
            Number = table.Number,
            Capacity = table.Capacity,
            State = table.State
        };
    }

    public async Task<TableDto?> ChangeStateAsync(int id, ChangeTableStateDto dto)
    {
        var table = await _tableRepository.GetByIdAsync(id);

        if (table is null)
            return null;

        table.State = dto.State;

        await _tableRepository.UpdateAsync(table);

        return new TableDto
        {
            Id = table.Id,
            Number = table.Number,
            Capacity = table.Capacity,
            State = table.State
        };
    }
}
