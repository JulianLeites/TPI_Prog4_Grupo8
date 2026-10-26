using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    
    public JwtService (IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(int userId,  string email, string role)
    {
        var secretKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("No se configuro la clave JWT");

        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("No se configuro el emisor JWT");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("No se configuro la audiencia JWT");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey)
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
