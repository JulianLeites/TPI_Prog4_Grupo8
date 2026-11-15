using Application.Models;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetAllAsync();
    Task<UserResponseDto?> GetByIdAsync(int id);
    Task<UserResponseDto?> UpdateAsync(int id, UserUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<UserResponseDto> CreateAsync(UserCreateDto dto);
    Task<bool> ChangePasswordAsync(int id, ChangePasswordDto dto);
    Task<UserResponseDto?> ChangeRoleAsync(int id, ChangeRoleDto dto);
    Task<IEnumerable<UserResponseDto>> GetByRoleIdAsync(int roleId);
}
