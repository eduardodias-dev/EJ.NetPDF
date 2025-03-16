using EJ.NetPDF.API.Data.Interfaces;
using EJ.NetPDF.API.Models;
using EJ.NetPDF.API.Services.ExternalRepositories;
using Refit;

namespace EJ.NetPDF.API.Services;

public class OrderService : IOrderService
{
    private readonly IPaymentExternalRepository _paymentExternalRepository;
    private readonly ICustomerExternalRepository _customerExternalRepository;
    private readonly IRepository<Order> _repo;
    private readonly IRepository<Product> _productRepo;

    public OrderService(IPaymentExternalRepository paymentExternalRepository, 
        IRepository<Order> repo, ICustomerExternalRepository customerExternalRepository, IRepository<Product> productRepo)
    {
        _paymentExternalRepository = paymentExternalRepository;
        _repo = repo;
        _customerExternalRepository = customerExternalRepository;
        _productRepo = productRepo;
    }

    public async Task<Order> CreateOrder(CreateOrderModel model)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }
        
        var dueDate = GetDueDate();
        var products = await _productRepo.GetAllAsync(0, 100);
        var items = model.Items.Select(x => new OrderItem()
        {
            Id = Guid.NewGuid(),
            Amount = x.Amount,
            Product = products.FirstOrDefault(p => p.Id == x.ProductId) ?? throw new InvalidOperationException($"Product {x.ProductId} not found."),
        });
        
        var order = new Order(model.CustomerId!, items.ToList());
        await _repo.AddAsync(order);
        
        var description = GetDescription(order);
        var addPaymentData = new AddPaymentModel(model.CustomerId!, model.PaymentType!, order.Total, dueDate, description);
        var paymentData = await CreatePayment(addPaymentData);
        
        order.SetPaymentId(paymentData.Id!);
        order.Process();
        
        await _repo.UpdateAsync(order);
        
        return order;
    }

    private async Task<Payment> CreatePayment(AddPaymentModel paymentData)
    {
        try
        {
            var payment = await _paymentExternalRepository.CreatePayment(paymentData);
            
            return payment;
        }
        catch (ApiException ex)
        {
            //Log ApiExceptions
            
            throw;
        }
    }

    private string GetDescription(Order order)
    {
        var description = $"Order {order.Id}";
        description += Environment.NewLine;
        description += "Items:";
        description += Environment.NewLine;
        foreach (var item in order.Items)
        {
            description += $"{item.Product!.Name}";
        }
        
        return description;
    }

    private DateTime GetDueDate()
    {
        return DateTime.Today;
    }
}