namespace EJ.NetPDF.API.Models;

public class OrderItem : Entity
{
    public int Amount { get; set; }
    public Product? Product { get; set; }
}