using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository
    : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public override async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dbSet
            .Include(user => user.Role)
            .ToListAsync();
    }

    public override async Task<User?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(user => user.Role)
            .FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(user => user.Role)
            .FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<IEnumerable<User>> GetByRoleIdAsync(int roleId)
    {
        return await _dbSet
            .Include(user => user.Role)
            .Where(user => user.RoleId == roleId)
            .ToListAsync();
    }
}
