using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories;

public class RoleRespository
    : GenericRepository<Role>, IRoleRepository
{
    public RoleRespository(ApplicationDbContext context) : base(context)
    {
    }
}
