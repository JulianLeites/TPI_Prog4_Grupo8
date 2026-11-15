using Domain.Enums;

namespace Domain.Entities;

public class OrderGroup
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public OrderGroupState State { get; set; } = OrderGroupState.InPreparation;
    public Order order { get; set; } = null!;
    public ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();
}
