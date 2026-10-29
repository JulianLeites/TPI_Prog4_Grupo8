using System.ComponentModel.DataAnnotations;

namespace Application.Models;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{6}$",
        ErrorMessage = "El código debe contener exactamente 6 dígitos.")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    [RegularExpression(@"^\S+$",
        ErrorMessage = "La contraseña no puede contener espacios.")]
    public string NewPassword { get; set; } = string.Empty;
}