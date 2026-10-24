using System;
using System.Collections.Generic;
using System.Text;
using Application.Models;

namespace Application.Interfaces;

public interface IReservationService
{
    Task<IEnumerable<ReservationDto>> GetAllAsync();
    Task<ReservationDto?> GetByIdAsync(int id);
    Task<ReservationDto> CreateAsync(CreateReservationDto dto);
    Task<bool> UpdateAsync(int id, UpdateReservationDto dto);
    Task<bool> DeleteAsync(int id);
    Task<ReservationDto?> ChangeStateAsync(int id, ChangeReservationStateDto dto);
}