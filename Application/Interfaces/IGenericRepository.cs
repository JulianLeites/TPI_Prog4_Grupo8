using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();

    Task<T?> GetByIdAsync(int id);
    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);
}