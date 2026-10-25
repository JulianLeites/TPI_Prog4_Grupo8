using System;
using System.Collections.Generic;
using System.Text;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _repository;
    private readonly ITableRepository _tableRepository;

    public ReservationService(IReservationRepository repository, ITableRepository tableRepository)
    {
        _repository = repository;
        _tableRepository = tableRepository;
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
        var entity = new Reservation
        {
            Date = dto.Date,
            Time = dto.Time,
            Diners = dto.Diners,
            UserId= dto.UserId,
            State = dto.State
        };

        foreach (var tableId in dto.TableIds)
        {
            var table = await _tableRepository.GetByIdAsync(tableId);

            if (table is null)
                throw new ArgumentException(
                    $"No existe una mesa con ID {tableId}.");

            entity.Tables.Add(table);
        }

        await _repository.AddAsync(entity);

        return new ReservationDto
        {
            Id = entity.Id,
            Date = entity.Date,
            Time = entity.Time,
            Diners = entity.Diners,
            UserId = entity.UserId,
            TableIds = entity.Tables.Select(t => t.Id).ToList()
        };
    }

    public async Task<bool> UpdateAsync(int id, UpdateReservationDto dto)
    {
        var reservation = await _repository.GetByIdAsync(id);
        if (reservation == null) return false;

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
}