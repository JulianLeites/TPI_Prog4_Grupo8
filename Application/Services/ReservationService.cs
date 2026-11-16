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
        if (reservationDateTime < DateTime.Now.AddHours(2))
            throw new ArgumentException("Las reservas deben hacerse con al menos 2 horas de anticipación.");

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
            State = ReservationState.Pending,
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

    public async Task<ReservationDto?> ChangeStateAsync(int id, ChangeReservationStateDto dto)
    {
        if (!Enum.IsDefined(typeof(ReservationState), dto.State))
            throw new ArgumentException("El estado de reserva no es válido.");

        var reservation = await _repository.GetByIdAsync(id);
        if (reservation is null)
            return null;

        if (reservation.State == dto.State)
            throw new ArgumentException($"La reserva ya se encuentra en estado {reservation.State}.");

        bool isValidTransition = reservation.State switch
        {
            ReservationState.Pending => dto.State is ReservationState.Confirmed or ReservationState.Cancelled,
            _ => false
        };

        if (!isValidTransition)
            throw new InvalidOperationException($"No se puede cambiar una reserva de estado '{reservation.State}' a '{dto.State}'.");

        if (dto.State == ReservationState.Confirmed &&
            reservation.Date.ToDateTime(reservation.Time) > DateTime.Now.AddMinutes(30))
            throw new InvalidOperationException("Solo se puede confirmar la reserva cuando el cliente llega.");

        if (dto.State == ReservationState.Confirmed)
        {
            foreach (var table in reservation.Tables)
            {
                if (table.State == TableState.Occupied)
                    throw new InvalidOperationException($"La mesa {table.Number} todavía está ocupada.");

                table.State = TableState.Occupied;
            }
        }

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
        if ((date.ToDateTime(time) - DateTime.Now).TotalMinutes < 60)
        {
            occupiedIds.AddRange(
                allTables.Where(t => t.State == TableState.Occupied).Select(t => t.Id));
        }

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

    public async Task<ReservationDto> CreateWalkInAsync(WalkInReservationDto dto, int userId)
    {
        var now = DateTime.Now;
        var date = DateOnly.FromDateTime(now);
        var time = new TimeOnly(now.Hour, now.Minute);

        var reservedTableIds = await _repository.GetOccupiedTableIdsAsync(date, time);
        var assignedTables = new List<Table>();

        foreach (var tableId in dto.TableIds.Distinct())
        {
            var table = await _tableRepository.GetByIdAsync(tableId)
                ?? throw new ArgumentException($"No existe una mesa con ID {tableId}.");

            if (table.State == TableState.Occupied)
                throw new InvalidOperationException($"La mesa {table.Number} ya está ocupada.");

            if (reservedTableIds.Contains(tableId))
                throw new InvalidOperationException($"La mesa {table.Number} tiene una reserva cercana a este horario.");

            assignedTables.Add(table);
        }

        int totalCapacity = assignedTables.Sum(t => t.Capacity);
        if (totalCapacity < dto.Diners)
            throw new ArgumentException($"La capacidad de las mesas ({totalCapacity}) es menor a la cantidad de comensales ({dto.Diners}).");

        foreach (var table in assignedTables)
            table.State = TableState.Occupied;

        var entity = new Reservation
        {
            Date = date,
            Time = time,
            Diners = dto.Diners,
            UserId = userId,
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



}


