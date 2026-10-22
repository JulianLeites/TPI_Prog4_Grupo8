using Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces;

public interface ITableService
{
    Task<IEnumerable<TableDto>> GetAllAsync();
    Task<TableDto?> GetByIdAsync(int id);
    Task<TableDto?> UpdateAsync(int id, TableUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<TableDto> CreateAsync(TableCreateDto dto);
    Task<TableDto?> ChangeStateAsync(int id, ChangeTableStateDto dto);
}
