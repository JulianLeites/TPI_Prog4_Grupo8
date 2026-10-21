using Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetAllAsync();
    Task<UserResponseDto?> GetByIdAsync(int id);
}
