using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces;

public interface IPasswordResetRepository : IGenericRepository<PasswordReset>
{
    Task<IEnumerable<PasswordReset>> GetUnusedByUserIdAsync(int userId);
    Task<PasswordReset?> GetLatestUnusedByUserIdAsync(int userId);
}
