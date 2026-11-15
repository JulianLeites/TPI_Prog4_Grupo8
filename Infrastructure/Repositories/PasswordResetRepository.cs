using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories;

public class PasswordResetRepository : GenericRepository<PasswordReset>, IPasswordResetRepository
{
    public PasswordResetRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PasswordReset>> GetUnusedByUserIdAsync( int userId)
    {
        return await _context.PasswordResets
            .Where(pr => pr.UserId == userId && !pr.IsUsed)
            .ToListAsync();
    }

    public async Task<PasswordReset?> GetLatestUnusedByUserIdAsync(int userId)
    {
        return await _context.PasswordResets
            .Where(pr => pr.UserId == userId && !pr.IsUsed)
            .OrderByDescending(pr => pr.CreatedAt)
            .FirstOrDefaultAsync();
    }
}
