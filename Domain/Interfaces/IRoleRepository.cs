using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces;

public interface IRoleRepository
    : IGenericRepository<Role>
{
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    Task<bool> HasUsersAsync(int roleId);
}
