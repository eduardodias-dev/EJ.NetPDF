namespace EJ.NetPDF.API.Models;

public class CreateOrderModel
{
    public string? CustomerId { get; set; }
    public string? PaymentType { get; set; }
    public Guid? ProductId { get; set; }
    
    public CreateOrderModel(string? customerId, string? paymentType, Guid? productId)
    {
        if (string.IsNullOrWhiteSpace(customerId)) throw new ArgumentNullException(nameof(customerId));
        if (string.IsNullOrWhiteSpace(paymentType)) throw new ArgumentNullException(nameof(paymentType));
        
        if ((productId ?? Guid.Empty) == Guid.Empty) throw new InvalidOperationException("Invalid product.");
        
        CustomerId = customerId;
        PaymentType = paymentType;
        ProductId = productId!.Value;
    }
}
