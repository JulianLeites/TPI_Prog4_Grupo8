using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
}
