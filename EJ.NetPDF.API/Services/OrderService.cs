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
        IRepository<Order> repo, IRepository<Product> productRepo, 
        ICustomerExternalRepository customerExternalRepository)
    {
        _paymentExternalRepository = paymentExternalRepository;
        _repo = repo;
        _productRepo = productRepo;
        _customerExternalRepository = customerExternalRepository;
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
        var description = GetDescription(order);
        
        var addPaymentData =
            new AddPaymentModel(model.CustomerId!, model.PaymentType!, order.Total, dueDate, description);
        var addOrderTask = _repo.AddAsync(order);
        var createPaymentTask = CreatePayment(addPaymentData);
        
        order.SetPaymentId((await createPaymentTask).Id!);
        order.Process();
        
        await addOrderTask; //ensure insertion before updating it
        await _repo.UpdateAsync(order);
        
        return order;
    }

    public async Task<OrderModel> GetOrderById(Guid id)
    {
        var order = await _repo.GetByIdAsync(id);
        var paymentTask = _paymentExternalRepository.GetPayment(order.PaymentId!.ToString());
        var customerTask = _customerExternalRepository.GetCustomer(order.CustomerId!.ToString());
        var items = order.Items.Select(x => new OrderItemModel
        {
            Amount = x.Amount,
            ProductId = x.Product?.Id ?? Guid.Empty,
        });

        return new OrderModel
        {
            PaymentData = await paymentTask,
            CustomerData = await customerTask,
            Items = items.ToList(),
            Status = order.Status.ToString(),
        };
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
        var description = "";
        
        foreach (var item in order.Items)
        {
            description += $"{item.Product!.Description}";
            description += Environment.NewLine;
        }
        
        return description;
    }

    private DateTime GetDueDate()
    {
        return DateTime.Today;
    }
}