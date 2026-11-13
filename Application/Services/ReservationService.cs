using System;
using System.Collections.Generic;
using System.Text;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Enums;

namespace Application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _repository;
    private readonly ITableRepository _tableRepository;
    private readonly IUserRepository _userRepository;

    public ReservationService(IReservationRepository repository, ITableRepository tableRepository, IUserRepository userRepository)
    {
        _repository = repository;
        _tableRepository = tableRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<ReservationDto>> GetAllAsync()
    {
        var reservations = await _repository.GetAllAsync();
        return reservations.Select(r => new ReservationDto
        {
            Id = r.Id,
            Date = r.Date,
            Time = r.Time,
            Diners = r.Diners,
            UserId = r.UserId,
            TableIds = r.Tables.Select(t => t.Id).ToList(),
            State = r.State

        });
    }

    public async Task<ReservationDto?> GetByIdAsync(int id)
    {
        var reservation = await _repository.GetByIdAsync(id);
        if (reservation == null) return null;

        return new ReservationDto
        {
            Id = reservation.Id,
            Date = reservation.Date,
            Time = reservation.Time,
            Diners = reservation.Diners,
            UserId = reservation.UserId,
            TableIds = reservation.Tables.Select(t => t.Id).ToList(),
            State = reservation.State
        };
    }

    public async Task<ReservationDto> CreateAsync(CreateReservationDto dto)
    {
        if (dto.Diners < 1)
            throw new ArgumentException("La cantidad de comensales debe ser al menos 1.");
        var reservationDateTime = dto.Date.ToDateTime(dto.Time);

        if (reservationDateTime <= DateTime.Now)
            throw new ArgumentException("La fecha y hora de la reserva deben ser posteriores al actual.");

        var user = await _userRepository.GetByIdAsync(dto.UserId);
        if (user is null)
            throw new ArgumentException($"No existe un usuario con ID {dto.UserId}.");

        var occupiedTableIds = await _repository.GetOccupiedTableIdsAsync(dto.Date, dto.Time);
        var allTables = (await _tableRepository.GetAllAsync()).ToList();

        var assignedTables = new List<Table>();

        if (dto.TableIds is not null && dto.TableIds.Any())
        {
            // Selección manual de mesas
            var distinctIds = dto.TableIds.Distinct().ToList();

            foreach (var tableId in distinctIds)
            {
                var table = allTables.FirstOrDefault(t => t.Id == tableId);
                if (table is null)
                    throw new ArgumentException($"No existe una mesa con ID {tableId}.");

                if (occupiedTableIds.Contains(tableId))
                    throw new InvalidOperationException($"La mesa {table.Number} no está disponible en ese horario.");

                assignedTables.Add(table);
            }

            int totalCapacity = assignedTables.Sum(t => t.Capacity);
            if (totalCapacity < dto.Diners)
                throw new ArgumentException($"La capacidad total de las mesas seleccionadas ({totalCapacity}) es menor a la cantidad de comensales ({dto.Diners}).");
        }
        else
        {
            // Asignación automática para clientes
            var freeTables = allTables
                .Where(t => !occupiedTableIds.Contains(t.Id))
                .OrderBy(t => t.Capacity)
                .ToList();

            // Mesa individual más chica que cubra los comensales
            var singleBestTable = freeTables.FirstOrDefault(t => t.Capacity >= dto.Diners);

            if (singleBestTable is not null)
            {
                assignedTables.Add(singleBestTable);
            }
            else
            {
                // Usar mesas libres de mayor a menor capacidad hasta cubrir comensales
                int accumulatedCapacity = 0;
                foreach (var table in freeTables.OrderByDescending(t => t.Capacity))
                {
                    assignedTables.Add(table);
                    accumulatedCapacity += table.Capacity;
                    if (accumulatedCapacity >= dto.Diners)
                        break;
                }

                if (accumulatedCapacity < dto.Diners)
                    throw new InvalidOperationException("No hay mesas disponibles con capacidad suficiente para esa fecha y horario.");
            }
        }

        var entity = new Reservation
        {
            Date = dto.Date,
            Time = dto.Time,
            Diners = dto.Diners,
            UserId= dto.UserId,
            State = ReservationState.Confirmed,
            Tables = assignedTables

        };

        await _repository.AddAsync(entity);

        return new ReservationDto
        {
            Id = entity.Id,
            Date = entity.Date,
            Time = entity.Time,
            Diners = entity.Diners,
            UserId = entity.UserId,
            TableIds = entity.Tables.Select(t => t.Id).ToList(),
            State = entity.State
        };
    }

    public async Task<bool> UpdateAsync(int id, UpdateReservationDto dto)
    {
        var reservation = await _repository.GetByIdAsync(id);
        if (reservation is null) return false;

        if (dto.Diners < 1)
            throw new ArgumentException("La cantidad de comensales debe ser al menos 1.");
        
        if (dto.Date.ToDateTime(dto.Time) <= DateTime.Now)
            throw new ArgumentException("La fecha y hora de la reserva deben ser posteriores al momento actual.");

        int currentCapacity = reservation.Tables.Sum(t => t.Capacity);
        if (currentCapacity < dto.Diners)
            throw new ArgumentException($"Las mesas asignadas tienen capacidad para {currentCapacity} personas, insuficiente para {dto.Diners} comensales.");

        var occupiedIds = await _repository.GetOccupiedTableIdsAsync(dto.Date, dto.Time, excludeReservationId: id);
        var conflictingTable = reservation.Tables.FirstOrDefault(t => occupiedIds.Contains(t.Id));

        if (conflictingTable is not null)
            throw new InvalidOperationException($"No se puede asignar ese horario porque no estan disponibles las mesas .");
        
        reservation.Date = dto.Date;
        reservation.Time = dto.Time;
        reservation.Diners = dto.Diners;

        await _repository.UpdateAsync(reservation);
        return true;
    }

    public async Task<ReservationDto?> ChangeStateAsync(int id, ChangeReservationStateDto dto)
    {
        var reservation = await _repository.GetByIdAsync(id);

        if (reservation == null)
            return null;

        reservation.State = dto.State;

        await _repository.UpdateAsync(reservation);

        return new ReservationDto
        {
            Id = reservation.Id,
            Date = reservation.Date,
            Time = reservation.Time,
            Diners = reservation.Diners,
            UserId = reservation.UserId,
            TableIds = reservation.Tables.Select(t => t.Id).ToList(),
            State = reservation.State
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var reservation = await _repository.GetByIdAsync(id);
        if (reservation == null) return false;

        await _repository.DeleteAsync(reservation);
        return true;
    }

    public async Task<IEnumerable<ReservationDto>> GetReservationsByUserIdAsync(int userId)
    {
        var reservations = await _repository.GetReservationsByUserIdAsync(userId);
        return reservations.Select(r => new ReservationDto
        {
            Id = r.Id,
            Date = r.Date,
            Time = r.Time,
            Diners = r.Diners,
            UserId = r.UserId,
            TableIds = r.Tables.Select(t => t.Id).ToList(),
            State = r.State
        });
    }

    public async Task<IEnumerable<ReservationDto>> GetReservationsByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        if (startDate > endDate)
        {
            throw new ArgumentException("La fecha de inicio no puede ser posterior a la fecha de fin.");
        }

        var reservations = await _repository.GetReservationsByDateRangeAsync(startDate, endDate);
        return reservations.Select(r => new ReservationDto
        {
            Id = r.Id,
            Date = r.Date,
            Time = r.Time,
            Diners = r.Diners,
            UserId = r.UserId,
            TableIds = r.Tables.Select(t => t.Id).ToList(),
            State = r.State
        });
    }


    public async Task<IEnumerable<ReservationDto>> GetReservationsByStateAsync(string state)
    {
        if (!Enum.TryParse(state, out ReservationState parsedState))
        {
            throw new ArgumentException($"Estado de reserva inválido: {state}");
        }
        var reservations = await _repository.GetReservationsByStateAsync(parsedState);
        return reservations.Select(r => new ReservationDto
        {
            Id = r.Id,
            Date = r.Date,
            Time = r.Time,
            Diners = r.Diners,
            UserId = r.UserId,
            TableIds = r.Tables.Select(t => t.Id).ToList(),
            State = r.State
        });
    }

    public async Task<IEnumerable<TableDto>> GetAvailableTablesAsync(DateOnly date, TimeOnly time, int diners)
    {
        if (diners < 1)
            throw new ArgumentException("La cantidad de comensales debe ser al menos 1.");

        var occupiedIds = await _repository.GetOccupiedTableIdsAsync(date, time);
        var allTables = await _tableRepository.GetAllAsync();

        return allTables
            .Where(t => !occupiedIds.Contains(t.Id))
            .OrderBy(t => t.Number)
            .Select(t => new TableDto
            {
                Id = t.Id,
                Number = t.Number,
                Capacity = t.Capacity,
                State = t.State
            });
    }

}
