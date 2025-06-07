namespace EJ.NetPDF.API.Models;

public class OrderModel
{
    public Guid Id { get; set; }
    public Subscription? SubscriptionData { get; set; }
    public Customer? CustomerData { get; set; }
    public string? Status { get; set; }
    public ProductModel? Product { get; set; }
}