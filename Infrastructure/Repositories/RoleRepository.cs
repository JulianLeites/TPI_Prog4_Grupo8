using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
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
}
