namespace Application.Models;

public class OrderDetailDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrize { get; set; }
    public decimal Subtotal { get; set; }
}