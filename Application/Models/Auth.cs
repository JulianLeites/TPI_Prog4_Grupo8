using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Models;

public class LoginDto
{
    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    [StringLength(254, ErrorMessage = "El email no puede superar los 254 caracteres.")]
    public string Email {  get; set; } = string.Empty;
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Password { get; set; } = string.Empty;
}

public class RegisterDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres")]
    [RegularExpression(@".*\S.", ErrorMessage = "El nombre no puede contener solo espacios")]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "El apellido no puede contener solamente espacios.")]
    public string Surname {  get; set; } = string.Empty;
    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    [StringLength(254, ErrorMessage = "El email no puede superar los 254 caracteres.")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(8, ErrorMessage = "La nueva contraseña debe tener al menos 8 caracteres.")]
    [RegularExpression(@"^\S+$", ErrorMessage = "La contraseña no puede contener espacios.")]
    public string Password { get; set; } = string.Empty;
    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
    [RegularExpression(@".*\S.*", ErrorMessage = "El teléfono no puede contener solamente espacios.")]
    public string PhoneNumber {  get; set; } = string.Empty;
}
