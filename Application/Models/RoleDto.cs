using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models;

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class RoleUpdateDto
{
    public string Name { get; set; } = string.Empty;
}

public class RoleCreateDto
{
    public string Name { get; set; } = string.Empty;
}
