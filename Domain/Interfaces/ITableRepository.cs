using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces;

public interface ITableRepository : IGenericRepository<Table>
{
    Task<IEnumerable<Table>> GetByStateAsync(TableState state);
    Task<bool> ExistsByNumberAsync(int number, int? excludeId = null);
    Task<Table?> GetByNumberAsync(int number);
    Task<bool> HasFutureConfirmedReservationsAsync(int tableId, DateOnly date, TimeOnly time);
}
