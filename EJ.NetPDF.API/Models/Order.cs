namespace EJ.NetPDF.API.Models;

public class Order
{
    public Guid Id { get; set; }
    public string PaymentId { get; set; }
    public string CustomerId { get; set; }
    public ICollection<OrderItem> Items { get; set; }
    
    public decimal Total => Items?.Sum(i => i.Product?.Price ?? 0) ?? 0;

}