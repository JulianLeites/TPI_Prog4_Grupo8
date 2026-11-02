using Application.Interfaces;
using Infrastructure.Persistance;
using Microsoft.Extensions.Configuration;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Initialization;

public class InitialAdminSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasherService;

    public InitialAdminSeeder(
        ApplicationDbContext context,
        IPasswordHasherService passwordHasherService)
    {
        _context = context;
        _passwordHasherService = passwordHasherService;
    }

    public async Task InitializeAsync(IConfiguration configuration)
    {
        var adminRole = await _context.Roles
            .SingleOrDefaultAsync(r => r.Name == "Admin");

        if (adminRole is null)
            throw new InvalidOperationException("No existe el rol Admin");

        var adminExists = await _context.Users
            .AnyAsync(u => u.RoleId == adminRole.Id);

        if (adminExists)
            return;

        var name = configuration["InitialAdmin:name"];
        var surname = configuration["InitialAdmin:surname"];
        var email = configuration["InitialAdmin:email"];
        var password = configuration["InitialAdmin:password"];

        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(surname) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("Falta configurar las credenciales del admin inicial");
        }

        if (password.Length < 8 || password.Any(char.IsWhiteSpace))
            throw new InvalidOperationException("La contraseña debe tener al menos caracteres y sin espacios");

        email = email.Trim().ToLowerInvariant();

        var existingUser = await _context.Users
            .AnyAsync(u => u.Email == email);

        if (existingUser)
            throw new InvalidOperationException("El email configurado para el administrador ya pertenece a otro usuario");

        var admin = new User
        {
            Name = name.Trim(),
            Surname = surname.Trim(),
            Email = email.Trim(),
            Password = _passwordHasherService.HashPassword(password),
            PhoneNumber = "Sin especificar",
            RoleId = adminRole.Id,
        };

        _context.Users.Add(admin);
        await _context.SaveChangesAsync();
    }
}
