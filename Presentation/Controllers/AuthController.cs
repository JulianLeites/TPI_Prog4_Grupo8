using Application.Interfaces;
using Application.Models;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);

        if (!result)
            return Conflict(new
            {
                message = "Ya existe un usuario con ese email"
            });

        return Ok(new
        {
            message = "Usuario registrado correctamente"
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var token = await _authService.LoginAsync(loginDto);

        if (token is null)
            return Unauthorized(new
            {
                message = "Email o Contraseña incorrectos"
            });

        return Ok(new
        {
            accessToken = token
        });
    }
}
