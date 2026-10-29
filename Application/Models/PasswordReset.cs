using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Application.Models;

public class PasswordReset
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public string CodeHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public int Attempts { get; set; }
    public DateTime CreatedAt { get; set; }
}
