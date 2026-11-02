using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories;

public class PasswordRecoveryRepository : IPasswordRecoveryRepository
{
    private readonly ApplicationDbContext _context;

    public PasswordRecoveryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ResetPasswordAtomicallyAsync(int userId, int passwordResetId, string newPasswordHash)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var now = DateTime.UtcNow;

            var affectedRows = await _context.PasswordResets
                .Where(r =>
                r.Id == passwordResetId &&
                r.UserId == userId &&
                !r.IsUsed &&
                r.ExpiresAt > now &&
                r.Attempts < 3)
                .ExecuteUpdateAsync(setters =>
                setters.SetProperty(r => r.IsUsed, true));

            if(affectedRows != 1)
            {
                await transaction.RollbackAsync();
                return false;
            }

            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Id == userId);

            if (user is null)
            {
                await transaction.RollbackAsync();
                return false;
            }

            user.Password = newPasswordHash;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
