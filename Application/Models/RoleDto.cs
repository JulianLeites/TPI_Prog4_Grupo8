using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Models;

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class RoleUpdateDto
{
    [Required(ErrorMessage = "El nombre del rol es obligatorio")]
    [StringLength(50, ErrorMessage = "El nombre del rol no puede superar los 50 caracteres")]
    public string Name { get; set; } = string.Empty;
}

public class RoleCreateDto
{
    [Required(ErrorMessage = "El nombre del rol es obligatorio")]
    [StringLength(50, ErrorMessage = "El nombre del rol no puede superar los 50 caracteres")]
    public string Name { get; set; } = string.Empty;
}
