namespace EJ.NetPDF.API.Models;

public class OrderModel
{
    public Payment PaymentData { get; set; }
    public Customer CustomerData { get; set; }
    public string Status { get; set; }
    public decimal Total { get; set; }
    public ICollection<OrderItemModel> Items { get; set; }

}