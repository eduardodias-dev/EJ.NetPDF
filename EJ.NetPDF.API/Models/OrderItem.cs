namespace EJ.NetPDF.API.Models;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Amount { get; set; }
    public Product? Product { get; set; }
}