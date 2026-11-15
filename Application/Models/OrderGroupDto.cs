using Domain.Enums;

namespace Application.Models;

public class OrderGroupDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public OrderGroupState State { get; set; }

    public ICollection<OrderDetailDto> Details { get; set; } = new List<OrderDetailDto>();
}

public class OrderProductSummaryDto
{
    public int Product_id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}
