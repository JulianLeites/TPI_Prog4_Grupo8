using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces;

public interface IPasswordRecoveryRepository
{
    Task<bool> ResetPasswordAtomicallyAsync(int userId, int passwordResetId, string newPasswordHash);
}
