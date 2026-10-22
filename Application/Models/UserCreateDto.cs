using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Models;

public class UserCreateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Surname { get; set; } = string.Empty;
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int RoleId { get; set; }
}
