using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        IGenericRepository<Role> roleRepository,
        IPasswordHasherService passwordHasherService,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasherService = passwordHasherService;
        _jwtService = jwtService;
    }

    public async Task<bool> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);

        if (existingUser is not null)
            return false;

        var roles = await _roleRepository.GetAllAsync();
        var userRole = roles.FirstOrDefault(
            role => role.Name == "User"
        );

        if (userRole is null)
            throw new InvalidOperationException("No existe el rol predeterminado");

        var user = new User
        {
            Name = registerDto.Name,
            Surname = registerDto.Surname,
            Email = registerDto.Email,
            PhoneNumber = registerDto.PhoneNumber,
            Password = _passwordHasherService.HashPassword(registerDto.Password),
            RoleId = userRole.Id
        };

        await _userRepository.AddAsync(user);

        return true;
    }

    public async Task<string?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email.Trim());

        if (user is null)
            return null;

        var isPasswordValid = _passwordHasherService.VerifyPassword(
            loginDto.Password,
            user.Password
        );

        if (!isPasswordValid)
            return null;

        return _jwtService.GenerateToken(
            user.Id,
            user.Email,
            user.Role.Name
        );
    }
}
