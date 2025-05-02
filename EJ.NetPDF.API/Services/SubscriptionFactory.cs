using EJ.NetPDF.API.Models;

namespace EJ.NetPDF.API.Services;

public class SubscriptionFactory : ISubscriptionFactory
{
    public AddSubscriptionModel Create(Order order)
    {
        var dueDate = GetDueDate();
        return new AddSubscriptionModel(order.CustomerId!, order.PaymentType!, order.Total,
                dueDate, order.Product.Cycle!, order.Description);
    }
    
    private DateTime GetDueDate()
    {
        return DateTime.Today.AddDays(5);
    }
}