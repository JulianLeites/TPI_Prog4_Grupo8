using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models;

public class LoginDto
{
    public string Email {  get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterDto
{
    public string Name { get; set; } = string.Empty;
    public string Surname {  get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber {  get; set; } = string.Empty;
}
