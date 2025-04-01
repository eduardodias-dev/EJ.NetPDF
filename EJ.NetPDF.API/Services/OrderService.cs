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
    private readonly ILogger<OrderService> _logger;

    public OrderService(IPaymentExternalRepository paymentExternalRepository, 
        IRepository<Order> repo, IRepository<Product> productRepo, 
        ICustomerExternalRepository customerExternalRepository, 
        ILogger<OrderService> logger)
    {
        _paymentExternalRepository = paymentExternalRepository;
        _repo = repo;
        _productRepo = productRepo;
        _customerExternalRepository = customerExternalRepository;
        _logger = logger;
    }

    public async Task<Order> CreateOrder(CreateOrderModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var dueDate = GetDueDate();
        var products = await _productRepo.GetAllAsync(0, 100);
        var product = products.FirstOrDefault(p => p.Id == model.ProductId) ??
                      throw new InvalidOperationException($"Product {model.ProductId} not found.");
        var order = new Order(model.CustomerId!, product);
        var description = GetDescription(order);
        var addSubscriptionModel =
            new AddSubscriptionModel(model.CustomerId!, model.PaymentType!, order.Total, dueDate, product.Cycle!, description);
        
        var addOrderTask = _repo.AddAsync(order);
        var createPaymentTask = CreateSubscription(addSubscriptionModel);
        
        order.SetPaymentId((await createPaymentTask).Id!);
        order.Process();
        
        await addOrderTask; //ensure insertion before updating it
        await _repo.UpdateAsync(order);
        
        return order;
    }

    public async Task<OrderModel> GetOrderById(Guid id)
    {
        var order = await _repo.GetByIdAsync(id);
        var subscriptionTask = _paymentExternalRepository.GetSubscription(order.PaymentId!);
        var paymentsTask = _paymentExternalRepository.GetSubscriptionPayments(order.PaymentId!);
        var customerTask = _customerExternalRepository.GetCustomer(order.CustomerId!);
        
        var subscriptionData = await subscriptionTask;
        subscriptionData.Payments = (await paymentsTask).Data;
        
        return new OrderModel
        {
            SubscriptionData = subscriptionData,
            CustomerData = await customerTask,
            Product = new ProductModel(order.Product.Id, order.Product.Name!, order.Product.Description!, order.Product.Price!, order.Product.Cycle!),
            Status = order.Status.ToString(),
        };
    }

    private async Task<Subscription> CreateSubscription(AddSubscriptionModel subscriptionData)
    {
        try
        {
            var subscription = await _paymentExternalRepository.CreateSubscription(subscriptionData);
            
            return subscription;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Error while adding subscription.");
            throw;
        }
    }

    private static string GetDescription(Order order)
    {
        return $"{order.Product!.Description}";
    }

    private static DateTime GetDueDate()
    {
        return DateTime.Today.AddDays(5);
    }
}