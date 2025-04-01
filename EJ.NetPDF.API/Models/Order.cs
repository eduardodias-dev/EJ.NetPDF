namespace EJ.NetPDF.API.Models;

public class Order : Entity
{
    public string? PaymentId { get; private set; }
    public string? CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Product Product { get; private set; }
    
    public decimal Total => Product.Price;
    
    public Order(string customerId, Product product)
    {
        CustomerId = customerId;
        Product = product;
        Status = OrderStatus.Created;
    }

    public void Process()
    {
        if (Status != OrderStatus.Created)
        {
            throw new InvalidOperationException($"Order {Id} has already been started.");
        }
        
        Status = OrderStatus.Processing;
    }

    public void Pay()
    {
        if (Status != OrderStatus.Processing)
        {
            throw new InvalidOperationException($"Order {Id} is not processing.");
        }

        Status = OrderStatus.Payed;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Payed)
        {
            throw new InvalidOperationException($"Order {Id} cannot be cancelled after payed.");
        }
        
        if (Status == OrderStatus.Completed)
        {
            throw new InvalidOperationException($"Order {Id} cannot be cancelled after completed.");
        }

        Status = OrderStatus.Canceled;
    }

    public void Complete()
    {
        if (Status == OrderStatus.Canceled)
        {
            throw new InvalidOperationException($"Order {Id} cannot be completed after canceled.");
        }

        if (Status != OrderStatus.Payed)
        {
            throw new InvalidOperationException($"Order {Id} cannot be completed without being payed.");
        }
        
        Status = OrderStatus.Completed;
    }

    public void SetPaymentId(string paymentId)
    {
        PaymentId = paymentId;
    }
}

public enum OrderStatus
{
    Created,
    Processing,
    Canceled,
    Payed,
    Completed
}
