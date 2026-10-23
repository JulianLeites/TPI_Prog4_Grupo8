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

    public ReservationService(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ReservationDto>> GetAllAsync()
    {
        var reservations = await _repository.GetAllAsync();
        return reservations.Select(r => new ReservationDto
        {
            Id = r.Id,
            Date = r.Date,
            Time = r.Time,
            Diners = r.Diners
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
            Diners = reservation.Diners
        };
    }

    public async Task<ReservationDto> CreateAsync(CreateReservationDto dto)
    {
        var entity = new Reservation
        {
            Date = dto.Date,
            Time = dto.Time,
            Diners = dto.Diners
        };

        await _repository.AddAsync(entity);

        return new ReservationDto
        {
            Id = entity.Id,
            Date = entity.Date,
            Time = entity.Time,
            Diners = entity.Diners
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

    public async Task<bool> DeleteAsync(int id)
    {
        var reservation = await _repository.GetByIdAsync(id);
        if (reservation == null) return false;

        await _repository.DeleteAsync(reservation);
        return true;
    }
}