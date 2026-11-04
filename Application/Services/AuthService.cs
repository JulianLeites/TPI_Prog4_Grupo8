using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IGenericRepository<Role> _roleRepository;
    private readonly IPasswordResetRepository _passwordResetRepository;
    private readonly IEmailService _emailService;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IJwtService _jwtService;
    private readonly IPasswordRecoveryRepository _passwordRecoveryRepository;

    public AuthService(
        IUserRepository userRepository,
        IGenericRepository<Role> roleRepository,
        IPasswordResetRepository passwordResetRepository,
        IEmailService emailService,
        IPasswordRecoveryRepository passwordRecoveryRepository,
        IPasswordHasherService passwordHasherService,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordResetRepository = passwordResetRepository;
        _passwordRecoveryRepository = passwordRecoveryRepository;
        _emailService = emailService;
        _passwordHasherService = passwordHasherService;
        _jwtService = jwtService;
    }

    public async Task<bool> RegisterAsync(RegisterDto registerDto)
    {
        var email = registerDto.Email.Trim().ToLowerInvariant();
        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (existingUser is not null)
            return false;

        var roles = await _roleRepository.GetAllAsync();
        var userRole = roles.FirstOrDefault(
            role => role.Name.Equals("user", StringComparison.OrdinalIgnoreCase)
        );

        if (userRole is null)
            throw new InvalidOperationException("No existe el rol predeterminado");

        var user = new User
        {
            Name = registerDto.Name,
            Surname = registerDto.Surname,
            Email = email,
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


    public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null)
            return;

        var resets = await _passwordResetRepository
            .GetUnusedByUserIdAsync(user.Id);

        foreach (var reset in resets)
        {
            reset.IsUsed = true;
            await _passwordResetRepository.UpdateAsync(reset);
        }

        var code = RandomNumberGenerator
            .GetInt32(0, 1_000_000)
            .ToString("D6");

        var codeHash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(code)));

        var passwordReset = new PasswordReset
        {
            UserId = user.Id,
            CodeHash = codeHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            IsUsed = false,
            Attempts = 0
        };

        await _passwordResetRepository.AddAsync(passwordReset);

        await _emailService.SendPasswordResetCodeAsync(user.Email, code);
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null)
            return false;

        var reset = await _passwordResetRepository
            .GetLatestUnusedByUserIdAsync(user.Id);

        if (reset is null)
            return false;

        if (reset.ExpiresAt <= DateTime.UtcNow)
            return false;

        if (reset.Attempts >= 3)
            return false;

        reset.Attempts++;

        var providedCodeHash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(dto.Code)));

        var hashesMatch = CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(reset.CodeHash),
            Convert.FromHexString(providedCodeHash));

        if (!hashesMatch)
        {
            await _passwordResetRepository.UpdateAsync(reset);
            return false;
        }

        var newPasswordHash = _passwordHasherService.HashPassword(dto.NewPassword);

        return await _passwordRecoveryRepository.ResetPasswordAtomicallyAsync(
            user.Id,
            reset.Id,
            newPasswordHash
        );
    }

}
