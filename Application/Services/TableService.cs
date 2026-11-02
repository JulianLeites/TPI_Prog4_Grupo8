using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;

    public TableService(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<IEnumerable<TableDto>> GetAllAsync(TableState? state)
    {
        var tables = state.HasValue 
            ? await _tableRepository.GetByStateAsync(state.Value)
            : await _tableRepository.GetAllAsync();

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

    public async Task<TableDto?> GetByNumberAsync(int number)
    {
        var table = await _tableRepository.GetByNumberAsync(number);

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

    public async Task<TableDto?> UpdateAsync(int id, TableUpdateDto dto)
    {
        var table = await _tableRepository.GetByIdAsync(id);

        if (table is null)
            return null;

        if (dto.Capacity < 1)
            throw new ArgumentException("La capacidad debe ser minimo 1.");

        if (dto.Number < 1)
            throw new ArgumentException("El número de mesa debe ser minimo 1.");


        if (await _tableRepository.ExistsByNumberAsync(dto.Number, id))
            throw new ArgumentException("Ya existe otra mesa con ese número.");

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

        var now = DateTime.Now;
        var date = DateOnly.FromDateTime(now);
        var time = TimeOnly.FromDateTime(now);

        var hasFutureReservations = await _tableRepository.HasFutureConfirmedReservationsAsync(id, date, time);

        if (hasFutureReservations)
            throw new InvalidOperationException("No se puede eliminar una mesa que tiene reservas futuras confirmadas");

        await _tableRepository.DeleteAsync(table);
        return true;
    }

    public async Task<TableDto> CreateAsync(TableCreateDto dto)
    {
        if (dto.Capacity < 1)
            throw new ArgumentException("La capacidad debe ser minimo 1.");
        
        if (dto.Number < 1)
            throw new ArgumentException("El número de mesa debe ser minimo 1.");

        if (await _tableRepository.ExistsByNumberAsync(dto.Number))
            throw new ArgumentException("Ya existe una mesa con ese numero");

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
