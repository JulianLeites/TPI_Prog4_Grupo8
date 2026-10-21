using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models;

public class UserResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public string RoleName {  get; set; } = string.Empty;
}
