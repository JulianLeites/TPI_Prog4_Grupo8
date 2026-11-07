using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;


namespace Domain.Interfaces;

public interface IReservationRepository : IGenericRepository<Reservation>
{
    Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(int userId);
    Task<IEnumerable<Reservation>> GetReservationsByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    Task<IEnumerable<Reservation>> GetReservationsByStateAsync(ReservationState state);

}