namespace EJ.NetPDF.API.Models;

public class CreateOrderModel
{
    public string? CustomerId { get; set; }
    public string? PaymentType { get; set; }
    
    public ICollection<OrderItemModel> Items { get; set; }

    public CreateOrderModel(string? customerId, string? paymentType, ICollection<OrderItemModel>? items)
    {
        if (string.IsNullOrWhiteSpace(customerId)) throw new ArgumentNullException(nameof(customerId));
        if (string.IsNullOrWhiteSpace(paymentType)) throw new ArgumentNullException(nameof(customerId));
        
        if (items?.Count == 0) throw new InvalidOperationException("Orders must have at least one item.");
        
        CustomerId = customerId;
        PaymentType = paymentType;
        Items = items;
    }
}

public class OrderItemModel
{
    public int Amount { get; set; }
    public Guid ProductId { get; set; }
}