using Domain.Entities;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Initialization;

public class RoleSeeder
{
    private readonly ApplicationDbContext _context;

    public RoleSeeder(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task InitializeAsync()
    {
        var requiredRoles = new List<Role>
        {
            new Role { Id = 1, Name = "admin" },
            new Role { Id = 2, Name = "waiter" },
            new Role { Id = 3, Name = "kitchen" },
            new Role { Id = 4, Name = "user" }
        };

        foreach(var requiredRole in requiredRoles)
        {
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == requiredRole.Id);

            if (existingRole is not null)
            {
                if(existingRole.Name != requiredRole.Name)
                {
                    throw new InvalidOperationException($"El Id {requiredRole.Id} esta ocupado por otro rol");
                }

                continue;
            }

            var nameAlredyExits = await _context.Roles
                .AnyAsync(r => r.Name == requiredRole.Name);

            if (nameAlredyExits)
            {
                throw new InvalidOperationException($"El rol {requiredRole.Name} ya existe con otro Id");
            }

            _context.Roles.Add(requiredRole);
        }
        await _context.SaveChangesAsync();
    }
}
