using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);

        if (!result)
            return Problem(
                statusCode: 409,
                title: "Conflicto de registro",
                detail: "Ya existe un usuario con ese email."
            );

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
            return Problem(
                statusCode: 401,
                title: "Autenticación fallida",
                detail: "Email o contraseña incorrectos."
            );

        return Ok(new
        {
            accessToken = token
        });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        try
        {
            await _authService.ForgotPasswordAsync(dto);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error durante el proceso de recuperacion de contraseña");
        }

        return Ok(new
        {
            message = "Si existe una cuenta asociada a ese correo recibirás un código de recuperacion de contraseña"
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var result = await _authService.ResetPasswordAsync(dto);

        if (!result)
            return Problem(
                statusCode: 400,
                title: "Restablecimiento fallido",
                detail: "No se pudo restablecer la contraseña. Verificá el código y los datos ingresados."
            );

        return Ok(new
        {
            message = "Contraseña restablecida correctamente"
        });
    }
}