using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IPasswordHasherService _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        IGenericRepository<Role> roleRepository,
        IPasswordHasherService passwordHasher
    )
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(user => new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RoleId = user.RoleId,
            RoleName = user.Role.Name
        });
    }

    public async Task<UserResponseDto?> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return null;

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RoleId = user.RoleId,
            RoleName = user.Role.Name
        };
    }

    public async Task<UserResponseDto?> UpdateAsync(
        int id,
        UserUpdateDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return null;

        var role = await _roleRepository.GetByIdAsync(dto.RoleId);

        if (role is null)
            throw new ArgumentException("El rol seleccionado no existe");
        user.Name = dto.Name;
        user.Surname = dto.Surname;
        user.Email = dto.Email;
        user.PhoneNumber = dto.PhoneNumber;
        user.RoleId = dto.RoleId;

        await _userRepository.UpdateAsync(user);

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RoleId = user.RoleId,
            RoleName = user.Role.Name
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return false;

        await _userRepository.DeleteAsync(user);
        return true;
    }

    public async Task<UserResponseDto> CreateAsync(UserCreateDto dto)
    {
        var role = await _roleRepository.GetByIdAsync(dto.RoleId);

        if (role is null)
            throw new ArgumentException("El rol no existe");

        var user = new User
        {
            Name = dto.Name,
            Surname = dto.Surname,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            RoleId = dto.RoleId,
            Password = _passwordHasher.HashPassword(dto.Password)
        };

        await _userRepository.AddAsync(user);

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RoleId = user.RoleId,
            RoleName = role.Name
        };
    }

    public async Task<bool> ChangePasswordAsync(int id, ChangePasswordDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return false;

        var isCurrentPasswordValid = _passwordHasher.VerifyPassword(
            dto.CurrentPassword,
            user.Password
        );

        if (!isCurrentPasswordValid)
            return false;

        user.Password = _passwordHasher.HashPassword(dto.NewPassword);

        await _userRepository.UpdateAsync(user);

        return true;
    }
}