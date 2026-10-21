using Application.Interfaces;
using Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _IUserRepository;

    public UserService(IUserRepository UserRepository)
    {
        _IUserRepository = UserRepository;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
    {
        var users = await _IUserRepository.GetAllAsync();

        return users.Select(user => new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        });
    }

    public async Task<UserResponseDto> GetByIdAsync(int id)
    {
        var user = await _IUserRepository.GetByIdAsync(id);

        if (user is null)
            return null;

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };
    }
}
