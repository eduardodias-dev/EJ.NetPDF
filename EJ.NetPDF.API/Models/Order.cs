namespace EJ.NetPDF.API.Models;

public class Order : Entity
{
    public string? PaymentId { get; private set; }
    public string? CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public ICollection<OrderItem> Items { get; private set; }
    
    public decimal Total => Items?.Sum(i => i.Product?.Price ?? 0) ?? 0;
    
    public Order(string customerId, 
        ICollection<OrderItem> items)
    {
        CustomerId = customerId;
        
        Items = items;
     
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

    public void AddItems(IEnumerable<OrderItem> items)
    {
        foreach (var item in items)
        {
            Items.Add(item);
        }
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
