namespace Application.Models;

public class OrderDetailDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public int Product_id { get; set; }
    public int Order_id { get; set; } 

}

public class CreateOrderDetailDto
{
    public int Quantity { get; set; }
   
    public int Product_id { get; set; }
    
    public int Order_id { get; set; } 

}

public class OrderDetailUpdateDto
{
    public int Quantity { get; set; }
   
}