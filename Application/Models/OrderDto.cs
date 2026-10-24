
using Domain.Entities;

namespace Application.Models;

public class OrderDto
{
    public int Id { get; set; }

    public Order.EnumState State { get; set; }
    public decimal Total { get; set; }
    public int Table_id { get; set; }
}

public class OrderCreateDto
{
    public decimal Total { get; set; }
    public int Table_id { get; set; }
}

public class ChangeOrderStateDto
{
    public Order.EnumState State { get; set; }
}