using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models;

public class ReservationDto
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public int Diners { get; set; }
}

public class CreateReservationDto
{
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public int Diners { get; set; }
}

public class UpdateReservationDto
{
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public int Diners { get; set; }
}