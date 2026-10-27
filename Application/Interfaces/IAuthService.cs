using Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterDto registerDto);
    Task <string?> LoginAsync(LoginDto loginDto);
}
