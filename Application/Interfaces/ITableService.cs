using Application.Models;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces;

public interface ITableService
{
    Task<IEnumerable<TableDto>> GetAllAsync(TableState? state);
    Task<TableDto?> GetByIdAsync(int id);
    Task<TableDto?> UpdateAsync(int id, TableUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<TableDto> CreateAsync(TableCreateDto dto);
    Task<TableDto?> ChangeStateAsync(int id, ChangeTableStateDto dto);
    Task<TableDto?> GetByNumberAsync(int number);
}
