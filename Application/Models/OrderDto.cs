
using Domain.Entities;
using Domain.Enums;

namespace Application.Models;

public class OrderDto
{
    public int Id { get; set; }

    public OrderState State { get; set; }
    public decimal Total { get; set; }
    public int Table_id { get; set; }
    public ICollection<OrderDetailDto> Details { get; set; } = new List<OrderDetailDto>();
}

public class OrderCreateDto
{
    public int Table_id { get; set; }
}

public class ChangeOrderStateDto
{
    public OrderState State { get; set; }
}