using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetCodeAsync(string email, string code);
}
