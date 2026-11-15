using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Models;

public class TableDto
{
    public int Id { get; set; }
    public int Number { get; set; }
    public int Capacity { get; set; }
    public TableState State { get; set; }
}

public class TableUpdateDto
{
    [Range(1, int.MaxValue,
        ErrorMessage = "El numero de mesa debe ser mayor que cero")]
    public int Number { get; set; }
    [Range(1, int.MaxValue,
        ErrorMessage = "La capacidad de la mesa debe ser mayor a cero")]
    public int Capacity { get; set; }
}

public class TableCreateDto
{
    [Range(1, int.MaxValue,
        ErrorMessage = "El numero de mesa debe ser mayor que cero")]
    public int Number { get; set; }
    [Range(1, int.MaxValue,
        ErrorMessage = "La capacidad de la mesa debe ser mayor a cero")]
    public int Capacity { get; set; }
}

public class ChangeTableStateDto
{
    [EnumDataType(typeof(TableState),
        ErrorMessage = "El estado de la mesa no es válido.")]
    public TableState State { get; set; }
}