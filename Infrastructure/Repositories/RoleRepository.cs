using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories;

public class RoleRepository
    : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        var query = _dbSet.Where(r => r.Name == name);

        if (excludeId.HasValue)
            query = query.Where(r => r.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<bool> HasUsersAsync(int roleId)
    {
        return await _context.Users.AnyAsync(u => u.RoleId == roleId);
    }
}
