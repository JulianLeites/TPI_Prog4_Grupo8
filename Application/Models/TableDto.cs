using Domain.Enums;
using System;
using System.Collections.Generic;
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
    public int Number { get; set; }
    public int Capacity { get; set; }
}

public class TableCreateDto
{
    public int Number { get; set; }
    public int Capacity { get; set; }
}

public class ChangeTableStateDto
{
    public TableState State { get; set; }
}