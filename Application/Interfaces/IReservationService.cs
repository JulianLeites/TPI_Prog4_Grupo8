using Application.Models;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces;

public interface IReservationService
{
    Task<IEnumerable<ReservationDto>> GetAllAsync();
    Task<ReservationDto?> GetByIdAsync(int id);
    Task<ReservationDto> CreateAsync(CreateReservationDto dto);
    Task<bool> UpdateAsync(int id, UpdateReservationDto dto);
    Task<bool> DeleteAsync(int id);
    Task<ReservationDto?> ChangeStateAsync(int id, ChangeReservationStateDto dto);
    Task<IEnumerable<ReservationDto>> GetReservationsByUserIdAsync(int userId);
    Task<IEnumerable<ReservationDto>> GetReservationsByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    Task<IEnumerable<ReservationDto>> GetReservationsByStateAsync(string state);
}